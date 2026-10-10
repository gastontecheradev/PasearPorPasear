using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class ContactoController : ControladorPublico
{
    public ContactoController(ApplicationDbContext ctx) : base(ctx) { }

    public async Task<IActionResult> Index()
    {
        var vm = await PrepararAsync(new ContactoVm(), "contacto");
        vm.CorreoContacto = await AjusteAsync("EmailContacto") ?? vm.CorreoContacto;
        vm.CanalWhatsApp = await CanalWhatsAppAsync();
        return View(vm);
    }
}
