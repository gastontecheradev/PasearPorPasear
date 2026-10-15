using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasearPorPasear.Models;

// ══════════════════════════════════════════════════════════════
//  Convenciones
//  ─────────────────────────────────────────────────────────────
//  · La propiedad sin sufijo es el español; `...En` y `...Pt` son las
//    traducciones. Es la misma convención del modelo anterior.
//  · Si una traducción viene vacía, la vista cae al español. Está
//    resuelto en la clase Traducir, al final del archivo.
//  · Las imágenes que sube Rosalía se guardan en la base, en
//    ImagenDatos + ImagenTipo. El contenido del seed en cambio apunta a
//    archivos estáticos de wwwroot mediante ImagenUrl.
//    Las vistas usan siempre ImagenSrc, que resuelve las dos: si hay
//    imagen cargada gana esa, y si no cae a la ruta estática. Así, el día
//    que se pase a Blob Storage sólo cambia ImagenSrc y ninguna vista.
//  · «Pasear por Pasear» y «#ArchivoDeFachadas» no se traducen nunca.
// ══════════════════════════════════════════════════════════════

/// <summary>
/// La implementan las entidades que llevan una imagen.
/// Es interfaz y no clase base a propósito: EF Core nunca mapea interfaces,
/// así que no hay riesgo de que arme una jerarquía de herencia sin querer.
/// </summary>
public interface ITieneImagen
{
    int Id { get; }

    /// <summary>Bytes de la imagen cargada desde el panel. Null si no se cargó ninguna.</summary>
    byte[]? ImagenDatos { get; set; }

    /// <summary>Tipo MIME de ImagenDatos: image/jpeg, image/webp…</summary>
    string? ImagenTipo { get; set; }

    /// <summary>Ruta estática, para el contenido del seed. La reemplaza ImagenDatos si hay.</summary>
    string? ImagenUrl { get; set; }

    string? ImagenAlt { get; set; }

    /// <summary>Lo que va en el src de la vista.</summary>
    string? ImagenSrc { get; }
}

// ──────────────────────────────────────────────────────────────
//  #ArchivoDeFachadas
// ──────────────────────────────────────────────────────────────
public class Fachada : ITieneImagen
{
    public int Id { get; set; }

    /// <summary>Número correlativo de ficha: es la serie del archivo.</summary>
    public int Numero { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;
    [StringLength(200)] public string TituloEn { get; set; } = string.Empty;
    [StringLength(200)] public string TituloPt { get; set; } = string.Empty;

    [StringLength(220)] public string Slug { get; set; } = string.Empty;

    [Required(ErrorMessage = "El barrio es obligatorio")]
    [StringLength(120)]
    public string Barrio { get; set; } = string.Empty;

    [StringLength(200)] public string? Calle { get; set; }

    [StringLength(600)] public string? Extracto { get; set; }
    [StringLength(600)] public string? ExtractoEn { get; set; }
    [StringLength(600)] public string? ExtractoPt { get; set; }

    public string Contenido { get; set; } = string.Empty;
    public string ContenidoEn { get; set; } = string.Empty;
    public string ContenidoPt { get; set; } = string.Empty;

    public byte[]? ImagenDatos { get; set; }
    [StringLength(100)] public string? ImagenTipo { get; set; }
    [StringLength(500)] public string? ImagenUrl { get; set; }
    [StringLength(300)] public string? ImagenAlt { get; set; }

    /// <summary>Imagen cargada si la hay; si no, la ruta estática del seed.</summary>
    [NotMapped]
    public string? ImagenSrc => ImagenDatos is { Length: > 0 }
        ? $"/Imagenes/Fachada/{Id}?v={(ActualizadaEn ?? CreadaEn).Ticks}"
        : ImagenUrl;

    /// <summary>Año de construcción, cuando se sabe.</summary>
    public int? AnioConstruccion { get; set; }

    [StringLength(600)] public string? MapaEmbedUrl { get; set; }

    /// <summary>Cuándo la encontró, que no es lo mismo que cuándo se publica.</summary>
    public DateTime FechaEncontrada { get; set; } = DateTime.Now;
    public DateTime FechaPublicacion { get; set; } = DateTime.Now;
    public DateTime CreadaEn { get; set; } = DateTime.Now;
    public DateTime? ActualizadaEn { get; set; }
    public bool Publicada { get; set; } = true;
}

// ──────────────────────────────────────────────────────────────
//  ¿Paseamos? — las cuatro propuestas
// ──────────────────────────────────────────────────────────────
public enum ClavePropuesta { Clubcito, AlSobre, RecorridosCreativos, Personalizados }

public class Propuesta : ITieneImagen
{
    public int Id { get; set; }

