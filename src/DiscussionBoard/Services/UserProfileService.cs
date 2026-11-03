using DiscussionBoard.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Services
{
    public class UserProfileService : IUserProfileService
    {
        public const int MinDisplayNameLength = 2;
        public const int MaxDisplayNameLength = 60;

        private readonly ApplicationDbContext _db;

        public UserProfileService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> IsDisplayNameAvailableAsync(string displayName, string? excludeUserId = null)
        {
            string trimmed = (displayName ?? string.Empty).Trim();

            if (trimmed.Length < MinDisplayNameLength || trimmed.Length > MaxDisplayNameLength)
            {
                return false;
            }

            string upper = trimmed.ToUpperInvariant();

            bool taken = await _db.Users.AnyAsync(u =>
                u.DisplayName.ToUpper() == upper &&
                (excludeUserId == null || u.Id != excludeUserId));

            return !taken;
        }
    }
}
