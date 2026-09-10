using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;

namespace PasearPorPasear.Controllers;

/// <summary>
/// Alta en el buzón de la Casita. El formulario aparece en varias páginas,
/// así que cada uno manda de dónde viene y a dónde volver.
/// </summary>
public class BuzonController : ControladorPublico
{
    public BuzonController(ApplicationDbContext ctx) : base(ctx) { }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Suscribir(string? email, string? origen, string? volverA)
    {
        var destino = DestinoSeguro(volverA);

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            TempData["Error"] = Idioma switch
            {
                "en" => "That email does not look valid.",
                "pt" => "Esse e-mail não parece válido.",
                _    => "Ese correo no parece válido."
            };
            return Redirect(destino);
        }

        email = email.Trim().ToLowerInvariant();
        var existente = await Ctx.SuscriptoresBuzon.FirstOrDefaultAsync(s => s.Email == email);

        if (existente is null)
        {
            Ctx.SuscriptoresBuzon.Add(new SuscriptorBuzon
            {
                Email = email,
                Origen = origen,
                SuscritoEn = DateTime.Now,
                Activo = true
            });
        }
        else if (!existente.Activo)
        {
            // Se había dado de baja y vuelve: se reactiva en lugar de fallar
            // por el índice único del correo.
            existente.Activo = true;
            existente.SuscritoEn = DateTime.Now;
        }

        await Ctx.SaveChangesAsync();

        TempData["Success"] = Idioma switch
        {
            "en" => "You are in. The next letter goes out at the start of the month.",
            "pt" => "Pronto. A próxima carta sai no começo do mês.",
            _    => "Listo. La próxima carta sale a principio de mes."
        };
        return Redirect(destino);
    }

    /// <summary>
    /// Sólo se aceptan rutas internas: un volverA con un dominio externo
    /// convertiría el formulario en una redirección abierta.
    /// </summary>
    private string DestinoSeguro(string? volverA) =>
        !string.IsNullOrWhiteSpace(volverA) && Url.IsLocalUrl(volverA) ? volverA : "/";
}
