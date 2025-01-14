﻿using ServerApp.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.BLL.Services.ViewModels
{

    public class InputProductVm
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        [SkipValidation]
        public decimal OldPrice { get; set; }
        public int StockQuantity { get; set; } = 0;
        public int? BrandId { get; set; }

        [SkipValidation]
        public string ImageUrl { get; set; }
        public string Manufacturer { get; set; }
        public bool IsActive { get; set; } = true;
        public string Color { get; set; }
        [SkipValidation]
        public int Discount { get; set; } = 0;

        // Navigation properties
        //public virtual InputBrandVm Brand { get; set; }
        //public virtual ICollection<Cart> Carts { get; set; }
        //public virtual ICollection<OrderItem> OrderItems { get; set; }
        //public virtual ICollection<WishList> WishLists { get; set; }
        //public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<InputProductSpecificationVm> ProductSpecifications { get; set; }
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
        public virtual BrandVm? Brand { get; set; }
        //public virtual ICollection<Cart> Carts { get; set; }
        //public virtual ICollection<OrderItem> OrderItems { get; set; }
        //public virtual ICollection<WishList> WishLists { get; set; }
        //public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<ProductSpecificationVm> ProductSpecifications { get; set; }
    }
}
