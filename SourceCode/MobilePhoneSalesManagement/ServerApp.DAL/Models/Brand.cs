namespace ServerApp.DAL.Models
{
    public class Brand
    {
        public int BrandId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
