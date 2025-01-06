using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.InterfaceServices;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<User> _userRepository;

        public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userRepository = unitOfWork.GenericRepository<User>();
        }

        public async Task<int> AddAsync(User entity)
        {
            await _userRepository.AddAsync(entity);
            var entityEntry = _unitOfWork.Context.Entry(entity);
            Console.WriteLine($"Entity state: {entityEntry.State}");
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(User entity)
        {
            _userRepository.UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity = _userRepository.GetByIdAsync(id);
            if (entity != null)
            {
                _userRepository.DeleteAsync(id);
                return _unitOfWork.SaveChanges();
            }
            return 0;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }
    }

}
