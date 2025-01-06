using Microsoft.EntityFrameworkCore;
using ServerApp.DAL.Data;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.DAL.Repositories
{
    public class CartRepository : IGenericRepository<Cart>
    {
        private readonly ShopDbContext _context;
        private readonly DbSet<Cart> _dbSet;

        public CartRepository(ShopDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Cart>();
        }

        public async Task<Cart> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(Cart entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cart entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

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
