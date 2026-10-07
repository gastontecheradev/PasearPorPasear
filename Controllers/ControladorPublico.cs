using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;
using System.Globalization;

namespace PasearPorPasear.Controllers;

/// <summary>
/// Base de los controladores públicos: resuelve el idioma y carga el
/// encabezado editable de cada sección.
/// </summary>
public abstract class ControladorPublico : Controller
{
    protected readonly ApplicationDbContext Ctx;

    protected ControladorPublico(ApplicationDbContext ctx) { Ctx = ctx; }

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

    /// <summary>Completa idioma y encabezado de sección en cualquier vista.</summary>
    protected async Task<T> PrepararAsync<T>(T vm, string? claveEncabezado = null) where T : VistaBase
    {
        vm.Idioma = Idioma;
        if (claveEncabezado is not null)
            vm.Encabezado = await Ctx.EncabezadosSeccion
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Clave == claveEncabezado);
        return vm;
    }

    /// <summary>Valor de un ajuste, en el idioma actual.</summary>
    protected async Task<string?> AjusteAsync(string clave)
    {
        var a = await Ctx.Ajustes.AsNoTracking().FirstOrDefaultAsync(x => x.Clave == clave);
        if (a is null) return null;
        var v = Traducir.Texto(Idioma, a.Valor, a.ValorEn, a.ValorPt);
        return string.IsNullOrWhiteSpace(v) ? null : v;
    }

    /// <summary>
    /// Canal de WhatsApp desde Ajustes, sólo si es una URL https válida.
    /// Null si no hay canal: las vistas no lo enlazan ni lo mencionan.
    /// </summary>
    protected async Task<string?> CanalWhatsAppAsync() =>
        EnlaceHelper.SoloHttps(await AjusteAsync("CanalWhatsApp"));

    /// <summary>Nota del buzón que invita al canal; null si no hay canal.</summary>
    protected static string? NotaCanalWhatsApp(VistaBase vm, string? canal) =>
        canal is null
            ? null
            : vm.T("También hay canal de WhatsApp, si preferís que te llegue por ahí.",
                   "There is also a WhatsApp channel, if you would rather get it there.",
                   "Também há canal de WhatsApp, se preferir receber por lá.");

    /// <summary>Los afiches que se muestran en el muro, ya filtrados por vigencia.</summary>
    protected IQueryable<CarteleraAfiche> AfichesVigentes() =>
        Ctx.CarteleraAfiches
           .AsNoTracking()
           .Where(a => a.Estado == EstadoAfiche.Publicado &&
                       (a.VigenteHasta == null || a.VigenteHasta >= DateTime.Today))
           .OrderBy(a => a.Orden)
           .ThenByDescending(a => a.CreadoEn);
}
