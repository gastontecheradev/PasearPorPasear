using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PasearPorPasear.Controllers;

/// <summary>
/// Extraído de OtherControllers.cs sin cambios de lógica: sólo se separó
/// en su propio archivo y se ordenaron los using.
/// </summary>
public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signIn;

    public AccountController(SignInManager<IdentityUser> signIn) { _signIn = signIn; }

    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl)
    {
        // Sin correo o contraseña no se consulta Identity (con null tira excepción).
        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrEmpty(password))
        {
            var resultado = await _signIn.PasswordSignInAsync(email.Trim(), password, isPersistent: true, lockoutOnFailure: true);
            if (resultado.Succeeded)
                return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/Admin");
        }

        // Mensaje genérico a propósito, también con la cuenta bloqueada: decir
        // «bloqueada» confirmaría que el correo existe.
        ViewBag.Error = "Credenciales incorrectas. Después de 5 intentos fallidos el ingreso se bloquea 15 minutos.";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();
}
