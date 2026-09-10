namespace PasearPorPasear.Services;

/// <summary>Una imagen leída del formulario, lista para guardar en la base.</summary>
public record ImagenSubida(byte[] Datos, string Tipo);

public interface IServicioImagenes
{
    /// <summary>
    /// Valida y lee el archivo subido. Devuelve null si no vino archivo, si está
    /// vacío, si la extensión no está permitida o si pasa el tamaño máximo.
    /// Nunca lanza por un archivo inválido: el controlador decide qué hacer con el null.
    /// </summary>
    Task<ImagenSubida?> LeerAsync(IFormFile? archivo, CancellationToken ct = default);

    /// <summary>Motivo del último rechazo, para mostrárselo a Rosalía en el formulario.</summary>
    string? UltimoMotivo { get; }
}

public class ServicioImagenes : IServicioImagenes
{
    /// <summary>
    /// 6 MB por imagen. El límite importa porque las imágenes van dentro de la
    /// base: en el tier Basic de Azure SQL hay 2 GB en total para todo.
    /// </summary>
    public const long TamanoMaximo = 6 * 1024 * 1024;

    private static readonly Dictionary<string, string> Permitidas =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"]  = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"]  = "image/png",
            [".webp"] = "image/webp",
            [".gif"]  = "image/gif"
        };

    // Firmas de archivo, para no confiar sólo en la extensión.
    private static readonly (byte[] Firma, string Tipo)[] Firmas =
    {
        (new byte[] { 0xFF, 0xD8, 0xFF },                         "image/jpeg"),
        (new byte[] { 0x89, 0x50, 0x4E, 0x47 },                   "image/png"),
        (new byte[] { 0x47, 0x49, 0x46, 0x38 },                   "image/gif"),
        (new byte[] { 0x52, 0x49, 0x46, 0x46 },                   "image/webp")   // RIFF….WEBP
    };

    private readonly ILogger<ServicioImagenes> _log;

    public ServicioImagenes(ILogger<ServicioImagenes> log) { _log = log; }

    public string? UltimoMotivo { get; private set; }

    public async Task<ImagenSubida?> LeerAsync(IFormFile? archivo, CancellationToken ct = default)
    {
        UltimoMotivo = null;

        if (archivo is null || archivo.Length == 0) return null;

        if (archivo.Length > TamanoMaximo)
        {
            UltimoMotivo = $"La imagen pesa {archivo.Length / 1024d / 1024d:0.#} MB y el máximo son " +
                           $"{TamanoMaximo / 1024 / 1024} MB. Achicala antes de subirla.";
            _log.LogWarning("Imagen rechazada por tamaño: {Nombre} ({Bytes} bytes)", archivo.FileName, archivo.Length);
            return null;
        }

        var extension = Path.GetExtension(archivo.FileName);
        if (string.IsNullOrEmpty(extension) || !Permitidas.TryGetValue(extension, out var tipo))
        {
            UltimoMotivo = "Formato no admitido. Se aceptan JPG, PNG, WEBP y GIF.";
            _log.LogWarning("Imagen rechazada por extensión: {Nombre}", archivo.FileName);
            return null;
        }

        using var memoria = new MemoryStream();
        await archivo.CopyToAsync(memoria, ct);
        var datos = memoria.ToArray();

        if (!CoincideLaFirma(datos))
        {
            UltimoMotivo = "El archivo no parece una imagen, aunque tenga esa extensión.";
            _log.LogWarning("Imagen rechazada por firma: {Nombre}", archivo.FileName);
            return null;
        }

        return new ImagenSubida(datos, tipo);
    }

    /// <summary>Compara los primeros bytes con las firmas conocidas de imagen.</summary>
    private static bool CoincideLaFirma(byte[] datos)
    {
        foreach (var (firma, _) in Firmas)
        {
            if (datos.Length < firma.Length) continue;
            var coincide = true;
            for (var i = 0; i < firma.Length; i++)
            {
                if (datos[i] != firma[i]) { coincide = false; break; }
            }
            if (coincide) return true;
        }
        return false;
    }
}
