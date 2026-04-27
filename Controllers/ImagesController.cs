using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;

namespace PasearPorPasear.Controllers;

// Serves uploaded images that are stored in the database.
// URL pattern: /Images/{kind}/{id}
// kind ∈ { Blog, ClubEntry, ClubPage, About, Tour }
public class ImagesController : Controller
{
    private readonly ApplicationDbContext _ctx;

    public ImagesController(ApplicationDbContext ctx) { _ctx = ctx; }

    [ResponseCache(Duration = 60 * 60 * 24 * 30, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Blog(int id)
    {
        var p = await _ctx.BlogPosts
            .Where(b => b.Id == id)
            .Select(b => new { b.ImageData, b.ImageContentType })
            .FirstOrDefaultAsync();
        return Serve(p?.ImageData, p?.ImageContentType);
    }

    [ResponseCache(Duration = 60 * 60 * 24 * 30, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> ClubEntry(int id)
    {
        var p = await _ctx.ClubDePaseoEntries
            .Where(b => b.Id == id)
            .Select(b => new { b.ImageData, b.ImageContentType })
            .FirstOrDefaultAsync();
        return Serve(p?.ImageData, p?.ImageContentType);
    }

    [ResponseCache(Duration = 60 * 60 * 24 * 30, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> ClubPage(int id)
    {
        var p = await _ctx.ClubDePaseoPages
            .Where(b => b.Id == id)
            .Select(b => new { b.ImageData, b.ImageContentType })
            .FirstOrDefaultAsync();
        return Serve(p?.ImageData, p?.ImageContentType);
    }

    [ResponseCache(Duration = 60 * 60 * 24 * 30, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> About(int id)
    {
        var p = await _ctx.AboutPages
            .Where(b => b.Id == id)
            .Select(b => new { b.ImageData, b.ImageContentType })
            .FirstOrDefaultAsync();
        return Serve(p?.ImageData, p?.ImageContentType);
    }

    [ResponseCache(Duration = 60 * 60 * 24 * 30, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Tour(int id)
    {
        var p = await _ctx.Tours
            .Where(b => b.Id == id)
            .Select(b => new { b.ImageData, b.ImageContentType })
            .FirstOrDefaultAsync();
        return Serve(p?.ImageData, p?.ImageContentType);
    }

    private IActionResult Serve(byte[]? data, string? contentType)
    {
        if (data is null || data.Length == 0 || string.IsNullOrEmpty(contentType))
            return NotFound();
        return File(data, contentType);
    }
}
