using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;

namespace PasearPorPasear.Controllers;

/// <summary>
/// Panel de Rosalía. Por ahora sólo el tablero con los números;
/// las pantallas de alta y edición vienen en la próxima tanda.
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _ctx;

    public AdminController(ApplicationDbContext ctx) { _ctx = ctx; }

    public async Task<IActionResult> Dashboard()
    {
        var hoy = DateTime.Today;

        var vm = new TableroVm
        {
            Fachadas = await _ctx.Fachadas.CountAsync(f => f.Publicada),
            ProximosEncuentros = await _ctx.ClubcitoEncuentros.CountAsync(c => c.Publicado && c.Fecha >= hoy),
            AfichesPublicados = await _ctx.CarteleraAfiches.CountAsync(a =>
                a.Estado == EstadoAfiche.Publicado && (a.VigenteHasta == null || a.VigenteHasta >= hoy)),
            AfichesPorRevisar = await _ctx.CarteleraAfiches.CountAsync(a => a.Estado == EstadoAfiche.PorRevisar)
        };

        ViewData["Title"] = "Panel";
        return View(vm);
    }

    public class TableroVm
    {
        public int Fachadas { get; set; }
        public int ProximosEncuentros { get; set; }
        public int AfichesPublicados { get; set; }
        public int AfichesPorRevisar { get; set; }
    }
}
