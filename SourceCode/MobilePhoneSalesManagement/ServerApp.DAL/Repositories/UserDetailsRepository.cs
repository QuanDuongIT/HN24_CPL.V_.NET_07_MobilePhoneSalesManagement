using Microsoft.EntityFrameworkCore;
using ServerApp.DAL.Data;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.DAL.Repositories
{
    public class UserDetailsRepository : IGenericRepository<UserDetails>
    {
        private readonly ShopDbContext _context;
        private readonly DbSet<UserDetails> _dbSet;

        public UserDetailsRepository(ShopDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<UserDetails>();
        }

        public async Task<UserDetails> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<UserDetails>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(UserDetails entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserDetails entity)
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
