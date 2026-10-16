using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class CarteleraController : ControladorPublico
{
    public CarteleraController(ApplicationDbContext ctx, ITextos textos) : base(ctx, textos) { }

    public async Task<IActionResult> Index()
    {
        var vm = Preparar(new CarteleraVm());
        vm.Afiches = await AfichesVigentes().ToListAsync();

        ViewData["Title"] = Textos["cartelera.titulo"];
        return View(vm);
    }
}
