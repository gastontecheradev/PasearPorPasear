using System.ComponentModel.DataAnnotations;

namespace PasearPorPasear.Models;

// ──────────────────────────────────────
// Blog Post
// ──────────────────────────────────────
public class BlogPost
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    public string TitleEn { get; set; } = string.Empty;

    [StringLength(200)]
    public string TitlePt { get; set; } = string.Empty;

    [StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
    public string ContentEn { get; set; } = string.Empty;
    public string ContentPt { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Excerpt { get; set; }
    [StringLength(500)]
    public string? ExcerptEn { get; set; }
    [StringLength(500)]
    public string? ExcerptPt { get; set; }

    public string? ImagePath { get; set; }
    public DateTime PublishDate { get; set; } = DateTime.Now;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public bool IsPublished { get; set; } = true;

    [StringLength(50)]
    public string? Category { get; set; }

    [StringLength(500)]
    public string? MapEmbedUrl { get; set; }
    [StringLength(200)]
    public string? LocationName { get; set; }
    [StringLength(200)]
    public string? LocationNameEn { get; set; }
    [StringLength(200)]
    public string? LocationNamePt { get; set; }
}

// ──────────────────────────────────────
// Club de Paseo
// ──────────────────────────────────────
public class ClubDePaseoPage
{
    public int Id { get; set; }
    [Required] public string Title { get; set; } = "Club de Paseo";
    [Required] public string TitleEn { get; set; } = "Walking Club";
    public string TitlePt { get; set; } = "Clube de Caminhada";
    public string Content { get; set; } = string.Empty;
    public string ContentEn { get; set; } = string.Empty;
    public string ContentPt { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

// ──────────────────────────────────────
// About / Sobre Mí
// ──────────────────────────────────────
public class AboutPage
{
    public int Id { get; set; }
    [Required] public string Title { get; set; } = "Sobre Mí";
    [Required] public string TitleEn { get; set; } = "About Me";
    public string TitlePt { get; set; } = "Sobre Mim";
    public string Content { get; set; } = string.Empty;
    public string ContentEn { get; set; } = string.Empty;
    public string ContentPt { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

// ──────────────────────────────────────
// Club de Paseo — Entries (itineraries/routes)
// ──────────────────────────────────────
public class ClubDePaseoEntry
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    [StringLength(200)]
    public string TitleEn { get; set; } = string.Empty;
    [StringLength(200)]
    public string TitlePt { get; set; } = string.Empty;

    [StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionPt { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ContentEn { get; set; } = string.Empty;
    public string ContentPt { get; set; } = string.Empty;

    public string? ImagePath { get; set; }
    [StringLength(100)] public string? Duration { get; set; }
    [StringLength(100)] public string? DurationEn { get; set; }
    [StringLength(100)] public string? DurationPt { get; set; }
    [StringLength(100)] public string? Distance { get; set; }
    [StringLength(500)] public string? MapEmbedUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public bool IsPublished { get; set; } = true;
}

// ──────────────────────────────────────
// Tours
// ──────────────────────────────────────
public class Tour
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del tour es obligatorio")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    [StringLength(200)] public string NameEn { get; set; } = string.Empty;
    [StringLength(200)] public string NamePt { get; set; } = string.Empty;

    [StringLength(200)] public string Slug { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionPt { get; set; } = string.Empty;

    public string? ImagePath { get; set; }
    [StringLength(100)] public string? Duration { get; set; }
    [StringLength(100)] public string? DurationEn { get; set; }
    [StringLength(100)] public string? DurationPt { get; set; }
    public decimal? Price { get; set; }
    [StringLength(200)] public string? MeetingPoint { get; set; }
    [StringLength(200)] public string? MeetingPointEn { get; set; }
    [StringLength(200)] public string? MeetingPointPt { get; set; }
    [StringLength(500)] public string? MapEmbedUrl { get; set; }
    public int MaxParticipants { get; set; } = 20;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<TourReservation> Reservations { get; set; } = new List<TourReservation>();
}

// ──────────────────────────────────────
// Tour Reservations
// ──────────────────────────────────────
public class TourReservation
{
    public int Id { get; set; }
    public int TourId { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")][StringLength(150)]
    public string FullName { get; set; } = string.Empty;
    [Required(ErrorMessage = "El email es obligatorio")][EmailAddress][StringLength(200)]
    public string Email { get; set; } = string.Empty;
    [StringLength(50)] public string? Phone { get; set; }
    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime ReservationDate { get; set; }
    [Range(1, 20, ErrorMessage = "Cantidad de personas entre 1 y 20")]
    public int NumberOfPeople { get; set; } = 1;
    [StringLength(500)] public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public Tour Tour { get; set; } = null!;
}

public enum ReservationStatus { Pending, Confirmed, Cancelled }

// ──────────────────────────────────────
// Newsletter Subscriber
// ──────────────────────────────────────
public class NewsletterSubscriber
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El email es obligatorio")][EmailAddress][StringLength(200)]
    public string Email { get; set; } = string.Empty;
    [StringLength(100)] public string? Name { get; set; }
    public DateTime SubscribedAt { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
}

// ──────────────────────────────────────
// Contact Message
// ──────────────────────────────────────
public class ContactMessage
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")][StringLength(150)]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "El email es obligatorio")][EmailAddress][StringLength(200)]
    public string Email { get; set; } = string.Empty;
    [StringLength(200)] public string? Subject { get; set; }
    [Required(ErrorMessage = "El mensaje es obligatorio")][StringLength(2000)]
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; } = false;
}

// ──────────────────────────────────────
// Site Settings
// ──────────────────────────────────────
public class SiteSetting
{
    public int Id { get; set; }
    [Required][StringLength(100)]
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueEn { get; set; } = string.Empty;
    public string ValuePt { get; set; } = string.Empty;
}
