using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace PasearPorPasear.Services;

/// <summary>
/// Los textos que se escriben en el panel se guardan como texto simple: cada
/// renglón (un Enter en el editor) es un renglón en la página, y los enlaces
/// van como [texto](dirección). Acá se pasan a HTML, escapando todo lo demás,
/// así que nada de lo que se escriba en el panel llega al sitio como código.
/// </summary>
public static class TextoConFormato
{
    /// <summary>[texto](dirección), en un solo renglón.</summary>
    private static readonly Regex Marca = new(@"\[([^\[\]\r\n]+)\]\(([^()\s]+)\)", RegexOptions.Compiled);

    /// <summary>
    /// Cada renglón, un párrafo; cada renglón en blanco, un párrafo vacío que
    /// ocupa su lugar. Para los textos largos: Sobre PPP, fichas, propuestas.
    /// </summary>
    public static string Parrafos(string? texto) =>
        string.Concat(Renglones(texto).Select(r => r.Length == 0 ? "<p><br></p>" : $"<p>{Renglon(r)}</p>"));

    /// <summary>
    /// Los renglones separados por saltos de línea, para textos que van dentro
    /// de un solo párrafo. Un renglón en blanco deja dos saltos seguidos.
    /// </summary>
    public static string EnUnParrafo(string? texto) =>
        string.Join("<br>", Renglones(texto).Select(Renglon));

    /// <summary>
    /// Los renglones tal como se escribieron: los vacíos del medio cuentan
    /// (cada Enter de más es un renglón en blanco); los del principio y del
    /// final, no.
    /// </summary>
    private static List<string> Renglones(string? texto)
    {
        var renglones = (texto ?? string.Empty).Split('\n').Select(r => r.Trim()).ToList();
        var desde = renglones.FindIndex(r => r.Length > 0);
        if (desde < 0) return new List<string>();
        var hasta = renglones.FindLastIndex(r => r.Length > 0);
        return renglones.GetRange(desde, hasta - desde + 1);
    }

    /// <summary>
    /// Un renglón escapado, con los enlaces armados. Una marca con una dirección
    /// que no sea https ni una página del sitio queda como texto.
    /// </summary>
    private static string Renglon(string renglon)
    {
        var html = new StringBuilder();
        var desde = 0;
        foreach (Match m in Marca.Matches(renglon))
        {
            var destino = EnlaceHelper.DestinoPermitido(m.Groups[2].Value);
            if (destino is null) continue;

            html.Append(WebUtility.HtmlEncode(renglon[desde..m.Index]));
            html.Append("<a class=\"enlace-texto\" href=\"").Append(WebUtility.HtmlEncode(destino)).Append('"');
            // Lo de afuera se abre aparte para no perder la página del sitio.
            if (!destino.StartsWith('/')) html.Append(" target=\"_blank\" rel=\"noopener\"");
            html.Append('>').Append(WebUtility.HtmlEncode(m.Groups[1].Value)).Append("</a>");
            desde = m.Index + m.Length;
        }
        html.Append(WebUtility.HtmlEncode(renglon[desde..]));
        return html.ToString();
    }

    // ── Formato anterior ──────────────────────────────────────────
    // Antes los textos largos se escribían en HTML, con cada párrafo entre
    // <p> y </p>. Lo guardado así se pasa una vez al formato actual.

    private static readonly Regex EtiquetaVieja = new(
        @"</?(p|br|a|strong|em|b|i|span|div|ul|ol|li|h[1-6])\b[^>]*>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex EnlaceViejo = new(
        @"<a\b[^>]*?\bhref\s*=\s*[""']([^""']*)[""'][^>]*>(.*?)</a>",
        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

    public static bool TieneHtmlViejo(string? texto) => texto is not null && EtiquetaVieja.IsMatch(texto);

    public static string DesdeHtml(string html)
    {
        // Los saltos de línea dentro del HTML no eran renglones: los marcaban las etiquetas.
        var t = Regex.Replace(html, @"\s*\r?\n\s*", " ");

        t = EnlaceViejo.Replace(t, m =>
        {
            var texto = WebUtility.HtmlDecode(Regex.Replace(m.Groups[2].Value, "<[^>]+>", ""))
                .Replace('[', '(').Replace(']', ')').Trim();
            var destino = EnlaceHelper.DestinoPermitido(WebUtility.HtmlDecode(m.Groups[1].Value));
            if (destino is null || texto.Length == 0) return texto;
            return $"[{texto}]({destino.Replace("(", "%28").Replace(")", "%29").Replace(" ", "%20")})";
        });

        t = Regex.Replace(t, @"<br\s*/?>|</(p|div|li|h[1-6])>", "\n", RegexOptions.IgnoreCase);
        t = Regex.Replace(t, "<[^>]+>", "");
        t = WebUtility.HtmlDecode(t);
        return string.Join("\n", t.Split('\n').Select(r => r.Trim()).Where(r => r.Length > 0));
    }
}
