using System.ComponentModel.DataAnnotations;

namespace SnackisForum.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]

        [StringLength(1000, ErrorMessage = "Comment text must be at most 1000 characters.")]
        public string Text { get; set; }
        public DateTime CreateAt { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public int CategoryId { get; set; }

    }
}
