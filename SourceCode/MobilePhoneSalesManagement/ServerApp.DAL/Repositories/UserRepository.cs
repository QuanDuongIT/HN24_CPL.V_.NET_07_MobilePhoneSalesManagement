using Microsoft.EntityFrameworkCore;
using ServerApp.DAL.Data;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories.Generic;

namespace ServerApp.DAL.Repositories
{
    public class UserRepository : IGenericRepository<User>
    {
        private readonly ShopDbContext _context;
        private readonly DbSet<User> _dbSet;

        public UserRepository(ShopDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<User>();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(User entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(User entity)
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

        public async Task<User> GetUserWithDetailsAsync(int id)
        {
            return await _dbSet.Include(u => u.UserDetails).FirstOrDefaultAsync(u => u.UserId == id);
        }
    }

}
