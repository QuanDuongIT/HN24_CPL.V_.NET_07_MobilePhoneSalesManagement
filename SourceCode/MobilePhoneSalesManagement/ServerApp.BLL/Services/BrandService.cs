using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services
{
    public interface IBrandService : IBaseService<Brand>
    {
        Task<ApiResponse<BrandVm>> AddBrandAsync(AddBrandVm brandVm);
        Task<ApiResponse<BrandVm>> UpdateBrandAsync(int id, AddBrandVm brandVm);

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
                            StatusCode=200,
                            Message=" Add success",
                            Data= new BrandVm
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
                StatusCode  =400,
                Message="Brand is empty"
            };
        }

        public async Task<ApiResponse<BrandVm>> UpdateBrandAsync(int id, AddBrandVm brandVm)
        {
            //// Lấy thương hiệu cần cập nhật
            //// sửa dụng luồng khác
            //var findBrand = await _unitOfWork.GenericRepository<Brand>().GetAsync(b =>
            //    b.Name == brandVm.Name
            //);
            //if(findBrand != null)
            //{
            //    // Trả về lỗi nếu không tìm thấy thương hiệu
            //    return new ApiResponse<BrandVm>
            //    {
            //        StatusCode = 404, // Mã trạng thái "Not Found"
            //        Message = "Brand name is empty",
            //        Data = null
            //    };
            //}
            var brand = await _unitOfWork.GenericRepository<Brand>().GetByIdAsync(id);
            
            if (brand == null)
            {
                // Trả về lỗi nếu không tìm thấy thương hiệu
                return new ApiResponse<BrandVm>
                {
                    StatusCode = 404, // Mã trạng thái "Not Found"
                    Message = "Brand not found",
                    Data = null
                };
            }

            // Cập nhật thông tin thương hiệu
            brand.Name = brandVm.Name;
            brand.IsActive = brandVm.IsActive;

            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                if (await UpdateAsync(brand) > 0)
                {
                    return new ApiResponse<BrandVm>
                    {
                        StatusCode = 200, // Thành công
                        Message = "Update success",
                        Data = new BrandVm
                        {
                            BrandId = brand.BrandId,
                            Name = brand.Name,
                            IsActive = brand.IsActive
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

        //public bool DeleteByIdAsync(int id)
        //{
        //    var entity = _brandRepository.GetByIdAsync(id);
        //    if (entity != null)
        //    {
        //        _brandRepository.DeleteAsync(id);
        //        _unitOfWork.SaveChanges();
        //        return true;
        //    }
        //    return false;
        //}

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
