﻿using ServerApp.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.BLL.Services.ViewModels
{
    public class AddProductSpecificationVm
    {
        public int ProductId { get; set; }
        public int SpecificationTypeId { get; set; }
        public string Value { get; set; } // Giá trị thông số (vd: "Đỏ", "15cm")

        // Navigation properties
        //public virtual Product Product { get; set; }
        //public virtual SpecificationType SpecificationType { get; set; }
    }
    public class ProductSpecificationVm
    {        
        public int ProductId { get; set; } 
        public int SpecificationTypeId { get; set; } 
        public string Value { get; set; } // Giá trị thông số (vd: "Đỏ", "15cm")

        // Navigation properties
        //public virtual Product Product { get; set; }
        public virtual SpecificationTypeVm SpecificationType { get; set; }
    }
}
