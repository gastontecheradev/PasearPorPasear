using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;
using PasearPorPasear.ViewModels;

namespace PasearPorPasear.Controllers.Admin;

/// <summary>
/// Los textos fijos de cada página del sitio (Services/CatalogoTextos.cs) y
/// la ficha de Sobre PPP, que además tiene sus dos fotos.
/// </summary>
[Route("Admin/Paginas")]
public class PaginasController : ControladorAdmin
{
    private readonly ITextos _textos;

    public PaginasController(ApplicationDbContext ctx, IServicioImagenes imagenes, ITextos textos)
        : base(ctx, imagenes)
    {
        _textos = textos;
    }

    [HttpGet("")]
    public IActionResult Index() => Redirect(AdminController.Entrada);

    // ── Textos de una página ──────────────────────────────────
    [HttpGet("{pagina}")]
    public async Task<IActionResult> Textos(string pagina)
    {
        var p = CatalogoTextos.Pagina(pagina);
        // Direcciones viejas (Encabezados, Ajustes) o mal escritas: a la portada.
        if (p is null) return Redirect(AdminController.Entrada);
        if (p.Clave == "sobre") return RedirectToAction(nameof(Sobre));

        ViewData["Title"] = p.Nombre;
        return View("Textos", await ArmarTextosAsync(p));
    }

