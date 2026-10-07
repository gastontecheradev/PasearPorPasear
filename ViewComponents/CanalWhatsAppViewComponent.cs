using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;

namespace PasearPorPasear.ViewComponents;

/// <summary>
/// Enlace al canal de WhatsApp del pie de página. Lee el mismo ajuste
/// «CanalWhatsApp» que la página de Contacto, así la dirección se cambia
/// en un solo lugar (/Admin/Paginas/Ajustes). Si el ajuste está vacío
/// o no es una URL https, no se muestra nada.
/// </summary>
public class CanalWhatsAppViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _ctx;
    private readonly ILogger<CanalWhatsAppViewComponent> _logger;

    public CanalWhatsAppViewComponent(ApplicationDbContext ctx, ILogger<CanalWhatsAppViewComponent> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        Ajuste? a;
        try
        {
            a = await _ctx.Ajustes.AsNoTracking().FirstOrDefaultAsync(x => x.Clave == "CanalWhatsApp");
        }
        catch (Exception ex)
        {
            // El pie también se dibuja en la página de error: si la base no
            // responde, se omite el enlace en vez de romper esa página.
            _logger.LogWarning(ex, "No se pudo leer el ajuste CanalWhatsApp.");
            return Content(string.Empty);
        }
        if (a is null) return Content(string.Empty);

        var idioma = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var url = EnlaceHelper.SoloHttps(Traducir.Texto(idioma, a.Valor, a.ValorEn, a.ValorPt));

        // View("Default", url): con View(url) un string se toma como nombre de vista, no como modelo.
        return url is null ? Content(string.Empty) : View("Default", url);
    }
}
