using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.BLL.Services.Base
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<T> _repository;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<T>();
        }

        // Thêm một thực thể vào cơ sở dữ liệu (Asynchronous)
        public async Task<int> AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        // Cập nhật thực thể trong cơ sở dữ liệu (Asynchronous)
        public async Task<int> UpdateAsync(T entity)
        {
            _repository.UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        // Xóa thực thể dựa trên ID (Asynchronous)
        public async Task<int> DeleteAsync(int id)
        {
            _repository.DeleteAsync(id);
            return await _unitOfWork.SaveChangesAsync();
        }

        // Lấy thực thể theo ID (Asynchronous)
        public async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // Lấy tất cả thực thể (Asynchronous)
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
    }

}
