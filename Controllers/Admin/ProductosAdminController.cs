using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers.Admin;

/// <summary>
/// ProductosAdmin y no Productos: ya hay un controlador público con ese
/// nombre y dos homónimos rompen el enrutamiento.
/// </summary>
[Route("Admin/Productos")]
public class ProductosAdminController : ControladorAdmin
{
    public ProductosAdminController(ApplicationDbContext ctx, IServicioImagenes imagenes)
        : base(ctx, imagenes) { }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var filas = await Ctx.Productos.AsNoTracking()
            .OrderBy(p => p.Orden)
            .Select(p => new
            {
                p.Id, p.Titulo, p.Precio, p.Estado, p.Orden, p.Visible,
                TieneImagen = p.ImagenDatos != null,
                p.ImagenUrl, p.ActualizadoEn, p.CreadoEn
            })
            .ToListAsync();

        var vm = filas.Select(p => new FilaAdminVm
        {
            Id = p.Id,
            Titulo = p.Titulo,
            ImagenSrc = Imagen.Src("Producto", p.Id, p.TieneImagen, p.ImagenUrl, p.ActualizadoEn ?? p.CreadoEn),
            Columna2 = p.Orden.ToString(),
            Columna3 = p.Estado switch
            {
                EstadoProducto.ALaVenta => p.Precio.HasValue ? $"$ {p.Precio.Value:N0}" : "sin precio",
                EstadoProducto.DescargaGratis => "descarga gratis",
                _ => "en preparación"
            },
            Estado = p.Visible ? "Visible" : "Oculto",
            ClaseEstado = p.Visible ? "pastilla--activo" : "pastilla--espera"
        }).ToList();

        ViewData["Title"] = "Productos";
        return View("ListadoAdmin", new ListadoAdminVm
        {
            Titulo = "Productos",
            Subtitulo = $"{vm.Count} producto(s).",
            UrlBase = "/Admin/Productos",
            UrlNueva = "/Admin/Productos/Nuevo",
            TextoNueva = "Producto nuevo",
            Buscador = "Buscar por nombre…",
            Vacio = "Todavía no hay ningún producto.",
            EncabezadoPrincipal = "Producto",
            Encabezado2 = "Orden",
            Encabezado3 = "Precio o estado",
            Filas = vm
        });
    }

    [HttpGet("Nuevo")]
    public async Task<IActionResult> Nuevo()
    {
        var orden = await Ctx.Productos.MaxAsync(p => (int?)p.Orden) ?? 0;
        ViewData["Title"] = "Producto nuevo";
        return View("Editar", new Producto { Orden = orden + 1 });
    }

    [HttpGet("Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id)
    {
        var p = await Ctx.Productos.FindAsync(id);
        if (p is null) return NotFound();
        ViewData["Title"] = p.Titulo;
        return View(p);
    }

    [HttpPost("Guardar")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public async Task<IActionResult> Guardar(Producto entrada, IFormFile? imagen, bool borrarImagen = false)
    {
        var esNuevo = entrada.Id == 0;
        var p = esNuevo ? new Producto() : await Ctx.Productos.FindAsync(entrada.Id);
        if (p is null) return NotFound();

        p.Titulo = entrada.Titulo?.Trim() ?? string.Empty;
        p.TituloEn = entrada.TituloEn?.Trim() ?? string.Empty;
        p.TituloPt = entrada.TituloPt?.Trim() ?? string.Empty;
        p.Descripcion = entrada.Descripcion?.Trim();
        p.DescripcionEn = entrada.DescripcionEn?.Trim();
        p.DescripcionPt = entrada.DescripcionPt?.Trim();
        p.ColorPanel = entrada.ColorPanel;
        p.Precio = entrada.Precio;
        p.Estado = entrada.Estado;
        p.ArchivoUrl = entrada.ArchivoUrl?.Trim();
        p.Orden = entrada.Orden;
        p.Visible = entrada.Visible;
        p.ImagenAlt = entrada.ImagenAlt?.Trim();

        if (!await AplicarImagenAsync(p, imagen, borrarImagen))
            return View("Editar", p);

        if (string.IsNullOrWhiteSpace(p.Titulo))
            ModelState.AddModelError(nameof(Producto.Titulo), "El nombre es obligatorio.");
        if (p.Estado == EstadoProducto.ALaVenta && (p.Precio is null || p.Precio <= 0))
            ModelState.AddModelError(nameof(Producto.Precio), "Un producto a la venta necesita precio.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = esNuevo ? "Producto nuevo" : p.Titulo;
            return View("Editar", p);
        }

        if (esNuevo || string.IsNullOrWhiteSpace(p.Slug))
        {
            p.Slug = SlugUnico(p.Titulo, slug => Ctx.Productos.Any(x => x.Slug == slug && x.Id != p.Id));
        }

        if (esNuevo)
        {
            p.CreadoEn = DateTime.Now;
            Ctx.Productos.Add(p);
        }
        else
        {
            p.ActualizadoEn = DateTime.Now;
        }

        await Ctx.SaveChangesAsync();
        TempData["Success"] = esNuevo ? "Producto creado." : "Cambios guardados.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Borrar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrar(int id)
    {
        var p = await Ctx.Productos.FindAsync(id);
        if (p is null) return NotFound();

        Ctx.Productos.Remove(p);
        await Ctx.SaveChangesAsync();
        TempData["Success"] = $"Se borró «{p.Titulo}».";
        return RedirectToAction(nameof(Index));
    }
}
