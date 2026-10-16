using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class ContactoController : ControladorPublico
{
    public ContactoController(ApplicationDbContext ctx, ITextos textos) : base(ctx, textos) { }

    public IActionResult Index()
    {
        ViewData["Title"] = Textos["contacto.titulo"];
        return View(Preparar(new ContactoVm()));
    }
}
