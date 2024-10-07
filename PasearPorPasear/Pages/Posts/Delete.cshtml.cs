using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasearPorPasear.Models;
using PasearPorPasear.Data;
using System.Threading.Tasks;

namespace PasearPorPasear.Pages.Posts
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Post Post { get; set; }

        // Cargar la publicación a eliminar
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Post = await _context.Posts.FindAsync(id);

            if (Post == null)
            {
                return NotFound();
            }

            return Page();
        }

        // Confirmar la eliminación de la publicación
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var postToDelete = await _context.Posts.FindAsync(id);

            if (postToDelete == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(postToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
