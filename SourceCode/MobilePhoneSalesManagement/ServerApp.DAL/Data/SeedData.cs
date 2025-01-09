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
                        new Brand { Name = "TechCorp", IsActive = true },
                        new Brand { Name = "PhoneInc", IsActive = true }
                    );
                    await context.SaveChangesAsync();  // Đảm bảo đã lưu Brands trước khi thêm Products
                }

                // Thêm sản phẩm với BrandId hợp lệ
                if (!context.Products.Any())
                {
                    context.Products.AddRange(
                        new Product
                        {
                            Name = "Laptop",
                            Description = "High-end laptop",
                            Price = 1000m,
                            OldPrice = 1200m,
                            StockQuantity = 10,
                            BrandId = 1,  // Đảm bảo BrandId này tồn tại trong bảng Brands
                            ImageUrl = "laptop.jpg",
                            Manufacturer = "TechCorp",
                            IsActive = true,
                            Color = "Silver",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Product
                        {
                            Name = "Smartphone",
                            Description = "Latest smartphone",
                            Price = 800m,
                            OldPrice = 900m,
                            StockQuantity = 20,
                            BrandId = 2,  // Đảm bảo BrandId này tồn tại trong bảng Brands
                            ImageUrl = "smartphone.jpg",
                            Manufacturer = "PhoneInc",
                            IsActive = true,
                            Color = "Black",
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
                        new SpecificationType
                        {
                            Name = "Màn hình"
                        },
                        new SpecificationType
                        {
                            Name = "Bộ nhớ"
                        }
                    );
                    await context.SaveChangesAsync();
                }

                // Seed ProductSpecifications
                if (!context.ProductSpecifications.Any())
                {
                    context.ProductSpecifications.AddRange(
                        new ProductSpecification
                        {
                            ProductId = 1,
                            SpecificationTypeId = 1,
                            Value = "5.1 inch"
                        },
                        new ProductSpecification
                        {
                            ProductId = 1,
                            SpecificationTypeId = 2,
                            Value = "60GB"
                        }
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
