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

        public CartService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            return await AddAsync(cart);
        }

        public async Task<bool> UpdateCartAsync(int id, CartVm cartVm)
        {
            var cart = await GetByIdAsync(id);
            cart.ProductId = cartVm.ProductId;
            cart.Quantity = cartVm.Quantity;
            cart.UserId = cartVm.UserId;
            cart.AddedAt = cartVm.AddedAt;
            return await UpdateAsync(cart) > 0;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            return await DeleteAsync(id) > 0;
        }

        public async Task<Cart?> GetByCartIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<IEnumerable<Cart>> GetAllCartAsync()
        {
            return await GetAllAsync();
        }
    }

}
