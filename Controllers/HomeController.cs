using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class HomeController : ControladorPublico
{
    public HomeController(ApplicationDbContext ctx) : base(ctx) { }

    public async Task<IActionResult> Index()
    {
        var vm = await PrepararAsync(new InicioVm(), "portada");

        vm.Definicion = await Ctx.EncabezadosSeccion.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Clave == "definicion");

        // Tira de fachadas. Se proyecta el flag, nunca los bytes.
        var fachadas = await Ctx.Fachadas.AsNoTracking()
            .Where(f => f.Publicada)
            .OrderByDescending(f => f.Numero)
            .Take(9)
            .Select(f => new
            {
                f.Id, f.Numero, f.Slug, f.Titulo, f.TituloEn, f.TituloPt, f.Barrio,
                TieneImagen = f.ImagenDatos != null,
                f.ImagenUrl, f.ImagenAlt, f.ActualizadaEn, f.CreadaEn
            })
            .ToListAsync();

        vm.TiraArchivo = fachadas.Select(f => new TarjetaVm
        {
            Titulo = vm.T(f.Titulo, f.TituloEn, f.TituloPt),
            Etiqueta = f.Barrio,
            Url = $"/Archivo/Ficha/{f.Slug}",
            ImagenSrc = Imagen.Src("Fachada", f.Id, f.TieneImagen, f.ImagenUrl, f.ActualizadaEn ?? f.CreadaEn),
            ImagenAlt = f.ImagenAlt
        }).ToList();

        vm.Afiches = await AfichesVigentes().Take(3).ToListAsync();

        var sobre = await Ctx.PaginasSobre.AsNoTracking()
            .Select(s => new
            {
                s.Id, s.Titulo, s.TituloEn, s.TituloPt,
                s.Contenido, s.ContenidoEn, s.ContenidoPt, s.Firma,
                TieneImagen = s.ImagenDatos != null,
                s.ImagenUrl, s.ImagenAlt, s.ActualizadaEn
            })
            .FirstOrDefaultAsync();

        if (sobre is not null)
        {
            vm.Sobre = new Models.PaginaSobre
            {
                Id = sobre.Id,
                Titulo = sobre.Titulo, TituloEn = sobre.TituloEn, TituloPt = sobre.TituloPt,
                Contenido = sobre.Contenido, ContenidoEn = sobre.ContenidoEn, ContenidoPt = sobre.ContenidoPt,
                Firma = sobre.Firma, ImagenAlt = sobre.ImagenAlt
            };
            vm.SobreImagenSrc = Imagen.Src("Sobre", sobre.Id, sobre.TieneImagen, sobre.ImagenUrl, sobre.ActualizadaEn);
        }

        vm.Buzon = new BuzonVm
        {
            Titulo = vm.T("El buzón de la Casita", "The Casita mailbox", "A caixa de correio da Casita"),
            Texto = await AjusteAsync("BuzonTexto") ?? string.Empty,
            EtiquetaBoton = vm.T("Suscribirme", "Subscribe", "Inscrever-me"),
            Nota = vm.T("También hay canal de WhatsApp, si preferís que te llegue por ahí.",
                        "There is also a WhatsApp channel, if you would rather get it there.",
                        "Também há canal de WhatsApp, se preferir receber por lá."),
            Origen = "portada",
            VolverA = "/"
        };

        ViewData["Title"] = vm.T("Inicio", "Home", "Início");
        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
