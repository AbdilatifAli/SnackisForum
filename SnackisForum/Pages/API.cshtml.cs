using Microsoft.AspNetCore.Mvc.RazorPages;
using SnackisForum.DAL;
using SnackisForum.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class APi : PageModel
{
    private readonly CategoryService _categoryService;

    public APi(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public List<Category> Categories { get; set; }

    public async Task OnGetAsync()
    {
        Categories = await _categoryService.GetCategoriesAsync();
    }
}
