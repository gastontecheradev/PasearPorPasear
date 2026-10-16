using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;

namespace PasearPorPasear.Services;

/// <summary>
/// Los textos fijos del sitio en el idioma de la visita. Se usa en las vistas
/// como @Textos["portada.frase"]. Lee la tabla entera una sola vez por request:
/// son un centenar de filas cortas.
/// </summary>
public interface ITextos
{
    /// <summary>El texto en el idioma actual. Si no se tradujo, en español.</summary>
    string this[string clave] { get; }

    /// <summary>Para los textos con HTML. Sólo los carga Rosalía desde el panel.</summary>
    IHtmlContent Html(string clave);

    /// <summary>Un texto con {correo} reemplazado, en negrita y con el resto escapado.</summary>
    IHtmlContent ConCorreo(string clave);

    /// <summary>Un enlace sólo si es https; si no, null y la vista no lo muestra.</summary>
    string? Enlace(string clave);

    /// <summary>Para los textos de tipo Interruptor.</summary>
    bool Activo(string clave);

    /// <summary>Lo guardado en la base, sin caer al valor de arranque. Para el panel.</summary>
    Task<Dictionary<string, TextoSitio>> GuardadosAsync();
}

public sealed class Textos : ITextos
{
    private readonly ApplicationDbContext _ctx;
    private readonly ILogger<Textos> _logger;
    private Dictionary<string, TextoSitio>? _guardados;

    public Textos(ApplicationDbContext ctx, ILogger<Textos> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    private static string Idioma => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
    {
        "en" => "en",
        "pt" => "pt",
        _ => "es"
    };

    /// <summary>
    /// El menú y el pie también se dibujan en la página de error: si la base
    /// no responde, se sigue con los textos de arranque en vez de romperla.
    /// </summary>
    private Dictionary<string, TextoSitio> Guardados
    {
        get
        {
            if (_guardados is not null) return _guardados;
            try
            {
                _guardados = _ctx.TextosSitio.AsNoTracking()
                    .ToDictionary(t => t.Clave, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudieron leer los textos del sitio; se usan los de arranque.");
                _guardados = new Dictionary<string, TextoSitio>(StringComparer.OrdinalIgnoreCase);
            }
            return _guardados;
        }
    }

    public async Task<Dictionary<string, TextoSitio>> GuardadosAsync() =>
        _guardados ??= await _ctx.TextosSitio.AsNoTracking()
            .ToDictionaryAsync(t => t.Clave, StringComparer.OrdinalIgnoreCase);

    public string this[string clave]
    {
        get
        {
            var def = CatalogoTextos.Buscar(clave);
            // Una clave mal escrita se ve en la página en vez de dejar un hueco.
            if (def is null) return $"[{clave}]";

            // Si hay fila manda la fila, aunque esté vacía: vaciar un texto
            // opcional desde el panel es una decisión, no un olvido.
            if (Guardados.TryGetValue(clave, out var t))
                return def.Traducible ? Traducir.Texto(Idioma, t.Valor, t.ValorEn, t.ValorPt) : t.Valor;

            return def.Traducible ? Traducir.Texto(Idioma, def.Es, def.En, def.Pt) : def.Es;
        }
    }

    public IHtmlContent Html(string clave) => new HtmlString(this[clave]);

    public IHtmlContent ConCorreo(string clave)
    {
        var correo = WebUtility.HtmlEncode(this["general.correo"]);
        var partes = this[clave].Split("{correo}");
        return new HtmlString(string.Join($"<strong>{correo}</strong>", partes.Select(WebUtility.HtmlEncode)));
    }

    public string? Enlace(string clave) => EnlaceHelper.SoloHttps(this[clave]);

    public bool Activo(string clave) => this[clave] == "si";
}
