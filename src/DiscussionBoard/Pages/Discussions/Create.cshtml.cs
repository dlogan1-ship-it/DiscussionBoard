using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    [Authorize]
    public class CreateThreadModel : PageModel
    {
        private readonly ForumService _forums;
        private readonly DiscussionThreadService _threads;

        [BindProperty]
        public ThreadInput Input { get; set; } = new();

        public Forum Forum { get; private set; } = null!;

        public CreateThreadModel(ForumService forums, DiscussionThreadService threads)
        {
            _forums = forums;
            _threads = threads;
        }

        public async Task<IActionResult> OnGetAsync(int forumId)
        {
            Forum? forum = await _forums.GetAsync(forumId);
            if (forum is null)
            {
                return NotFound();
            }

            Forum = forum;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int forumId)
        {
            Forum? forum = await _forums.GetAsync(forumId);
            if (forum is null)
            {
                return NotFound();
            }

            Forum = forum;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var thread = new DiscussionThread
            {
                ForumId = forumId,
                Title = Input.Title,
                Content = Input.Content,
                ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            await _threads.AddAsync(thread);

            return RedirectToPage("/Discussions/Details", new { threadId = thread.Id });
        }
    }
}
