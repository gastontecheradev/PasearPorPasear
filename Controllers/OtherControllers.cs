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

    public async Task<IActionResult> Index()
    {
        var page = await _ctx.ClubDePaseoPages.FirstOrDefaultAsync();
        return View(page);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit()
    {
        var page = await _ctx.ClubDePaseoPages.FirstOrDefaultAsync();
        if (page is null) return NotFound();
        return View(page);
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ClubDePaseoPage page, IFormFile? imageFile)
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
        ViewBag.ItineraryCount = await _ctx.Itineraries.CountAsync();
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
