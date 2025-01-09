using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.InterfaceServices;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services
{
    public class CartService : BaseService<Cart>, ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Cart> _cartRepository;

        public CartService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _cartRepository = unitOfWork.GenericRepository<Cart>();
        }

        public async Task<int> AddCartAsync(CartVm cartVm)
        {

            var cart = new Cart()
            {
                UserId = cartVm.UserId,
                ProductId = cartVm.ProductId,
                Quantity = cartVm.Quantity,
                AddedAt = cartVm.AddedAt
            };
            await _cartRepository.AddAsync(cart);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateCartAsync(int id, CartVm cartVm)
        {
            var cart = await GetByIdAsync(id);
            cart.ProductId = cartVm.ProductId;
            cart.Quantity = cartVm.Quantity;
            cart.UserId = cartVm.UserId;
            cart.AddedAt = cartVm.AddedAt;
            await _cartRepository.UpdateAsync(cart);
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
