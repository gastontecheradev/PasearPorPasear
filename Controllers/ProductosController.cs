using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class ProductosController : ControladorPublico
{
    public ProductosController(ApplicationDbContext ctx) : base(ctx) { }

    public async Task<IActionResult> Index()
    {
        var vm = await PrepararAsync(new ProductosVm(), "productos");

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
            Titulo = vm.T("Te avisamos cuando salgan", "We'll let you know when they're out", "Avisamos quando saírem"),
            Texto = vm.T("Todavía estamos con las pruebas de impresión. Cuando estén listos, el aviso sale primero por el buzón de la Casita.",
                         "We are still on the printing tests. When they are ready, the news goes out first through the Casita mailbox.",
                         "Ainda estamos nos testes de impressão. Quando estiverem prontos, o aviso sai primeiro pela caixa de correio da Casita."),
            EtiquetaBoton = vm.T("Avisame", "Notify me", "Avise-me"),
            Origen = "productos",
            VolverA = "/Productos",
            Clase = "recuadro"
        };

        ViewData["Title"] = vm.TituloSeccion;
        return View(vm);
    }
}
