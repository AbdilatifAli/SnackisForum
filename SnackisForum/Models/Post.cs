
namespace SnackisForum.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public DateTime CreateAt{ get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }

        public List<Comment> Comments { get; internal set; }
    }
}
