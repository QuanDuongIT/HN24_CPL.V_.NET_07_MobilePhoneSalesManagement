using ServerApp.BLL.Services.Base;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services
{
    public class OrderItemService : BaseService<OrderItem>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;

        public OrderItemService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderItemRepository = unitOfWork.GenericRepository<OrderItem>();
        }

        public async Task<int> AddAsync(OrderItem entity)
        {
            await _orderItemRepository.AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(OrderItem entity)
        {
            await _orderItemRepository.UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public bool Delete(int id)
        {
            var entity = _orderItemRepository.GetByIdAsync(id);
            if (entity != null)
            {
                _orderItemRepository.DeleteAsync(id);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<OrderItem?> GetByIdAsync(int id)
        {
            return await _orderItemRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            return await _orderItemRepository.GetAllAsync();
        }
    }

}
