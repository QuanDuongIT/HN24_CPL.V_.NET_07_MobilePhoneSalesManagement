using Microsoft.EntityFrameworkCore;
using ServerApp.DAL.Models;

namespace ServerApp.DAL.Data
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

        // DbSet for each table
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Specification> Specifications { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // User và UserDetails: Quan hệ 1-1
        //    modelBuilder.Entity<User>()
        //        .HasOne(u => u.UserDetails)
        //        .WithOne(ud => ud.User)
        //        .HasForeignKey<UserDetails>(ud => ud.UserId)
        //        .OnDelete(DeleteBehavior.Cascade); // Xóa User sẽ xóa UserDetails

        //    // Product và Brand: Quan hệ nhiều-1
        //    modelBuilder.Entity<Product>()
        //        .HasOne(p => p.Brand)
        //        .WithMany(b => b.Products)
        //        .HasForeignKey(p => p.BrandId)
        //        .OnDelete(DeleteBehavior.SetNull); // Xóa Brand không xóa Product

        //    // Cấu hình quan hệ cho Cart
        //    modelBuilder.Entity<Cart>()
        //        .HasOne<User>() // Một Cart thuộc một User
        //        .WithMany() // Một User có thể có nhiều Cart
        //        .HasForeignKey(c => c.UserId) // Khóa ngoại
        //        .OnDelete(DeleteBehavior.Cascade); // Xóa User sẽ xóa Cart

        //    // Xóa cấu hình thừa hoặc xung đột
        //    modelBuilder.Entity<Cart>()
        //        .HasOne<User>() // Một Cart thuộc một User khác (nếu có)
        //        .WithMany()
        //        .HasForeignKey(c => c.UserId) // Khóa ngoại thừa
        //        .OnDelete(DeleteBehavior.Restrict);

        //    modelBuilder.Entity<Cart>()
        //        .HasOne<Product>() // Liên kết với Product
        //        .WithMany() // Một Product có thể xuất hiện trong nhiều Cart
        //        .HasForeignKey(c => c.ProductId) // Khóa ngoại là ProductId
        //        .OnDelete(DeleteBehavior.Restrict); // Không xóa Cart khi Product bị xóa

        //    // Quan hệ Order và User: Quan hệ nhiều-1
        //    modelBuilder.Entity<Order>()
        //        .HasOne<User>()
        //        .WithMany()
        //        .HasForeignKey(o => o.UserId)
        //        .OnDelete(DeleteBehavior.Restrict); // Tránh multiple cascade paths

        //    // OrderItem và Order: Quan hệ nhiều-1
        //    modelBuilder.Entity<OrderItem>()
        //        .HasOne<Order>()
        //        .WithMany()
        //        .HasForeignKey(oi => oi.OrderId)
        //        .OnDelete(DeleteBehavior.Restrict); // Tránh multiple cascade paths

        //    // OrderItem và Product: Quan hệ nhiều-1
        //    modelBuilder.Entity<OrderItem>()
        //        .HasOne<Product>()
        //        .WithMany()
        //        .HasForeignKey(oi => oi.ProductId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // WishList và User: Quan hệ nhiều-1
        //    modelBuilder.Entity<WishList>()
        //        .HasOne<User>()
        //        .WithMany()
        //        .HasForeignKey(wl => wl.UserId)
        //        .OnDelete(DeleteBehavior.Restrict); // Tránh multiple cascade paths

        //    // WishList và Product: Quan hệ nhiều-1
        //    modelBuilder.Entity<WishList>()
        //        .HasOne<Product>()
        //        .WithMany()
        //        .HasForeignKey(wl => wl.ProductId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.Entity<WishList>()
        //        .HasOne<Product>()
        //        .WithMany()
        //        .HasForeignKey(r => r.ProductId) // Khóa ngoại phụ
        //        .OnDelete(DeleteBehavior.Restrict);


        //    // Review và User: Quan hệ nhiều-1
        //    modelBuilder.Entity<Review>()
        //        .HasOne<User>()
        //        .WithMany()
        //        .HasForeignKey(r => r.UserId)
        //        .OnDelete(DeleteBehavior.Restrict); // Tránh multiple cascade paths

        //    // Review và Product: Quan hệ nhiều-1
        //    modelBuilder.Entity<Review>()
        //        .HasOne<Product>() // Một Review thuộc một Product
        //        .WithMany() // Một Product có nhiều Review
        //        .HasForeignKey(r => r.ProductId) // Khóa ngoại chính
        //        .OnDelete(DeleteBehavior.Cascade); // Xóa Product sẽ xóa Review

        //    // Nếu `ProductId1` là cột cần thiết
        //    modelBuilder.Entity<Review>()
        //        .HasOne<Product>() // Một Review có thể liên quan đến Product khác (nếu cần)
        //        .WithMany()
        //        .HasForeignKey(r => r.ProductId) // Khóa ngoại phụ
        //        .OnDelete(DeleteBehavior.Restrict);
        //}

    }
}
