namespace PasearPorPasear.Services;

public record UploadedImage(byte[] Data, string ContentType);

public class FileUploadService
{
    private static readonly Dictionary<string, string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".gif"] = "image/gif",
        [".webp"] = "image/webp"
    };

    // Read the uploaded file fully into memory and return bytes + content type.
    // Returns null if no file, empty file, or unsupported extension.
    public async Task<UploadedImage?> ReadImageAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0) return null;

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.TryGetValue(ext, out var contentType))
            return null;

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return new UploadedImage(ms.ToArray(), contentType);
    }
}
