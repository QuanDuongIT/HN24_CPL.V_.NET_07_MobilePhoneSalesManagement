namespace ServerApp.DAL.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; } = true;
        public string Role { get; set; } = "client";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual UserDetails? UserDetails { get; set; } // Không bắt buộc
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>(); // Danh sách rỗng
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
