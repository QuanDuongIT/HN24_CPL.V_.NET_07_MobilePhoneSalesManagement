using ServerApp.BLL.Services.Base;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services
{
    public class WishListService : BaseService<WishList>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<WishList> _wishListRepository;

        public WishListService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _wishListRepository = unitOfWork.GenericRepository<WishList>();
        }

        public async Task<int> AddAsync(WishList entity)
        {
            await _wishListRepository.AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(WishList entity)
        {
            await _wishListRepository.UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public bool Delete(int id)
        {
            var entity = _wishListRepository.GetByIdAsync(id);
            if (entity != null)
            {
                _wishListRepository.DeleteAsync(id);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<WishList?> GetByIdAsync(int id)
        {
            return await _wishListRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<WishList>> GetAllAsync()
        {
            return await _wishListRepository.GetAllAsync();
        }
    }

}
