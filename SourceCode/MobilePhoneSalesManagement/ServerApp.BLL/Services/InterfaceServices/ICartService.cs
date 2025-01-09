using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.BLL.Services.InterfaceServices
{
    public interface ICartService : IBaseService<Cart>
    {

        Task<int> AddCartAsync(CartVm cartVm);

        Task<bool> UpdateCartAsync(int id, CartVm cartVm);

        Task<bool> DeleteByIdAsync(int id);

        Task<Cart?> GetByCartIdAsync(int id);

        Task<IEnumerable<Cart>> GetAllCartAsync();
    }
}
