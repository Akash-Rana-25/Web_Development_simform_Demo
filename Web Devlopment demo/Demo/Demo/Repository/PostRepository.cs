using Demo.Context;
using Demo.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Demo.Repository
{
    public class PostRepository : IRepository<Post>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public PostRepository(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            var cacheKey = $"Post_{id}";
            if (_cache.TryGetValue(cacheKey, out Post post))
            {
                return post;
            }

            post = await _context.Set<Post>().Include(p => p.User).Include(p => p.Comments) .FirstOrDefaultAsync(p => p.Id == id);

            if (post != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(cacheKey, post, cacheOptions);
            }

            return post;
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Set<Post>()
                .Include(p => p.User)
                .Include(p => p.Comments)
                .ToListAsync();
        }

        public async Task AddAsync(Post entity)
        {
            await _context.Set<Post>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post entity)
        {
            _context.Set<Post>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<Post>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<Post>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }

}
