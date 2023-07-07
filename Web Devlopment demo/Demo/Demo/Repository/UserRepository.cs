using Demo.Context;
using Demo.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Demo.Repository
{
    public class UserRepository : IRepository<User>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public UserRepository(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var cacheKey = $"User_{id}";
            if (_cache.TryGetValue(cacheKey, out User user))
            {
                return user;
            }

            user = await _context.Set<User>().FindAsync(id);
            if (user != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(cacheKey, user, cacheOptions);
            }

            return user;
        }
        //public async Task<User> GetByIdAsync(int id)
        //{
        //    return await _context.Set<User>().FindAsync(id);
        //}

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Set<User>().ToListAsync();
        }

        public async Task AddAsync(User entity)
        {
            await _context.Set<User>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User entity)
        {
            _context.Set<User>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<User>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<User>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
