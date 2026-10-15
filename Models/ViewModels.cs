using PasearPorPasear.Models;

namespace PasearPorPasear.ViewModels;

// ══════════════════════════════════════════════════════════════
//  CUIDADO CON LAS IMÁGENES
//  ─────────────────────────────────────────────────────────────
//  Los bytes viven en la base. Un listado NUNCA debe traerlos: con
//  doce fichas serían decenas de megabytes por request.
//  Por eso los controladores proyectan `f.ImagenDatos != null`
//  (que en SQL es un IS NOT NULL y no transfiere nada) y arman la
//  URL con Imagen.Src. La propiedad ImagenSrc de las entidades sólo
//  sirve cuando se cargó la entidad completa, típicamente en el panel.
// ══════════════════════════════════════════════════════════════
public static class Imagen
{
    /// <summary>
    /// URL de la imagen: la ruta del controlador si hay imagen cargada,
    /// y si no la ruta estática del seed. El sello de tiempo rompe la caché
    /// del navegador cuando se reemplaza la foto.
    /// </summary>
    public static string? Src(string tipo, int id, bool tieneImagen, string? url, DateTime sello) =>
        tieneImagen ? $"/Imagenes/{tipo}/{id}?v={sello.Ticks}" : url;
}

/// <summary>
/// Base de todas las vistas públicas: resuelve el idioma de las entradas.
/// Los textos fijos de cada página salen de @Textos (Services/Textos.cs).
/// </summary>
public abstract class VistaBase
{
    public string Idioma { get; set; } = "es";

    public string T(string es, string? en, string? pt) => Traducir.Texto(Idioma, es, en, pt);
    public string? TO(string? es, string? en, string? pt) => Traducir.Opcional(Idioma, es, en, pt);
}

// ── Tarjeta genérica de fachada ──
public class TarjetaVm
{
    public string Titulo { get; set; } = string.Empty;
    public string Url { get; set; } = "#";
    public string? Etiqueta { get; set; }
    public string? Extracto { get; set; }
    public string? ImagenSrc { get; set; }
    public string? ImagenAlt { get; set; }
    public string? PieIzquierda { get; set; }
    public string? PieDerecha { get; set; }
}

// ── Bloque de suscripción al buzón (formulario de MailerLite), reutilizable ──
public class BuzonVm
{
    public string Titulo { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public string EtiquetaBoton { get; set; } = string.Empty;
    public string? Nota { get; set; }
    /// <summary>
    /// A dónde lleva el botón. Por defecto, al formulario de la newsletter en MailerLite;
    /// una ruta interna (p. ej. «/Contacto#buzon») abre en la misma pestaña.
    /// </summary>
    public string Enlace { get; set; } = "https://preview.mailerlite.io/forms/2403548/193692652342872057/share";
    /// <summary>Clase extra del recuadro, para adaptarlo a la banda donde va.</summary>
    public string? Clase { get; set; }

    /// <summary>
    /// Variante del botón. Sobre banda petróleo hay que usar «boton--claro»:
    /// el principal es petróleo sobre petróleo y desaparece.
    /// </summary>
    public string ClaseBoton { get; set; } = "boton--principal";
}

// ── Portada ──
public class InicioVm : VistaBase
{
    public List<TarjetaVm> TiraArchivo { get; set; } = new();
    public List<AficheVm> Afiches { get; set; } = new();
    public PaginaSobre? Sobre { get; set; }
    public string? SobreImagenSrc { get; set; }
    public BuzonVm Buzon { get; set; } = new();
}

// ── Sobre PPP ──
public class SobreVm : VistaBase
{
    public PaginaSobre Pagina { get; set; } = new();
    public string? ImagenSrc { get; set; }
    public string? ProyectoImagenSrc { get; set; }

    public string Titulo => T(Pagina.Titulo, Pagina.TituloEn, Pagina.TituloPt);
    public string Contenido => T(Pagina.Contenido, Pagina.ContenidoEn, Pagina.ContenidoPt);
    public string ContenidoAutora => T(Pagina.ContenidoAutora, Pagina.ContenidoAutoraEn, Pagina.ContenidoAutoraPt);
}

// ── Cartelera de barrio ──
public class CarteleraVm : VistaBase
{
    public List<AficheVm> Afiches { get; set; } = new();
}

/// <summary>
/// Un afiche sin los bytes de la imagen: la imagen la sirve
/// ImagenesController por su URL. Lo usan el muro y el listado del panel.
/// </summary>
public class AficheVm
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? ImagenAlt { get; set; }
    public bool TieneImagen { get; set; }
    public DateTime Sello { get; set; }
    public DateTime? VigenteHasta { get; set; }
    public EstadoAfiche Estado { get; set; }

    public string? ImagenSrc => Imagen.Src("Afiche", Id, TieneImagen, null, Sello);

    /// <summary>El texto alternativo, o el nombre si no lo completaron.</summary>
    public string Alt => string.IsNullOrWhiteSpace(ImagenAlt) ? Titulo : ImagenAlt;

