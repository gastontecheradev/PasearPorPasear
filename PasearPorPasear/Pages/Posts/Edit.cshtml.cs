using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Models;
using System.Threading.Tasks;
using PasearPorPasear.Data;

namespace PasearPorPasear.Pages.Posts
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Post Post { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Post = await _context.Posts.FindAsync(id);

            if (Post == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Buscar la publicación original en la base de datos
            var postToUpdate = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if (postToUpdate == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Actualizar los campos del objeto original con los valores editados
            postToUpdate.Titulo = Post.Titulo;
            postToUpdate.Desarrollo = Post.Desarrollo;
            postToUpdate.FechaEvento = Post.FechaEvento;

            // Guardar cambios en la base de datos
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