    /// <summary>Identifica la propuesta en el código sin depender del Id ni del texto.</summary>
    public ClavePropuesta Clave { get; set; }

    [Required][StringLength(200)] public string Titulo { get; set; } = string.Empty;
    [StringLength(200)] public string TituloEn { get; set; } = string.Empty;
    [StringLength(200)] public string TituloPt { get; set; } = string.Empty;

    [StringLength(220)] public string Slug { get; set; } = string.Empty;

    /// <summary>Pastilla arriba del título: «Todos los meses», «A pedido»…</summary>
    [StringLength(80)] public string? Etiqueta { get; set; }
    [StringLength(80)] public string? EtiquetaEn { get; set; }
    [StringLength(80)] public string? EtiquetaPt { get; set; }

    public string Descripcion { get; set; } = string.Empty;
    public string DescripcionEn { get; set; } = string.Empty;
    public string DescripcionPt { get; set; } = string.Empty;

    public byte[]? ImagenDatos { get; set; }
    [StringLength(100)] public string? ImagenTipo { get; set; }
    [StringLength(500)] public string? ImagenUrl { get; set; }
    [StringLength(300)] public string? ImagenAlt { get; set; }

    /// <summary>Imagen cargada si la hay; si no, la ruta estática del seed.</summary>
    [NotMapped]
    public string? ImagenSrc => ImagenDatos is { Length: > 0 }
        ? $"/Imagenes/Propuesta/{Id}?v={(ActualizadaEn ?? CreadaEn).Ticks}"
        : ImagenUrl;

    /// <summary>
    /// Mientras esté en true la vista muestra el aviso de «falta definir»
    /// en lugar de los datos. Es para «Al sobre» y «Recorridos creativos».
    /// </summary>
    public bool PendienteDeDefinir { get; set; }

    public int Orden { get; set; }
    public bool Activa { get; set; } = true;
    public DateTime CreadaEn { get; set; } = DateTime.Now;
    public DateTime? ActualizadaEn { get; set; }

    public ICollection<PropuestaDato> Datos { get; set; } = new List<PropuestaDato>();
}

/// <summary>
/// Cada casillero de la tabla de datos de una propuesta: «1 h 40 / caminando».
/// Va como entidad aparte para que se puedan agregar o sacar sin tocar columnas.
/// </summary>
public class PropuestaDato
{
    public int Id { get; set; }
    public int PropuestaId { get; set; }
    public int Orden { get; set; }

    [Required][StringLength(80)] public string Valor { get; set; } = string.Empty;
    [StringLength(80)] public string ValorEn { get; set; } = string.Empty;
    [StringLength(80)] public string ValorPt { get; set; } = string.Empty;

    [Required][StringLength(80)] public string Etiqueta { get; set; } = string.Empty;
    [StringLength(80)] public string EtiquetaEn { get; set; } = string.Empty;
    [StringLength(80)] public string EtiquetaPt { get; set; } = string.Empty;

    public Propuesta Propuesta { get; set; } = null!;
}

// ──────────────────────────────────────────────────────────────
//  Cartelera de barrio
// ──────────────────────────────────────────────────────────────
/// <summary>Los seis diseños de afiche del manual de marca.</summary>
public enum ColorAfiche { Crema, Mostaza, Petroleo, Coral, Durazno, Rayas }

/// <summary>
/// Los valores van explícitos porque se guardan como entero: antes existía
/// «PorRevisar» (0) y los números de los demás no pueden moverse.
/// </summary>
public enum EstadoAfiche { Publicado = 1, Archivado = 2 }

/// <summary>
/// Rosalía arma cada afiche aparte, sobre la plantilla de la cartelera, y lo
/// sube ya terminado como imagen: todo lo que dice el afiche va dentro de la
/// imagen. Por eso acá sólo queda un nombre para reconocerlo en el panel y el
/// texto alternativo para quien no puede ver la imagen.
/// </summary>
public class CarteleraAfiche : ITieneImagen
{
    public int Id { get; set; }

