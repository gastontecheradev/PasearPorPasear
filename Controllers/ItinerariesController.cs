using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;

namespace PasearPorPasear.Controllers;

public class ItinerariesController : Controller
{
    private readonly ApplicationDbContext _ctx;
    private readonly FileUploadService _upload;

    public ItinerariesController(ApplicationDbContext ctx, FileUploadService upload)
    {
        _ctx = ctx;
        _upload = upload;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _ctx.Itineraries.Where(i => i.IsPublished).OrderByDescending(i => i.CreatedAt).ToListAsync();
        return View(items);
    }

    [Route("Itineraries/Detail/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var item = await _ctx.Itineraries.FirstOrDefaultAsync(i => i.Slug == slug && i.IsPublished);
        if (item is null) return NotFound();
        return View(item);
    }

    // ──────────── ADMIN ────────────

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        return View(await _ctx.Itineraries.OrderByDescending(i => i.CreatedAt).ToListAsync());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new Itinerary());

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Itinerary item, IFormFile? imageFile)
    {
        ModelState.Remove("Slug");
        if (!ModelState.IsValid) return View(item);
        item.Slug = SlugHelper.GenerateSlug(item.Title);
        if (await _ctx.Itineraries.AnyAsync(i => i.Slug == item.Slug))
            item.Slug += "-" + DateTime.Now.Ticks.ToString()[^6..];
        var img = await _upload.UploadImageAsync(imageFile);
        if (img is not null) item.ImagePath = img;
        item.CreatedAt = DateTime.Now;
        _ctx.Itineraries.Add(item);
        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Itinerario creado.";
        return RedirectToAction(nameof(Manage));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _ctx.Itineraries.FindAsync(id);
        if (item is null) return NotFound();
        return View(item);
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Itinerary item, IFormFile? imageFile)
    {
        if (id != item.Id) return NotFound();
        var existing = await _ctx.Itineraries.FindAsync(id);
        if (existing is null) return NotFound();
        ModelState.Remove("Slug");
        if (!ModelState.IsValid) return View(item);

        existing.Title = item.Title;
        existing.TitleEn = item.TitleEn;
        existing.TitlePt = item.TitlePt;
        existing.Description = item.Description;
        existing.DescriptionEn = item.DescriptionEn;
        existing.DescriptionPt = item.DescriptionPt;
        existing.Content = item.Content;
        existing.ContentEn = item.ContentEn;
        existing.ContentPt = item.ContentPt;
        existing.Duration = item.Duration;
        existing.DurationEn = item.DurationEn;
        existing.DurationPt = item.DurationPt;
        existing.Distance = item.Distance;
        existing.MapEmbedUrl = item.MapEmbedUrl;
        existing.IsPublished = item.IsPublished;
        existing.UpdatedAt = DateTime.Now;

        var img = await _upload.UploadImageAsync(imageFile);
        if (img is not null) { _upload.DeleteImage(existing.ImagePath); existing.ImagePath = img; }

        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Itinerario actualizado.";
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _ctx.Itineraries.FindAsync(id);
        if (item is null) return NotFound();
        _upload.DeleteImage(item.ImagePath);
        _ctx.Itineraries.Remove(item);
        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Itinerario eliminado.";
        return RedirectToAction(nameof(Manage));
    }
}
