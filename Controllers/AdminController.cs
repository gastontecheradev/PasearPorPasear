using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PasearPorPasear.Controllers;

/// <summary>
/// Entrada al panel. Ya no hay tablero: se entra directo a los textos de
/// la portada. /Admin/Dashboard se conserva para enlaces y marcadores viejos.
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public const string Entrada = "/Admin/Paginas/portada";

    [HttpGet("Admin")]
    [HttpGet("Admin/Dashboard")]
    public IActionResult Index() => Redirect(Entrada);
}
