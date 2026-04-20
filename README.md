# Pasear por Pasear

A full-featured web application for **Pasear por Pasear** — a Montevideo-based tourism and walking blog created by Rosalía Souza. The site serves as a platform to share urban walks, guided tours, a walking club, and stories about the city of Montevideo, Uruguay.

Built with **ASP.NET Core MVC (.NET 10)**, **SQLite**, and **ASP.NET Core Identity**, with full trilingual support (Spanish, English, Portuguese).

🔗 **Live site:** [pasearporpasear](https://pasearporpasear.somee.com/)

---

## Features

- **Blog** — Paginated posts with cover images, location embeds, excerpts, and slug-based URLs
- **Tours** — Guided tour listings with pricing, meeting points, duration, and an integrated reservation system
- **Club de Paseo (Walking Club)** — Community walking group page with individual route entries
- **About** — Personal profile page for the site owner
- **Contact** — Contact form with message storage and admin inbox
- **Newsletter** — Subscriber management with opt-in/opt-out support
- **Admin Panel** — Role-protected dashboard to manage all content (posts, tours, reservations, messages, subscribers, and site settings)
- **Multilingual (i18n)** — Full support for Spanish (`es`), English (`en`), and Portuguese (`pt`) via cookie-based culture selection
- **Image Uploads** — Local file upload service supporting JPG, PNG, GIF, and WebP formats
- **SEO** — Open Graph meta tags and semantic HTML throughout

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| ORM | Entity Framework Core 10 |
| Database | SQLite |
| Auth | ASP.NET Core Identity with Role-based access |
| Frontend | Custom CSS + vanilla JavaScript |
| Localization | ASP.NET Core `IStringLocalizer` + cookie-based culture |

---

## Project Structure

```
PasearPorPasear/
├── Controllers/
│   ├── BlogController.cs          # Blog CRUD + public views
│   ├── HomeController.cs          # Landing page
│   ├── LanguageController.cs      # Language switcher (cookie-based)
│   ├── OtherControllers.cs        # ClubDePaseo, About, Contact, Newsletter, Admin
│   └── ToursController.cs         # Tours CRUD + reservations
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   └── DbSeeder.cs                # Seeds admin user, roles, and default content
├── Migrations/                    # EF Core migration history
├── Models/
│   └── Models.cs                  # All domain models (see below)
├── Services/
│   ├── FileUploadService.cs       # Image upload + deletion
│   └── SlugHelper.cs              # Auto-generates URL slugs
├── Views/
│   ├── About/
│   ├── Account/                   # Login / access denied
│   ├── Admin/                     # Dashboard
│   ├── Blog/
│   ├── ClubDePaseo/
│   ├── Contact/
│   ├── Home/
│   ├── Newsletter/
│   ├── Tours/
│   └── Shared/                    # _Layout.cshtml, partials
├── wwwroot/
│   ├── css/site.css
│   ├── js/site.js
│   └── uploads/                   # User-uploaded images (gitignored)
├── Program.cs
├── appsettings.json
└── appsettings.Example.json       # Template for configuration
```

---

## Domain Models

| Model | Description |
|---|---|
| `BlogPost` | Blog articles with multilingual title, content, excerpt, location, and slug |
| `Tour` | Guided tour with price, duration, meeting point, max participants |
| `TourReservation` | Booking request linked to a tour (Pending / Confirmed / Cancelled) |
| `ClubDePaseoPage` | Introductory page for the walking club |
| `ClubDePaseoEntry` | Individual walk/route within the club |
| `AboutPage` | Single-instance page for the author's bio |
| `ContactMessage` | Incoming contact form submissions |
| `NewsletterSubscriber` | Email subscribers with active/inactive status |
| `SiteSetting` | Key-value store for site-wide configurable text (multilingual) |

---

The admin panel is available at `/Admin/Dashboard` and provides access to:

- Blog post management
- Tour and reservation management
- Walking club page and entries
- Contact message inbox
- Newsletter subscriber list
- Site settings editor
