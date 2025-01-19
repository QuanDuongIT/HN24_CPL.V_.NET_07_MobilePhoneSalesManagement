using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
        Task<PagedResult<BrandVm>> GetAllBrandAsync(int? pageNumber, int? pageSize, Expression<Func<Brand, bool>>? filter = null,
      string sortField = "updatedDate", bool orderBy = true);
        Task<PagedResult<BrandVm>> GetAllBrandAsync(int? pageNumber, int? pageSize, bool filter = true,
            string sortField = "updatedDate", bool orderBy = true);
        Task<PagedResult<BrandVm>> GetAllBrandAsync(int? pageNumber, int? pageSize, string search = "",
            string sortField = "updatedDate", bool orderBy = true);
    }
}
