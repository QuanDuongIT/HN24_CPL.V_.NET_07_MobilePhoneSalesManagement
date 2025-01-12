using Microsoft.EntityFrameworkCore;
using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services;
public interface IProductService : IBaseService<Product>
    {
        Task<IEnumerable<ProductVm>> GetAllProductAsync();
        Task<IEnumerable<ProductVm>> FilterProductsAsync(FilterRequest filterRequest);
        Task<ProductVm> AddProductAsync(InputProductVm brandVm);
        Task<ProductVm> UpdateProductAsync(int id, InputProductVm brandVm);
        Task<ProductDetailVm> GetProductDetailsAsync(int id);
       Task<ProductVm> DeleteProductAsync(int id);

       Task<ProductVm?> GetByProductIdAsync(int id);
      
       Task<bool> AddProductToCartAsync(int productId, CartVm cartVm);
}
    public class OroductService : BaseService<Product>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpecificationTypeService _specificationTypeService;
    
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Brand> _brandRepository;


    public OroductService(IUnitOfWork unitOfWork, ISpecificationTypeService specificationTypeService) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _specificationTypeService = specificationTypeService;
            _brandRepository = unitOfWork.GenericRepository<Brand>();
    }

        public async Task<ProductVm> AddProductAsync(InputProductVm productVm)
        {
            ValidateModelPropertiesWithAttribute(productVm);

            var findProduct = await _unitOfWork.GenericRepository<Product>().GetAsync(p =>
                p.Name == productVm.Name
            );

            if (findProduct == null)
            {
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
                    Color = productVm.Color

                };


                if (await AddAsync(product) > 0)
                {
                    // Tạo ProductSpecifications cho sản phẩm
                    foreach (var spec in productVm.ProductSpecifications)
                    {
                        var specificationType = await _specificationTypeService
                            .GetBySpecificationTypeIdAsync(spec.SpecificationTypeId);


                        // Kiểm tra xem SpecificationType đã tồn tại chưa, nếu chưa thì tạo mới
                        var specificationTypeVm = new InputSpecificationTypeVm
                        {
                            Name = spec.SpecificationType.Name
                        };

                        if (specificationType != null)
                        {
                            specificationType=await _specificationTypeService
                                .UpdateSpecificationTypeAsync(spec.SpecificationTypeId, specificationTypeVm);
                        }
                        else
                        {
                            specificationType = await _specificationTypeService
                                .AddSpecificationTypeAsync(specificationTypeVm);
                        }

                        // Tạo ProductSpecification
                        var productSpecification = new ProductSpecification
                        {
                            ProductId = product.ProductId,
                            SpecificationTypeId = specificationType.SpecificationTypeId,
                            Value = spec.Value
                        };

                        // Thêm ProductSpecification vào cơ sở dữ liệu
                        await _unitOfWork.GenericRepository<ProductSpecification>().AddAsync(productSpecification);
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.SaveChangesAsync();

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
                        CreatedAt = product.CreatedAt,
                        UpdatedAt = product.UpdatedAt
                    };
                }
                throw new ArgumentException("Failed to add product");
            }
            throw new ExceptionBusinessLogic("Product name is already in use.");
        }


        public async Task<ProductVm> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.GenericRepository<Product>().GetByIdAsync(id);

            if (product == null)
            {
                throw new ExceptionNotFound("Product not found");
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _unitOfWork.GenericRepository<Product>().Delete(id);

            if (_unitOfWork.SaveChanges() > 0)
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
                    Discount = 0, // Gán mặc định hoặc tính toán tùy theo logic
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                };
            }

            // Nếu lưu thất bại
            throw new ArgumentException("Failed to delete product");

        }

        public async Task<IEnumerable<ProductVm>> GetAllProductAsync()
        {
            var resuilt= await GetAllAsync(
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
                Discount = 0, // Gán mặc định hoặc tính toán tùy theo logic
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                Brand = new BrandVm
                {
                    BrandId= product.Brand.BrandId,
                    Name = product.Brand.Name,
                    IsActive=product.Brand.IsActive,
                    CreatedAt=product.CreatedAt,
                    UpdatedAt=product.Brand.UpdatedAt
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

        public async Task<ProductVm?> GetByProductIdAsync(int id)
        {
            var product = await GetOneAsync(p=>
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
                Discount = 0, // Gán mặc định hoặc tính toán tùy theo logic
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                Brand = new BrandVm
                {
                    BrandId = product.Brand.BrandId,
                    Name = product.Brand.Name,
                    IsActive = product.Brand.IsActive,
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

        public Task<ProductVm> UpdateProductAsync(int id, InputProductVm brandVm)
        {
            throw new NotImplementedException();
        }
    public async Task<ProductDetailVm> GetProductDetailAsync(int productId)
    {
        // Lấy sản phẩm từ database
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            return null;  // Trả về null nếu không tìm thấy sản phẩm
        }

        // Lấy thông tin danh mục và thương hiệu (nếu cần)
       
        var brand = await _brandRepository.GetByIdAsync(product.BrandId);

        // Tạo đối tượng ProductDetailVm để trả về
        var productDetailVm = new ProductDetailVm
        {
            ProductId = product.ProductId,
            ProductName = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
    
               // Lấy tên thương hiệu từ Brand
            CreatedDate = product.CreatedDate,
            UpdatedDate = product.UpdatedDate
        };

        return productDetailVm;
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

