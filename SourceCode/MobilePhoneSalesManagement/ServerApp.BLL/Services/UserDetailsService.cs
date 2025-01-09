using ServerApp.BLL.Services.Base;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services
{
    public class UserDetailsService : BaseService<UserDetails>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserDetails> _userDetailsRepository;

        public UserDetailsService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userDetailsRepository = unitOfWork.GenericRepository<UserDetails>();
        }

        public async Task<int> AddAsync(UserDetails entity)
        {
            await _userDetailsRepository.AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(UserDetails entity)
        {
            await _userDetailsRepository.UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public bool Delete(int id)
        {
            var entity = _userDetailsRepository.GetByIdAsync(id);
            if (entity != null)
            {
                _userDetailsRepository.DeleteAsync(id);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<UserDetails?> GetByIdAsync(int id)
        {
            return await _userDetailsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<UserDetails>> GetAllAsync()
        {
            return await _userDetailsRepository.GetAllAsync();
        }
    }

}
