using Microsoft.EntityFrameworkCore;
using ServerApp.DAL.Data;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.DAL.Repositories
{
    public class OrderRepository : IGenericRepository<Order>
    {
        private readonly ShopDbContext _context;
        private readonly DbSet<Order> _dbSet;

        public OrderRepository(ShopDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Order>();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(Order entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order entity)
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
