using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers;

public class ContactoController : ControladorPublico
{
    public ContactoController(ApplicationDbContext ctx) : base(ctx) { }

    public async Task<IActionResult> Index() => View(await ArmarAsync(new MensajeContacto()));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(MensajeContacto mensaje)
    {
        if (!ModelState.IsValid)
            return View(await ArmarAsync(mensaje));

        mensaje.EnviadoEn = DateTime.Now;
        mensaje.Leido = false;
        Ctx.MensajesContacto.Add(mensaje);
        await Ctx.SaveChangesAsync();

        TempData["Success"] = Idioma switch
        {
            "en" => "Message sent. We reply in a few days — not always right away, but we reply.",
            "pt" => "Mensagem enviada. Respondemos em alguns dias: nem sempre na hora, mas respondemos.",
            _    => "Mensaje enviado. Contestamos en unos días: no siempre enseguida, pero contestamos."
        };
        return RedirectToAction(nameof(Index));
    }

    private async Task<ContactoVm> ArmarAsync(MensajeContacto mensaje)
    {
        var vm = await PrepararAsync(new ContactoVm { Mensaje = mensaje }, "contacto");
        vm.CorreoContacto = await AjusteAsync("EmailContacto") ?? vm.CorreoContacto;
        vm.CanalWhatsApp = await AjusteAsync("CanalWhatsApp");
        vm.Buzon = new BuzonVm
        {
            Titulo = vm.T("El buzón de la Casita", "The Casita mailbox", "A caixa de correio da Casita"),
            Texto = await AjusteAsync("BuzonTexto") ?? string.Empty,
            EtiquetaBoton = vm.T("Suscribirme", "Subscribe", "Inscrever-me"),
            Nota = vm.T("También hay canal de WhatsApp, si preferís que te llegue por ahí.",
                        "There is also a WhatsApp channel, if you would rather get it there.",
                        "Também há canal de WhatsApp, se preferir receber por lá."),
            Origen = "contacto",
            VolverA = "/Contacto#buzon",
            Clase = "recuadro"
        };
        return vm;
    }
}
