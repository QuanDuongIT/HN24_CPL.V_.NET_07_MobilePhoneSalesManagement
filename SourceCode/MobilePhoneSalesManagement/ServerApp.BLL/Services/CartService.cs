using ServerApp.BLL.Services.Base;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services
{
    public class CartService : BaseService<Cart>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Cart> _cartRepository;

        public CartService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _cartRepository = unitOfWork.GenericRepository<Cart>();
        }

        public async Task<int> AddAsync(Cart entity)
        {
            await _cartRepository.AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Cart entity)
        {
            _cartRepository.UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public bool Delete(int id)
        {
            var entity = _cartRepository.GetByIdAsync(id);
            if (entity != null)
            {
                _cartRepository.DeleteAsync(id);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<Cart?> GetByIdAsync(int id)
        {
            return await _cartRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _cartRepository.GetAllAsync();
        }
    }

}
