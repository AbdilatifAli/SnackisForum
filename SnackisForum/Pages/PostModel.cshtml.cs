using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SnackisForum.Models;

namespace SnackisForum.Pages
{
    public class PostModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PostModel(Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Models.Post Post { get; set; }
        [BindProperty]
        public IFormFile UploadedImage { get; set; }
        public List<Models.Category> Categories { get; set; }

        public async Task OnGetAsync()
        {
            Categories = await _context.Categories.ToListAsync();
        }

        public async Task<IActionResult> OnPostCreatePostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            string fileName = null;
            if (UploadedImage != null)
            {
                fileName = Path.GetRandomFileName() + Path.GetExtension(UploadedImage.FileName);
                var filePath = Path.Combine("wwwroot/userImages", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedImage.CopyToAsync(fileStream);
                }
            }

            Post.CreateAt = DateTime.Now;
            Post.Image = fileName;
            Post.UserId = user.Id;

            _context.Posts.Add(Post);
            await _context.SaveChangesAsync();

            return RedirectToPage("/PostDetails", new { postId = Post.Id });
        }
    }
}