    public bool EstaVencido => VigenteHasta.HasValue && VigenteHasta.Value.Date < DateTime.Today;
}

// ── Contacto ──
public class ContactoVm : VistaBase { }

// ══════════════════════════════════════════════════════════════
//  ¿Paseamos?
// ══════════════════════════════════════════════════════════════
public class DatoVm
{
    public string Valor { get; set; } = string.Empty;
    public string Etiqueta { get; set; } = string.Empty;
}

public class PropuestaVm
{
    public int Id { get; set; }
    public ClavePropuesta Clave { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Etiqueta { get; set; }
    public string DescripcionHtml { get; set; } = string.Empty;
    public string? ImagenSrc { get; set; }
    public string? ImagenAlt { get; set; }
    public bool PendienteDeDefinir { get; set; }
    public List<DatoVm> Datos { get; set; } = new();

    /// <summary>El Clubcito lleva al formulario de novedades; el resto, a contacto.</summary>
    public string? Enlace { get; set; }
    public string? TextoEnlace { get; set; }
}

public class PaseosVm : VistaBase
{
    public List<PropuestaVm> Propuestas { get; set; } = new();
}

// ══════════════════════════════════════════════════════════════
//  #ArchivoDeFachadas
// ══════════════════════════════════════════════════════════════
public class FichaVm
{
    public int Numero { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Url { get; set; } = "#";
    public string Barrio { get; set; } = string.Empty;
    public string? Calle { get; set; }
    public string? Extracto { get; set; }
    public string? ImagenSrc { get; set; }
    public string? ImagenAlt { get; set; }
    public DateTime Fecha { get; set; }

    public string NumeroFormateado => $"N.º {Numero}";
}

public class ArchivoVm : VistaBase
{
    public List<FichaVm> Fichas { get; set; } = new();
    public List<string> Barrios { get; set; } = new();
    public string? BarrioActivo { get; set; }
    public int Pagina { get; set; } = 1;
    public int TotalPaginas { get; set; } = 1;

    /// <summary>Números de página a mostrar: primera, vecinas y última.</summary>
    public IEnumerable<int> PaginasVisibles
    {
        get
        {
            var vistas = new SortedSet<int> { 1, TotalPaginas };
            for (var i = Pagina - 1; i <= Pagina + 1; i++)
                if (i >= 1 && i <= TotalPaginas) vistas.Add(i);
            return vistas;
        }
    }
}

public class FichaDetalleVm : VistaBase
{
    public FichaVm Ficha { get; set; } = new();
    public string ContenidoHtml { get; set; } = string.Empty;
    public int? AnioConstruccion { get; set; }
    public string? MapaEmbedUrl { get; set; }
    /// <summary>Otras fachadas del mismo barrio.</summary>
    public List<FichaVm> Cercanas { get; set; } = new();
    public FichaVm? Anterior { get; set; }
    public FichaVm? Siguiente { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  Productos
// ══════════════════════════════════════════════════════════════
public class ProductoVm
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ImagenSrc { get; set; }
    public string? ImagenAlt { get; set; }
    public string ClasePanel { get; set; } = "producto--mostaza";
    public decimal? Precio { get; set; }
    public EstadoProducto Estado { get; set; }
    public string? ArchivoUrl { get; set; }
}

public class ProductosVm : VistaBase
{
    public List<ProductoVm> Productos { get; set; } = new();
    public BuzonVm Buzon { get; set; } = new();
    public bool HayAlgoALaVenta => Productos.Any(p => p.Estado == EstadoProducto.ALaVenta);
}

// ══════════════════════════════════════════════════════════════
//  Panel de administración
// ══════════════════════════════════════════════════════════════

/// <summary>Un campo con sus tres idiomas, para el parcial _CamposIdioma.</summary>
public class CamposIdiomaVm
{
    /// <summary>Nombre base del campo; las traducciones agregan En y Pt.</summary>
    public string Nombre { get; set; } = string.Empty;
    public string Etiqueta { get; set; } = string.Empty;
    public string? Es { get; set; }
    public string? En { get; set; }
    public string? Pt { get; set; }
    /// <summary>true para textarea, false para input de una línea.</summary>
    public bool Largo { get; set; }
    /// <summary>Alto del textarea: «campo-largo» para textos largos, «campo-medio» para un párrafo.</summary>
    public string ClaseLargo { get; set; } = "campo-largo";
    public string? Pista { get; set; }
    public int? Maximo { get; set; }

    /// <summary>
    /// Prefijo del id HTML. Hace falta cuando una misma página repite el
    /// campo (los títulos de sección): si no, los label apuntan al primero.
    /// </summary>
    public string? IdBase { get; set; }
    public string Id => IdBase ?? Nombre;

    /// <summary>Nombres de las traducciones cuando no siguen la convención Nombre+En/Pt.</summary>
    public string? NombreEn { get; set; }
    public string? NombrePt { get; set; }

