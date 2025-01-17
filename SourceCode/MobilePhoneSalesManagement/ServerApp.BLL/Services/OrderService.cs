using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;

namespace ServerApp.BLL.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int userId, CustomerInfoVm customerInfo);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> ProcessPaymentAsync(int orderId, PaymentVm paymentVm);
        Task<bool> CompleteOrderAsync(int orderId);
    }


    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderItemService _orderItemService;

        public OrderService(IUnitOfWork unitOfWork, IOrderItemService orderItemService) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderItemService = orderItemService;
        }

        public async Task<Order> CreateOrderAsync(int userId, CustomerInfoVm customerInfo)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {

                var model = customerInfo.OrderInfo;
                var order = new Order
                {
                    UserId = userId,
                    TotalPrice = customerInfo.TotalAmount,
                    OrderStatus = customerInfo.OrderStatus ?? "pending",
                    Address = model.Address,
                    Country = model.Country,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Phone = model.Phone,
                    Postcode = model.Postcode,
                    Note = model.Note,
                    PaymentStatus = ""
                };

                await _unitOfWork.OrderRepository.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                foreach (var item in customerInfo.CartItems)
                {
                    await _orderItemService.AddOrderItemAsync(new OrderItem
                    {
                        OrderId = order.OrderId,
                        Price = item.Price,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                    });
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return order;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");
            return order;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");

            order.OrderStatus = status;
            order.UpdatedAt = DateTime.Now;

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> ProcessPaymentAsync(int orderId, PaymentVm paymentVm)
        {
            // Kiểm tra thông tin thanh toán và xử lý
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
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
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }

            return false;
        }
        public async Task<bool> CompleteOrderAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                // Đảm bảo rằng đơn hàng có trạng thái "Đã thanh toán" trước khi hoàn tất
                if (order.PaymentStatus == "Paid")
                {
                    // Thay đổi trạng thái đơn hàng sang "Completed" (hoàn tất)
                    order.OrderStatus = "Completed";  // Giả sử có thuộc tính OrderStatus
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }
    }
}
