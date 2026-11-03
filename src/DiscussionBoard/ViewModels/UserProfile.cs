using System.ComponentModel.DataAnnotations;
using DiscussionBoard.Services;

namespace DiscussionBoard.ViewModels
{
    public class UserProfile
    {
        [Required(ErrorMessage = "Please enter a display name.")]
        [StringLength(UserProfileService.MaxDisplayNameLength,
            MinimumLength = UserProfileService.MinDisplayNameLength,
            ErrorMessage = "Display name must be between {2} and {1} characters.")]
        [Display(Name = "Display name")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please choose an avatar.")]
        [Display(Name = "Avatar")]
        public string AvatarId { get; set; } = AvatarCatalog.DefaultId;
    }
}
