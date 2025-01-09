using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;

namespace ServerApp.BLL.Services
{
    public interface IProductService : IBaseService<Product>
    {
        Task<IEnumerable<ProductVm>> GetAllProductAsync();
    }
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<ProductVm>> GetAllProductAsync()
        {
            var resuilt= await GetAllAsync(
                    includesProperties: "ProductSpecifications,ProductSpecifications.SpecificationType"
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
                Brand = product.Brand, // Bao gồm dữ liệu liên kết Brand
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
            });

            return productViewModels;
        }
    }
}
