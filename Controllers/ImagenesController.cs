using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;

namespace PasearPorPasear.Controllers;

/// <summary>
/// Sirve las imágenes que están guardadas en la base.
/// Patrón de URL: /Imagenes/{tipo}/{id}?v={sello}
/// El parámetro v lo agrega ImagenSrc a partir de la fecha de actualización:
/// es lo que hace que el navegador deje de servir la imagen vieja cuando
/// Rosalía la reemplaza. Sin eso la URL no cambiaría nunca.
/// </summary>
[Route("Imagenes")]
public class ImagenesController : Controller
{
    /// <summary>Proyección común, para poder unificar las consultas de cada tabla.</summary>
    private sealed record Imagen(byte[]? Datos, string? Tipo);

    private readonly ApplicationDbContext _ctx;

    public ImagenesController(ApplicationDbContext ctx) { _ctx = ctx; }

    [HttpGet("{tipo}/{id:int}")]
    [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Ver(string tipo, int id, CancellationToken ct)
    {
        var imagen = tipo.ToLowerInvariant() switch
        {
            "fachada" => await _ctx.Fachadas
                .Where(x => x.Id == id)
                .Select(x => new Imagen(x.ImagenDatos, x.ImagenTipo))
                .FirstOrDefaultAsync(ct),

            "propuesta" => await _ctx.Propuestas
                .Where(x => x.Id == id)
                .Select(x => new Imagen(x.ImagenDatos, x.ImagenTipo))
                .FirstOrDefaultAsync(ct),

            "encuentro" => await _ctx.ClubcitoEncuentros
                .Where(x => x.Id == id)
                .Select(x => new Imagen(x.ImagenDatos, x.ImagenTipo))
                .FirstOrDefaultAsync(ct),

            "afiche" => await _ctx.CarteleraAfiches
                .Where(x => x.Id == id)
                .Select(x => new Imagen(x.ImagenDatos, x.ImagenTipo))
                .FirstOrDefaultAsync(ct),

            "producto" => await _ctx.Productos
                .Where(x => x.Id == id)
                .Select(x => new Imagen(x.ImagenDatos, x.ImagenTipo))
                .FirstOrDefaultAsync(ct),

            "sobre" => await _ctx.PaginasSobre
                .Where(x => x.Id == id)
                .Select(x => new Imagen(x.ImagenDatos, x.ImagenTipo))
                .FirstOrDefaultAsync(ct),

            _ => null
        };

        if (imagen?.Datos is not { Length: > 0 } || string.IsNullOrEmpty(imagen.Tipo))
            return NotFound();

        return File(imagen.Datos, imagen.Tipo);
    }
}
