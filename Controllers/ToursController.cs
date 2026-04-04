using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;

namespace PasearPorPasear.Controllers;

public class ToursController : Controller
{
    private readonly ApplicationDbContext _ctx;
    private readonly FileUploadService _upload;

    public ToursController(ApplicationDbContext ctx, FileUploadService upload)
    {
        _ctx = ctx;
        _upload = upload;
    }

    // GET: /Tours
    public async Task<IActionResult> Index()
    {
        var tours = await _ctx.Tours.Where(t => t.IsActive).OrderBy(t => t.Name).ToListAsync();
        return View(tours);
    }

    // GET: /Tours/Detail/{slug}
    [Route("Tours/Detail/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var tour = await _ctx.Tours.FirstOrDefaultAsync(t => t.Slug == slug && t.IsActive);
        if (tour is null) return NotFound();
        ViewBag.Reservation = new TourReservation { TourId = tour.Id, ReservationDate = DateTime.Today.AddDays(1) };
        return View(tour);
    }

    // POST: /Tours/Reserve
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(TourReservation reservation)
    {
        var tour = await _ctx.Tours.FindAsync(reservation.TourId);
        if (tour is null) return NotFound();

        ModelState.Remove("Tour");
        if (!ModelState.IsValid)
        {
            ViewBag.Reservation = reservation;
            return View("Detail", tour);
        }

        reservation.CreatedAt = DateTime.Now;
        reservation.Status = ReservationStatus.Pending;
        _ctx.TourReservations.Add(reservation);
        await _ctx.SaveChangesAsync();

        TempData["ReservationSuccess"] = "¡Reserva enviada! Te contactaremos pronto para confirmar.";
        return RedirectToAction("Detail", new { slug = tour.Slug });
    }

    // ──────────── ADMIN ────────────

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var tours = await _ctx.Tours.OrderBy(t => t.Name).ToListAsync();
        return View(tours);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new Tour());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Tour tour, IFormFile? imageFile)
    {
        ModelState.Remove("Slug");
        if (!ModelState.IsValid) return View(tour);

        tour.Slug = SlugHelper.GenerateSlug(tour.Name);
        if (await _ctx.Tours.AnyAsync(t => t.Slug == tour.Slug))
            tour.Slug += "-" + DateTime.Now.Ticks.ToString()[^6..];

        var img = await _upload.UploadImageAsync(imageFile);
        if (img is not null) tour.ImagePath = img;

        tour.CreatedAt = DateTime.Now;
        _ctx.Tours.Add(tour);
        await _ctx.SaveChangesAsync();

        TempData["Success"] = "Tour creado.";
        return RedirectToAction(nameof(Manage));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var tour = await _ctx.Tours.FindAsync(id);
        if (tour is null) return NotFound();
        return View(tour);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Tour tour, IFormFile? imageFile)
    {
        if (id != tour.Id) return NotFound();
        var existing = await _ctx.Tours.FindAsync(id);
        if (existing is null) return NotFound();
        ModelState.Remove("Slug");
        if (!ModelState.IsValid) return View(tour);

        existing.Name = tour.Name;
        existing.NameEn = tour.NameEn;
        existing.NamePt = tour.NamePt;
        existing.Description = tour.Description;
        existing.DescriptionEn = tour.DescriptionEn;
        existing.DescriptionPt = tour.DescriptionPt;
        existing.Duration = tour.Duration;
        existing.DurationEn = tour.DurationEn;
        existing.DurationPt = tour.DurationPt;
        existing.Price = tour.Price;
        existing.MeetingPoint = tour.MeetingPoint;
        existing.MeetingPointEn = tour.MeetingPointEn;
        existing.MeetingPointPt = tour.MeetingPointPt;
        existing.MapEmbedUrl = tour.MapEmbedUrl;
        existing.MaxParticipants = tour.MaxParticipants;
        existing.IsActive = tour.IsActive;
        existing.UpdatedAt = DateTime.Now;

        var img = await _upload.UploadImageAsync(imageFile);
        if (img is not null) { _upload.DeleteImage(existing.ImagePath); existing.ImagePath = img; }

        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Tour actualizado.";
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var tour = await _ctx.Tours.Include(t => t.Reservations).FirstOrDefaultAsync(t => t.Id == id);
        if (tour is null) return NotFound();
        _upload.DeleteImage(tour.ImagePath);
        _ctx.Tours.Remove(tour);
        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Tour eliminado.";
        return RedirectToAction(nameof(Manage));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reservations(int? tourId)
    {
        var query = _ctx.TourReservations.Include(r => r.Tour).AsQueryable();
        if (tourId.HasValue) query = query.Where(r => r.TourId == tourId.Value);
        var reservations = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        ViewBag.Tours = await _ctx.Tours.OrderBy(t => t.Name).ToListAsync();
        ViewBag.SelectedTourId = tourId;
        return View(reservations);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateReservationStatus(int id, ReservationStatus status)
    {
        var res = await _ctx.TourReservations.FindAsync(id);
        if (res is null) return NotFound();
        res.Status = status;
        await _ctx.SaveChangesAsync();
        TempData["Success"] = "Estado actualizado.";
        return RedirectToAction(nameof(Reservations));
    }
}
