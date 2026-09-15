using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;

namespace PasearPorPasear.Controllers.Admin;

/// <summary>
/// Las cuatro propuestas de ¿Paseás conmigo? Son fijas: se editan, no se
/// crean ni se borran, porque la portada y el menú cuentan con que estén.
/// </summary>
[Route("Admin/Propuestas")]
public class PropuestasController : ControladorAdmin
{
    /// <summary>Cuántos casilleros de datos ofrece el formulario. El diseño no aguanta más.</summary>
    private const int Casilleros = 4;

    public PropuestasController(ApplicationDbContext ctx, IServicioImagenes imagenes)
        : base(ctx, imagenes) { }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Propuestas";
        return View(await Ctx.Propuestas.AsNoTracking()
            .Include(p => p.Datos)
            .OrderBy(p => p.Orden)
            .ToListAsync());
    }

    [HttpGet("Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id)
    {
        var p = await Ctx.Propuestas
            .Include(x => x.Datos.OrderBy(d => d.Orden))
            .FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return NotFound();

        ViewData["Title"] = p.Titulo;
        return View(p);
    }

    [HttpPost("Guardar")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public async Task<IActionResult> Guardar(
        Propuesta entrada, IFormFile? imagen, bool borrarImagen = false,
        string[]? datoValor = null, string[]? datoValorEn = null, string[]? datoValorPt = null,
        string[]? datoEtiqueta = null, string[]? datoEtiquetaEn = null, string[]? datoEtiquetaPt = null)
    {
        var p = await Ctx.Propuestas
            .Include(x => x.Datos)
            .FirstOrDefaultAsync(x => x.Id == entrada.Id);
        if (p is null) return NotFound();

        p.Titulo = entrada.Titulo?.Trim() ?? string.Empty;
        p.TituloEn = entrada.TituloEn?.Trim() ?? string.Empty;
        p.TituloPt = entrada.TituloPt?.Trim() ?? string.Empty;
        p.Etiqueta = entrada.Etiqueta?.Trim();
        p.EtiquetaEn = entrada.EtiquetaEn?.Trim();
        p.EtiquetaPt = entrada.EtiquetaPt?.Trim();
        p.Descripcion = entrada.Descripcion ?? string.Empty;
        p.DescripcionEn = entrada.DescripcionEn ?? string.Empty;
        p.DescripcionPt = entrada.DescripcionPt ?? string.Empty;
        p.PendienteDeDefinir = entrada.PendienteDeDefinir;
        p.Activa = entrada.Activa;
        p.Orden = entrada.Orden;
        p.ImagenAlt = entrada.ImagenAlt?.Trim();

        if (!await AplicarImagenAsync(p, imagen, borrarImagen))
            return View("Editar", p);

        if (string.IsNullOrWhiteSpace(p.Titulo))
        {
            ModelState.AddModelError(nameof(Propuesta.Titulo), "El título es obligatorio.");
            ViewData["Title"] = p.Titulo;
            return View("Editar", p);
        }

        // Los casilleros de datos se reemplazan enteros: es más simple y más
        // predecible que emparejar por id, y son cuatro filas.
        Ctx.PropuestaDatos.RemoveRange(p.Datos);
        p.Datos.Clear();

        for (var i = 0; i < Casilleros; i++)
        {
            var valor = Tomar(datoValor, i);
            var etiqueta = Tomar(datoEtiqueta, i);

            // Un casillero vacío simplemente no se guarda.
            if (string.IsNullOrWhiteSpace(valor) || string.IsNullOrWhiteSpace(etiqueta)) continue;

            p.Datos.Add(new PropuestaDato
            {
                Orden = i + 1,
                Valor = valor.Trim(),
                ValorEn = Tomar(datoValorEn, i)?.Trim() ?? string.Empty,
                ValorPt = Tomar(datoValorPt, i)?.Trim() ?? string.Empty,
                Etiqueta = etiqueta.Trim(),
                EtiquetaEn = Tomar(datoEtiquetaEn, i)?.Trim() ?? string.Empty,
                EtiquetaPt = Tomar(datoEtiquetaPt, i)?.Trim() ?? string.Empty
            });
        }

        p.ActualizadaEn = DateTime.Now;
        await Ctx.SaveChangesAsync();

        TempData["Success"] = "Cambios guardados.";
        return RedirectToAction(nameof(Index));
    }

    private static string? Tomar(string[]? arreglo, int i) =>
        arreglo is not null && i < arreglo.Length ? arreglo[i] : null;
}
