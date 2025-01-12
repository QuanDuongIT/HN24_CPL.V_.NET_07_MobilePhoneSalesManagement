using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;

namespace ServerApp.BLL.Services
{
    public interface IBrandService : IBaseService<Brand>
    {
        Task<BrandVm> AddBrandAsync(InputBrandVm brandVm);
        Task<BrandVm> UpdateBrandAsync(int id, InputBrandVm brandVm);

        Task<BrandVm> DeleteBrandAsync(int id);

        Task<BrandVm?> GetByBrandIdAsync(int id);

        Task<IEnumerable<BrandVm>> GetAllBrandAsync();
        Task<int> UpdateBrandAsync(Brand brand);
    }
    public class BrandService : BaseService<Brand>, IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BrandVm>> GetAllBrandAsync()
        {
            var brands = await GetAllAsync(
                includesProperties: "Products"
            );

            var BrandViewModels = brands.Select(brand => new BrandVm
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                ImageUrl=brand.ImageUrl,
                IsActive = brand.IsActive,
                CreatedAt= brand.CreatedAt,
                UpdatedAt= brand.UpdatedAt
            });

            return BrandViewModels;
        }

        public async Task<BrandVm?> GetByBrandIdAsync(int id)
        {
            var brand = await GetByIdAsync(id);
            if (brand == null)
            {
                throw new ExceptionNotFound("Brand not found");
            }
            var brandVm = new BrandVm
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                ImageUrl=brand.ImageUrl,
                IsActive = brand.IsActive,
                CreatedAt = brand.CreatedAt,
                UpdatedAt = brand.UpdatedAt
            };

            return brandVm;

        }
        public async Task<BrandVm> AddBrandAsync(InputBrandVm brandVm)
        {
            ValidateModelPropertiesWithAttribute(brandVm);
            
            var findBrand = await _unitOfWork.GenericRepository<Brand>().GetAsync(b =>
                b.Name == brandVm.Name
            );
            if (findBrand == null)
            {
                var brand = new Brand
                {
                    Name = brandVm.Name,
                    ImageUrl = brandVm.ImageUrl,
                    IsActive = brandVm.IsActive,
                };

                if (await AddAsync(brand) > 0)
                {
                    return new BrandVm
                    {
                        BrandId = brand.BrandId,
                        Name = brand.Name,
                        ImageUrl=brand.ImageUrl,
                        IsActive = brand.IsActive
                    };
                }
                throw new ArgumentException("Failed to update brand");
            }
            throw new ExceptionBusinessLogic("Brand name is already in use.");

        }

        public async Task<BrandVm> UpdateBrandAsync(int id, InputBrandVm brandVm)
        {
            ValidateModelPropertiesWithAttribute(brandVm);
            //Tìm brand
            var brand = await _unitOfWork.GenericRepository<Brand>().GetByIdAsync(id);
            if (brand == null)
            {
                throw new ExceptionNotFound("Brand not found");
            }

            //Tìm brand có tên trùng với dữ liệu nhập vào (trừ brand tìm được phía trên)
            var findBrand = await _unitOfWork.GenericRepository<Brand>().GetAsync(b =>
                b.BrandId != id &&
                b.Name == brandVm.Name
             );
            if (findBrand != null)
            {
                throw new ExceptionBusinessLogic("Brand name is already in use.");
            }
            brand.Name = brandVm.Name;
            brand.IsActive = brandVm.IsActive;
            brand.ImageUrl = brandVm.ImageUrl;
            brand.UpdatedAt = DateTime.Now;
            var result = await _unitOfWork.GenericRepository<Brand>().ModifyAsync(brand);

            if (result <= 0)
            {
                throw new ArgumentException("Failed to update brand");
            }

            return new BrandVm
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                ImageUrl=brand.ImageUrl,
                IsActive = brand.IsActive,
                CreatedAt = brand.CreatedAt
            };
            
        }
        public async Task<int> UpdateBrandAsync(Brand brand)
        {
            // Cập nhật thông tin Brand
            brand.Name = brand.Name;
            brand.ImageUrl = brand.ImageUrl;
            brand.IsActive = brand.IsActive;
            brand.UpdatedAt = DateTime.Now;

            // Cập nhật Brand vào cơ sở dữ liệu
            return await _unitOfWork.GenericRepository<Brand>().ModifyAsync(brand);
        }
        public async Task<BrandVm> DeleteBrandAsync(int id)
        {
            var brand = await _unitOfWork.GenericRepository<Brand>().GetByIdAsync(id);

            if (brand == null)
            {
                throw new ExceptionNotFound("Brand not found");
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _unitOfWork.GenericRepository<Brand>().Delete(id);

            if (_unitOfWork.SaveChanges() > 0)
            {
                return new BrandVm
                {
                    BrandId = brand.BrandId,
                    Name = brand.Name,
                    ImageUrl=brand.ImageUrl,
                    IsActive = brand.IsActive
                };
            }

            // Nếu lưu thất bại
            throw new ArgumentException("Failed to delete brand");
            
        }

    }

}
