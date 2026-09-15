using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasearPorPasear.Data;
using PasearPorPasear.Models;
using PasearPorPasear.Services;

namespace PasearPorPasear.Controllers.Admin;

/// <summary>
/// Base de las pantallas del panel: autorización, layout y el trabajo
/// repetido de guardar imágenes y slugs.
/// </summary>
[Authorize(Roles = "Admin")]
public abstract class ControladorAdmin : Controller
{
    protected readonly ApplicationDbContext Ctx;
    protected readonly IServicioImagenes Imagenes;

    protected ControladorAdmin(ApplicationDbContext ctx, IServicioImagenes imagenes)
    {
        Ctx = ctx;
        Imagenes = imagenes;
    }

    /// <summary>
    /// Aplica al modelo la imagen que vino del formulario: la reemplaza, la
    /// borra o la deja como está. Devuelve false sólo si el archivo se
    /// rechazó, dejando el motivo en ModelState.
    /// </summary>
    protected async Task<bool> AplicarImagenAsync(
        ITieneImagen entidad, IFormFile? archivo, bool borrarImagen, CancellationToken ct = default)
    {
        if (borrarImagen)
        {
            entidad.ImagenDatos = null;
            entidad.ImagenTipo = null;
            return true;
        }

        if (archivo is null || archivo.Length == 0) return true;   // no tocó la imagen

        var subida = await Imagenes.LeerAsync(archivo, ct);
        if (subida is null)
        {
            ModelState.AddModelError("imagen", Imagenes.UltimoMotivo ?? "No se pudo leer la imagen.");
            return false;
        }

        entidad.ImagenDatos = subida.Datos;
        entidad.ImagenTipo = subida.Tipo;
        return true;
    }

    /// <summary>
    /// Slug a partir del título, garantizando que no se repita. Si ya existe,
    /// le agrega un sufijo numérico en vez de fallar contra el índice único.
    /// El predicado lo arma quien llama, que es el único que sabe si hay que
    /// excluir la propia fila (al editar) o no (al crear).
    /// </summary>
    protected static string SlugUnico(string titulo, Func<string, bool> yaExiste)
    {
        var baseSlug = SlugHelper.GenerateSlug(titulo);
        if (string.IsNullOrWhiteSpace(baseSlug)) baseSlug = "sin-titulo";

        var slug = baseSlug;
        var n = 2;
        while (yaExiste(slug))
        {
            slug = $"{baseSlug}-{n}";
            n++;
        }
        return slug;
    }

    /// <summary>Datos de la imagen para el parcial lateral del formulario.</summary>
    protected static ViewModels.CampoImagenVm CampoImagen(ITieneImagen e, string tipo, int id, DateTime sello)
    {
        var cargada = e.ImagenDatos is { Length: > 0 };
        return new ViewModels.CampoImagenVm
        {
            Src = cargada ? $"/Imagenes/{tipo}/{id}?v={sello.Ticks}" : e.ImagenUrl,
            Alt = e.ImagenAlt,
            EsCargada = cargada
        };
    }
}
