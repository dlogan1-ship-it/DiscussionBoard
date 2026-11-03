using System.ComponentModel.DataAnnotations;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Identity;

namespace DiscussionBoard.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<DiscussionThread> DiscussionThreads { get; set; } = new List<DiscussionThread>();

        public ICollection<Post> Posts { get; set; } = new List<Post>();

        [MaxLength(60)]
        public string DisplayName { get; set; } = string.Empty;

        [MaxLength(32)]
        public string AvatarId { get; set; } = AvatarCatalog.DefaultId;
    }
}
