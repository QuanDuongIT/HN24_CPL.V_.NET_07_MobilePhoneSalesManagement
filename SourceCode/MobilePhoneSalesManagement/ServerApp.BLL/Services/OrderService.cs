using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services
{

    public class IOrderService : BaseService<Order>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Order> _orderRepository;

        public IOrderService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = unitOfWork.GenericRepository<Order>();
        }

        public async Task<int> AddAsync(Order entity)
        {
            await _orderRepository.AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }
        public async Task<bool> ProcessPaymentAsync(int orderId, PaymentVm paymentVm)
        {
            // Kiểm tra thông tin thanh toán và xử lý
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                // Thực hiện xử lý thanh toán (có thể kết nối với một hệ thống thanh toán bên ngoài)
                if (paymentVm.PaymentMethod == "Credit Card")
                {
                    // Xử lý thanh toán qua thẻ tín dụng
                }
                else if (paymentVm.PaymentMethod == "PayPal")
                {
                    // Xử lý thanh toán qua PayPal
                }

                // Cập nhật trạng thái thanh toán của đơn hàng trong hệ thống
                order.PaymentStatus = "Paid";  // Giả sử có thuộc tính PaymentStatus
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }

            return false;
        }
        public async Task<bool> CompleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                // Đảm bảo rằng đơn hàng có trạng thái "Đã thanh toán" trước khi hoàn tất
                if (order.PaymentStatus == "Paid")
                {
                    // Thay đổi trạng thái đơn hàng sang "Completed" (hoàn tất)
                    order.OrderStatus = "Completed";  // Giả sử có thuộc tính OrderStatus
                    await _orderRepository.UpdateAsync(order);
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }
        public async Task<bool> UpdateAsync(Order entity)
        {
            await _orderRepository.UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public bool Delete(int id)
        {
            var entity = _orderRepository.GetByIdAsync(id);
            if (entity != null)
            {
                _orderRepository.DeleteAsync(id);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAllAsync();
        }
    }

}
