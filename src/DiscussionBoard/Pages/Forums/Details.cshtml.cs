using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages
{
    public class ForumDetailsModel : PageModel
    {
        private readonly ForumService _forums;
        private readonly DiscussionThreadService _threads;

        public Forum Forum { get; private set; } = null!;

        public List<DiscussionThread> Threads { get; private set; } = [];

        public ForumDetailsModel(ForumService forums, DiscussionThreadService threads)
        {
            _forums = forums;
            _threads = threads;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Forum? forum = await _forums.GetAsync(id);
            if (forum is null)
            {
                return NotFound();
            }

            Forum = forum;
            Threads = await _threads.GetForForumAsync(id);
            return Page();
        }
    }
}
