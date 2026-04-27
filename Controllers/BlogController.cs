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
    public async Task<IActionResult> Index(string? category = null, int page = 1)
    {
        const int pageSize = 6;
        // Project without ImageData to avoid loading binary blobs into the listing.
        var query = _ctx.BlogPosts
            .Where(p => p.IsPublished)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        query = query.OrderByDescending(p => p.PublishDate);

        var total = await query.CountAsync();
        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new BlogPost
            {
                Id = p.Id,
                Title = p.Title,
                TitleEn = p.TitleEn,
                TitlePt = p.TitlePt,
                Slug = p.Slug,
                Excerpt = p.Excerpt,
                ExcerptEn = p.ExcerptEn,
                ExcerptPt = p.ExcerptPt,
                ImagePath = p.ImagePath,
                PublishDate = p.PublishDate,
                IsPublished = p.IsPublished,
                Category = p.Category,
                LocationName = p.LocationName,
                LocationNameEn = p.LocationNameEn,
                LocationNamePt = p.LocationNamePt
            })
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.CurrentCategory = category;
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
        var posts = await _ctx.BlogPosts
            .OrderByDescending(p => p.PublishDate)
            .Select(p => new BlogPost
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                ImagePath = p.ImagePath,
                PublishDate = p.PublishDate,
                IsPublished = p.IsPublished,
                Category = p.Category
            })
            .ToListAsync();
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
        ModelState.Remove("Slug");

        if (!ModelState.IsValid) return View(post);

        post.Slug = SlugHelper.GenerateSlug(post.Title);

        if (await _ctx.BlogPosts.AnyAsync(p => p.Slug == post.Slug))
            post.Slug += "-" + DateTime.Now.Ticks.ToString()[^6..];

        var img = await _upload.ReadImageAsync(imageFile);
        if (img is not null)
        {
            post.ImageData = img.Data;
            post.ImageContentType = img.ContentType;
        }

        post.CreatedAt = DateTime.Now;
        _ctx.BlogPosts.Add(post);
        await _ctx.SaveChangesAsync();

        // Now that we have the Id, set ImagePath to the controller URL.
        if (img is not null)
        {
            post.ImagePath = $"/Images/Blog/{post.Id}";
            await _ctx.SaveChangesAsync();
        }

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
        existing.Category = post.Category;
        existing.UpdatedAt = DateTime.Now;

        if (!string.IsNullOrEmpty(post.Slug) && post.Slug != existing.Slug)
        {
            existing.Slug = SlugHelper.GenerateSlug(post.Slug);
        }

        var img = await _upload.ReadImageAsync(imageFile);
        if (img is not null)
        {
            existing.ImageData = img.Data;
            existing.ImageContentType = img.ContentType;
            existing.ImagePath = $"/Images/Blog/{existing.Id}";
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

        _ctx.BlogPosts.Remove(post);
        await _ctx.SaveChangesAsync();

        TempData["Success"] = "Publicación eliminada.";
        return RedirectToAction(nameof(Manage));
    }
}
