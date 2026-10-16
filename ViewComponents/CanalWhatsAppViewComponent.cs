using Microsoft.AspNetCore.Mvc;
using PasearPorPasear.Services;

namespace PasearPorPasear.ViewComponents;

/// <summary>
/// Enlace al canal de WhatsApp del pie de página. Lee el mismo texto
/// «general.whatsapp» que la página de Contacto, así la dirección se cambia
/// en un solo lugar (Menú, pie y enlaces). Si está vacío o no es una URL
/// https, no se muestra nada.
/// </summary>
public class CanalWhatsAppViewComponent : ViewComponent
{
    private readonly ITextos _textos;

    public CanalWhatsAppViewComponent(ITextos textos) { _textos = textos; }

    public IViewComponentResult Invoke()
    {
        var url = _textos.Enlace("general.whatsapp");

        return url is null ? Content(string.Empty) : View("Default", (url, _textos["general.pie-whatsapp"]));
    }
}
