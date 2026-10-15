using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class CarteleraController : ControladorPublico
{
    public CarteleraController(ApplicationDbContext ctx) : base(ctx) { }

    public async Task<IActionResult> Index()
    {
        var vm = await PrepararAsync(new CarteleraVm(), "cartelera");

        vm.Afiches = await AfichesVigentes().ToListAsync();
        vm.CorreoCartelera = await AjusteAsync("EmailContacto") ?? vm.CorreoCartelera;

        ViewData["Title"] = vm.TituloSeccion;
        return View(vm);
    }
}
