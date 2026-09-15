using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;

namespace PasearPorPasear.Controllers.Admin;

/// <summary>Lo que llega del sitio: mensajes, consultas y altas del buzón.</summary>
[Route("Admin/Bandejas")]
public class BandejasController : ControladorAdmin
{
    public BandejasController(ApplicationDbContext ctx, IServicioImagenes imagenes)
        : base(ctx, imagenes) { }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Mensajes";
        return View(await Ctx.MensajesContacto.AsNoTracking()
            .OrderByDescending(m => m.EnviadoEn)
            .Take(200)
            .ToListAsync());
    }

    [HttpPost("Leido/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarLeido(int id, bool leido = true)
    {
        var m = await Ctx.MensajesContacto.FindAsync(id);
        if (m is null) return NotFound();

        m.Leido = leido;
        await Ctx.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("BorrarMensaje/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BorrarMensaje(int id)
    {
        var m = await Ctx.MensajesContacto.FindAsync(id);
        if (m is null) return NotFound();

        Ctx.MensajesContacto.Remove(m);
        await Ctx.SaveChangesAsync();
        TempData["Success"] = "Mensaje borrado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Consultas")]
    public async Task<IActionResult> Consultas()
    {
        ViewData["Title"] = "Consultas de paseo";
        return View(await Ctx.ConsultasPaseo.AsNoTracking()
            .Include(c => c.Propuesta)
            .OrderByDescending(c => c.CreadaEn)
            .Take(200)
            .ToListAsync());
    }

    [HttpPost("Consulta/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarConsulta(int id, EstadoConsulta estado)
    {
        var c = await Ctx.ConsultasPaseo.FindAsync(id);
        if (c is null) return NotFound();

        c.Estado = estado;
        await Ctx.SaveChangesAsync();
        return RedirectToAction(nameof(Consultas));
    }

    [HttpGet("Buzon")]
    public async Task<IActionResult> Buzon()
    {
        ViewData["Title"] = "El buzón de la Casita";
        return View(await Ctx.SuscriptoresBuzon.AsNoTracking()
            .OrderByDescending(s => s.SuscritoEn)
            .ToListAsync());
    }

    /// <summary>
    /// Descarga de los correos activos, para pegar en el gestor de newsletter.
    /// </summary>
    [HttpGet("Buzon/Csv")]
    public async Task<IActionResult> BuzonCsv()
    {
        var correos = await Ctx.SuscriptoresBuzon.AsNoTracking()
            .Where(s => s.Activo)
            .OrderBy(s => s.SuscritoEn)
            .Select(s => new { s.Email, s.Origen, s.SuscritoEn })
            .ToListAsync();

        var sb = new System.Text.StringBuilder("correo,origen,fecha\n");
        foreach (var c in correos)
            sb.Append($"{c.Email},{c.Origen},{c.SuscritoEn:yyyy-MM-dd}\n");

        return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()),
                    "text/csv", $"buzon-{DateTime.Today:yyyy-MM-dd}.csv");
    }
}
