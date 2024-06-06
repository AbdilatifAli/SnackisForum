namespace SnackisForum.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string  UserId { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsAdmin { get; set; }

    }
}
