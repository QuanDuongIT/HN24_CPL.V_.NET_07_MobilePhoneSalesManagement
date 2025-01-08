namespace ServerApp.DAL.Models
{
    public class SpecificationType
    {
        public int SpecificationTypeId { get; set; } // Primary Key
        public string Name { get; set; } // Tên thông số (vd: Màu sắc, Kích thước)

        // Navigation properties
        public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; }
    }

}