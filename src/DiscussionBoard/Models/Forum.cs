using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    public class Forum : EntityBase
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public ICollection<DiscussionThread> DiscussionThreads { get; set; } = new List<DiscussionThread>();
    }
}
