using ServerApp.BLL.Services.Base;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services
{
    public class BrandService : BaseService<Brand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Brand> _brandRepository;

        public BrandService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _brandRepository = unitOfWork.GenericRepository<Brand>();
        }

        public async Task<int> AddAsync(Brand entity)
        {
            await _brandRepository.AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Brand entity)
        {
            _brandRepository.UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public bool Delete(int id)
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

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _brandRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _brandRepository.GetAllAsync();
        }
    }

}
