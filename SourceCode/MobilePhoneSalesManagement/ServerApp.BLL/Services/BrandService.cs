using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;

namespace ServerApp.BLL.Services
{
    public interface IBrandService : IBaseService<Brand>
    {
        Task<ApiResponse<BrandVm>> AddBrandAsync(AddBrandVm brandVm);
        Task<BrandVm> UpdateBrandAsync(int id, AddBrandVm brandVm);

        //bool DeleteByIdAsync(int id);

        Task<BrandVm?> GetByBrandIdAsync(int id);

        Task<IEnumerable<BrandVm>> GetAllBrandAsync();
    }
    public class BrandService : BaseService<Brand>, IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<BrandVm>> AddBrandAsync(AddBrandVm brandVm)
        {
            var findBrand = await _unitOfWork.GenericRepository<Brand>().GetAsync(b =>
                b.Name == brandVm.Name
            );
            if (findBrand == null)
            {
                var brand = new Brand
                {
                    Name = brandVm.Name,
                    IsActive = brandVm.IsActive,
                };

                if (await AddAsync(brand) > 0)
                {
                    return new ApiResponse<BrandVm>
                    {
                        StatusCode = 200,
                        Message = " Add success",
                        Data = new BrandVm
                        {
                            BrandId = brand.BrandId,
                            Name = brand.Name,
                            IsActive = brand.IsActive
                        }
                    };
                }
            }
            return new ApiResponse<BrandVm>
            {
                StatusCode = 400,
                Message = "Brand is empty"
            };
        }

        public async Task<BrandVm> UpdateBrandAsync(int id, AddBrandVm brandVm)
        {

            var brand = await _unitOfWork.GenericRepository<Brand>().GetByIdAsync(id);
            if (brand == null)
            {
                throw new ExceptionNotFound("Brand not found"); 
            }

            if (brand.Name == brandVm.Name)
            {
                throw new ExceptionBusinessLogic("Brand name is empty");
            }
            brand.Name = brandVm.Name;
            brand.IsActive = brandVm.IsActive;

            try
            {
                var result = await _unitOfWork.GenericRepository<Brand>().ModifyAsync(brand);

                if (result <= 0)
                {
                    throw new ArgumentException("Failed to update brand"); 
                }

                return new BrandVm
                {
                    BrandId = brand.BrandId,
                    Name = brand.Name,
                    IsActive = brand.IsActive
                };
            }
            catch (Exception ex)
            {
                // Nếu lỗi xảy ra, ném ngoại lệ
                throw new Exception($"Internal server error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<BrandVm>> DeleteByIdAsync(int id)
        {

            var entity = await _unitOfWork.GenericRepository<Brand>().GetByIdAsync(id);

            if (entity == null)
            {
                // Trả về lỗi nếu không tìm thấy thương hiệu
                return new ApiResponse<BrandVm>
                {
                    StatusCode = 404, // Mã trạng thái "Not Found"
                    Message = "Brand not found",
                    Data = null
                };
            }

            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                _unitOfWork.GenericRepository<Brand>().Delete(id);

                if (_unitOfWork.SaveChanges() > 0)
                {
                    return new ApiResponse<BrandVm>
                    {
                        StatusCode = 200, // Thành công
                        Message = "Update success",
                        Data = new BrandVm
                        {
                            BrandId = entity.BrandId,
                            Name = entity.Name,
                            IsActive = entity.IsActive
                        }
                    };
                }

                // Nếu lưu thất bại
                return new ApiResponse<BrandVm>
                {
                    StatusCode = 500, // Lỗi server
                    Message = "Failed to update brand",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi cập nhật
                return new ApiResponse<BrandVm>
                {
                    StatusCode = 500, // Lỗi server
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<BrandVm?> GetByBrandIdAsync(int id)
        {
            var brand = await GetByIdAsync(id);

            var BrandViewModel = new BrandVm
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
            };

            return BrandViewModel;
        }

        public async Task<IEnumerable<BrandVm>> GetAllBrandAsync()
        {
            var Brands = await GetAllAsync(
                    includesProperties: "Products"
                );

            var BrandViewModels = Brands.Select(c => new BrandVm
            {
                BrandId = c.BrandId,
                Name = c.Name,
                IsActive = c.IsActive
            });

            return BrandViewModels;
        }

    }

}
