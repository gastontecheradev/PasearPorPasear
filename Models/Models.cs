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
//  ¿Paseás conmigo? — las cuatro propuestas
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
    public ICollection<ConsultaPaseo> Consultas { get; set; } = new List<ConsultaPaseo>();
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
//  El Clubcito — la salida mensual
// ──────────────────────────────────────────────────────────────
public enum EstadoEncuentro { Proximo, Realizado, SinDefinir }

public class ClubcitoEncuentro : ITieneImagen
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;
    [StringLength(200)] public string TituloEn { get; set; } = string.Empty;
    [StringLength(200)] public string TituloPt { get; set; } = string.Empty;

    [StringLength(220)] public string Slug { get; set; } = string.Empty;

    /// <summary>Fecha de la salida: ordena la lista y decide el estado.</summary>
    public DateTime Fecha { get; set; }

    [StringLength(120)] public string? Barrio { get; set; }

    [StringLength(200)] public string? PuntoEncuentro { get; set; }
    [StringLength(200)] public string? PuntoEncuentroEn { get; set; }
    [StringLength(200)] public string? PuntoEncuentroPt { get; set; }

    [StringLength(600)] public string? Extracto { get; set; }
    [StringLength(600)] public string? ExtractoEn { get; set; }
    [StringLength(600)] public string? ExtractoPt { get; set; }

    /// <summary>La crónica, que se escribe después de la salida.</summary>
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
        ? $"/Imagenes/Encuentro/{Id}?v={(ActualizadoEn ?? CreadoEn).Ticks}"
        : ImagenUrl;

    [StringLength(600)] public string? MapaEmbedUrl { get; set; }

    /// <summary>Cuánta gente fue. Se carga después y se muestra en la tarjeta.</summary>
    public int? Asistentes { get; set; }

    public EstadoEncuentro Estado { get; set; } = EstadoEncuentro.Proximo;
    public bool Publicado { get; set; } = true;
    public DateTime CreadoEn { get; set; } = DateTime.Now;
    public DateTime? ActualizadoEn { get; set; }
}

// ──────────────────────────────────────────────────────────────
//  Cartelera de barrio
// ──────────────────────────────────────────────────────────────
/// <summary>Los seis diseños de afiche del manual de marca.</summary>
public enum ColorAfiche { Crema, Mostaza, Petroleo, Coral, Durazno, Rayas }

public enum EstadoAfiche { PorRevisar, Publicado, Archivado }

/// <summary>
/// Los afiches los manda el barrio por correo y son avisos locales, así que
/// van en un solo idioma a propósito: nadie traduce el aviso del gato perdido
/// de la esquina. Si algún día hace falta, se agregan los sufijos.
/// </summary>
public class CarteleraAfiche : ITieneImagen
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(120)] public string? Barrio { get; set; }

    [Required(ErrorMessage = "El texto del afiche es obligatorio")]
    [StringLength(400, ErrorMessage = "Cuatro o cinco líneas como máximo, para que entre en el afiche")]
    public string Texto { get; set; } = string.Empty;

    /// <summary>Teléfono, correo o lo que haya mandado quien publica.</summary>
    [StringLength(200)] public string? Contacto { get; set; }

    /// <summary>Null = sin vencimiento, como un oficio.</summary>
    public DateTime? VigenteHasta { get; set; }

    public ColorAfiche Color { get; set; } = ColorAfiche.Crema;
    public EstadoAfiche Estado { get; set; } = EstadoAfiche.PorRevisar;

    public byte[]? ImagenDatos { get; set; }
    [StringLength(100)] public string? ImagenTipo { get; set; }
    [StringLength(500)] public string? ImagenUrl { get; set; }
    [StringLength(300)] public string? ImagenAlt { get; set; }

    /// <summary>Imagen cargada si la hay; si no, la ruta estática del seed.</summary>
    [NotMapped]
    public string? ImagenSrc => ImagenDatos is { Length: > 0 }
        ? $"/Imagenes/Afiche/{Id}?v={(ActualizadoEn ?? CreadoEn).Ticks}"
        : ImagenUrl;

    /// <summary>De qué correo llegó, para poder contestarle a quien lo mandó.</summary>
    [EmailAddress][StringLength(200)] public string? EmailRemitente { get; set; }

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

    [Required] public string Titulo { get; set; } = "Sobre Pasear por Pasear";
    [Required] public string TituloEn { get; set; } = "About Pasear por Pasear";
    public string TituloPt { get; set; } = "Sobre Pasear por Pasear";

    public string Contenido { get; set; } = string.Empty;
    public string ContenidoEn { get; set; } = string.Empty;
    public string ContenidoPt { get; set; } = string.Empty;

    [StringLength(120)] public string Firma { get; set; } = "Rosalía Souza";

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

