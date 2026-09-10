using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Models;

namespace PasearPorPasear.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Fachada> Fachadas => Set<Fachada>();
    public DbSet<Propuesta> Propuestas => Set<Propuesta>();
    public DbSet<PropuestaDato> PropuestaDatos => Set<PropuestaDato>();
    public DbSet<ClubcitoEncuentro> ClubcitoEncuentros => Set<ClubcitoEncuentro>();
    public DbSet<CarteleraAfiche> CarteleraAfiches => Set<CarteleraAfiche>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<PaginaSobre> PaginasSobre => Set<PaginaSobre>();
    public DbSet<EncabezadoSeccion> EncabezadosSeccion => Set<EncabezadoSeccion>();
    public DbSet<SuscriptorBuzon> SuscriptoresBuzon => Set<SuscriptorBuzon>();
    public DbSet<ConsultaPaseo> ConsultasPaseo => Set<ConsultaPaseo>();
    public DbSet<MensajeContacto> MensajesContacto => Set<MensajeContacto>();
    public DbSet<Ajuste> Ajustes => Set<Ajuste>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── #ArchivoDeFachadas ──
        builder.Entity<Fachada>(e =>
        {
            e.HasIndex(f => f.Slug).IsUnique();
            e.HasIndex(f => f.Numero).IsUnique();
            // El listado filtra por barrio y ordena por número: índice compuesto.
            e.HasIndex(f => new { f.Barrio, f.Numero });
            e.HasIndex(f => f.Publicada);
            e.Property(f => f.FechaEncontrada).HasColumnType("datetime2");
            e.Property(f => f.FechaPublicacion).HasColumnType("datetime2");
        });

        // ── ¿Paseás conmigo? ──
        builder.Entity<Propuesta>(e =>
        {
            e.HasIndex(p => p.Slug).IsUnique();
            // Sólo puede haber una propuesta por clave.
            e.HasIndex(p => p.Clave).IsUnique();
            e.Property(p => p.Clave).HasConversion<int>();
        });

        builder.Entity<PropuestaDato>(e =>
        {
            e.HasOne(d => d.Propuesta)
             .WithMany(p => p.Datos)
             .HasForeignKey(d => d.PropuestaId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(d => new { d.PropuestaId, d.Orden });
        });

        // ── El Clubcito ──
        builder.Entity<ClubcitoEncuentro>(e =>
        {
            e.HasIndex(c => c.Slug).IsUnique();
            e.HasIndex(c => c.Fecha);
            e.Property(c => c.Fecha).HasColumnType("datetime2");
            e.Property(c => c.Estado).HasConversion<int>();
        });

        // ── Cartelera ──
        builder.Entity<CarteleraAfiche>(e =>
        {
            e.HasIndex(a => new { a.Estado, a.VigenteHasta });
            e.Property(a => a.Color).HasConversion<int>();
            e.Property(a => a.Estado).HasConversion<int>();
            e.Property(a => a.VigenteHasta).HasColumnType("datetime2");
        });

        // ── Productos ──
        builder.Entity<Producto>(e =>
        {
            e.HasIndex(p => p.Slug).IsUnique();
            e.Property(p => p.Precio).HasColumnType("decimal(18,2)");
            e.Property(p => p.Estado).HasConversion<int>();
            e.Property(p => p.ColorPanel).HasConversion<int>();
        });

        // ── Encabezados de sección ──
        builder.Entity<EncabezadoSeccion>(e =>
        {
            e.HasIndex(s => s.Clave).IsUnique();
        });

        // ── Buzón de la Casita ──
        builder.Entity<SuscriptorBuzon>(e =>
        {
            e.HasIndex(s => s.Email).IsUnique();
        });

        // ── Consultas de paseo ──
        builder.Entity<ConsultaPaseo>(e =>
        {
            e.HasOne(c => c.Propuesta)
             .WithMany(p => p.Consultas)
             .HasForeignKey(c => c.PropuestaId)
             // Si se borra una propuesta, la consulta queda huérfana pero se conserva:
             // son datos de alguien que escribió y hay que poder contestarle.
             .OnDelete(DeleteBehavior.SetNull);
            e.Property(c => c.Estado).HasConversion<int>();
            e.HasIndex(c => c.CreadaEn);
        });

        // ── Mensajes de contacto ──
        builder.Entity<MensajeContacto>(e =>
        {
            e.HasIndex(m => new { m.Leido, m.EnviadoEn });
        });

        // ── Ajustes ──
        builder.Entity<Ajuste>(e =>
        {
            e.HasIndex(a => a.Clave).IsUnique();
        });
    }
}
