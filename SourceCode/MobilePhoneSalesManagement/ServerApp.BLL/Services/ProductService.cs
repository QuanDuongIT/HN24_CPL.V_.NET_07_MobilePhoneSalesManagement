using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;

namespace ServerApp.BLL.Services
{
    public interface IProductService : IBaseService<Product>
    {
        Task<IEnumerable<ProductVm>> GetAllProductAsync();
        Task<ProductVm> AddProductAsync(InputProductVm brandVm);
        Task<ProductVm> UpdateProductAsync(int id, InputProductVm brandVm);

        Task<ProductVm> DeleteProductAsync(int id);

        Task<ProductVm?> GetByProductIdAsync(int id);
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
    }
}