/// <summary>
/// Título y bajada de cada sección, editables desde el panel sin tocar vistas.
/// Una sola tabla buscada por Clave (archivo, paseos, clubcito, cartelera,
/// productos, contacto) en vez de una tabla por página.
/// </summary>
public class EncabezadoSeccion
{
    public int Id { get; set; }

    [Required][StringLength(60)] public string Clave { get; set; } = string.Empty;

    [Required][StringLength(200)] public string Titulo { get; set; } = string.Empty;
    [StringLength(200)] public string TituloEn { get; set; } = string.Empty;
    [StringLength(200)] public string TituloPt { get; set; } = string.Empty;

    /// <summary>Admite HTML: la portada la usa para el texto de «Qué es Pasear por Pasear».</summary>
    [StringLength(2000)] public string? Bajada { get; set; }
    [StringLength(2000)] public string? BajadaEn { get; set; }
    [StringLength(2000)] public string? BajadaPt { get; set; }

    public DateTime ActualizadoEn { get; set; } = DateTime.Now;
}

// ──────────────────────────────────────────────────────────────
//  El buzón de la Casita  (antes NewsletterSubscriber)
// ──────────────────────────────────────────────────────────────
public class SuscriptorBuzon
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Ese correo no parece válido")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [StringLength(120)] public string? Nombre { get; set; }

    /// <summary>Desde qué formulario llegó: portada, clubcito, productos, contacto.</summary>
    [StringLength(40)] public string? Origen { get; set; }

    public DateTime SuscritoEn { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;
}

// ──────────────────────────────────────────────────────────────
//  Consultas y mensajes
// ──────────────────────────────────────────────────────────────
public enum EstadoConsulta { Pendiente, Contestada, Confirmada, Cancelada }

public class ConsultaPaseo
{
    public int Id { get; set; }

    /// <summary>Null si la propuesta se borró: la consulta se conserva igual.</summary>
    public int? PropuestaId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Ese correo no parece válido")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [StringLength(50)] public string? Telefono { get; set; }

    public DateTime? FechaDeseada { get; set; }

    [Range(1, 40, ErrorMessage = "Entre 1 y 40 personas")]
    public int CantidadPersonas { get; set; } = 2;

    [StringLength(1000)] public string? Notas { get; set; }

    public EstadoConsulta Estado { get; set; } = EstadoConsulta.Pendiente;
    public DateTime CreadaEn { get; set; } = DateTime.Now;

    public Propuesta? Propuesta { get; set; }
}

public class MensajeContacto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Ese correo no parece válido")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [StringLength(200)] public string? Asunto { get; set; }

    [Required(ErrorMessage = "El mensaje es obligatorio")]
    [StringLength(2000)]
    public string Mensaje { get; set; } = string.Empty;

    public DateTime EnviadoEn { get; set; } = DateTime.Now;
    public bool Leido { get; set; }
}

// ──────────────────────────────────────────────────────────────
//  Ajustes del sitio
// ──────────────────────────────────────────────────────────────
public class Ajuste
{
    public int Id { get; set; }

    [Required][StringLength(100)]
    public string Clave { get; set; } = string.Empty;

    public string Valor { get; set; } = string.Empty;
    public string ValorEn { get; set; } = string.Empty;
    public string ValorPt { get; set; } = string.Empty;
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
