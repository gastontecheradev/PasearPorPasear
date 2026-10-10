using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class PaseosController : ControladorPublico
{
    public PaseosController(ApplicationDbContext ctx) : base(ctx) { }

    public async Task<IActionResult> Index()
    {
        var vm = await PrepararAsync(new PaseosVm(), "paseos");

        var filas = await Ctx.Propuestas.AsNoTracking()
            .Where(p => p.Activa)
            .OrderBy(p => p.Orden)
            .Select(p => new
            {
                p.Id, p.Clave, p.Titulo, p.TituloEn, p.TituloPt,
                p.Etiqueta, p.EtiquetaEn, p.EtiquetaPt,
                p.Descripcion, p.DescripcionEn, p.DescripcionPt,
                p.PendienteDeDefinir,
                TieneImagen = p.ImagenDatos != null,
                p.ImagenUrl, p.ImagenAlt, p.ActualizadaEn, p.CreadaEn,
                Datos = p.Datos.OrderBy(d => d.Orden)
                    .Select(d => new { d.Valor, d.ValorEn, d.ValorPt, d.Etiqueta, d.EtiquetaEn, d.EtiquetaPt })
                    .ToList()
            })
            .ToListAsync();

        vm.Propuestas = filas.Select(p => new PropuestaVm
        {
            Id = p.Id,
            Clave = p.Clave,
            Titulo = vm.T(p.Titulo, p.TituloEn, p.TituloPt),
            Etiqueta = vm.TO(p.Etiqueta, p.EtiquetaEn, p.EtiquetaPt),
            DescripcionHtml = vm.T(p.Descripcion, p.DescripcionEn, p.DescripcionPt),
            ImagenSrc = Imagen.Src("Propuesta", p.Id, p.TieneImagen, p.ImagenUrl, p.ActualizadaEn ?? p.CreadaEn),
            ImagenAlt = p.ImagenAlt,
            PendienteDeDefinir = p.PendienteDeDefinir,
            Datos = p.Datos.Select(d => new DatoVm
            {
                Valor = vm.T(d.Valor, d.ValorEn, d.ValorPt),
                Etiqueta = vm.T(d.Etiqueta, d.EtiquetaEn, d.EtiquetaPt)
            }).ToList(),
            // El Clubcito lleva al formulario de novedades (MailerLite); el resto, a la página de contacto.
            Enlace = p.Clave == ClavePropuesta.Clubcito
                ? "https://preview.mailerlite.io/forms/2403548/194248063120835606/share"
                : "/Contacto",
            TextoEnlace = p.Clave == ClavePropuesta.Clubcito
                ? vm.T("Recibir novedades", "Get updates", "Receber novidades")
                : vm.T("Consultar", "Get in touch", "Consultar")
        }).ToList();

        ViewData["Title"] = vm.TituloSeccion;
        return View(vm);
    }
}
