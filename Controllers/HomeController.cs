using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class HomeController : ControladorPublico
{
    public HomeController(ApplicationDbContext ctx, ITextos textos) : base(ctx, textos) { }

    public async Task<IActionResult> Index()
    {
        var vm = Preparar(new InicioVm());

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
            vm.Sobre = new PaginaSobre
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
            Titulo = Textos["portada.buzon-titulo"],
            Texto = Textos["portada.buzon-texto"],
            EtiquetaBoton = Textos["portada.buzon-boton"],
            // La nota invita al canal: sin canal cargado, no se menciona.
            Nota = Textos.Enlace("general.whatsapp") is null ? null : Textos["portada.buzon-nota"],
            // El formulario está embebido en Contacto; desde la portada se va ahí.
            Enlace = "/Contacto#buzon"
        };

        ViewData["Title"] = Textos["general.menu-inicio"];
        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
