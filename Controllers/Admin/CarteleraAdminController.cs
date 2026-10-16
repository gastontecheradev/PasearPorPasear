using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

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
        // Primero lo pegado, después lo archivado. Sin los bytes de la imagen:
        // la miniatura la pide el navegador por su URL.
        var afiches = await Ctx.CarteleraAfiches.AsNoTracking()
            .OrderBy(a => a.Estado == EstadoAfiche.Publicado ? 0 : 1)
            .ThenBy(a => a.Orden)
            .ThenByDescending(a => a.CreadoEn)
            .Select(a => new AficheVm
            {
                Id = a.Id,
                Titulo = a.Titulo,
                ImagenAlt = a.ImagenAlt,
                TieneImagen = a.ImagenDatos != null,
                Sello = a.ActualizadoEn ?? a.CreadoEn,
                VigenteHasta = a.VigenteHasta,
                Estado = a.Estado,
                Categoria = a.Categoria
            })
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
    public async Task<IActionResult> Guardar(
        [Bind(nameof(CarteleraAfiche.Id), nameof(CarteleraAfiche.Titulo), nameof(CarteleraAfiche.ImagenAlt),
              nameof(CarteleraAfiche.VigenteHasta), nameof(CarteleraAfiche.Estado), nameof(CarteleraAfiche.Categoria),
              nameof(CarteleraAfiche.Orden))]
        CarteleraAfiche entrada,
        IFormFile? imagen)
    {
        var esNuevo = entrada.Id == 0;
        var a = esNuevo ? new CarteleraAfiche() : await Ctx.CarteleraAfiches.FindAsync(entrada.Id);
        if (a is null) return NotFound();

        a.Titulo = entrada.Titulo?.Trim() ?? string.Empty;
        a.ImagenAlt = entrada.ImagenAlt?.Trim();
        a.VigenteHasta = entrada.VigenteHasta;
        a.Estado = Enum.IsDefined(entrada.Estado) ? entrada.Estado : EstadoAfiche.Publicado;
        a.Categoria = entrada.Categoria;
        a.Orden = entrada.Orden;

        ViewData["Title"] = esNuevo ? "Afiche nuevo" : a.Titulo;

        // La imagen no se puede quitar, sólo reemplazar: es el afiche.
        if (!await AplicarImagenAsync(a, imagen, borrarImagen: false))
            return View("Editar", a);

        // «Elegí una» llega vacía: en lugar del error genérico del enlazado, uno que diga qué hacer.
        ModelState.Remove(nameof(CarteleraAfiche.Categoria));
        if (!Enum.IsDefined(a.Categoria))
            ModelState.AddModelError(nameof(CarteleraAfiche.Categoria), "Elegí una categoría: es la que usa el muro para filtrar.");

        if (a.ImagenDatos is not { Length: > 0 })
            ModelState.AddModelError("imagen", "Falta la imagen del afiche. Subí la plantilla ya terminada.");

        if (!ModelState.IsValid)
            return View("Editar", a);

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
        TempData["Success"] = esNuevo
            ? (a.Estado == EstadoAfiche.Publicado ? $"«{a.Titulo}» quedó pegado en la cartelera." : $"«{a.Titulo}» quedó guardado, archivado.")
            : "Cambios guardados.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Pegar o archivar de un clic, desde el listado.</summary>
    [HttpPost("Estado/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, EstadoAfiche estado)
    {
        if (!Enum.IsDefined(estado)) return BadRequest();

        var a = await Ctx.CarteleraAfiches.FindAsync(id);
        if (a is null) return NotFound();

        a.Estado = estado;
        a.ActualizadoEn = DateTime.Now;
        await Ctx.SaveChangesAsync();

        TempData["Success"] = estado == EstadoAfiche.Publicado
            ? $"«{a.Titulo}» quedó pegado en la cartelera."
            : $"«{a.Titulo}» se archivó: ya no se ve en el sitio.";
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
