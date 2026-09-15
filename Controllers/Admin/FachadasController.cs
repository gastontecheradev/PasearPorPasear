using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers.Admin;

[Route("Admin/Fachadas")]
public class FachadasController : ControladorAdmin
{
    public FachadasController(ApplicationDbContext ctx, IServicioImagenes imagenes)
        : base(ctx, imagenes) { }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        // Sin ImagenDatos: el listado traería megabytes.
        var filas = await Ctx.Fachadas.AsNoTracking()
            .OrderByDescending(f => f.Numero)
            .Select(f => new
            {
                f.Id, f.Numero, f.Titulo, f.Barrio, f.Calle, f.FechaEncontrada, f.Publicada,
                TieneImagen = f.ImagenDatos != null,
                f.ImagenUrl, f.ActualizadaEn, f.CreadaEn
            })
            .ToListAsync();

        var vm = filas.Select(f => new FilaAdminVm
        {
            Id = f.Id,
            Titulo = f.Titulo,
            Detalle = f.Calle,
            ImagenSrc = Imagen.Src("Fachada", f.Id, f.TieneImagen, f.ImagenUrl, f.ActualizadaEn ?? f.CreadaEn),
            Columna2 = $"N.º {f.Numero}",
            Columna3 = f.Barrio + " · " + f.FechaEncontrada.ToString("dd/MM/yyyy"),
            Estado = f.Publicada ? "Publicada" : "Borrador",
            ClaseEstado = f.Publicada ? "pastilla--activo" : "pastilla--espera"
        }).ToList();

        ViewData["Title"] = "Fachadas";
        return View("ListadoAdmin", new ListadoAdminVm
        {
            Titulo = "Fachadas",
            Subtitulo = $"{vm.Count} ficha(s) en el archivo.",
            UrlBase = "/Admin/Fachadas",
            UrlNueva = "/Admin/Fachadas/Nueva",
            TextoNueva = "Fachada nueva",
            Buscador = "Buscar por título, calle o barrio…",
            Vacio = "Todavía no hay ninguna fachada. Empezá por la primera.",
            EncabezadoPrincipal = "Ficha",
            Encabezado2 = "N.º",
            Encabezado3 = "Barrio y fecha",
            Filas = vm
        });
    }

    [HttpGet("Nueva")]
    public async Task<IActionResult> Nueva()
    {
        // El número siguiente de la serie, para no tener que buscarlo a mano.
        // El cast a int? evita que MaxAsync falle con la tabla vacía.
        var ultimo = await Ctx.Fachadas.MaxAsync(f => (int?)f.Numero) ?? 0;
        var f = new Fachada { Numero = ultimo + 1, FechaEncontrada = DateTime.Today, FechaPublicacion = DateTime.Today };
        ViewData["Title"] = "Fachada nueva";
        return View("Editar", f);
    }

    [HttpGet("Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id)
    {
        var f = await Ctx.Fachadas.FindAsync(id);
        if (f is null) return NotFound();
        ViewData["Title"] = f.Titulo;
        return View(f);
    }

    [HttpPost("Guardar")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public async Task<IActionResult> Guardar(Fachada entrada, IFormFile? imagen, bool borrarImagen = false)
    {
        var esNueva = entrada.Id == 0;
        var f = esNueva ? new Fachada() : await Ctx.Fachadas.FindAsync(entrada.Id);
        if (f is null) return NotFound();

        f.Numero = entrada.Numero;
        f.Titulo = entrada.Titulo?.Trim() ?? string.Empty;
        f.TituloEn = entrada.TituloEn?.Trim() ?? string.Empty;
        f.TituloPt = entrada.TituloPt?.Trim() ?? string.Empty;
        f.Barrio = entrada.Barrio?.Trim() ?? string.Empty;
        f.Calle = entrada.Calle?.Trim();
        f.Extracto = entrada.Extracto?.Trim();
        f.ExtractoEn = entrada.ExtractoEn?.Trim();
        f.ExtractoPt = entrada.ExtractoPt?.Trim();
        f.Contenido = entrada.Contenido ?? string.Empty;
        f.ContenidoEn = entrada.ContenidoEn ?? string.Empty;
        f.ContenidoPt = entrada.ContenidoPt ?? string.Empty;
        f.AnioConstruccion = entrada.AnioConstruccion;
        f.MapaEmbedUrl = entrada.MapaEmbedUrl?.Trim();
        f.FechaEncontrada = entrada.FechaEncontrada;
        f.FechaPublicacion = entrada.FechaPublicacion;
        f.Publicada = entrada.Publicada;
        f.ImagenAlt = entrada.ImagenAlt?.Trim();

        if (!await AplicarImagenAsync(f, imagen, borrarImagen))
            return View("Editar", f);

        if (string.IsNullOrWhiteSpace(f.Titulo))
            ModelState.AddModelError(nameof(Fachada.Titulo), "El título es obligatorio.");
        if (string.IsNullOrWhiteSpace(f.Barrio))
            ModelState.AddModelError(nameof(Fachada.Barrio), "El barrio es obligatorio.");
        if (await Ctx.Fachadas.AnyAsync(x => x.Numero == f.Numero && x.Id != f.Id))
            ModelState.AddModelError(nameof(Fachada.Numero), $"Ya existe la ficha N.º {f.Numero}.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = esNueva ? "Fachada nueva" : f.Titulo;
            return View("Editar", f);
        }

        // El slug se calcula acá y no en la vista: es parte de la URL pública
        // y tiene índice único, así que no puede depender de lo que se tipee.
        if (esNueva || string.IsNullOrWhiteSpace(f.Slug))
        {
            f.Slug = SlugUnico($"{f.Numero}-{f.Titulo}",
                slug => Ctx.Fachadas.Any(x => x.Slug == slug && x.Id != f.Id));
        }

        if (esNueva)
        {
            f.CreadaEn = DateTime.Now;
            Ctx.Fachadas.Add(f);
        }
        else
        {
            f.ActualizadaEn = DateTime.Now;
        }

        await Ctx.SaveChangesAsync();
        TempData["Success"] = esNueva ? "Fachada creada." : "Cambios guardados.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Borrar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrar(int id)
    {
        var f = await Ctx.Fachadas.FindAsync(id);
        if (f is null) return NotFound();

        Ctx.Fachadas.Remove(f);
        await Ctx.SaveChangesAsync();
        TempData["Success"] = $"Se borró «{f.Titulo}».";
        return RedirectToAction(nameof(Index));
    }
}
