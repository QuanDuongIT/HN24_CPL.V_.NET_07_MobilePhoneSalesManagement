using Microsoft.EntityFrameworkCore;
using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;

namespace ServerApp.BLL.Services
{
    public interface IProductService : IBaseService<Product>
    {
        Task<IEnumerable<ProductVm>> GetAllProductAsync();
        Task<IEnumerable<ProductVm>> FilterProductsAsync(FilterRequest filterRequest);
        Task<ProductVm> AddProductAsync(InputProductVm brandVm);
        Task<ProductVm> UpdateProductAsync(int id, InputProductVm brandVm);
        Task<int> UpdateProductAsync(Product product);

        Task<ProductVm> DeleteProductAsync(int id);

        Task<ProductVm?> GetByProductIdAsync(int id);
        Task<IEnumerable<ProductSpecificationVm>> GetProductSpecificationsByProductIdAsync(int productId);
    }
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpecificationTypeService _specificationTypeService;

        public ProductService(IUnitOfWork unitOfWork, ISpecificationTypeService specificationTypeService) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _specificationTypeService = specificationTypeService;
        }

        private async Task<List<ProductSpecification>> ProcessSpecificationTypesAsync(
    IEnumerable<InputProductSpecificationVm> productSpecifications)
        {
            var productSpecificationsToAdd = new List<ProductSpecification>();

            foreach (var spec in productSpecifications)
            {
                // Kiểm tra SpecificationType có tồn tại không
                var specificationType = await _specificationTypeService.GetBySpecificationTypeIdAsync(spec.SpecificationTypeId);

                var specificationTypeVm = new InputSpecificationTypeVm
                {
                    Name = spec.SpecificationType.Name
                };

                if (specificationType != null)
                {
                    // Cập nhật SpecificationType
                    specificationType = await _specificationTypeService.UpdateSpecificationTypeAsync(spec.SpecificationTypeId, specificationTypeVm);
                }
                else
                {
                    // Thêm mới SpecificationType
                    specificationType = await _specificationTypeService.AddSpecificationTypeAsync(specificationTypeVm);
                }

                if (specificationType == null)
                {
                    throw new ArgumentException($"Failed to process SpecificationType: {spec.SpecificationType.Name}");
                }

                // Thêm ProductSpecification vào danh sách
                productSpecificationsToAdd.Add(new ProductSpecification
                {
                    SpecificationTypeId = specificationType.SpecificationTypeId,
                    Value = spec.Value
                });
            }

            return productSpecificationsToAdd;
        }
        private async Task<List<ProductSpecification>> ProcessAndSyncSpecificationsAsync(int productId, IEnumerable<InputProductSpecificationVm> productSpecifications)
        {
            var productSpecificationsToAddOrUpdate = new List<ProductSpecification>();

            // Lấy tất cả các ProductSpecification hiện tại của sản phẩm từ cơ sở dữ liệu
            var existingProductSpecifications = await _unitOfWork.GenericRepository<ProductSpecification>()
                .GetAllAsync(ps => ps.ProductId == productId);

            // Xử lý các SpecificationType mới và cập nhật ProductSpecification
            foreach (var spec in productSpecifications)
            {
                var specificationType = await _specificationTypeService.GetBySpecificationTypeIdAsync(spec.SpecificationTypeId);

                var specificationTypeVm = new InputSpecificationTypeVm
                {
                    Name = spec.SpecificationType.Name
                };

                if (specificationType != null)
                {
                    // Cập nhật SpecificationType nếu đã tồn tại
                    specificationType = await _specificationTypeService.UpdateSpecificationTypeAsync(spec.SpecificationTypeId, specificationTypeVm);
                }
                else
                {
                    // Thêm mới SpecificationType nếu không tồn tại
                    specificationType = await _specificationTypeService.AddSpecificationTypeAsync(specificationTypeVm);
                }

                if (specificationType == null)
                {
                    throw new ArgumentException($"Failed to process SpecificationType: {spec.SpecificationType.Name}");
                }

                // Kiểm tra xem ProductSpecification này đã tồn tại chưa trong danh sách hiện tại
                var existingProductSpec = existingProductSpecifications
                    .FirstOrDefault(ps => ps.SpecificationTypeId == specificationType.SpecificationTypeId);

                if (existingProductSpec != null)
                {
                    // Cập nhật giá trị nếu đã tồn tại
                    existingProductSpec.Value = spec.Value;
                    productSpecificationsToAddOrUpdate.Add(existingProductSpec);
                }
                else
                {
                    // Thêm mới ProductSpecification nếu chưa tồn tại
                    var newProductSpecification = new ProductSpecification
                    {
                        ProductId = productId,
                        SpecificationTypeId = specificationType.SpecificationTypeId,
                        Value = spec.Value
                    };
                    productSpecificationsToAddOrUpdate.Add(newProductSpecification);
                }
            }

            // Xóa các ProductSpecification không còn tồn tại trong danh sách mới
            var productSpecificationsToDelete = existingProductSpecifications
                .Where(ps => !productSpecifications.Any(spec => spec.SpecificationTypeId == ps.SpecificationTypeId))
                .ToList();

            // Xóa các ProductSpecification không còn sử dụng
            foreach (var productSpecification in productSpecificationsToDelete)
            {
                _unitOfWork.GenericRepository<ProductSpecification>().Delete(productSpecification);
            }

            // Trả về danh sách các ProductSpecification cần thêm hoặc cập nhật
            return productSpecificationsToAddOrUpdate;
        }

        public async Task<ProductVm> AddProductAsync(InputProductVm productVm)
        {
            ValidateModelPropertiesWithAttribute(productVm);

            // Kiểm tra xem Product đã tồn tại hay chưa
            var findProduct = await _unitOfWork.GenericRepository<Product>().GetAsync(p =>
                p.Name == productVm.Name
            );

            if (findProduct != null)
            {
                throw new ExceptionBusinessLogic("Product name is already in use.");
            }

            // Bắt đầu transaction
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Xử lý SpecificationType và chuẩn bị danh sách ProductSpecification
                var productSpecificationsToAdd = await ProcessSpecificationTypesAsync(productVm.ProductSpecifications);

                // Tạo Product
                var product = new Product
                {
                    Name = productVm.Name,
                    Description = productVm.Description,
                    Price = productVm.Price,
                    OldPrice = productVm.OldPrice,
                    StockQuantity = productVm.StockQuantity,
                    BrandId = productVm.BrandId,
                    ImageUrl = productVm.ImageUrl,
                    Manufacturer = productVm.Manufacturer,
                    IsActive = productVm.IsActive,
                    Color = productVm.Color,
                    Discount = productVm.Discount
                };

                // Thêm Product vào cơ sở dữ liệu
                await _unitOfWork.GenericRepository<Product>().AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                // Gắn ProductId vào từng ProductSpecification và thêm vào database
                foreach (var productSpecification in productSpecificationsToAdd)
                {
                    productSpecification.ProductId = product.ProductId;
                    await _unitOfWork.GenericRepository<ProductSpecification>().AddAsync(productSpecification);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _unitOfWork.SaveChangesAsync();

                // Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                // Trả về ProductVm
                return new ProductVm
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    OldPrice = product.OldPrice,
                    StockQuantity = product.StockQuantity,
                    BrandId = product.BrandId,
                    ImageUrl = product.ImageUrl,
                    Manufacturer = product.Manufacturer,
                    IsActive = product.IsActive,
                    Color = product.Color,
                    Discount = product.Discount,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine($"Error: {ex.Message}");

                // Rollback transaction nếu có lỗi
                await _unitOfWork.RollbackTransactionAsync();

                // Ném lại ngoại lệ để bên ngoài xử lý
                throw new ArgumentException($"{ex.Message}", ex);
            }
        }

        public async Task<ProductVm> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.GenericRepository<Product>().GetByIdAsync(id);

            if (product == null)
            {
                throw new ExceptionNotFound("Product not found");
            }
            var isdelete = 0;
            if (product.IsActive == true)
            {
                product.IsActive = false;
                isdelete = await UpdateProductAsync(product);
            }
            else
            {
                _unitOfWork.GenericRepository<Product>().Delete(id);
                isdelete = _unitOfWork.SaveChanges();
            }

            if (isdelete > 0)
            {
                return new ProductVm
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    OldPrice = product.OldPrice,
                    StockQuantity = product.StockQuantity,
                    BrandId = product.BrandId,
                    ImageUrl = product.ImageUrl,
                    Manufacturer = product.Manufacturer,
                    IsActive = product.IsActive,
                    Color = product.Color,
                    Discount = product.Discount,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                };
            }

            // Nếu lưu thất bại
            throw new ArgumentException("Failed to delete product");

        }

        public async Task<IEnumerable<ProductVm>> GetAllProductAsync()
        {
            var resuilt = await GetAllAsync(
                    includesProperties: "Brand,ProductSpecifications,ProductSpecifications.SpecificationType"
                );
            var productViewModels = resuilt.Select(product => new ProductVm
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                OldPrice = product.OldPrice,
                StockQuantity = product.StockQuantity,
                BrandId = product.BrandId,
                ImageUrl = product.ImageUrl,
                Manufacturer = product.Manufacturer,
                IsActive = product.IsActive,
                Color = product.Color,
                Discount = product.Discount,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                Brand = new BrandVm
                {
                    BrandId = product.Brand.BrandId,
                    Name = product.Brand.Name,
                    IsActive = product.Brand.IsActive,
                    ImageUrl = product.Brand.ImageUrl,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.Brand.UpdatedAt
                }, // Bao gồm dữ liệu liên kết Brand
                ProductSpecifications = product.ProductSpecifications.Select(ps => new ProductSpecificationVm
                {
                    ProductId = ps.ProductId,
                    SpecificationTypeId = ps.SpecificationTypeId,
                    Value = ps.Value,
                    CreatedAt = ps.CreatedAt,
                    UpdatedAt = ps.UpdatedAt,
                    SpecificationType = new SpecificationTypeVm
                    {
                        SpecificationTypeId = ps.SpecificationType.SpecificationTypeId,
                        Name = ps.SpecificationType.Name,
                        CreatedAt = ps.SpecificationType.CreatedAt,
                        UpdatedAt = ps.SpecificationType.UpdatedAt,
                    }
                }).ToList()
            });

            return productViewModels;
        }
        public async Task<IEnumerable<ProductSpecificationVm>> GetProductSpecificationsByProductIdAsync(int productId)
        {
            // Lấy sản phẩm với ProductId và bao gồm ProductSpecifications cùng với SpecificationType
            var product = await GetByProductIdAsync(productId);

            if (product == null)
            {
                throw new ExceptionNotFound("Product not found");
            }

            // Lọc và chuyển đổi dữ liệu ProductSpecifications thành ProductSpecificationVm
            var productSpecificationViewModels = product.ProductSpecifications.Select(ps => new ProductSpecificationVm
            {
                ProductId = ps.ProductId,
                SpecificationTypeId = ps.SpecificationTypeId,
                Value = ps.Value,
                SpecificationType = new SpecificationTypeVm
                {
                    SpecificationTypeId = ps.SpecificationType.SpecificationTypeId,
                    Name = ps.SpecificationType.Name
                }
            }).ToList();

            return productSpecificationViewModels;
        }



        public async Task<ProductVm?> GetByProductIdAsync(int id)
        {
            var product = await GetOneAsync(p =>
                p.ProductId == id,
                    //&& p.ProductSpecifications.Select(ps=>ps.),
                    includesProperties: "Brand,ProductSpecifications,ProductSpecifications.SpecificationType"
                );
            if (product == null)
            {
                throw new ExceptionNotFound("Product not found");
            }
            var productVm = new ProductVm
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                OldPrice = product.OldPrice,
                StockQuantity = product.StockQuantity,
                BrandId = product.BrandId,
                ImageUrl = product.ImageUrl,
                Manufacturer = product.Manufacturer,
                IsActive = product.IsActive,
                Color = product.Color,
                Discount = product.Discount, // Gán mặc định hoặc tính toán tùy theo logic
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                Brand = new BrandVm
                {
                    BrandId = product.Brand.BrandId,
                    Name = product.Brand.Name,
                    IsActive = product.Brand.IsActive,
                    ImageUrl = product.Brand.ImageUrl,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.Brand.UpdatedAt
                }, // Bao gồm dữ liệu liên kết Brand
                ProductSpecifications = product.ProductSpecifications.Select(ps => new ProductSpecificationVm
                {
                    ProductId = ps.ProductId,
                    SpecificationTypeId = ps.SpecificationTypeId,
                    Value = ps.Value,
                    SpecificationType = new SpecificationTypeVm
                    {
                        SpecificationTypeId = ps.SpecificationType.SpecificationTypeId,
                        Name = ps.SpecificationType.Name
                    }
                }).ToList()
            };

            return productVm;
        }

        public async Task<ProductVm> UpdateProductAsync(int id, InputProductVm productVm)
        {
            ValidateModelPropertiesWithAttribute(productVm);

            // Kiểm tra xem Product có tồn tại không
            var existingProduct = await _unitOfWork.GenericRepository<Product>().GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new ExceptionBusinessLogic("Product not found.");
            }

            // Bắt đầu transaction
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Xử lý và đồng bộ các ProductSpecification
                var productSpecificationsToAddOrUpdate = await ProcessAndSyncSpecificationsAsync(id, productVm.ProductSpecifications);

                // Cập nhật thông tin Product
                existingProduct.Name = productVm.Name;
                existingProduct.Description = productVm.Description;
                existingProduct.Price = productVm.Price;
                existingProduct.OldPrice = productVm.OldPrice;
                existingProduct.StockQuantity = productVm.StockQuantity;
                existingProduct.BrandId = productVm.BrandId;
                existingProduct.ImageUrl = productVm.ImageUrl;
                existingProduct.Manufacturer = productVm.Manufacturer;
                existingProduct.IsActive = productVm.IsActive;
                existingProduct.Color = productVm.Color;
                existingProduct.Discount = productVm.Discount;

                // Cập nhật Product vào cơ sở dữ liệu
                _unitOfWork.GenericRepository<Product>().Update(existingProduct);
                await _unitOfWork.SaveChangesAsync();

                // Lưu các ProductSpecification đã thay đổi vào cơ sở dữ liệu
                foreach (var productSpecification in productSpecificationsToAddOrUpdate)
                {
                    // Kiểm tra xem ProductSpecification có tồn tại trong cơ sở dữ liệu không
                    var existingSpec = await _unitOfWork.GenericRepository<ProductSpecification>().GetAsync(ps =>
                        ps.ProductId == id && ps.SpecificationTypeId == productSpecification.SpecificationTypeId);

                    if (existingSpec != null)
                    {
                        // Cập nhật nếu đã tồn tại
                        existingSpec.Value = productSpecification.Value;
                        _unitOfWork.GenericRepository<ProductSpecification>().Update(existingSpec);
                    }
                    else
                    {
                        // Thêm mới nếu chưa tồn tại
                        productSpecification.ProductId = id; // Gán ProductId
                        await _unitOfWork.GenericRepository<ProductSpecification>().AddAsync(productSpecification);
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _unitOfWork.SaveChangesAsync();

                // Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                // Trả về ProductVm
                return new ProductVm
                {
                    ProductId = existingProduct.ProductId,
                    Name = existingProduct.Name,
                    Description = existingProduct.Description,
                    Price = existingProduct.Price,
                    OldPrice = existingProduct.OldPrice,
                    StockQuantity = existingProduct.StockQuantity,
                    BrandId = existingProduct.BrandId,
                    ImageUrl = existingProduct.ImageUrl,
                    Manufacturer = existingProduct.Manufacturer,
                    IsActive = existingProduct.IsActive,
                    Color = existingProduct.Color,
                    Discount = existingProduct.Discount,
                    CreatedAt = existingProduct.CreatedAt,
                    UpdatedAt = existingProduct.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine($"Error: {ex.Message}");

                // Rollback transaction nếu có lỗi
                await _unitOfWork.RollbackTransactionAsync();

                // Ném lại ngoại lệ để bên ngoài xử lý
                throw new ArgumentException($"{ex.Message}", ex);
            }


        }
        public async Task<int> UpdateProductAsync(Product product)
        {
            // Cập nhật thông tin Product
            product.Name = product.Name;
            product.Description = product.Description;
            product.Price = product.Price;
            product.OldPrice = product.OldPrice;
            product.StockQuantity = product.StockQuantity;
            product.BrandId = product.BrandId;
            product.ImageUrl = product.ImageUrl;
            product.Manufacturer = product.Manufacturer;
            product.IsActive = product.IsActive;
            product.Color = product.Color;
            product.Discount = product.Discount;

            // Cập nhật Product vào cơ sở dữ liệu
            
            return await _unitOfWork.GenericRepository<Product>().ModifyAsync(product);
        }

        public async Task<IEnumerable<ProductVm>> FilterProductsAsync(FilterRequest filterRequest)
        {
            try
            {
                var query = await GetAllAsync(includesProperties: "Brand,ProductSpecifications,ProductSpecifications.SpecificationType");

                // Lọc theo Hãng sản xuất
                if (filterRequest.Brands != null && filterRequest.Brands.Any())
                {
                    query = query.Where(p => p.Brand != null && filterRequest.Brands.Contains(p.Brand.Name));
                }

                // Lọc theo Giá bán
                if (filterRequest.Prices != null && filterRequest.Prices.Any())
                {
                    foreach (var price in filterRequest.Prices)
                    {
                        switch (price)
                        {
                            case "under2m":
                                query = query.Where(p => p.Price < 2000000);
                                break;
                            case "2to5m":
                                query = query.Where(p => p.Price >= 2000000 && p.Price <= 5000000);
                                break;
                            case "5to10m":
                                query = query.Where(p => p.Price >= 5000000 && p.Price <= 10000000);
                                break;
                            case "10to15m":
                                query = query.Where(p => p.Price >= 10000000 && p.Price <= 15000000);
                                break;
                            case "above15m":
                                query = query.Where(p => p.Price >= 15000000);
                                break;
                        }
                    }
                }

                // Lọc theo Màn hình
                if (filterRequest.ScreenSizes != null && filterRequest.ScreenSizes.Any())
                {
                    var screenSizeTypeId = await _unitOfWork.GenericRepository<SpecificationType>()
                                                             .GetQuery()
                                                             .Where(s => s.Name == "ScreenSize")
                                                             .Select(s => s.SpecificationTypeId)
                                                             .FirstOrDefaultAsync();
                    if (screenSizeTypeId == 0)
                    {
                        throw new Exception("ScreenSize SpecificationTypeId not found.");
                    }

                    foreach (var screenSize in filterRequest.ScreenSizes)
                    {
                        switch (screenSize)
                        {
                            case "under5":
                                query = query.Where(p => p.ProductSpecifications
                                    .Any(ps => ps.SpecificationTypeId == screenSizeTypeId &&
                                               double.TryParse(ps.Value, out var size) && size < 5.0));
                                break;
                            case "above6":
                                query = query.Where(p => p.ProductSpecifications
                                    .Any(ps => ps.SpecificationTypeId == screenSizeTypeId &&
                                               double.TryParse(ps.Value, out var size) && size > 6.0));
                                break;
                        }
                    }
                }

                // Lọc theo Bộ nhớ trong
                if (filterRequest.InternalMemory != null && filterRequest.InternalMemory.Any())
                {
                    var storageTypeId = await _unitOfWork.GenericRepository<SpecificationType>()
                                                         .GetQuery()
                                                         .Where(s => s.Name == "Storage")
                                                         .Select(s => s.SpecificationTypeId)
                                                         .FirstOrDefaultAsync();
                    if (storageTypeId == 0)
                    {
                        throw new Exception("Storage SpecificationTypeId not found.");
                    }

                    foreach (var internalMemory in filterRequest.InternalMemory)
                    {
                        switch (internalMemory)
                        {
                            case "under32":
                                query = query.Where(p => p.ProductSpecifications
                                    .Any(ps => ps.SpecificationTypeId == storageTypeId &&
                                               int.TryParse(ps.Value, out var size) && size <= 32));
                                break;
                            case "64and128":
                                query = query.Where(p => p.ProductSpecifications
                                    .Any(ps => ps.SpecificationTypeId == storageTypeId &&
                                               int.TryParse(ps.Value, out var size) && (size == 64 || size == 128)));
                                break;
                            case "256and512":
                                query = query.Where(p => p.ProductSpecifications
                                    .Any(ps => ps.SpecificationTypeId == storageTypeId &&
                                               int.TryParse(ps.Value, out var size) && (size == 256 || size == 512)));
                                break;
                            case "above512":
                                query = query.Where(p => p.ProductSpecifications
                                    .Any(ps => ps.SpecificationTypeId == storageTypeId &&
                                               int.TryParse(ps.Value, out var size) && size > 512));
                                break;
                        }
                    }
                }

                // Sắp xếp
                switch (filterRequest.Sort)
                {
                    case "banchay":
                        query = query.OrderByDescending(p => p.OrderItems
                            .Where(oi => oi.Order.OrderStatus == "done" && oi.ProductId == p.ProductId)
                            .Sum(oi => oi.Quantity));
                        break;
                    case "giathap":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "giacao":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                }

                // Chuyển sang ViewModel và trả về
                return query.Select(p => new ProductVm
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    OldPrice = p.OldPrice,
                    StockQuantity = p.StockQuantity,
                    BrandId = p.BrandId,
                    ImageUrl = p.ImageUrl,
                    Manufacturer = p.Manufacturer,
                    IsActive = p.IsActive,
                    Color = p.Color,
                    Discount = p.Discount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                }).ToList();
            }
            catch (Exception ex)
            {
                // Ghi log lỗi hoặc xử lý tùy vào yêu cầu dự án
                Console.WriteLine($"An error occurred: {ex.Message}");
                return Enumerable.Empty<ProductVm>();
            }
        }
    }
}
