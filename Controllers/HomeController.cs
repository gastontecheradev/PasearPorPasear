using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;

namespace PasearPorPasear.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _ctx;

    public HomeController(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Settings = await _ctx.SiteSettings.ToDictionaryAsync(s => s.Key, s => s);
        ViewBag.LatestPosts = await _ctx.BlogPosts
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishDate)
            .Take(3)
            .ToListAsync();
        ViewBag.Tours = await _ctx.Tours
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync();
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