    /// <summary>Nombre para reconocerlo en el panel. Si no hay texto alternativo, se usa como tal.</summary>
    [Required(ErrorMessage = "Poné un nombre para reconocer el afiche en el listado")]
    [StringLength(120)]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>Null = sin vencimiento, como un oficio.</summary>
    public DateTime? VigenteHasta { get; set; }

    public EstadoAfiche Estado { get; set; } = EstadoAfiche.Publicado;

    /// <summary>El afiche en sí. Obligatoria: un afiche sin imagen no se puede mostrar.</summary>
    public byte[]? ImagenDatos { get; set; }
    [StringLength(100)] public string? ImagenTipo { get; set; }
    [StringLength(500)] public string? ImagenUrl { get; set; }

    /// <summary>Lo que dice el afiche, para los lectores de pantalla.</summary>
    [StringLength(300)] public string? ImagenAlt { get; set; }

    [NotMapped]
    public string? ImagenSrc => ImagenDatos is { Length: > 0 }
        ? $"/Imagenes/Afiche/{Id}?v={(ActualizadoEn ?? CreadoEn).Ticks}"
        : ImagenUrl;

    public int Orden { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.Now;
    public DateTime? ActualizadoEn { get; set; }

    /// <summary>Se calcula, no se guarda: tenía fecha y ya pasó.</summary>
    [NotMapped]
    public bool EstaVencido => VigenteHasta.HasValue && VigenteHasta.Value.Date < DateTime.Today;
}

// ──────────────────────────────────────────────────────────────
//  Productos
// ──────────────────────────────────────────────────────────────
public enum EstadoProducto { EnPreparacion, ALaVenta, DescargaGratis }

public class Producto : ITieneImagen
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(160)]
    public string Titulo { get; set; } = string.Empty;
    [StringLength(160)] public string TituloEn { get; set; } = string.Empty;
    [StringLength(160)] public string TituloPt { get; set; } = string.Empty;

    [StringLength(180)] public string Slug { get; set; } = string.Empty;

    [StringLength(600)] public string? Descripcion { get; set; }
    [StringLength(600)] public string? DescripcionEn { get; set; }
    [StringLength(600)] public string? DescripcionPt { get; set; }

    public byte[]? ImagenDatos { get; set; }
    [StringLength(100)] public string? ImagenTipo { get; set; }
    [StringLength(500)] public string? ImagenUrl { get; set; }
    [StringLength(300)] public string? ImagenAlt { get; set; }

    /// <summary>Imagen cargada si la hay; si no, la ruta estática del seed.</summary>
    [NotMapped]
    public string? ImagenSrc => ImagenDatos is { Length: > 0 }
        ? $"/Imagenes/Producto/{Id}?v={(ActualizadoEn ?? CreadoEn).Ticks}"
        : ImagenUrl;

    /// <summary>Fondo del panel ilustrado. Reusa la paleta de los afiches.</summary>
    public ColorAfiche ColorPanel { get; set; } = ColorAfiche.Mostaza;

    /// <summary>Null mientras esté en preparación.</summary>
    public decimal? Precio { get; set; }

    public EstadoProducto Estado { get; set; } = EstadoProducto.EnPreparacion;

    /// <summary>Para las descargas gratis, como la cartelera imprimible.</summary>
    [StringLength(500)] public string? ArchivoUrl { get; set; }

    public int Orden { get; set; }
    public bool Visible { get; set; } = true;
    public DateTime CreadoEn { get; set; } = DateTime.Now;
    public DateTime? ActualizadoEn { get; set; }
}

