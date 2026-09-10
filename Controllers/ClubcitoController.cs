using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class ClubcitoController : ControladorPublico
{
    public ClubcitoController(ApplicationDbContext ctx) : base(ctx) { }

    /// <summary>Proyección del listado. Nunca incluye ImagenDatos.</summary>
    private sealed record FilaEncuentro(
        int Id, string Slug, DateTime Fecha, string? Barrio, EstadoEncuentro Estado, int? Asistentes,
        string Titulo, string? TituloEn, string? TituloPt,
        string? Extracto, string? ExtractoEn, string? ExtractoPt,
        string? PuntoEncuentro, string? PuntoEncuentroEn, string? PuntoEncuentroPt,
        bool TieneImagen, string? ImagenUrl, string? ImagenAlt,
        DateTime? ActualizadoEn, DateTime CreadoEn);

    public async Task<IActionResult> Index()
    {
        var vm = await PrepararAsync(new ClubcitoVm(), "clubcito");
        var hoy = DateTime.Today;

        // La fecha manda, no el campo Estado: así el listado no envejece mal
        // si alguien se olvida de marcar un encuentro como realizado.
        var filas = await Ctx.ClubcitoEncuentros.AsNoTracking()
            .Where(c => c.Publicado)
            .Select(c => new FilaEncuentro(
                c.Id, c.Slug, c.Fecha, c.Barrio, c.Estado, c.Asistentes,
                c.Titulo, c.TituloEn, c.TituloPt,
                c.Extracto, c.ExtractoEn, c.ExtractoPt,
                c.PuntoEncuentro, c.PuntoEncuentroEn, c.PuntoEncuentroPt,
                c.ImagenDatos != null, c.ImagenUrl, c.ImagenAlt,
                c.ActualizadoEn, c.CreadoEn))
            .ToListAsync();

        EncuentroVm Armar(FilaEncuentro c) => new()
        {
            Titulo = vm.T(c.Titulo, c.TituloEn, c.TituloPt),
            Url = $"/Clubcito/Encuentro/{c.Slug}",
            Fecha = c.Fecha,
            Barrio = c.Barrio,
            PuntoEncuentro = vm.TO(c.PuntoEncuentro, c.PuntoEncuentroEn, c.PuntoEncuentroPt),
            Extracto = vm.TO(c.Extracto, c.ExtractoEn, c.ExtractoPt),
            ImagenSrc = Imagen.Src("Encuentro", c.Id, c.TieneImagen, c.ImagenUrl, c.ActualizadoEn ?? c.CreadoEn),
            ImagenAlt = c.ImagenAlt,
            Asistentes = c.Asistentes,
            Estado = c.Estado
        };

        vm.Proximos = filas.Where(c => c.Fecha.Date >= hoy)
                           .OrderBy(c => c.Fecha)
                           .Select(Armar).ToList();

        vm.Anteriores = filas.Where(c => c.Fecha.Date < hoy)
                             .OrderByDescending(c => c.Fecha)
                             .Take(6)
                             .Select(Armar).ToList();

        vm.Buzon = new BuzonVm
        {
            Titulo = vm.T("Para enterarte del próximo", "To hear about the next one", "Para saber do próximo"),
            Texto = vm.T("Las salidas del Clubcito se avisan por el buzón de la Casita, una vez por mes. No hay otra lista ni otro grupo.",
                         "Clubcito walks are announced through the Casita mailbox, once a month. There is no other list and no other group.",
                         "As saídas do Clubcito são avisadas pela caixa de correio da Casita, uma vez por mês. Não há outra lista nem outro grupo."),
            EtiquetaBoton = vm.T("Sumarme al buzón", "Join the mailbox", "Entrar na caixa"),
            Origen = "clubcito",
            VolverA = "/Clubcito",
            // El bloque va sobre banda petróleo: el botón principal se perdería.
            ClaseBoton = "boton--claro"
        };

        ViewData["Title"] = vm.TituloSeccion;
        return View(vm);
    }

    [Route("Clubcito/Encuentro/{slug}")]
    public async Task<IActionResult> Encuentro(string slug)
    {
        var c = await Ctx.ClubcitoEncuentros.AsNoTracking()
            .Where(x => x.Slug == slug && x.Publicado)
            .Select(x => new
            {
                x.Id, x.Slug, x.Fecha, x.Barrio, x.Estado, x.Asistentes, x.MapaEmbedUrl,
                x.Titulo, x.TituloEn, x.TituloPt,
                x.Contenido, x.ContenidoEn, x.ContenidoPt,
                x.PuntoEncuentro, x.PuntoEncuentroEn, x.PuntoEncuentroPt,
                TieneImagen = x.ImagenDatos != null,
                x.ImagenUrl, x.ImagenAlt, x.ActualizadoEn, x.CreadoEn
            })
            .FirstOrDefaultAsync();

        if (c is null) return NotFound();

        var vm = await PrepararAsync(new EncuentroDetalleVm
        {
            ContenidoHtml = Traducir.Texto(Idioma, c.Contenido, c.ContenidoEn, c.ContenidoPt),
            MapaEmbedUrl = c.MapaEmbedUrl
        });

        vm.Encuentro = new EncuentroVm
        {
            Titulo = vm.T(c.Titulo, c.TituloEn, c.TituloPt),
            Url = $"/Clubcito/Encuentro/{c.Slug}",
            Fecha = c.Fecha,
            Barrio = c.Barrio,
            PuntoEncuentro = vm.TO(c.PuntoEncuentro, c.PuntoEncuentroEn, c.PuntoEncuentroPt),
            ImagenSrc = Imagen.Src("Encuentro", c.Id, c.TieneImagen, c.ImagenUrl, c.ActualizadoEn ?? c.CreadoEn),
            ImagenAlt = c.ImagenAlt,
            Asistentes = c.Asistentes,
            Estado = c.Estado
        };

        ViewData["Title"] = vm.Encuentro.Titulo;
        return View(vm);
    }
}
