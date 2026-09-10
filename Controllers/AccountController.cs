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
        var resultado = await _signIn.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false);
        if (resultado.Succeeded)
            return LocalRedirect(returnUrl ?? "/Admin/Dashboard");

        // Mensaje genérico a propósito: no conviene decir si el correo existe.
        ViewBag.Error = "Credenciales incorrectas.";
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
