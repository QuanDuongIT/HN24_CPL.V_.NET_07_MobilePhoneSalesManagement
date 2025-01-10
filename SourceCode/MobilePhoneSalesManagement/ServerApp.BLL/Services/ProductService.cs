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
}
