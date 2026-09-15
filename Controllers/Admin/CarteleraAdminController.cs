using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;

namespace PasearPorPasear.Controllers.Admin;

/// <summary>
/// Se llama CarteleraAdmin y no Cartelera para no chocar con el controlador
/// público del mismo nombre: dos controladores homónimos hacen que el
/// enrutamiento no sepa cuál elegir y la petición falla.
/// </summary>
[Route("Admin/Cartelera")]
public class CarteleraAdminController : ControladorAdmin
{
    public CarteleraAdminController(ApplicationDbContext ctx, IServicioImagenes imagenes)
        : base(ctx, imagenes) { }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        // Primero lo que espera revisión, después lo vigente, al final lo vencido.
        var afiches = await Ctx.CarteleraAfiches.AsNoTracking()
            .OrderBy(a => a.Estado == EstadoAfiche.PorRevisar ? 0 : 1)
            .ThenBy(a => a.Orden)
            .ThenByDescending(a => a.CreadoEn)
            .ToListAsync();

        ViewData["Title"] = "Cartelera";
        return View(afiches);
    }

    [HttpGet("Nuevo")]
    public IActionResult Nuevo()
    {
        ViewData["Title"] = "Afiche nuevo";
        return View("Editar", new CarteleraAfiche { Estado = EstadoAfiche.Publicado });
    }

    [HttpGet("Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id)
    {
        var a = await Ctx.CarteleraAfiches.FindAsync(id);
        if (a is null) return NotFound();
        ViewData["Title"] = a.Titulo;
        return View(a);
    }

    [HttpPost("Guardar")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public async Task<IActionResult> Guardar(CarteleraAfiche entrada, IFormFile? imagen, bool borrarImagen = false)
    {
        var esNuevo = entrada.Id == 0;
        var a = esNuevo ? new CarteleraAfiche() : await Ctx.CarteleraAfiches.FindAsync(entrada.Id);
        if (a is null) return NotFound();

        a.Titulo = entrada.Titulo?.Trim() ?? string.Empty;
        a.Barrio = entrada.Barrio?.Trim();
        a.Texto = entrada.Texto?.Trim() ?? string.Empty;
        a.Contacto = entrada.Contacto?.Trim();
        a.VigenteHasta = entrada.VigenteHasta;
        a.Color = entrada.Color;
        a.Estado = entrada.Estado;
        a.Orden = entrada.Orden;
        a.EmailRemitente = entrada.EmailRemitente?.Trim();
        a.ImagenAlt = entrada.ImagenAlt?.Trim();

        if (!await AplicarImagenAsync(a, imagen, borrarImagen))
            return View("Editar", a);

        if (string.IsNullOrWhiteSpace(a.Titulo))
            ModelState.AddModelError(nameof(CarteleraAfiche.Titulo), "El título es obligatorio.");
        if (string.IsNullOrWhiteSpace(a.Texto))
            ModelState.AddModelError(nameof(CarteleraAfiche.Texto), "El texto del afiche es obligatorio.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = esNuevo ? "Afiche nuevo" : a.Titulo;
            return View("Editar", a);
        }

        if (esNuevo)
        {
            a.CreadoEn = DateTime.Now;
            Ctx.CarteleraAfiches.Add(a);
        }
        else
        {
            a.ActualizadoEn = DateTime.Now;
        }

        await Ctx.SaveChangesAsync();
        TempData["Success"] = esNuevo ? "Afiche pegado." : "Cambios guardados.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Publicar o archivar de un clic, desde el listado.</summary>
    [HttpPost("Estado/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, EstadoAfiche estado)
    {
        var a = await Ctx.CarteleraAfiches.FindAsync(id);
        if (a is null) return NotFound();

        a.Estado = estado;
        a.ActualizadoEn = DateTime.Now;
        await Ctx.SaveChangesAsync();

        TempData["Success"] = estado switch
        {
            EstadoAfiche.Publicado => $"«{a.Titulo}» quedó pegado en la cartelera.",
            EstadoAfiche.Archivado => $"«{a.Titulo}» se archivó.",
            _ => "Estado actualizado."
        };
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Borrar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrar(int id)
    {
        var a = await Ctx.CarteleraAfiches.FindAsync(id);
        if (a is null) return NotFound();

        Ctx.CarteleraAfiches.Remove(a);
        await Ctx.SaveChangesAsync();
        TempData["Success"] = $"Se borró «{a.Titulo}».";
        return RedirectToAction(nameof(Index));
    }
}