    [HttpPost("{pagina}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Textos(string pagina, string[] clave, string?[] es, string?[] en, string?[] pt)
    {
        var p = CatalogoTextos.Pagina(pagina);
        if (p is null || p.Clave == "sobre") return NotFound();

        var enviados = Leer(clave, es, en, pt);
        if (!Validar(p, enviados))
        {
            ViewData["Title"] = p.Nombre;
            return View("Textos", await ArmarTextosAsync(p, enviados));
        }

        await GuardarTextosAsync(p, enviados);
        TempData["Success"] = $"Se guardaron los textos de «{p.Nombre}».";
        return Redirect(p.UrlTextos);
    }

    // ── Sobre PPP ─────────────────────────────────────────────
    [HttpGet("Sobre")]
    public async Task<IActionResult> Sobre()
    {
        var pagina = await Ctx.PaginasSobre.FirstOrDefaultAsync();
        if (pagina is null)
        {
            // No debería pasar: lo crea el seeder. Pero si falta, se crea vacía
            // en vez de romper la pantalla.
            pagina = new PaginaSobre();
            Ctx.PaginasSobre.Add(pagina);
            await Ctx.SaveChangesAsync();
        }

        ViewData["Title"] = "Sobre PPP";
        ViewData["Textos"] = await ArmarTextosAsync(CatalogoTextos.Pagina("sobre")!);
        return View(pagina);
    }

    [HttpPost("Sobre")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public async Task<IActionResult> Sobre(PaginaSobre entrada, IFormFile? imagen, bool borrarImagen = false,
        IFormFile? imagenProyecto = null, bool borrarImagenProyecto = false,
        string[]? clave = null, string?[]? es = null, string?[]? en = null, string?[]? pt = null)
    {
        var p = await Ctx.PaginasSobre.FirstOrDefaultAsync();
        if (p is null) return NotFound();

        p.Titulo = entrada.Titulo?.Trim() ?? string.Empty;
        p.TituloEn = entrada.TituloEn?.Trim() ?? string.Empty;
        p.TituloPt = entrada.TituloPt?.Trim() ?? string.Empty;
        p.Contenido = entrada.Contenido ?? string.Empty;
        p.ContenidoEn = entrada.ContenidoEn ?? string.Empty;
        p.ContenidoPt = entrada.ContenidoPt ?? string.Empty;
        p.Firma = entrada.Firma?.Trim() ?? string.Empty;
        p.ContenidoAutora = entrada.ContenidoAutora ?? string.Empty;
        p.ContenidoAutoraEn = entrada.ContenidoAutoraEn ?? string.Empty;
        p.ContenidoAutoraPt = entrada.ContenidoAutoraPt ?? string.Empty;
        p.ImagenAlt = entrada.ImagenAlt?.Trim();
        p.ProyectoImagenAlt = entrada.ProyectoImagenAlt?.Trim();

        var paginaTextos = CatalogoTextos.Pagina("sobre")!;
        var enviados = Leer(clave ?? [], es ?? [], en ?? [], pt ?? []);

        var retratoOk = await AplicarImagenAsync(p, imagen, borrarImagen);
        var proyectoOk = await AplicarImagenProyectoAsync(p, imagenProyecto, borrarImagenProyecto);
        var textosOk = Validar(paginaTextos, enviados);
        if (!retratoOk || !proyectoOk || !textosOk)
        {
            ViewData["Title"] = "Sobre PPP";
            ViewData["Textos"] = await ArmarTextosAsync(paginaTextos, enviados);
            return View(p);
        }

        p.ActualizadaEn = DateTime.Now;
        await GuardarTextosAsync(paginaTextos, enviados);   // guarda también la ficha

        TempData["Success"] = "Cambios guardados.";
        return RedirectToAction(nameof(Sobre));
    }

    /// <summary>
    /// Igual que AplicarImagenAsync, pero para la foto de la sección del proyecto:
    /// la página tiene dos imágenes y ITieneImagen cubre sólo el retrato.
    /// </summary>
    private async Task<bool> AplicarImagenProyectoAsync(PaginaSobre p, IFormFile? archivo, bool borrar)
    {
        if (borrar)
        {
            p.ProyectoImagenDatos = null;
            p.ProyectoImagenTipo = null;
            return true;
        }

        if (archivo is null || archivo.Length == 0) return true;

        var subida = await Imagenes.LeerAsync(archivo);
        if (subida is null)
        {
            ModelState.AddModelError("imagenProyecto", Imagenes.UltimoMotivo ?? "No se pudo leer la imagen.");
            return false;
        }

        p.ProyectoImagenDatos = subida.Datos;
        p.ProyectoImagenTipo = subida.Tipo;
        return true;
    }

    // ── Textos: armar, validar y guardar ──────────────────────
    private sealed record Enviado(string? Es, string? En, string? Pt);

    /// <summary>
    /// Los arreglos llegan alineados por índice. Sólo se tienen en cuenta las
    /// claves del catálogo: nada de lo que llegue puede crear claves nuevas.
    /// </summary>
    private static Dictionary<string, Enviado> Leer(string[] clave, string?[] es, string?[] en, string?[] pt)
    {
        string? En(string?[] a, int i) => i < a.Length ? a[i]?.Trim() : null;

        var r = new Dictionary<string, Enviado>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < clave.Length; i++)
        {
            if (CatalogoTextos.Buscar(clave[i]) is null) continue;
            r[clave[i]] = new Enviado(En(es, i), En(en, i), En(pt, i));
        }
        return r;
    }

    private bool Validar(PaginaPanel pagina, Dictionary<string, Enviado> enviados)
    {
        var correo = new EmailAddressAttribute();

        foreach (var def in CatalogoTextos.DePagina(pagina.Clave))
        {
            if (!enviados.TryGetValue(def.Clave, out var e)) continue;
            var valor = e.Es ?? string.Empty;
            var nombre = $"«{def.Grupo}: {def.Nombre}»";

            if (valor.Length == 0)
            {
                if (!def.Opcional && def.Tipo != TipoTexto.Interruptor)
                    ModelState.AddModelError(def.Clave, $"{nombre} no puede quedar vacío.");
                continue;
            }

            switch (def.Tipo)
            {
                case TipoTexto.Enlace when EnlaceHelper.SoloHttps(valor) is null:
                    ModelState.AddModelError(def.Clave, $"{nombre} tiene que ser una dirección completa que empiece por https://.");
                    break;
                case TipoTexto.Correo when !correo.IsValid(valor):
                    ModelState.AddModelError(def.Clave, $"{nombre} no parece un correo: revisá que tenga @ y el dominio.");
                    break;
                case TipoTexto.Interruptor when valor is not ("si" or "no"):
                    ModelState.AddModelError(def.Clave, $"{nombre} tiene que ser sí o no.");
                    break;
                case TipoTexto.Linea or TipoTexto.Dato when valor.Length > 300:
                    ModelState.AddModelError(def.Clave, $"{nombre} es demasiado largo: hasta 300 caracteres.");
                    break;
            }
        }
        return ModelState.IsValid;
    }

    /// <summary>
    /// Crea o actualiza una fila por cada texto de la página. Lo que no se
    /// traduce (enlaces, correos, datos) guarda sólo el español.
    /// </summary>
    private async Task GuardarTextosAsync(PaginaPanel pagina, Dictionary<string, Enviado> enviados)
    {
        var claves = CatalogoTextos.DePagina(pagina.Clave).Select(d => d.Clave).ToList();
        var filas = await Ctx.TextosSitio.Where(t => claves.Contains(t.Clave)).ToDictionaryAsync(t => t.Clave);

        foreach (var def in CatalogoTextos.DePagina(pagina.Clave))
        {
            if (!enviados.TryGetValue(def.Clave, out var e)) continue;

            if (!filas.TryGetValue(def.Clave, out var fila))
            {
                fila = new TextoSitio { Clave = def.Clave };
                Ctx.TextosSitio.Add(fila);
            }

            fila.Valor = e.Es ?? string.Empty;
            fila.ValorEn = def.Traducible ? e.En ?? string.Empty : string.Empty;
            fila.ValorPt = def.Traducible ? e.Pt ?? string.Empty : string.Empty;
            fila.ActualizadoEn = DateTime.Now;
        }

        await Ctx.SaveChangesAsync();
    }

    /// <summary>
    /// Los textos de la página con lo que esté vigente: lo guardado, o el
    /// valor de arranque si nunca se tocó. Si el formulario volvió con un
    /// error, se muestra lo que se había escrito.
    /// </summary>
    private async Task<TextosPaginaVm> ArmarTextosAsync(PaginaPanel pagina, Dictionary<string, Enviado>? enviados = null)
    {
        var guardados = await _textos.GuardadosAsync();
        var indice = 0;

        var grupos = CatalogoTextos.DePagina(pagina.Clave)
            .GroupBy(d => d.Grupo)
            .Select(g => new GrupoTextosVm
            {
                Nombre = g.Key,
                Campos = g.Select(def =>
                {
                    var campo = new CampoTextoVm { Indice = indice++, Def = def };
                    if (enviados is not null && enviados.TryGetValue(def.Clave, out var e))
                        (campo.Es, campo.En, campo.Pt) = (e.Es, e.En, e.Pt);
                    else if (guardados.TryGetValue(def.Clave, out var t))
                        (campo.Es, campo.En, campo.Pt) = (t.Valor, t.ValorEn, t.ValorPt);
                    else
                        (campo.Es, campo.En, campo.Pt) = (def.Es, def.En, def.Pt);
                    return campo;
                }).ToList()
            })
            .ToList();

        return new TextosPaginaVm { Pagina = pagina, Grupos = grupos };
    }
}
