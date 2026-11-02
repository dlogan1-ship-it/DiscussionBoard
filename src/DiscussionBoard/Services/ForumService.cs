using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Services
{
    public class ForumService
    {
        private readonly ApplicationDbContext _db;

        public ForumService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Forum>> GetAllAsync()
        {
            return await _db.Forums
                .AsNoTracking()
                .Where(f => !f.IsDeleted)
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<Forum?> GetAsync(int id)
        {
            return await _db.Forums
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }
    }
}
