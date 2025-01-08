﻿using Microsoft.EntityFrameworkCore;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Repositories.Generic;
using System.Linq.Expressions;
using System.Linq;
using BusinessLogic;

namespace ServerApp.BLL.Services.Base
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> AddAsync(T entity)
        {
            await _unitOfWork.GenericRepository<T>().AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            await _unitOfWork.GenericRepository<T>().UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        // Xóa thực thể dựa trên ID (Asynchronous)
        public async Task<int> DeleteAsync(int id)
        {
            await _unitOfWork.GenericRepository<T>().DeleteAsync(id);
            return await _unitOfWork.SaveChangesAsync();
        }

        // Lấy thực thể theo ID (Asynchronous)
        public async Task<T> GetByIdAsync(int id)
        {
            return await _unitOfWork.GenericRepository<T>().GetByIdAsync(id);
        }

        // Lấy tất cả thực thể (Asynchronous)
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _unitOfWork.GenericRepository<T>().GetAllAsync();
        }
        
        public virtual int Add(T entity)
        {
            if (entity != null)
            {
                _unitOfWork.GenericRepository<T>().Add(entity);

                return _unitOfWork.SaveChanges();
            }

            throw new ArgumentNullException(nameof(entity));
        }


        public virtual int Delete(Guid id)
        {
            if (id != Guid.Empty)
            {
                _unitOfWork.GenericRepository<T>().Delete(id);

                return _unitOfWork.SaveChanges();
            }

            throw new ArgumentNullException(nameof(id));
        }
        public virtual int Delete(int id)
        {
            try
            {
                _unitOfWork.GenericRepository<T>().Delete(id);
                return _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(nameof(id));
            }
        }
        public virtual async Task<int> DeleteAsync(Guid id)
        {
            if (id != Guid.Empty)
            {
                _unitOfWork.GenericRepository<T>().Delete(id);

                return await _unitOfWork.SaveChangesAsync();
            }

            throw new ArgumentNullException(nameof(id));
        }
        public virtual async Task<bool> DeleteAsync(T entity)
        {
            if (entity != null)
            {
                _unitOfWork.GenericRepository<T>().Delete(entity);
                var changes = await _unitOfWork.SaveChangesAsync();
                return changes > 0; // Trả về true nếu có thay đổi trong DB, ngược lại false
            }

            throw new ArgumentNullException(nameof(entity), "The entity to delete cannot be null.");
        }
        public virtual IEnumerable<T> GetAll()
        {
            return _unitOfWork.GenericRepository<T>().GetAll();
        }


        public virtual T? GetById(Guid id)
        {
            return _unitOfWork.GenericRepository<T>().GetById(id);
        }
        public virtual T? GetById(int id)
        {
            return _unitOfWork.GenericRepository<T>().GetById(id);
        }
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.GenericRepository<T>().GetByIdAsync(id);
        }
        
        public virtual int Update(T entity)
        {
            if (entity != null)
            {
                _unitOfWork.GenericRepository<T>().Update(entity);

                return _unitOfWork.SaveChanges();
            }

            throw new ArgumentNullException(nameof(entity));
        }


        public async Task<PaginatedResult<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includesProperties = "",
            int pageIndex = 1,
            int pageSize = 10)
        {
            var repository = _unitOfWork.GenericRepository<T>();

            // Lấy dữ liệu từ repository
            IQueryable<T> query = repository.GetQuery();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Include các thuộc tính liên quan
            foreach (var includeProperty in includesProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            // Sắp xếp dữ liệu nếu có
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Trả về kết quả phân trang
            return await PaginatedResult<T>.CreateAsync(query, pageIndex, pageSize);
        }
        public async Task<IEnumerable<T>> GetAllAsync(
    Expression<Func<T, bool>>? filter = null,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    string includesProperties = ""
)
        {
            // Lấy repository tương ứng với loại T
            var repository = _unitOfWork.GenericRepository<T>();

            // Lấy query gốc
            IQueryable<T> query = repository.GetQuery();

            // Áp dụng filter nếu có
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Bao gồm các thuộc tính (relationships) nếu có
            if (!string.IsNullOrWhiteSpace(includesProperties))
            {
                foreach (var includeProperty in includesProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty); // Bao gồm quan hệ
                }
            }

            // Áp dụng sắp xếp nếu có
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Trả về kết quả dưới dạng danh sách với các thay đổi
            return await query.ToListAsync();
        }

        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includesProperties = ""
        )
        {
            // Lấy repository tương ứng với loại T
            var repository = _unitOfWork.GenericRepository<T>();

            // Lấy query gốc
            IQueryable<T> query = repository.GetQuery();

            // Áp dụng filter nếu có
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Bao gồm các thuộc tính (relationships) nếu có
            if (!string.IsNullOrWhiteSpace(includesProperties))
            {
                foreach (var includeProperty in includesProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty); // Bao gồm quan hệ
                }
            }

            // Áp dụng sắp xếp nếu có
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Trả về đối tượng đầu tiên hoặc null nếu không tìm thấy
            return await query.FirstOrDefaultAsync();
        }
    }

}