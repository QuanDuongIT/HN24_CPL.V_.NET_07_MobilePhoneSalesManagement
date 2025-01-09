using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ServerApp.DAL.Data;
using System.Linq.Expressions;

namespace ServerApp.DAL.Repositories.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ShopDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ShopDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Get by Id
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Get all entities
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Add new entity
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // Update entity
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        // Delete entity by Id
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(Guid id)
        {
            var entity = _dbSet.Find(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public void Delete(T entity)
        {

            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includesProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includesProperties))
            {
                foreach (var property in includesProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query.Include(property);
                }
            }


            return orderBy != null ? orderBy(query) : query;
        }
        public async Task<T?> GetAsync(
             Expression<Func<T, bool>>? filter = null,
             Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
             string includesProperties = ""
         )
        {
            IQueryable<T> query = _dbSet;

            // Áp dụng filter nếu có
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Bao gồm các thuộc tính (relationships) nếu có
            if (!string.IsNullOrWhiteSpace(includesProperties))
            {
                foreach (var property in includesProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property); // Bao gồm quan hệ
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

        public T? GetById(Guid id)
        {
            return _dbSet.Find(id);
        }
        public T? GetById(int id)
        {
            return _dbSet.Find(id);
        }
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public IQueryable<T> GetQuery()
        {
            return _dbSet;
        }

        public IQueryable<T> GetQuery(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate != null)
            {
                return await _dbSet.Where(predicate).ToListAsync();
            }
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        public async Task<T?> GetAsync(
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            // Thêm Include nếu được truyền vào
            if (include != null)
            {
                query = include(query);
            }

            // Áp dụng bộ lọc
            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
        )
        {
            IQueryable<T> query = _context.Set<T>();

            // Áp dụng filter nếu có
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Áp dụng include nếu có
            if (include != null)
            {
                query = include(query);
            }

            return await query.ToListAsync();
        }

    }


}
