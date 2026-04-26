using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;

namespace PasearPorPasear.Controllers;

// ══════════════════════════════════════
// CLUB DE PASEO
// ══════════════════════════════════════
public class ClubDePaseoController : Controller
{
    private readonly ApplicationDbContext _ctx;
    private readonly FileUploadService _upload;

    public ClubDePaseoController(ApplicationDbContext ctx, FileUploadService upload)
    {
        _ctx = ctx;
        _upload = upload;
    }

    // Public: shows page intro + list of entries
    public async Task<IActionResult> Index(string? category = null, int page = 1)
    {
        ViewBag.Page = await _ctx.ClubDePaseoPages.FirstOrDefaultAsync();

        const int pageSize = 6;
        var query = _ctx.ClubDePaseoEntries
            .Where(e => e.IsPublished)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(e => e.Category == category);
        }

        query = query.OrderByDescending(e => e.PublishDate);

        var total = await query.CountAsync();
        var entries = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.CurrentCategory = category;
        return View(entries);
    }

    // Public: detail of a single entry
    [Route("ClubDePaseo/Detail/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var entry = await _ctx.ClubDePaseoEntries.FirstOrDefaultAsync(e => e.Slug == slug && e.IsPublished);
        if (entry is null) return NotFound();
        return View(entry);
    }

    // ──────────── ADMIN: Page ────────────

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditPage()
    {
        var page = await _ctx.ClubDePaseoPages.FirstOrDefaultAsync();
        if (page is null) return NotFound();
        return View(page);
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPage(ClubDePaseoPage page, IFormFile? imageFile)
    {
        var existing = await _ctx.ClubDePaseoPages.FirstOrDefaultAsync();
        if (existing is null) return NotFound();
        if (!ModelState.IsValid) return View(page);

        existing.Title = page.Title;
        existing.TitleEn = page.TitleEn;
        existing.TitlePt = page.TitlePt;
        existing.Content = page.Content;
        existing.ContentEn = page.ContentEn;
        existing.ContentPt = page.ContentPt;
        existing.UpdatedAt = DateTime.Now;

        var img = await _upload.UploadImageAsync(imageFile);
        if (img is not null) { _upload.DeleteImage(existing.ImagePath); existing.ImagePath = img; }

        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Página actualizada.";
        return RedirectToAction(nameof(Index));
    }

    // ──────────── ADMIN: Entries CRUD ────────────

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var entries = await _ctx.ClubDePaseoEntries.OrderByDescending(e => e.PublishDate).ToListAsync();
        return View(entries);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new ClubDePaseoEntry { PublishDate = DateTime.Now });

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClubDePaseoEntry entry, IFormFile? imageFile)
    {
        ModelState.Remove("Slug");
        if (!ModelState.IsValid) return View(entry);

        entry.Slug = SlugHelper.GenerateSlug(entry.Title);
        if (await _ctx.ClubDePaseoEntries.AnyAsync(e => e.Slug == entry.Slug))
            entry.Slug += "-" + DateTime.Now.Ticks.ToString()[^6..];

        var img = await _upload.UploadImageAsync(imageFile);
        if (img is not null) entry.ImagePath = img;

        entry.CreatedAt = DateTime.Now;
        _ctx.ClubDePaseoEntries.Add(entry);
        await _ctx.SaveChangesAsync();

        TempData["Success"] = "Entrada creada.";
        return RedirectToAction(nameof(Manage));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var entry = await _ctx.ClubDePaseoEntries.FindAsync(id);
        if (entry is null) return NotFound();
        return View(entry);
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClubDePaseoEntry entry, IFormFile? imageFile)
    {
        if (id != entry.Id) return NotFound();
        var existing = await _ctx.ClubDePaseoEntries.FindAsync(id);
        if (existing is null) return NotFound();

        ModelState.Remove("Slug");
        if (!ModelState.IsValid) return View(entry);

        existing.Title = entry.Title;
        existing.TitleEn = entry.TitleEn;
        existing.TitlePt = entry.TitlePt;
        existing.Content = entry.Content;
        existing.ContentEn = entry.ContentEn;
        existing.ContentPt = entry.ContentPt;
        existing.Excerpt = entry.Excerpt;
        existing.ExcerptEn = entry.ExcerptEn;
        existing.ExcerptPt = entry.ExcerptPt;
        existing.PublishDate = entry.PublishDate;
        existing.IsPublished = entry.IsPublished;
        existing.MapEmbedUrl = entry.MapEmbedUrl;
        existing.LocationName = entry.LocationName;
        existing.LocationNameEn = entry.LocationNameEn;
        existing.LocationNamePt = entry.LocationNamePt;
        existing.Category = entry.Category;
        existing.UpdatedAt = DateTime.Now;

        var img = await _upload.UploadImageAsync(imageFile);
        if (img is not null) { _upload.DeleteImage(existing.ImagePath); existing.ImagePath = img; }

        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Entrada actualizada.";
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await _ctx.ClubDePaseoEntries.FindAsync(id);
        if (entry is null) return NotFound();
        _upload.DeleteImage(entry.ImagePath);
        _ctx.ClubDePaseoEntries.Remove(entry);
        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Entrada eliminada.";
        return RedirectToAction(nameof(Manage));
    }
}

