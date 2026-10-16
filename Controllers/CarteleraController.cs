using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class CarteleraController : ControladorPublico
{
    public CarteleraController(ApplicationDbContext ctx, ITextos textos) : base(ctx, textos) { }

    public async Task<IActionResult> Index(string? categoria)
    {
        // Una categoría que no existe se trata como «Todos».
        var activa = CategoriasAfiche.PorSlug(categoria)?.Categoria;
        var vm = Preparar(new CarteleraVm { CategoriaActiva = activa });

        var vigentes = AfichesVigentes();
        vm.HayAfiches = await vigentes.AnyAsync();
        vm.Afiches = vm.HayAfiches
            ? await (activa is null ? vigentes : vigentes.Where(a => a.Categoria == activa)).ToListAsync()
            : new();

        ViewData["Title"] = Textos["cartelera.titulo"];
        return View(vm);
    }
}
