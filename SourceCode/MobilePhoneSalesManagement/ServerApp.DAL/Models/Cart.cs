namespace ServerApp.DAL.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime AddedAt { get; set; } = DateTime.Now;

        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}
