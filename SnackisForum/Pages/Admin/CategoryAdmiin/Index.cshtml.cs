using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SnackisForum.Data;
using SnackisForum.Models;

namespace SnackisForum.Pages.Admin.CategoryAdmiin
{
    public class IndexModel : PageModel
    {
        private readonly SnackisForum.Data.ApplicationDbContext _context;

        public IndexModel(SnackisForum.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Category> Category { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Category = await _context.Categories.ToListAsync();
        }
    }
}
