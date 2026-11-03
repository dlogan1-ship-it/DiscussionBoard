using DiscussionBoard.Models;
using DiscussionBoard.Services;
using DiscussionBoard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages
{
    [Authorize]
    public class MyProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserProfileService _profiles;

        [BindProperty]
        public UserProfile Input { get; set; } = new();

        public string CurrentAvatarId { get; private set; } = AvatarCatalog.DefaultId;

        [TempData]
        public string? StatusMessage { get; set; }

        public MyProfileModel(UserManager<ApplicationUser> userManager, IUserProfileService profiles)
        {
            _userManager = userManager;
            _profiles = profiles;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Challenge();
            }

            CurrentAvatarId = SafeAvatar(user.AvatarId);
            Input = new UserProfile
            {
                DisplayName = user.DisplayName,
                AvatarId = CurrentAvatarId
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Challenge();
            }

            CurrentAvatarId = SafeAvatar(user.AvatarId);

            Input.DisplayName = (Input.DisplayName ?? string.Empty).Trim();
            ModelState.ClearValidationState(nameof(Input));
            TryValidateModel(Input, nameof(Input));

            if (!string.IsNullOrEmpty(Input.AvatarId) && !AvatarCatalog.IsValid(Input.AvatarId))
            {
                ModelState.AddModelError("Input.AvatarId", "Please choose one of the available avatars.");
            }

            if (ModelState.IsValid &&
                !await _profiles.IsDisplayNameAvailableAsync(Input.DisplayName, user.Id))
            {
                ModelState.AddModelError("Input.DisplayName", "That display name is already taken.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            user.DisplayName = Input.DisplayName;
            user.AvatarId = Input.AvatarId;

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            StatusMessage = "Your profile has been updated.";
            return RedirectToPage();
        }

        private static string SafeAvatar(string? avatarId) =>
            AvatarCatalog.IsValid(avatarId) ? avatarId! : AvatarCatalog.DefaultId;
    }
}
