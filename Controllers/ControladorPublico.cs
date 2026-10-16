using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;
using System.Globalization;

namespace PasearPorPasear.Controllers;

/// <summary>
/// Base de los controladores públicos: resuelve el idioma y da acceso a los
/// textos editables del sitio (las vistas los leen directo con @Textos).
/// </summary>
public abstract class ControladorPublico : Controller
{
    protected readonly ApplicationDbContext Ctx;
    protected readonly ITextos Textos;

    protected ControladorPublico(ApplicationDbContext ctx, ITextos textos)
    {
        Ctx = ctx;
        Textos = textos;
    }

    /// <summary>
    /// El middleware de localización ya dejó puesta la cultura actual;
    /// no hace falta ir a buscar la cookie a mano.
    /// </summary>
    protected string Idioma => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
    {
        "en" => "en",
        "pt" => "pt",
        _    => "es"
    };

    /// <summary>Completa el idioma en cualquier vista.</summary>
    protected T Preparar<T>(T vm) where T : VistaBase
    {
        vm.Idioma = Idioma;
        return vm;
    }

    /// <summary>
    /// Los afiches que se muestran en el muro, ya filtrados por vigencia.
    /// Sin los bytes de la imagen: el navegador la pide aparte por su URL.
    /// </summary>
    protected IQueryable<AficheVm> AfichesVigentes() =>
        Ctx.CarteleraAfiches
           .AsNoTracking()
           .Where(a => a.Estado == EstadoAfiche.Publicado &&
                       a.ImagenDatos != null &&
                       (a.VigenteHasta == null || a.VigenteHasta >= DateTime.Today))
           .OrderBy(a => a.Orden)
           .ThenByDescending(a => a.CreadoEn)
           .Select(a => new AficheVm
           {
               Id = a.Id,
               Titulo = a.Titulo,
               ImagenAlt = a.ImagenAlt,
               TieneImagen = true,
               Sello = a.ActualizadoEn ?? a.CreadoEn,
               VigenteHasta = a.VigenteHasta,
               Estado = a.Estado,
               Categoria = a.Categoria
           });
}
