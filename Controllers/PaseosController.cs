using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class PaseosController : ControladorPublico
{
    public PaseosController(ApplicationDbContext ctx, ITextos textos) : base(ctx, textos) { }

    public async Task<IActionResult> Index()
    {
        var vm = Preparar(new PaseosVm());

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
            // El Clubcito y Al sobre llevan a sus formularios de MailerLite; el resto, a la página de contacto.
            Enlace = p.Clave switch
            {
                ClavePropuesta.Clubcito => Textos.Enlace("paseos.enlace-clubcito") ?? "/Contacto",
                ClavePropuesta.AlSobre => Textos.Enlace("paseos.enlace-alsobre") ?? "/Contacto",
                _ => "/Contacto"
            },
            TextoEnlace = p.Clave switch
            {
                ClavePropuesta.Clubcito => Textos["paseos.boton-clubcito"],
                ClavePropuesta.AlSobre => Textos["paseos.boton-alsobre"],
                _ => Textos["paseos.boton-consultar"]
            }
        }).ToList();

        ViewData["Title"] = Textos["paseos.titulo"];
        return View(vm);
    }

    /// <summary>
    /// El Clubcito ya no tiene página propia: sigue como propuesta de
    /// ¿Paseamos?. Las direcciones viejas (y sus encuentros) llevan ahí.
    /// </summary>
    [HttpGet("Clubcito/{**resto}")]
    public IActionResult ClubcitoViejo() => RedirectPermanent("/Paseos");
}
