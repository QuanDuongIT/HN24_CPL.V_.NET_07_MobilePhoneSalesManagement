using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services
{
    public interface IBrandService : IBaseService<Brand>
    {
        Task<int> AddBrandAsync(BrandVm brandVm);
        Task<bool> UpdateBrandAsync(int id, BrandVm brandVm);

        bool DeleteByIdAsync(int id);

        Task<Brand?> GetByBrandIdAsync(int id);

        Task<IEnumerable<Brand>> GetAllBrandAsync();
    }
    public class BrandService : BaseService<Brand>, IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Brand> _brandRepository;

        public BrandService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _brandRepository = unitOfWork.GenericRepository<Brand>();
        }

        public async Task<int> AddBrandAsync(BrandVm brandVm)
        {
            var Brand = new Brand
            {
                Name = brandVm.Name,
                IsActive = brandVm.IsActive,
            };

            await _brandRepository.AddAsync(Brand);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateBrandAsync(int id, BrandVm brandVm)
        {
            var Brand = await GetByIdAsync(id);
            if (Brand == null)
            {
                throw new ArgumentException("Brand not found.");
            }
            Brand.Name = brandVm.Name;
            Brand.IsActive = brandVm.IsActive;
            await _brandRepository.UpdateAsync(Brand);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public bool DeleteByIdAsync(int id)
        {
            var entity = _brandRepository.GetByIdAsync(id);
            if (entity != null)
            {
                _brandRepository.DeleteAsync(id);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<Brand?> GetByBrandIdAsync(int id)
        {
            return await _brandRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Brand>> GetAllBrandAsync()
        {
            return await _brandRepository.GetAllAsync();
        }

    }

}
