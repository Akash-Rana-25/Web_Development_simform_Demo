using Demo.Context;
using Demo.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Demo.Repository
{
    public class CommentRepository : IRepository<Comment>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public CommentRepository(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Comment> GetByIdAsync(int id)
        {
            var cacheKey = $"Comment_{id}";
            if (_cache.TryGetValue(cacheKey, out Comment comment))
            {
                return comment;
            }

            comment = await _context.Set<Comment>()
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(cacheKey, comment, cacheOptions);
            }

            return comment;
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _context.Set<Comment>()
                .Include(c => c.Post)
                .ToListAsync();
        }

        public async Task AddAsync(Comment entity)
        {
            await _context.Set<Comment>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment entity)
        {
            _context.Set<Comment>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<Comment>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<Comment>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }

}
