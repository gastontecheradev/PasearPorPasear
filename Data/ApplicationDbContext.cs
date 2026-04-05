using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Models;

namespace PasearPorPasear.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<ClubDePaseoPage> ClubDePaseoPages => Set<ClubDePaseoPage>();
    public DbSet<ClubDePaseoEntry> ClubDePaseoEntries => Set<ClubDePaseoEntry>();
    public DbSet<AboutPage> AboutPages => Set<AboutPage>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourReservation> TourReservations => Set<TourReservation>();
    public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Blog
        builder.Entity<BlogPost>(e =>
        {
            e.HasIndex(b => b.Slug).IsUnique();
            e.Property(b => b.PublishDate).HasColumnType("TEXT");
        });

        // Club de Paseo Entries
        builder.Entity<ClubDePaseoEntry>(e =>
        {
            e.HasIndex(i => i.Slug).IsUnique();
        });

        // Tour
        builder.Entity<Tour>(e =>
        {
            e.HasIndex(t => t.Slug).IsUnique();
            e.Property(t => t.Price).HasColumnType("REAL");
        });

        // Tour Reservation
        builder.Entity<TourReservation>(e =>
        {
            e.HasOne(r => r.Tour)
             .WithMany(t => t.Reservations)
             .HasForeignKey(r => r.TourId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Newsletter
        builder.Entity<NewsletterSubscriber>(e =>
        {
            e.HasIndex(n => n.Email).IsUnique();
        });

        // Site Settings
        builder.Entity<SiteSetting>(e =>
        {
            e.HasIndex(s => s.Key).IsUnique();
        });
    }
}
