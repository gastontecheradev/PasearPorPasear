namespace PasearPorPasear.Services;

public class FileUploadService
{
    private readonly IWebHostEnvironment _env;

    public FileUploadService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string?> UploadImageAsync(IFormFile? file, string folder = "uploads")
    {
        if (file is null || file.Length == 0) return null;

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext)) return null;

        var uploadsDir = Path.Combine(_env.WebRootPath, folder);
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/{folder}/{fileName}";
    }

    public void DeleteImage(string? imagePath)
    {
        if (string.IsNullOrEmpty(imagePath) || imagePath.StartsWith("http")) return;

        var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
