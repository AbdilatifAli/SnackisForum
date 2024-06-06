using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SnackisForum.Data;
using SnackisForum.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnackisForum.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public int? SelectedCategoryId { get; set; }

        public List<Category> Categories { get; set; }

        public List<Post> Posts { get; set; }

        [BindProperty]
        public Comment NewComment { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Categories = await _context.Categories.ToListAsync();
            Posts = new List<Post>();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Categories = await _context.Categories.ToListAsync();

            if (SelectedCategoryId.HasValue)
            {
                Posts = await _context.Posts
                                      .Where(p => p.CategoryId == SelectedCategoryId.Value)
                                      .Include(p => p.Comments)
                                      .ToListAsync();
            }
            else
            {
                Posts = new List<Post>();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddCommentAsync(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                Text = NewComment.Text,
                CreateAt = DateTime.Now,
                UserId = user.Id,
                PostId = postId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { categoryId = SelectedCategoryId });
        }
    }
}
