using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;

namespace PasearPorPasear.Controllers.Admin;

/// <summary>
/// Textos del sitio que no son entradas: la página Sobre PPP, los títulos
/// y bajadas de cada sección, y los ajustes sueltos.
/// </summary>
[Route("Admin/Paginas")]
public class PaginasController : ControladorAdmin
{
    public PaginasController(ApplicationDbContext ctx, IServicioImagenes imagenes)
        : base(ctx, imagenes) { }

    [HttpGet("")]
    public IActionResult Index() => RedirectToAction(nameof(Sobre));

    // ── Sobre PPP ─────────────────────────────────────────────
    [HttpGet("Sobre")]
    public async Task<IActionResult> Sobre()
    {
        var pagina = await Ctx.PaginasSobre.FirstOrDefaultAsync();
        if (pagina is null)
        {
            // No debería pasar: lo crea el seeder. Pero si falta, se crea vacía
            // en vez de romper la pantalla.
            pagina = new PaginaSobre();
            Ctx.PaginasSobre.Add(pagina);
            await Ctx.SaveChangesAsync();
        }

        ViewData["Title"] = "Sobre PPP";
        return View(pagina);
    }

    [HttpPost("Sobre")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public async Task<IActionResult> Sobre(PaginaSobre entrada, IFormFile? imagen, bool borrarImagen = false)
    {
        var p = await Ctx.PaginasSobre.FirstOrDefaultAsync();
        if (p is null) return NotFound();

        p.Titulo = entrada.Titulo?.Trim() ?? string.Empty;
        p.TituloEn = entrada.TituloEn?.Trim() ?? string.Empty;
        p.TituloPt = entrada.TituloPt?.Trim() ?? string.Empty;
        p.Contenido = entrada.Contenido ?? string.Empty;
        p.ContenidoEn = entrada.ContenidoEn ?? string.Empty;
        p.ContenidoPt = entrada.ContenidoPt ?? string.Empty;
        p.Firma = entrada.Firma?.Trim() ?? string.Empty;
        p.ImagenAlt = entrada.ImagenAlt?.Trim();

        if (!await AplicarImagenAsync(p, imagen, borrarImagen))
        {
            ViewData["Title"] = "Sobre PPP";
            return View(p);
        }

        p.ActualizadaEn = DateTime.Now;
        await Ctx.SaveChangesAsync();

        TempData["Success"] = "Cambios guardados.";
        return RedirectToAction(nameof(Sobre));
    }

    // ── Encabezados de sección ────────────────────────────────
    [HttpGet("Encabezados")]
    public async Task<IActionResult> Encabezados()
    {
        ViewData["Title"] = "Títulos de sección";
        return View(await Ctx.EncabezadosSeccion.OrderBy(e => e.Clave).ToListAsync());
    }

    [HttpPost("Encabezados/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GuardarEncabezado(int id, EncabezadoSeccion entrada)
    {
        var e = await Ctx.EncabezadosSeccion.FindAsync(id);
        if (e is null) return NotFound();

        // La clave no se toca: la buscan los controladores.
        e.Titulo = entrada.Titulo?.Trim() ?? string.Empty;
        e.TituloEn = entrada.TituloEn?.Trim() ?? string.Empty;
        e.TituloPt = entrada.TituloPt?.Trim() ?? string.Empty;
        e.Bajada = entrada.Bajada?.Trim();
        e.BajadaEn = entrada.BajadaEn?.Trim();
        e.BajadaPt = entrada.BajadaPt?.Trim();
        e.ActualizadoEn = DateTime.Now;

        await Ctx.SaveChangesAsync();
        TempData["Success"] = $"Se guardó «{e.Clave}».";
        return RedirectToAction(nameof(Encabezados));
    }

    // ── Ajustes ───────────────────────────────────────────────
    [HttpGet("Ajustes")]
    public async Task<IActionResult> Ajustes()
    {
        ViewData["Title"] = "Ajustes";
        return View(await Ctx.Ajustes.OrderBy(a => a.Clave).ToListAsync());
    }

    [HttpPost("Ajustes")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ajustes(int[] id, string[] valor, string[] valorEn, string[] valorPt)
    {
        var ajustes = await Ctx.Ajustes.ToListAsync();

        for (var i = 0; i < id.Length; i++)
        {
            var a = ajustes.FirstOrDefault(x => x.Id == id[i]);
            if (a is null) continue;

            a.Valor = i < valor.Length ? valor[i]?.Trim() ?? string.Empty : string.Empty;
            a.ValorEn = i < valorEn.Length ? valorEn[i]?.Trim() ?? string.Empty : string.Empty;
            a.ValorPt = i < valorPt.Length ? valorPt[i]?.Trim() ?? string.Empty : string.Empty;
        }

        await Ctx.SaveChangesAsync();
        TempData["Success"] = "Ajustes guardados.";
        return RedirectToAction(nameof(Ajustes));
    }
}
