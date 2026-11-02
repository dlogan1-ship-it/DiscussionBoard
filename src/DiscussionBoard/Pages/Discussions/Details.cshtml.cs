using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    public class DiscussionDetailsModel : PageModel
    {
        private readonly DiscussionThreadService _threads;
        private readonly PostService _posts;

        public DiscussionThread Discussion { get; private set; } = null!;

        public List<PostTreeBuilder.Node> Posts { get; private set; } = [];

        public bool IsAuthor { get; private set; }

        [BindProperty]
        public ReplyInput Input { get; set; } = new();

        public DiscussionDetailsModel(DiscussionThreadService threads, PostService posts)
        {
            _threads = threads;
            _posts = posts;
        }

        public async Task<IActionResult> OnGetAsync(int threadId)
        {
            return await LoadAsync(threadId) ? Page() : NotFound();
        }

        public async Task<IActionResult> OnPostReplyAsync(int threadId)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Challenge();
            }

            if (!await LoadAsync(threadId))
            {
                return NotFound();
            }

            if (Input.ParentPostId.HasValue &&
                !await _posts.ExistsInThreadAsync(Input.ParentPostId.Value, threadId))
            {
                ModelState.AddModelError(string.Empty, "The post you replied to does not belong to this discussion.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            string title = "Re: " + Discussion.Title;

            var post = new Post
            {
                DiscussionThreadId = threadId,
                ParentPostId = Input.ParentPostId,
                Title = title.Length > 200 ? title[..200] : title,
                Content = Input.Content,
                ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            await _posts.AddAsync(post);

            return RedirectToPage("/Discussions/Details", new { threadId });
        }

        private async Task<bool> LoadAsync(int threadId)
        {
            DiscussionThread? thread = await _threads.GetAsync(threadId);
            if (thread is null)
            {
                return false;
            }

            Discussion = thread;
            IsAuthor = thread.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier);
            Posts = PostTreeBuilder.Build(await _posts.GetForThreadAsync(threadId));
            return true;
        }
    }

    public class ReplyInput
    {
        public int? ParentPostId { get; set; }

        [Required]
        [StringLength(10000)]
        public string Content { get; set; } = string.Empty;
    }
}
