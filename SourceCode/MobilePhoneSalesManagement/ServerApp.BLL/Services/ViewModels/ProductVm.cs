using ServerApp.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.BLL.Services.ViewModels
{
    public class AddProductVm
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public int StockQuantity { get; set; } = 0;
        public int? BrandId { get; set; }
        public string ImageUrl { get; set; }
        public string Manufacturer { get; set; }
        public bool IsActive { get; set; } = true;
        public string Color { get; set; }
        public int Discount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Brand Brand { get; set; }
        //public virtual ICollection<Cart> Carts { get; set; }
        //public virtual ICollection<OrderItem> OrderItems { get; set; }
        //public virtual ICollection<WishList> WishLists { get; set; }
        //public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; }
    }
    public class ProductVm
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public int StockQuantity { get; set; } = 0;
        public int? BrandId { get; set; }
        public string ImageUrl { get; set; }
        public string Manufacturer { get; set; }
        public bool IsActive { get; set; } = true;
        public string Color { get; set; }
        public int Discount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Brand Brand { get; set; }
        //public virtual ICollection<Cart> Carts { get; set; }
        //public virtual ICollection<OrderItem> OrderItems { get; set; }
        //public virtual ICollection<WishList> WishLists { get; set; }
        //public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<ProductSpecificationVm> ProductSpecifications { get; set; }
    }
}
