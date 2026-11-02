using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    [Authorize]
    public class EditThreadModel : PageModel
    {
        private readonly DiscussionThreadService _threads;

        [BindProperty]
        public ThreadInput Input { get; set; } = new();

        public int ThreadId { get; private set; }

        public EditThreadModel(DiscussionThreadService threads)
        {
            _threads = threads;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            DiscussionThread? thread = await _threads.GetAsync(id);
            if (thread is null)
            {
                return NotFound();
            }

            if (!IsAuthor(thread))
            {
                return Forbid();
            }

            ThreadId = id;
            Input = new ThreadInput { Title = thread.Title, Content = thread.Content };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            DiscussionThread? thread = await _threads.GetAsync(id);
            if (thread is null)
            {
                return NotFound();
            }

            if (!IsAuthor(thread))
            {
                return Forbid();
            }

            ThreadId = id;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool updated = await _threads.UpdateAsync(new DiscussionThread
            {
                Id = id,
                Title = Input.Title,
                Content = Input.Content
            });

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToPage("/Discussions/Details", new { threadId = id });
        }

        private bool IsAuthor(DiscussionThread thread) =>
            thread.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
