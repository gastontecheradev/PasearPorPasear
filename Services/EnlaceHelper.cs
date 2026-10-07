namespace PasearPorPasear.Services;

public static class EnlaceHelper
{
    /// <summary>
    /// Devuelve la URL sólo si es absoluta y https; si no, null.
    /// Para enlaces que se cargan desde el panel (Ajustes) y se publican
    /// tal cual: evita que un valor mal tipeado o un «javascript:» llegue al sitio.
    /// </summary>
    public static string? SoloHttps(string? valor) =>
        Uri.TryCreate(valor?.Trim(), UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps
            ? uri.ToString()
            : null;
}
