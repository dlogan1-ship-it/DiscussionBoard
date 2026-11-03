using DiscussionBoard.Models;

namespace DiscussionBoard.Services
{
    public static class AuthorDisplay
    {
        public const string UnconfiguredName = "Unconfigured User";

        public static string Name(ApplicationUser? user) =>
            string.IsNullOrWhiteSpace(user?.DisplayName) ? UnconfiguredName : user.DisplayName;

        public static string Avatar(ApplicationUser? user) =>
            AvatarCatalog.ImagePath(user?.AvatarId);
    }
}
