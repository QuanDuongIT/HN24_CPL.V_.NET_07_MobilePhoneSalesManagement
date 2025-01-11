using Microsoft.EntityFrameworkCore;
using ServerApp.DAL.Data;
using ServerApp.DAL.Models;

namespace ServerApp.DAL.Seed
{
    public static class SeedData
    {
        public static async Task SeedAsync(ShopDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra bảng Brands trước khi thêm sản phẩm
                if (!context.Brands.Any())
                {
                    context.Brands.AddRange(
                        new Brand { 
                            Name = "Samsung",
                            ImageUrl = "http://example.com/image1.jpg",
                            IsActive = true 
                        },
                        new Brand { 
                            Name = "Apple",
                            ImageUrl = "http://example.com/image1.jpg",
                            IsActive = true 
                        }
                    );
                    await context.SaveChangesAsync();  // Đảm bảo đã lưu Brands trước khi thêm Products
                }

                // Thêm sản phẩm với BrandId hợp lệ
                if (!context.Products.Any())
                {
                    context.Products.AddRange(
                        new Product
                        {
                            Name = "Product 1",
                            Description = "Description for Product 1",
                            Price = 3000000,
                            OldPrice = 3500000,
                            StockQuantity = 50,
                            BrandId = 1,
                            ImageUrl = "http://example.com/image1.jpg",
                            Manufacturer = "Manufacturer 1",
                            IsActive = true,
                            Color = "Black",
                            Discount=10,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Product
                        {
                            Name = "Product 2",
                            Description = "Description for Product 2",
                            Price = 4500000,
                            OldPrice = 5000000,
                            StockQuantity = 30,
                            BrandId = 2,
                            ImageUrl = "http://example.com/image2.jpg",
                            Manufacturer = "Manufacturer 2",
                            IsActive = true,
                            Color = "White",
                            Discount=20,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Product
                        {
                            Name = "Product 3",
                            Description = "Description for Product 3",
                            Price = 6000000,
                            OldPrice = 6500000,
                            StockQuantity = 60,
                            BrandId = 1,
                            ImageUrl = "http://example.com/image3.jpg",
                            Manufacturer = "Manufacturer 3",
                            IsActive = false,
                            Color = "Blue",
                            Discount = 15,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Product
                        {
                            Name = "Product 4",
                            Description = "Description for Product 4",
                            Price = 5500000,
                            OldPrice = 6000000,
                            StockQuantity = 45,
                            BrandId = 2,
                            ImageUrl = "http://example.com/image4.jpg",
                            Manufacturer = "Manufacturer 4",
                            IsActive = false,
                            Color = "Red",
                            Discount = 10,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Product
                        {
                            Name = "Product 5",
                            Description = "Description for Product 5",
                            Price = 7000000,
                            OldPrice = 7500000,
                            StockQuantity = 25,
                            BrandId = 1,
                            ImageUrl = "http://example.com/image5.jpg",
                            Manufacturer = "Manufacturer 5",
                            IsActive = true,
                            Color = "Green",
                            Discount = 5,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Product
                        {
                            Name = "Product 6",
                            Description = "Description for Product 6",
                            Price = 8000000,
                            OldPrice = 8500000,
                            StockQuantity = 35,
                            BrandId = 2,
                            ImageUrl = "http://example.com/image6.jpg",
                            Manufacturer = "Manufacturer 6",
                            IsActive = true,
                            Color = "Yellow",
                            Discount = 25,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Product
                        {
                            Name = "Product 7",
                            Description = "Description for Product 7",
                            Price = 9000000,
                            OldPrice = 9500000,
                            StockQuantity = 40,
                            BrandId = 2,
                            ImageUrl = "http://example.com/image7.jpg",
                            Manufacturer = "Manufacturer 7",
                            IsActive = false,
                            Color = "Pink",
                            Discount = 30,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Product
                        {
                            Name = "Product 8",
                            Description = "Description for Product 8",
                            Price = 10000000,
                            OldPrice = 10500000,
                            StockQuantity = 50,
                            BrandId = 1,
                            ImageUrl = "http://example.com/image8.jpg",
                            Manufacturer = "Manufacturer 8",
                            IsActive = true,
                            Color = "Purple",
                            Discount = 12,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        }
                    );
                    await context.SaveChangesAsync();
                }

                // Seed SpecificationType
                if (!context.SpecificationTypes.Any())
                {
                    context.SpecificationTypes.AddRange(
                        new SpecificationType { Name = "Color" },
                        new SpecificationType { Name = "ScreenSize" },
                        new SpecificationType { Name = "RAM" },
                        new SpecificationType { Name = "Storage" }
                    );
                    await context.SaveChangesAsync();
                }

                // Seed ProductSpecifications
                if (!context.ProductSpecifications.Any())
                {
                    context.ProductSpecifications.AddRange(
                        new ProductSpecification { ProductId = 1, SpecificationTypeId = 1, Value = "Black" },
                        new ProductSpecification { ProductId = 1, SpecificationTypeId = 2, Value = "6,5" },
                        new ProductSpecification { ProductId = 2, SpecificationTypeId = 1, Value = "White" },
                        new ProductSpecification { ProductId = 2, SpecificationTypeId = 3, Value = "8GB" },
                        new ProductSpecification { ProductId = 2, SpecificationTypeId = 4, Value = "128" }
                    );
                    await context.SaveChangesAsync();
                }

                // Commit transaction nếu mọi thứ thành công
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Rollback transaction nếu xảy ra lỗi
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static void EnableIdentityInsert(ShopDbContext context, string tableName, bool enable)
        {
            var rawSql = enable
                ? $"SET IDENTITY_INSERT {tableName} ON;"
                : $"SET IDENTITY_INSERT {tableName} OFF;";
            context.Database.ExecuteSqlRaw(rawSql);
        }
    }
}