// ══════════════════════════════════════
// ABOUT
// ══════════════════════════════════════
public class AboutController : Controller
{
    private readonly ApplicationDbContext _ctx;
    private readonly FileUploadService _upload;

    public AboutController(ApplicationDbContext ctx, FileUploadService upload)
    {
        _ctx = ctx;
        _upload = upload;
    }

    public async Task<IActionResult> Index()
    {
        var page = await _ctx.AboutPages.FirstOrDefaultAsync();
        return View(page);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit()
    {
        var page = await _ctx.AboutPages.FirstOrDefaultAsync();
        if (page is null) return NotFound();
        return View(page);
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AboutPage page, IFormFile? imageFile)
    {
        var existing = await _ctx.AboutPages.FirstOrDefaultAsync();
        if (existing is null) return NotFound();
        if (!ModelState.IsValid) return View(page);

        existing.Title = page.Title;
        existing.TitleEn = page.TitleEn;
        existing.TitlePt = page.TitlePt;
        existing.Content = page.Content;
        existing.ContentEn = page.ContentEn;
        existing.ContentPt = page.ContentPt;
        existing.UpdatedAt = DateTime.Now;

        var img = await _upload.UploadImageAsync(imageFile);
        if (img is not null) { _upload.DeleteImage(existing.ImagePath); existing.ImagePath = img; }

        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Página actualizada.";
        return RedirectToAction(nameof(Index));
    }
}

// ══════════════════════════════════════
// NEWSLETTER
// ══════════════════════════════════════
public class NewsletterController : Controller
{
    private readonly ApplicationDbContext _ctx;

    public NewsletterController(ApplicationDbContext ctx) { _ctx = ctx; }

    public async Task<IActionResult> Index()
    {
        var mailchimpUrl = await _ctx.SiteSettings
            .Where(s => s.Key == "MailchimpFormUrl")
            .Select(s => s.Value)
            .FirstOrDefaultAsync();
        ViewBag.MailchimpFormUrl = mailchimpUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(string email, string? name)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["Error"] = "El email es obligatorio.";
            return RedirectToAction(nameof(Index));
        }

        if (await _ctx.NewsletterSubscribers.AnyAsync(n => n.Email == email))
        {
            TempData["Info"] = "Ya estás suscripto.";
            return RedirectToAction(nameof(Index));
        }

        _ctx.NewsletterSubscribers.Add(new NewsletterSubscriber
        {
            Email = email,
            Name = name,
            SubscribedAt = DateTime.Now
        });
        await _ctx.SaveChangesAsync();

        TempData["Success"] = "¡Te suscribiste exitosamente!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var subs = await _ctx.NewsletterSubscribers.OrderByDescending(s => s.SubscribedAt).ToListAsync();
        ViewBag.MailchimpUrl = (await _ctx.SiteSettings.FirstOrDefaultAsync(s => s.Key == "MailchimpFormUrl"))?.Value;
        return View(subs);
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateMailchimp(string mailchimpUrl)
    {
        var setting = await _ctx.SiteSettings.FirstOrDefaultAsync(s => s.Key == "MailchimpFormUrl");
        if (setting is not null)
        {
            setting.Value = mailchimpUrl ?? "";
            await _ctx.SaveChangesAsync();
        }
        TempData["Success"] = "URL de Mailchimp actualizada.";
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveSubscriber(int id)
    {
        var sub = await _ctx.NewsletterSubscribers.FindAsync(id);
        if (sub is not null)
        {
            _ctx.NewsletterSubscribers.Remove(sub);
            await _ctx.SaveChangesAsync();
        }
        TempData["Success"] = "Suscriptor eliminado.";
        return RedirectToAction(nameof(Manage));
    }
}

// ══════════════════════════════════════
// CONTACT
// ══════════════════════════════════════
public class ContactController : Controller
{
    private readonly ApplicationDbContext _ctx;

