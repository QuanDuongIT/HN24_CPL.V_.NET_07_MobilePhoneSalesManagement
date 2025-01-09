using Microsoft.EntityFrameworkCore;
using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;

namespace ServerApp.BLL.Services
{
    public interface IUserService : IBaseService<User>
    {
        Task<int> AddUserAsync(UserVm userVm);
        Task<bool> UpdateUserAsync(int id, UserVm userVm);

        Task<bool> DeleteByIdAsync(int id);

        Task<UserVm?> GetByUserIdAsync(int id);

        Task<IEnumerable<UserVm>> GetAllUserAsync();
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserVm>> FilterUsersByLastActiveAsync(int days);
    }
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> AddUserAsync(UserVm userVm)
        {
            var user = new User()
            {
                UserName = userVm.UserName,
                Email = userVm.Email,
                PasswordHash = userVm.PasswordHash,
                Status = userVm.Status,
                Role = userVm.Role,
            };
            await AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return user.UserId;
        }

        public async Task<bool> UpdateUserAsync(int id, UserVm userVm)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            user.UserName = userVm.UserName;
            user.Email = userVm.Email;
            user.PasswordHash = userVm.PasswordHash;
            user.Status = userVm.Status;
            user.Role = userVm.Role;
            user.LastOnlineAt = userVm.LastOnlineAt;

            await UpdateAsync(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<UserVm?> GetByUserIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(
                filter: u => u.UserId == id,
                include: query => query.Include(u => u.UserDetails)
            );
            if (user == null)
            {
                return null;
            }

            // Chuyển đổi đối tượng User thành UserVm
            var result = new UserVm
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                PasswordHash = user.PasswordHash,
                Status = user.Status,
                LastOnlineAt = user.LastOnlineAt,
                FullName = user.UserDetails.FullName,
                DateOfBirth = user.UserDetails.DateOfBirth,
                Gender = user.UserDetails.Gender,
                Address = user.UserDetails.Address,
                PhoneNumber = user.UserDetails.PhoneNumber
            };

            return result;
        }

        public async Task<IEnumerable<UserVm>> GetAllUserAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync(
                filter: null,
                include: query => query.Include(u => u.UserDetails)
            );
            var result = users.Select(user => new UserVm
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                PasswordHash = user.PasswordHash,
                Status = user.Status,
                LastOnlineAt = user.LastOnlineAt,
                FullName = user.UserDetails.FullName,
                DateOfBirth = user.UserDetails.DateOfBirth,
                Gender = user.UserDetails.Gender,
                Address = user.UserDetails.Address,
                PhoneNumber = user.UserDetails.PhoneNumber
            });

            return result;
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // Sử dụng UserRepository từ UnitOfWork để tìm kiếm người dùng theo email
            return await _unitOfWork.UserRepository
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<UserVm>> FilterUsersByLastActiveAsync(int days)
        {
            // Xác định thời điểm cần so sánh
            var cutoffDate = DateTime.Now.AddDays(-days);

            var filteredUsers = await _unitOfWork.UserRepository.GetAllAsync(
                filter: user => user.LastOnlineAt >= cutoffDate,
                include: query => query.Include(u => u.UserDetails)
            );
            var result = filteredUsers.Select(user => new UserVm
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                PasswordHash = user.PasswordHash,
                Status = user.Status,
                LastOnlineAt = user.LastOnlineAt,
                FullName = user.UserDetails.FullName,
                DateOfBirth = user.UserDetails.DateOfBirth,
                Gender = user.UserDetails.Gender,
                Address = user.UserDetails.Address,
                PhoneNumber = user.UserDetails.PhoneNumber
            });

            return result;
        }


    }

}
