using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscussionBoard.Services
{
    public class DiscussionThreadService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<DiscussionThreadService> _logger;

        public DiscussionThreadService(ApplicationDbContext db, ILogger<DiscussionThreadService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public DiscussionThreadService(ApplicationDbContext db)
            : this(db, NullLogger<DiscussionThreadService>.Instance)
        {
        }

        public async Task<DiscussionThread?> GetAsync(int id)
        {
            return await _db.DiscussionThreads
                .Include(t => t.Forum)
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<List<DiscussionThread>> GetForForumAsync(int forumId)
        {
            return await _db.DiscussionThreads
                .AsNoTracking()
                .Include(t => t.ApplicationUser)
                .Where(t => t.ForumId == forumId && !t.IsDeleted)
                .OrderByDescending(t => t.Id)
                .ToListAsync();
        }

        public async Task AddAsync(DiscussionThread thread)
        {
            thread.CreatedAt = DateTimeOffset.UtcNow;
            _db.DiscussionThreads.Add(thread);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e, "Could not add discussion thread {Title}", thread.Title);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(DiscussionThread thread)
        {
            DiscussionThread? existing = await _db.DiscussionThreads
                .FirstOrDefaultAsync(t => t.Id == thread.Id && !t.IsDeleted);

            if (existing is null)
            {
                return false;
            }

            existing.Title = thread.Title;
            existing.Content = thread.Content;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e, "Could not update discussion thread {ThreadId}", thread.Id);
                throw;
            }

            return true;
        }
    }
}
