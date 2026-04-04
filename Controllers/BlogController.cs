using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;

namespace PasearPorPasear.Controllers;

public class BlogController : Controller
{
    private readonly ApplicationDbContext _ctx;
    private readonly FileUploadService _upload;

    public BlogController(ApplicationDbContext ctx, FileUploadService upload)
    {
        _ctx = ctx;
        _upload = upload;
    }

    // GET: /Blog
    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 6;
        var query = _ctx.BlogPosts
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishDate);

        var total = await query.CountAsync();
        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        return View(posts);
    }

    // GET: /Blog/Post/{slug}
    [Route("Blog/Post/{slug}")]
    public async Task<IActionResult> Post(string slug)
    {
        var post = await _ctx.BlogPosts.FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);
        if (post is null) return NotFound();
        return View(post);
    }

    // ──────────── ADMIN CRUD ────────────

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var posts = await _ctx.BlogPosts.OrderByDescending(p => p.PublishDate).ToListAsync();
        return View(posts);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new BlogPost { PublishDate = DateTime.Now });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogPost post, IFormFile? imageFile)
    {
        // Slug is auto-generated, remove from validation
        ModelState.Remove("Slug");

        if (!ModelState.IsValid) return View(post);

        post.Slug = SlugHelper.GenerateSlug(post.Title);

        // Check unique slug
        if (await _ctx.BlogPosts.AnyAsync(p => p.Slug == post.Slug))
            post.Slug += "-" + DateTime.Now.Ticks.ToString()[^6..];

        var imagePath = await _upload.UploadImageAsync(imageFile);
        if (imagePath is not null) post.ImagePath = imagePath;

        post.CreatedAt = DateTime.Now;
        _ctx.BlogPosts.Add(post);
        await _ctx.SaveChangesAsync();

        TempData["Success"] = "Publicación creada exitosamente.";
        return RedirectToAction(nameof(Manage));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _ctx.BlogPosts.FindAsync(id);
        if (post is null) return NotFound();
        return View(post);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogPost post, IFormFile? imageFile)
    {
        if (id != post.Id) return NotFound();

        var existing = await _ctx.BlogPosts.FindAsync(id);
        if (existing is null) return NotFound();

        ModelState.Remove("Slug");
        if (!ModelState.IsValid) return View(post);

        existing.Title = post.Title;
        existing.TitleEn = post.TitleEn;
        existing.TitlePt = post.TitlePt;
        existing.Content = post.Content;
        existing.ContentEn = post.ContentEn;
        existing.ContentPt = post.ContentPt;
        existing.Excerpt = post.Excerpt;
        existing.ExcerptEn = post.ExcerptEn;
        existing.ExcerptPt = post.ExcerptPt;
        existing.PublishDate = post.PublishDate;
        existing.IsPublished = post.IsPublished;
        existing.MapEmbedUrl = post.MapEmbedUrl;
        existing.LocationName = post.LocationName;
        existing.LocationNameEn = post.LocationNameEn;
        existing.LocationNamePt = post.LocationNamePt;
        existing.UpdatedAt = DateTime.Now;

        if (!string.IsNullOrEmpty(post.Slug) && post.Slug != existing.Slug)
        {
            existing.Slug = SlugHelper.GenerateSlug(post.Slug);
        }

        var imagePath = await _upload.UploadImageAsync(imageFile);
        if (imagePath is not null)
        {
            _upload.DeleteImage(existing.ImagePath);
            existing.ImagePath = imagePath;
        }

        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Publicación actualizada.";
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _ctx.BlogPosts.FindAsync(id);
        if (post is null) return NotFound();

        _upload.DeleteImage(post.ImagePath);
        _ctx.BlogPosts.Remove(post);
        await _ctx.SaveChangesAsync();

        TempData["Success"] = "Publicación eliminada.";
        return RedirectToAction(nameof(Manage));
    }
}