    public ContactController(ApplicationDbContext ctx) { _ctx = ctx; }

    public IActionResult Index() => View(new ContactMessage());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactMessage msg)
    {
        if (!ModelState.IsValid) return View(msg);

        msg.SentAt = DateTime.Now;
        _ctx.ContactMessages.Add(msg);
        await _ctx.SaveChangesAsync();

        TempData["Success"] = "¡Mensaje enviado! Te responderemos pronto.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var msgs = await _ctx.ContactMessages.OrderByDescending(m => m.SentAt).ToListAsync();
        return View(msgs);
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id)
    {
        var msg = await _ctx.ContactMessages.FindAsync(id);
        if (msg is not null) { msg.IsRead = true; await _ctx.SaveChangesAsync(); }
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var msg = await _ctx.ContactMessages.FindAsync(id);
        if (msg is not null) { _ctx.ContactMessages.Remove(msg); await _ctx.SaveChangesAsync(); }
        TempData["Success"] = "Mensaje eliminado.";
        return RedirectToAction(nameof(Manage));
    }
}

// ══════════════════════════════════════
// ACCOUNT (Login/Logout)
// ══════════════════════════════════════
public class AccountController : Controller
{
    private readonly Microsoft.AspNetCore.Identity.SignInManager<Microsoft.AspNetCore.Identity.IdentityUser> _signIn;

    public AccountController(Microsoft.AspNetCore.Identity.SignInManager<Microsoft.AspNetCore.Identity.IdentityUser> signIn)
    {
        _signIn = signIn;
    }

    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl)
    {
        var result = await _signIn.PasswordSignInAsync(email, password, true, false);
        if (result.Succeeded)
            return LocalRedirect(returnUrl ?? "/Admin/Dashboard");

        ViewBag.Error = "Credenciales incorrectas.";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();
}

// ══════════════════════════════════════
// ADMIN DASHBOARD
// ══════════════════════════════════════
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _ctx;

    public AdminController(ApplicationDbContext ctx) { _ctx = ctx; }

    public async Task<IActionResult> Dashboard()
    {
        ViewBag.PostCount = await _ctx.BlogPosts.CountAsync();
        ViewBag.TourCount = await _ctx.Tours.CountAsync();
        ViewBag.ReservationCount = await _ctx.TourReservations.CountAsync(r => r.Status == ReservationStatus.Pending);
        ViewBag.SubscriberCount = await _ctx.NewsletterSubscribers.CountAsync(s => s.IsActive);
        ViewBag.MessageCount = await _ctx.ContactMessages.CountAsync(m => !m.IsRead);
        ViewBag.ClubEntryCount = await _ctx.ClubDePaseoEntries.CountAsync();
        return View();
    }

    public async Task<IActionResult> Settings()
    {
        var settings = await _ctx.SiteSettings.OrderBy(s => s.Key).ToListAsync();
        return View(settings);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(Dictionary<string, string> values, Dictionary<string, string> valuesEn, Dictionary<string, string> valuesPt)
    {
        foreach (var kvp in values)
        {
            var setting = await _ctx.SiteSettings.FirstOrDefaultAsync(s => s.Key == kvp.Key);
            if (setting is not null)
            {
                setting.Value = kvp.Value ?? "";
                if (valuesEn.TryGetValue(kvp.Key, out var en))
                    setting.ValueEn = en ?? "";
                if (valuesPt.TryGetValue(kvp.Key, out var pt))
                    setting.ValuePt = pt ?? "";
            }
        }
        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Configuración guardada.";
        return RedirectToAction(nameof(Settings));
    }
}
