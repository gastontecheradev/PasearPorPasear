using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers.Admin;

[Route("Admin/Encuentros")]
public class EncuentrosController : ControladorAdmin
{
    public EncuentrosController(ApplicationDbContext ctx, IServicioImagenes imagenes)
        : base(ctx, imagenes) { }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var hoy = DateTime.Today;

        var filas = await Ctx.ClubcitoEncuentros.AsNoTracking()
            .OrderByDescending(c => c.Fecha)
            .Select(c => new
            {
                c.Id, c.Titulo, c.Fecha, c.Barrio, c.Asistentes, c.Estado, c.Publicado,
                TieneImagen = c.ImagenDatos != null,
                c.ImagenUrl, c.ActualizadoEn, c.CreadoEn
            })
            .ToListAsync();

        var vm = filas.Select(c => new FilaAdminVm
        {
            Id = c.Id,
            Titulo = c.Titulo,
            Detalle = c.Barrio,
            ImagenSrc = Imagen.Src("Encuentro", c.Id, c.TieneImagen, c.ImagenUrl, c.ActualizadoEn ?? c.CreadoEn),
            Columna2 = c.Fecha.ToString("dd/MM/yyyy"),
            Columna3 = c.Asistentes.HasValue ? $"{c.Asistentes} personas" : "—",
            Estado = !c.Publicado ? "Borrador"
                   : c.Estado == EstadoEncuentro.SinDefinir ? "Sin definir"
                   : c.Fecha.Date >= hoy ? "Por venir" : "Realizado",
            ClaseEstado = !c.Publicado ? "pastilla--espera"
                        : c.Fecha.Date >= hoy ? "pastilla--activo" : "pastilla--vencido"
        }).ToList();

        ViewData["Title"] = "El Clubcito";
        return View("ListadoAdmin", new ListadoAdminVm
        {
            Titulo = "El Clubcito",
            Subtitulo = $"{vm.Count} encuentro(s).",
            UrlBase = "/Admin/Encuentros",
            UrlNueva = "/Admin/Encuentros/Nuevo",
            TextoNueva = "Encuentro nuevo",
            Buscador = "Buscar por título o barrio…",
            Vacio = "Todavía no hay ningún encuentro.",
            EncabezadoPrincipal = "Encuentro",
            Encabezado2 = "Fecha",
            Encabezado3 = "Asistentes",
            Filas = vm
        });
    }

    [HttpGet("Nuevo")]
    public IActionResult Nuevo()
    {
        ViewData["Title"] = "Encuentro nuevo";
        // Por defecto, el mismo día del mes siguiente: el Clubcito sale una vez al mes.
        return View("Editar", new ClubcitoEncuentro { Fecha = DateTime.Today.AddMonths(1) });
    }

    [HttpGet("Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id)
    {
        var c = await Ctx.ClubcitoEncuentros.FindAsync(id);
        if (c is null) return NotFound();
        ViewData["Title"] = c.Titulo;
        return View(c);
    }

    [HttpPost("Guardar")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public async Task<IActionResult> Guardar(ClubcitoEncuentro entrada, IFormFile? imagen, bool borrarImagen = false)
    {
        var esNuevo = entrada.Id == 0;
        var c = esNuevo ? new ClubcitoEncuentro() : await Ctx.ClubcitoEncuentros.FindAsync(entrada.Id);
        if (c is null) return NotFound();

        c.Titulo = entrada.Titulo?.Trim() ?? string.Empty;
        c.TituloEn = entrada.TituloEn?.Trim() ?? string.Empty;
        c.TituloPt = entrada.TituloPt?.Trim() ?? string.Empty;
        c.Fecha = entrada.Fecha;
        c.Barrio = entrada.Barrio?.Trim();
        c.PuntoEncuentro = entrada.PuntoEncuentro?.Trim();
        c.PuntoEncuentroEn = entrada.PuntoEncuentroEn?.Trim();
        c.PuntoEncuentroPt = entrada.PuntoEncuentroPt?.Trim();
        c.Extracto = entrada.Extracto?.Trim();
        c.ExtractoEn = entrada.ExtractoEn?.Trim();
        c.ExtractoPt = entrada.ExtractoPt?.Trim();
        c.Contenido = entrada.Contenido ?? string.Empty;
        c.ContenidoEn = entrada.ContenidoEn ?? string.Empty;
        c.ContenidoPt = entrada.ContenidoPt ?? string.Empty;
        c.MapaEmbedUrl = entrada.MapaEmbedUrl?.Trim();
        c.Asistentes = entrada.Asistentes;
        c.Estado = entrada.Estado;
        c.Publicado = entrada.Publicado;
        c.ImagenAlt = entrada.ImagenAlt?.Trim();

        if (!await AplicarImagenAsync(c, imagen, borrarImagen))
            return View("Editar", c);

        if (string.IsNullOrWhiteSpace(c.Titulo))
            ModelState.AddModelError(nameof(ClubcitoEncuentro.Titulo), "El título es obligatorio.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = esNuevo ? "Encuentro nuevo" : c.Titulo;
            return View("Editar", c);
        }

        if (esNuevo || string.IsNullOrWhiteSpace(c.Slug))
        {
            c.Slug = SlugUnico($"{c.Titulo}-{c.Fecha:yyyy-MM}",
                slug => Ctx.ClubcitoEncuentros.Any(x => x.Slug == slug && x.Id != c.Id));
        }

        if (esNuevo)
        {
            c.CreadoEn = DateTime.Now;
            Ctx.ClubcitoEncuentros.Add(c);
        }
        else
        {
            c.ActualizadoEn = DateTime.Now;
        }

        await Ctx.SaveChangesAsync();
        TempData["Success"] = esNuevo ? "Encuentro creado." : "Cambios guardados.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Borrar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrar(int id)
    {
        var c = await Ctx.ClubcitoEncuentros.FindAsync(id);
        if (c is null) return NotFound();

        Ctx.ClubcitoEncuentros.Remove(c);
        await Ctx.SaveChangesAsync();
        TempData["Success"] = $"Se borró «{c.Titulo}».";
        return RedirectToAction(nameof(Index));
    }
}
