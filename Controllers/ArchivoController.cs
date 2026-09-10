using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class ArchivoController : ControladorPublico
{
    private const int PorPagina = 12;

    public ArchivoController(ApplicationDbContext ctx) : base(ctx) { }

    /// <summary>Proyección del listado. Nunca incluye ImagenDatos.</summary>
    private sealed record FilaFachada(
        int Id, int Numero, string Slug, string Barrio, string? Calle, DateTime FechaEncontrada,
        string Titulo, string? TituloEn, string? TituloPt,
        string? Extracto, string? ExtractoEn, string? ExtractoPt,
        bool TieneImagen, string? ImagenUrl, string? ImagenAlt,
        DateTime? ActualizadaEn, DateTime CreadaEn);

    private static IQueryable<FilaFachada> Proyectar(IQueryable<Fachada> q) =>
        q.Select(f => new FilaFachada(
            f.Id, f.Numero, f.Slug, f.Barrio, f.Calle, f.FechaEncontrada,
            f.Titulo, f.TituloEn, f.TituloPt,
            f.Extracto, f.ExtractoEn, f.ExtractoPt,
            f.ImagenDatos != null, f.ImagenUrl, f.ImagenAlt,
            f.ActualizadaEn, f.CreadaEn));

    private FichaVm AFicha(FilaFachada f, VistaBase vm) => new()
    {
        Numero = f.Numero,
        Titulo = vm.T(f.Titulo, f.TituloEn, f.TituloPt),
        Url = $"/Archivo/Ficha/{f.Slug}",
        Barrio = f.Barrio,
        Calle = f.Calle,
        Extracto = vm.TO(f.Extracto, f.ExtractoEn, f.ExtractoPt),
        ImagenSrc = Imagen.Src("Fachada", f.Id, f.TieneImagen, f.ImagenUrl, f.ActualizadaEn ?? f.CreadaEn),
        ImagenAlt = f.ImagenAlt,
        Fecha = f.FechaEncontrada
    };

    public async Task<IActionResult> Index(string? barrio = null, int pagina = 1)
    {
        if (pagina < 1) pagina = 1;

        var vm = await PrepararAsync(new ArchivoVm { BarrioActivo = barrio, Pagina = pagina }, "archivo");

        var publicadas = Ctx.Fachadas.AsNoTracking().Where(f => f.Publicada);

        vm.TotalFachadas = await publicadas.CountAsync();
        vm.Barrios = await publicadas.Select(f => f.Barrio).Distinct().OrderBy(b => b).ToListAsync();
        vm.TotalBarrios = vm.Barrios.Count;
        vm.DesdeAnio = vm.TotalFachadas == 0 ? null : await publicadas.MinAsync(f => f.FechaEncontrada.Year);

        var filtradas = string.IsNullOrWhiteSpace(barrio)
            ? publicadas
            : publicadas.Where(f => f.Barrio == barrio);

        var total = await filtradas.CountAsync();
        vm.TotalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)PorPagina));
        if (pagina > vm.TotalPaginas) { vm.Pagina = pagina = vm.TotalPaginas; }

        var filas = await Proyectar(filtradas.OrderByDescending(f => f.Numero))
            .Skip((pagina - 1) * PorPagina)
            .Take(PorPagina)
            .ToListAsync();

        vm.Fichas = filas.Select(f => AFicha(f, vm)).ToList();

        ViewData["Title"] = vm.TituloSeccion;
        return View(vm);
    }

    [Route("Archivo/Ficha/{slug}")]
    public async Task<IActionResult> Ficha(string slug)
    {
        var f = await Ctx.Fachadas.AsNoTracking()
            .Where(x => x.Slug == slug && x.Publicada)
            .Select(x => new
            {
                Fila = new FilaFachada(
                    x.Id, x.Numero, x.Slug, x.Barrio, x.Calle, x.FechaEncontrada,
                    x.Titulo, x.TituloEn, x.TituloPt,
                    x.Extracto, x.ExtractoEn, x.ExtractoPt,
                    x.ImagenDatos != null, x.ImagenUrl, x.ImagenAlt,
                    x.ActualizadaEn, x.CreadaEn),
                x.Contenido, x.ContenidoEn, x.ContenidoPt,
                x.AnioConstruccion, x.MapaEmbedUrl
            })
            .FirstOrDefaultAsync();

        if (f is null) return NotFound();

        var vm = await PrepararAsync(new FichaDetalleVm
        {
            ContenidoHtml = Traducir.Texto(Idioma, f.Contenido, f.ContenidoEn, f.ContenidoPt),
            AnioConstruccion = f.AnioConstruccion,
            MapaEmbedUrl = f.MapaEmbedUrl
        });

        vm.Ficha = AFicha(f.Fila, vm);

        var publicadas = Ctx.Fachadas.AsNoTracking().Where(x => x.Publicada);

        // Otras del mismo barrio, para el bloque «de la misma cuadra».
        var cercanas = await Proyectar(publicadas
                .Where(x => x.Barrio == f.Fila.Barrio && x.Id != f.Fila.Id)
                .OrderByDescending(x => x.Numero)
                .Take(3))
            .ToListAsync();
        vm.Cercanas = cercanas.Select(c => AFicha(c, vm)).ToList();

        // Anterior y siguiente por número de ficha.
        var anterior = await Proyectar(publicadas
            .Where(x => x.Numero < f.Fila.Numero).OrderByDescending(x => x.Numero).Take(1)).FirstOrDefaultAsync();
        var siguiente = await Proyectar(publicadas
            .Where(x => x.Numero > f.Fila.Numero).OrderBy(x => x.Numero).Take(1)).FirstOrDefaultAsync();

        vm.Anterior = anterior is null ? null : AFicha(anterior, vm);
        vm.Siguiente = siguiente is null ? null : AFicha(siguiente, vm);

        ViewData["Title"] = vm.Ficha.Titulo;
        return View(vm);
    }
}
