namespace ServerApp.DAL.Models
{
    public class ProductSpecification
    {
        public int ProductSpecificationId { get; set; } // Primary Key
        public int ProductId { get; set; } // FK to Product
        public int SpecificationTypeId { get; set; } // FK to SpecificationType
        public string Value { get; set; } // Giá trị thông số (vd: "Đỏ", "15cm")

        // Navigation properties
        public virtual Product Product { get; set; }
        public virtual SpecificationType SpecificationType { get; set; }
    }

}
