using Microsoft.EntityFrameworkCore;
using ServerApp.DAL.Data;

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
    }


}
