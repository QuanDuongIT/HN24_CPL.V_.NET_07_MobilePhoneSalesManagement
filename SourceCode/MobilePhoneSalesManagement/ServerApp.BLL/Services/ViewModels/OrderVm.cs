namespace ServerApp.BLL.Services.ViewModels
{
    public class OrderVm
    {
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderStatus { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Postcode { get; set; }
        public string Note { get; set; }
        public string PaymentStatus { get; set; }
        public List<OrderItemVm> OrderItems { get; set; }
    }

    public class OrderItemVm
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public ProductOrderVm? Product { get; set; }
    }
    public class CustomerInfoVm
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string Note { get; set; }
    }
    public class OrderAdminVm
    {
        public int OrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<OrderItemVm> CartItems { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? OrderStatus { get; set; }
        public int? CustomerId { get; set; }
        public CustomerInfoVm OrderInfo { get; set; }
    }
    public class ProductOrderVm
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
    }
}
