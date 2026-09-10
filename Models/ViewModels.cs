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

/// <summary>Base de todas las vistas: resuelve el idioma y el encabezado de sección.</summary>
public abstract class VistaBase
{
    public string Idioma { get; set; } = "es";
    public EncabezadoSeccion? Encabezado { get; set; }

    public string T(string es, string? en, string? pt) => Traducir.Texto(Idioma, es, en, pt);
    public string? TO(string? es, string? en, string? pt) => Traducir.Opcional(Idioma, es, en, pt);

    public string TituloSeccion =>
        Encabezado is null ? string.Empty : T(Encabezado.Titulo, Encabezado.TituloEn, Encabezado.TituloPt);

    public string? BajadaSeccion =>
        Encabezado is null ? null : TO(Encabezado.Bajada, Encabezado.BajadaEn, Encabezado.BajadaPt);
}

// ── Tarjeta genérica: sirve para fachadas y para encuentros ──
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

// ── Bloque de suscripción al buzón, reutilizable ──
public class BuzonVm
{
    public string Titulo { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public string EtiquetaBoton { get; set; } = string.Empty;
    public string? Nota { get; set; }
    /// <summary>Desde qué formulario llegó: portada, clubcito, productos, contacto.</summary>
    public string Origen { get; set; } = "portada";
    /// <summary>A dónde vuelve después de suscribirse.</summary>
    public string VolverA { get; set; } = "/";
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
    public EncabezadoSeccion? Definicion { get; set; }
    public List<TarjetaVm> TiraArchivo { get; set; } = new();
    public List<CarteleraAfiche> Afiches { get; set; } = new();
    public PaginaSobre? Sobre { get; set; }
    public string? SobreImagenSrc { get; set; }
    public BuzonVm Buzon { get; set; } = new();
}

// ── Sobre PPP ──
public class SobreVm : VistaBase
{
    public PaginaSobre Pagina { get; set; } = new();
    public string? ImagenSrc { get; set; }

    public string Titulo => T(Pagina.Titulo, Pagina.TituloEn, Pagina.TituloPt);
    public string Contenido => T(Pagina.Contenido, Pagina.ContenidoEn, Pagina.ContenidoPt);
}

// ── Cartelera de barrio ──
public class CarteleraVm : VistaBase
{
    public List<CarteleraAfiche> Afiches { get; set; } = new();
    public string CorreoCartelera { get; set; } = "pasearporpasear@gmail.com";
}

// ── Contacto ──
public class ContactoVm : VistaBase
{
    public MensajeContacto Mensaje { get; set; } = new();
    public BuzonVm Buzon { get; set; } = new();
    public string CorreoContacto { get; set; } = "pasearporpasear@gmail.com";
    public string? CanalWhatsApp { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  ¿Paseás conmigo?
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

    /// <summary>El Clubcito tiene página propia; el resto lleva al formulario.</summary>
    public string? Enlace { get; set; }
    public string? TextoEnlace { get; set; }
}

public class PaseosVm : VistaBase
{
    public List<PropuestaVm> Propuestas { get; set; } = new();
    public ConsultaPaseo Consulta { get; set; } = new();
}

// ══════════════════════════════════════════════════════════════
//  El Clubcito
// ══════════════════════════════════════════════════════════════
public class EncuentroVm
{
    public string Titulo { get; set; } = string.Empty;
    public string Url { get; set; } = "#";
    public DateTime Fecha { get; set; }
    public string? Barrio { get; set; }
    public string? PuntoEncuentro { get; set; }
    public string? Extracto { get; set; }
    public string? ImagenSrc { get; set; }
    public string? ImagenAlt { get; set; }
    public int? Asistentes { get; set; }
    public EstadoEncuentro Estado { get; set; }

    public string Dia => Fecha.Day.ToString("00");
    public string MesCorto => Fecha.ToString("MMM").TrimEnd('.').ToLowerInvariant();
}

public class ClubcitoVm : VistaBase
{
    public List<EncuentroVm> Proximos { get; set; } = new();
    public List<EncuentroVm> Anteriores { get; set; } = new();
    public BuzonVm Buzon { get; set; } = new();
}

public class EncuentroDetalleVm : VistaBase
{
    public EncuentroVm Encuentro { get; set; } = new();
    public string ContenidoHtml { get; set; } = string.Empty;
    public string? MapaEmbedUrl { get; set; }
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
    public int TotalFachadas { get; set; }
    public int TotalBarrios { get; set; }
    public int? DesdeAnio { get; set; }

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
