using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class SobreController : ControladorPublico
{
    public SobreController(ApplicationDbContext ctx) : base(ctx) { }

    public async Task<IActionResult> Index()
    {
        var fila = await Ctx.PaginasSobre.AsNoTracking()
            .Select(s => new
            {
                s.Id, s.Titulo, s.TituloEn, s.TituloPt,
                s.Contenido, s.ContenidoEn, s.ContenidoPt, s.Firma,
                TieneImagen = s.ImagenDatos != null,
                s.ImagenUrl, s.ImagenAlt, s.ActualizadaEn
            })
            .FirstOrDefaultAsync();

        if (fila is null) return NotFound();

        var vm = await PrepararAsync(new SobreVm
        {
            Pagina = new PaginaSobre
            {
                Id = fila.Id,
                Titulo = fila.Titulo, TituloEn = fila.TituloEn, TituloPt = fila.TituloPt,
                Contenido = fila.Contenido, ContenidoEn = fila.ContenidoEn, ContenidoPt = fila.ContenidoPt,
                Firma = fila.Firma, ImagenAlt = fila.ImagenAlt
            },
            ImagenSrc = Imagen.Src("Sobre", fila.Id, fila.TieneImagen, fila.ImagenUrl, fila.ActualizadaEn)
        });

        ViewData["Title"] = vm.Titulo;
        return View(vm);
    }
}
