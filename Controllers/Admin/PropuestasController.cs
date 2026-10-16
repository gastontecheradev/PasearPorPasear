using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers.Admin;

/// <summary>
/// Las cuatro propuestas de ¿Paseamos? Son fijas: se editan, no se
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
        // Sin ImagenDatos: la miniatura la pide el navegador por su URL.
        var filas = await Ctx.Propuestas.AsNoTracking()
            .OrderBy(p => p.Orden)
            .Select(p => new
            {
                p.Id, p.Titulo, p.Etiqueta, p.Orden, p.Activa, p.PendienteDeDefinir,
                Casilleros = p.Datos.Count,
                TieneImagen = p.ImagenDatos != null,
                p.ImagenUrl, p.ActualizadaEn, p.CreadaEn
            })
            .ToListAsync();

        var vm = filas.Select(p => new FilaAdminVm
        {
            Id = p.Id,
            Titulo = p.Titulo,
            Detalle = p.Etiqueta,
            ImagenSrc = Imagen.Src("Propuesta", p.Id, p.TieneImagen, p.ImagenUrl, p.ActualizadaEn ?? p.CreadaEn),
            Columna2 = p.Orden.ToString(),
            Columna3 = p.Casilleros == 1 ? "1 casillero" : $"{p.Casilleros} casilleros",
            Estado = !p.Activa ? "Oculta" : p.PendienteDeDefinir ? "Falta definir" : "Activa",
            ClaseEstado = !p.Activa ? "pastilla--vencido" : p.PendienteDeDefinir ? "pastilla--espera" : "pastilla--activo",
            Filtro = !p.Activa ? "oculta" : p.PendienteDeDefinir ? "pendiente" : "activa"
        }).ToList();

        ViewData["Title"] = "¿Paseamos?";
        return View("ListadoAdmin", new ListadoAdminVm
        {
            Pagina = "paseos",
            Titulo = "¿Paseamos?",
            Subtitulo = "Las cuatro propuestas son fijas: se editan, no se crean ni se borran.",
            UrlBase = "/Admin/Propuestas",
            Filtros = { new("activa", "Activas"), new("pendiente", "Falta definir"), new("oculta", "Ocultas") },
            Buscador = "Buscar por nombre…",
            Vacio = "No hay propuestas cargadas.",
            EncabezadoPrincipal = "Propuesta",
            Encabezado2 = "Orden",
            Encabezado3 = "Datos",
            Filas = vm
        });
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
