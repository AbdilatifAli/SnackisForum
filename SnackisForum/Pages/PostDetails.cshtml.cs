using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SnackisForum.Data;
using SnackisForum.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SnackisForum.Pages
{
	public class PostDetailsModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;

		public PostDetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[BindProperty]
		public Comment NewComment { get; set; }

		[BindProperty]
		public Report NewReport { get; set; }

		public List<Post> Posts { get; set; }

		public async Task<IActionResult> OnGetAsync(int? deleteId)
		{
			if (deleteId.HasValue)
			{
				var postToBeDeleted = await _context.Posts.FindAsync(deleteId);
				if (postToBeDeleted != null)
				{
					if (System.IO.File.Exists(Path.Combine("./wwwroot/userImages", postToBeDeleted.Image)))
					{
						System.IO.File.Delete(Path.Combine("./wwwroot/userImages", postToBeDeleted.Image));
					}
					_context.Posts.Remove(postToBeDeleted);
					await _context.SaveChangesAsync();
				}
			}

			Posts = await _context.Posts.Include(p => p.Comments).ToListAsync();
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

			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostReportAsync(int? postId, int? commentId)
		{
			var user = await _userManager.GetUserAsync(User);
			var report = new Report
			{
				Reason = NewReport.Reason,
				UserId = user?.Id ?? "Anonymous",
				PostId = postId,
				CommentId = commentId,
				CreatedAt = DateTime.UtcNow
			};

			_context.Reports.Add(report);
			await _context.SaveChangesAsync();

			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostDeletePostAsync(int postId, int commentId)
		{
			var post = await _context.Posts.FindAsync(postId);
			if (post == null)
			{
				return NotFound();
			}

			var user = await _userManager.GetUserAsync(User);
			if (post.UserId != user.Id && !User.IsInRole("Admin"))
			{
				return Forbid();
			}

			if (!string.IsNullOrEmpty(post.Image) && System.IO.File.Exists(Path.Combine("./wwwroot/userImages", post.Image)))
			{
				System.IO.File.Delete(Path.Combine("./wwwroot/userImages", post.Image));
			}

			// Delete associated comments
			var comments = _context.Comments.Where(c => c.PostId == postId);
			_context.Comments.RemoveRange(comments);

			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();

			return RedirectToPage("/PostDetails");
		}
	}
}