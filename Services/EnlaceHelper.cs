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

    /// <summary>
    /// Para los enlaces dentro de los textos: https:// o una ruta del sitio
    /// («/Paseos»). Se rechaza «//» y «/\»: el navegador los toma como otro dominio.
    /// </summary>
    public static string? DestinoPermitido(string url) =>
        url.StartsWith('/') && !url.StartsWith("//") && !url.Contains('\\')
            ? url
            : SoloHttps(url);
}
