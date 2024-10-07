using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasearPorPasear.Pages.Posts
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Post> Posts { get; set; }

        public async Task OnGetAsync()
        {
            Posts = await _context.Posts.ToListAsync();
        }

        // Método para eliminar una publicación
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var postToDelete = await _context.Posts.FindAsync(id);

            if (postToDelete == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(postToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}