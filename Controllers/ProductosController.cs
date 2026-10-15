using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class ProductosController : ControladorPublico
{
    public ProductosController(ApplicationDbContext ctx, ITextos textos) : base(ctx, textos) { }

    public async Task<IActionResult> Index()
    {
        var vm = Preparar(new ProductosVm());

        var filas = await Ctx.Productos.AsNoTracking()
            .Where(p => p.Visible)
            .OrderBy(p => p.Orden)
            .Select(p => new
            {
                p.Id, p.Titulo, p.TituloEn, p.TituloPt,
                p.Descripcion, p.DescripcionEn, p.DescripcionPt,
                p.ColorPanel, p.Precio, p.Estado, p.ArchivoUrl,
                TieneImagen = p.ImagenDatos != null,
                p.ImagenUrl, p.ImagenAlt, p.ActualizadoEn, p.CreadoEn
            })
            .ToListAsync();

        vm.Productos = filas.Select(p => new ProductoVm
        {
            Titulo = vm.T(p.Titulo, p.TituloEn, p.TituloPt),
            Descripcion = vm.TO(p.Descripcion, p.DescripcionEn, p.DescripcionPt),
            ImagenSrc = Imagen.Src("Producto", p.Id, p.TieneImagen, p.ImagenUrl, p.ActualizadoEn ?? p.CreadoEn),
            ImagenAlt = p.ImagenAlt,
            ClasePanel = p.ColorPanel switch
            {
                ColorAfiche.Petroleo => "producto--petroleo",
                ColorAfiche.Durazno  => "producto--durazno",
                ColorAfiche.Coral    => "producto--durazno",
                ColorAfiche.Rayas    => "producto--piedra",
                ColorAfiche.Crema    => "producto--crema2",
                _                    => "producto--mostaza"
            },
            Precio = p.Precio,
            Estado = p.Estado,
            ArchivoUrl = p.ArchivoUrl
        }).ToList();

        vm.Buzon = new BuzonVm
        {
            Titulo = Textos["productos.buzon-titulo"],
            Texto = Textos["productos.buzon-texto"],
            EtiquetaBoton = Textos["productos.buzon-boton"],
            Enlace = Textos.Enlace("general.buzon-formulario") ?? "/Contacto#buzon",
            Clase = "recuadro"
        };

        ViewData["Title"] = Textos["productos.titulo"];
        return View(vm);
    }
}