// ──────────────────────────────────────────────────────────────
//  Sobre PPP
// ──────────────────────────────────────────────────────────────
public class PaginaSobre : ITieneImagen
{
    public int Id { get; set; }

    [Required] public string Titulo { get; set; } = "Sobre Pasear Por Pasear";
    [Required] public string TituloEn { get; set; } = "About Pasear por Pasear";
    public string TituloPt { get; set; } = "Sobre Pasear Por Pasear";

    public string Contenido { get; set; } = string.Empty;
    public string ContenidoEn { get; set; } = string.Empty;
    public string ContenidoPt { get; set; } = string.Empty;

    /// <summary>La foto de la sección del proyecto, a la derecha del texto.</summary>
    public byte[]? ProyectoImagenDatos { get; set; }
    [StringLength(100)] public string? ProyectoImagenTipo { get; set; }
    [StringLength(500)] public string? ProyectoImagenUrl { get; set; }
    [StringLength(300)] public string? ProyectoImagenAlt { get; set; }

    /// <summary>El nombre de Rosalía: es el título de la segunda sección de la página.</summary>
    [StringLength(120)] public string Firma { get; set; } = "Rosalía Souza";

    /// <summary>La sección sobre Rosalía, debajo de la del proyecto.</summary>
    public string ContenidoAutora { get; set; } = string.Empty;
    public string ContenidoAutoraEn { get; set; } = string.Empty;
    public string ContenidoAutoraPt { get; set; } = string.Empty;

    /// <summary>El retrato. Rosalía lo cambia desde el panel.</summary>
    public byte[]? ImagenDatos { get; set; }
    [StringLength(100)] public string? ImagenTipo { get; set; }
    [StringLength(500)] public string? ImagenUrl { get; set; }
    [StringLength(300)] public string? ImagenAlt { get; set; }

    /// <summary>Imagen cargada si la hay; si no, la ruta estática del seed.</summary>
    [NotMapped]
    public string? ImagenSrc => ImagenDatos is { Length: > 0 }
        ? $"/Imagenes/Sobre/{Id}?v={(ActualizadaEn).Ticks}"
        : ImagenUrl;

    public DateTime ActualizadaEn { get; set; } = DateTime.Now;
}

// ──────────────────────────────────────────────────────────────
//  Textos del sitio
// ──────────────────────────────────────────────────────────────
/// <summary>
/// Cualquier texto o enlace fijo del sitio que no es una entrada: títulos y
/// bajadas de cada página, botones, el pie, el menú, los datos de contacto.
/// Qué claves existen, en qué página van y su valor de arranque está en
/// Services/CatalogoTextos.cs; acá sólo se guarda lo que Rosalía cambió.
/// </summary>
public class TextoSitio
{
    public int Id { get; set; }

    /// <summary>«pagina.nombre», por ejemplo «portada.frase».</summary>
    [Required][StringLength(80)] public string Clave { get; set; } = string.Empty;

    public string Valor { get; set; } = string.Empty;
    public string ValorEn { get; set; } = string.Empty;
    public string ValorPt { get; set; } = string.Empty;

    public DateTime ActualizadoEn { get; set; } = DateTime.Now;
}

// ──────────────────────────────────────────────────────────────
//  Elección de idioma con caída al español
// ──────────────────────────────────────────────────────────────
public static class Traducir
{
    /// <summary>
    /// Devuelve la variante del idioma pedido; si viene vacía, cae al español.
    /// Uso en las vistas:  @Traducir.Texto(culture, m.Titulo, m.TituloEn, m.TituloPt)
    /// </summary>
    public static string Texto(string? culture, string es, string? en, string? pt) => culture switch
    {
        "en" => string.IsNullOrWhiteSpace(en) ? es : en,
        "pt" => string.IsNullOrWhiteSpace(pt) ? es : pt,
        _    => es
    };

    /// <summary>Igual que Texto pero para campos que pueden ser null.</summary>
    public static string? Opcional(string? culture, string? es, string? en, string? pt) => culture switch
    {
        "en" => string.IsNullOrWhiteSpace(en) ? es : en,
        "pt" => string.IsNullOrWhiteSpace(pt) ? es : pt,
        _    => es
    };
}