    public string NombreDe(string sufijo) => sufijo switch
    {
        "En" => NombreEn ?? Nombre + "En",
        "Pt" => NombrePt ?? Nombre + "Pt",
        _ => Nombre
    };
}

/// <summary>Estado de la imagen de una entidad, para el parcial _CampoImagen.</summary>
public class CampoImagenVm
{
    public string? Src { get; set; }
    public string? Alt { get; set; }
    /// <summary>true si la subió Rosalía; false si es la imagen estática del seed.</summary>
    public bool EsCargada { get; set; }

    /// <summary>Para cuando una pantalla tiene más de una imagen.</summary>
    public string Titulo { get; set; } = "Imagen";
    public string CampoArchivo { get; set; } = "imagen";
    public string CampoAlt { get; set; } = "ImagenAlt";
    public string CampoBorrar { get; set; } = "borrarImagen";
}

/// <summary>Barra fija de guardar, al pie de los formularios del panel.</summary>
public class BarraGuardarVm
{
    /// <summary>Lo que hace el botón, con el mismo nombre que el aviso posterior: «Crear la ficha».</summary>
    public string TextoGuardar { get; set; } = "Guardar cambios";
    public string? UrlVolver { get; set; }
    public string TextoVolver { get; set; } = "Volver al listado";
    public string EstadoInicial { get; set; } = "Sin cambios.";
}

/// <summary>Botón de borrar de la pantalla de edición, para el parcial _ZonaBorrar.</summary>
public class ZonaBorrarVm
{
    public string Url { get; set; } = string.Empty;
    /// <summary>Lo que se nombra en la confirmación.</summary>
    public string Nombre { get; set; } = string.Empty;
    public string Texto { get; set; } = "Borrar";
    public string Explicacion { get; set; } = "No se puede deshacer.";
}

/// <summary>Fila del listado del panel. Sirve para cualquier entidad con imagen.</summary>
public class FilaAdminVm
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Detalle { get; set; }
    public string? ImagenSrc { get; set; }
    public string? Columna2 { get; set; }
    public string? Columna3 { get; set; }
    public string? Estado { get; set; }
    public string? ClaseEstado { get; set; }
    /// <summary>Clave del filtro al que pertenece la fila («borrador», «proximo»…).</summary>
    public string? Filtro { get; set; }
}

/// <summary>Un botón de filtro del listado: la clave coincide con FilaAdminVm.Filtro.</summary>
public record FiltroAdminVm(string Clave, string Texto);

/// <summary>
/// Listado genérico del panel. Fachadas, productos y propuestas
/// comparten la misma pantalla: cambia el texto y las rutas, no la estructura.
/// </summary>
public class ListadoAdminVm
{
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    /// <summary>Base de las rutas: «/Admin/Fachadas». De ahí sale Editar.</summary>
    public string UrlBase { get; set; } = string.Empty;
    /// <summary>Null cuando no se pueden crear (las propuestas son fijas).</summary>
    public string? UrlNueva { get; set; }
    public string TextoNueva { get; set; } = string.Empty;
    /// <summary>Página del sitio a la que pertenece el listado (CatalogoTextos.Paginas).</summary>
    public string Pagina { get; set; } = string.Empty;
    public List<FiltroAdminVm> Filtros { get; set; } = new();
    public string Buscador { get; set; } = "Buscar…";
    public string Vacio { get; set; } = "Todavía no hay nada.";
    public string EncabezadoPrincipal { get; set; } = "Nombre";
    public string? Encabezado2 { get; set; }
    public string? Encabezado3 { get; set; }
    public List<FilaAdminVm> Filas { get; set; } = new();
}

/// <summary>
/// Los textos editables de una página del sitio, agrupados como se ven en
/// ella. Cada campo lleva su índice: el formulario los manda como arreglos
/// alineados (clave[i], es[i], en[i], pt[i]).
/// </summary>
public class TextosPaginaVm
{
    public PasearPorPasear.Services.PaginaPanel Pagina { get; set; } = null!;
    public List<GrupoTextosVm> Grupos { get; set; } = new();
}

public class GrupoTextosVm
{
    public string Nombre { get; set; } = string.Empty;
    public List<CampoTextoVm> Campos { get; set; } = new();
}

public class CampoTextoVm
{
    public int Indice { get; set; }
    public PasearPorPasear.Services.DefTexto Def { get; set; } = null!;
    public string? Es { get; set; }
    public string? En { get; set; }
    public string? Pt { get; set; }
}

/// <summary>Cabecera de una página en el panel, con sus pestañas.</summary>
public class CabeceraPaginaVm
{
    public PasearPorPasear.Services.PaginaPanel Pagina { get; set; } = null!;
    /// <summary>«listado» o «textos».</summary>
    public string Activa { get; set; } = "textos";
    /// <summary>Cuántas entradas tiene el listado, para la pestaña.</summary>
    public int? Cuenta { get; set; }
}
