using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscussionBoard.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PostService> _logger;

        public PostService(ApplicationDbContext db, ILogger<PostService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public PostService(ApplicationDbContext db)
            : this(db, NullLogger<PostService>.Instance)
        {
        }

        public async Task<List<Post>> GetForThreadAsync(int threadId)
        {
            return await _db.Posts
                .AsNoTracking()
                .Include(p => p.ApplicationUser)
                .Where(p => p.DiscussionThreadId == threadId && !p.IsDeleted)
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<bool> ExistsInThreadAsync(int postId, int threadId)
        {
            return await _db.Posts
                .AnyAsync(p => p.Id == postId && p.DiscussionThreadId == threadId && !p.IsDeleted);
        }

        public async Task AddAsync(Post post)
        {
            post.CreatedAt = DateTimeOffset.UtcNow;
            _db.Posts.Add(post);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e, "Could not add post to thread {ThreadId}", post.DiscussionThreadId);
                throw;
            }
        }
    }
}
