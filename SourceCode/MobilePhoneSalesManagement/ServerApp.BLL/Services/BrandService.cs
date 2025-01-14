﻿using Microsoft.EntityFrameworkCore;
using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace ServerApp.BLL.Services
{
    public interface IBrandService : IBaseService<Brand>
    {
        Task<BrandVm> AddBrandAsync(InputBrandVm brandVm);
        Task<BrandVm> UpdateBrandAsync(int id, InputBrandVm brandVm);

        Task<BrandVm> DeleteBrandAsync(int id);

        Task<BrandVm?> GetByBrandIdAsync(int id);

        Task<IEnumerable<BrandVm>> GetAllBrandAsync();
        Task<int> UpdateBrandAsync(Brand brand);
        Task<PagedResult<BrandVm>> GetAllBrandAsync(int? pageNumber, int? pageSize);
        Task<PagedResult<BrandVm>> GetAllBrandAsync(int? pageNumber, int? pageSize, bool filter = true);
        Task<PagedResult<BrandVm>> GetAllBrandAsync(int? pageNumber, int? pageSize, string search = "");
    }
    public class BrandService : BaseService<Brand>, IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BrandVm>> GetAllBrandAsync()
        {
            var brands = await GetAllAsync(
                includesProperties: "Products"
            );

            var BrandViewModels = brands.Select(brand => new BrandVm
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                ImageUrl=brand.ImageUrl,
                IsActive = brand.IsActive,
                ProductCount=brand.Products.Count,
                CreatedAt= brand.CreatedAt,
                UpdatedAt= brand.UpdatedAt
            });

            return BrandViewModels;
        }
        public async Task<PagedResult<BrandVm>> GetAllBrandAsync(int? pageNumber, int? pageSize)
        {
            int currentPage = pageNumber ?? 1; 
            int currentPageSize = pageSize ?? 10;

            var query = await GetAllAsync(
                includesProperties: "Products"
            );

            var totalCount = query.Count();
            var paginatedBrands = query
                .OrderBy(u => u.BrandId)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();
            var brandVms = paginatedBrands.Select(brand => new BrandVm
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                ImageUrl = brand.ImageUrl,
                IsActive = brand.IsActive,
                ProductCount = brand.Products?.Count ?? 0, 
                CreatedAt = brand.CreatedAt,
                UpdatedAt = brand.UpdatedAt
            });
           

            return new PagedResult<BrandVm>
            {
                CurrentPage = currentPage,
                PageSize = currentPageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / currentPageSize),
                Items = brandVms
            };
        }


        public async Task<BrandVm?> GetByBrandIdAsync(int id)
        {
            var brand = await GetByIdAsync(id);
            if (brand == null)
            {
                throw new ExceptionNotFound("Brand not found");
            }
            var brandVm = new BrandVm
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                ImageUrl=brand.ImageUrl,
                IsActive = brand.IsActive,
                ProductCount = brand.Products?.Count ?? 0, 
                CreatedAt = brand.CreatedAt,
                UpdatedAt = brand.UpdatedAt
            };

            return brandVm;

        }
        
        public async Task<PagedResult<BrandVm>> GetAllBrandAsync(int? pageNumber, int? pageSize,bool filter= true)
        {
            int currentPage = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 10;

            var query = await GetAllAsync(b=>
            b.IsActive== filter&&b.BrandId!=0,
                includesProperties: "Products"
            );

            var totalCount = query.Count();
            var paginatedBrands = query
                .OrderBy(u => u.BrandId)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();
            var brandVms = paginatedBrands.Select(brand => new BrandVm
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                ImageUrl = brand.ImageUrl,
                IsActive = brand.IsActive,
                ProductCount = brand.Products?.Count ?? 0, 
                CreatedAt = brand.CreatedAt,
                UpdatedAt = brand.UpdatedAt
            });


            return new PagedResult<BrandVm>
            {
                CurrentPage = currentPage,
                PageSize = currentPageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / currentPageSize),
                Items = brandVms
            };
        }
        public async Task<PagedResult<BrandVm>> GetAllBrandAsync(int? pageNumber, int? pageSize, string search)
        {
            int currentPage = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 10;

            var query = await GetAllAsync(b =>
                b.Name.Contains(search),
                includesProperties: "Products"
            );

            var totalCount = query.Count();
            var paginatedBrands = query
                .OrderBy(u => u.BrandId)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();
            var brandVms = paginatedBrands.Select(brand => new BrandVm
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                ImageUrl = brand.ImageUrl,
                IsActive = brand.IsActive,
                ProductCount = brand.Products?.Count ?? 0, 
                CreatedAt = brand.CreatedAt,
                UpdatedAt = brand.UpdatedAt
            });


            return new PagedResult<BrandVm>
            {
                CurrentPage = currentPage,
                PageSize = currentPageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / currentPageSize),
                Items = brandVms
            };
        }
        public async Task<BrandVm> AddBrandAsync(InputBrandVm brandVm)
        {
            ValidateModelPropertiesWithAttribute(brandVm);
            
            var findBrand = await _unitOfWork.GenericRepository<Brand>().GetAsync(b =>
                b.Name == brandVm.Name
            );
            if (findBrand == null)
            {
                var brand = new Brand
                {
                    Name = brandVm.Name,
                    ImageUrl = brandVm.ImageUrl,
                    IsActive = brandVm.IsActive,
                };

                if (await AddAsync(brand) > 0)
                {
                    return new BrandVm
                    {
                        BrandId = brand.BrandId,
                        Name = brand.Name,
                        ImageUrl=brand.ImageUrl,
                        IsActive = brand.IsActive
                    };
                }
                throw new ArgumentException("Failed to update brand");
            }
            throw new ExceptionBusinessLogic("Brand name is already in use.");

        }

        public async Task<BrandVm> UpdateBrandAsync(int id, InputBrandVm brandVm)
        {
            ValidateModelPropertiesWithAttribute(brandVm);
            //Tìm brand
            var brand = await _unitOfWork.GenericRepository<Brand>().GetByIdAsync(id);
            if (brand == null)
            {
                throw new ExceptionNotFound("Brand not found");
            }

            //Tìm brand có tên trùng với dữ liệu nhập vào (trừ brand tìm được phía trên)
            var findBrand = await _unitOfWork.GenericRepository<Brand>().GetAsync(b =>
                b.BrandId != id &&
                b.Name == brandVm.Name
             );
            if (findBrand != null)
            {
                throw new ExceptionBusinessLogic("Brand name is already in use.");
            }
            brand.Name = brandVm.Name;
            brand.IsActive = brandVm.IsActive;
            brand.ImageUrl = brandVm.ImageUrl;
            brand.UpdatedAt = DateTime.Now;
            var result = await _unitOfWork.GenericRepository<Brand>().ModifyAsync(brand);

            if (result <= 0)
            {
                throw new ArgumentException("Failed to update brand");
            }

            return new BrandVm
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                ImageUrl=brand.ImageUrl,
                ProductCount = brand.Products?.Count ?? 0, 
                IsActive = brand.IsActive,
                CreatedAt = brand.CreatedAt
            };
            
        }
        public async Task<int> UpdateBrandAsync(Brand brand)
        {
            // Cập nhật thông tin Brand
            brand.Name = brand.Name;
            brand.ImageUrl = brand.ImageUrl;
            brand.IsActive = brand.IsActive;
            brand.UpdatedAt = DateTime.Now;

            // Cập nhật Brand vào cơ sở dữ liệu
            return await _unitOfWork.GenericRepository<Brand>().ModifyAsync(brand);
        }
        public async Task<BrandVm> DeleteBrandAsync(int id)
        {
            var brand = await _unitOfWork.GenericRepository<Brand>().GetByIdAsync(id);

            if (brand == null)
            {
                throw new ExceptionNotFound("Brand not found");
            }
            var isdelete = 0;
            if (brand.IsActive == true)
            {
                brand.IsActive = false;
                isdelete = await UpdateBrandAsync(brand);
            }
            else
            {
                _unitOfWork.GenericRepository<Brand>().Delete(id);
                isdelete = _unitOfWork.SaveChanges();
            }

            if (isdelete > 0)
            {
                return new BrandVm
                {
                    BrandId = brand.BrandId,
                    Name = brand.Name,
                    ProductCount = brand.Products?.Count ?? 0, 
                    ImageUrl = brand.ImageUrl,
                    IsActive = brand.IsActive
                };
            }
            throw new ArgumentException("Failed to delete brand");
        }

    }

}
