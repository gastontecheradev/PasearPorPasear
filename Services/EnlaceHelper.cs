using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace PasearPorPasear.Services;

public static class EnlaceHelper
{
    /// <summary>
    /// Devuelve la URL sólo si es absoluta y https; si no, null.
    /// Para enlaces que se cargan desde el panel (textos del sitio) y se publican
    /// tal cual: evita que un valor mal tipeado o un «javascript:» llegue al sitio.
    /// </summary>
    public static string? SoloHttps(string? valor) =>
        Uri.TryCreate(valor?.Trim(), UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps
            ? uri.ToString()
            : null;

    /// <summary>[texto](dirección), en una sola línea.</summary>
    private static readonly Regex Marca = new(@"\[([^\[\]\r\n]+)\]\(([^()\s]+)\)", RegexOptions.Compiled);

    /// <summary>
    /// Convierte cada [texto](https://…) o [texto](/Paseos) en un enlace en negrita.
    /// Con esHtml = false (los textos comunes) todo lo demás se escapa; con true
    /// (los campos que ya admiten HTML) se deja como está. Una marca con una
    /// dirección que no sea https ni una página del sitio queda como texto.
    /// </summary>
    public static string ConEnlaces(string? texto, bool esHtml = false)
    {
        if (string.IsNullOrEmpty(texto)) return string.Empty;

        string Escapar(string s) => esHtml ? s : WebUtility.HtmlEncode(s);

        var html = new StringBuilder();
        var desde = 0;
        foreach (Match m in Marca.Matches(texto))
        {
            var destino = Destino(m.Groups[2].Value);
            if (destino is null) continue;

            html.Append(Escapar(texto[desde..m.Index]));
            html.Append("<a class=\"enlace-texto\" href=\"").Append(WebUtility.HtmlEncode(destino)).Append('"');
            // Lo de afuera se abre aparte para no perder la página del sitio.
            if (!destino.StartsWith('/')) html.Append(" target=\"_blank\" rel=\"noopener\"");
            html.Append('>').Append(Escapar(m.Groups[1].Value)).Append("</a>");
            desde = m.Index + m.Length;
        }
        html.Append(Escapar(texto[desde..]));
        return html.ToString();
    }

    /// <summary>
    /// https:// o una ruta del sitio («/Paseos»). Se rechaza «//» y «/\»: el
    /// navegador los toma como otro dominio.
    /// </summary>
    private static string? Destino(string url) =>
        url.StartsWith('/') && !url.StartsWith("//") && !url.Contains('\\')
            ? url
            : SoloHttps(url);
}
