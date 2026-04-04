using Microsoft.AspNetCore.Identity;
using PasearPorPasear.Models;

namespace PasearPorPasear.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await ctx.Database.EnsureCreatedAsync();

        if (!await roleMgr.RoleExistsAsync("Admin"))
            await roleMgr.CreateAsync(new IdentityRole("Admin"));

        var adminEmail = config["AdminSettings:Email"] ?? "admin@admin.com";
        var adminPassword = config["AdminSettings:Password"] ?? "Admin123!";

        if (await userMgr.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await userMgr.CreateAsync(admin, adminPassword);
            if (result.Succeeded) await userMgr.AddToRoleAsync(admin, "Admin");
        }

        // ── About ──
        if (!ctx.AboutPages.Any())
        {
            ctx.AboutPages.Add(new AboutPage
            {
                Title = "Sobre Mí", TitleEn = "About Me", TitlePt = "Sobre Mim",
                Content = @"<p>¡Hola! Soy <strong>Rosalía Souza</strong>, creadora de <em>Pasear por Pasear</em>.</p>
<p>Montevideo es mi ciudad y mi pasión. Este proyecto nació de la convicción de que la mejor manera de conocer un lugar es a pie, sin prisa, dejándose sorprender.</p>
<p>Soy guía de turismo certificada, fotógrafa aficionada y eterna curiosa de la historia montevideana.</p>",
                ContentEn = @"<p>Hi! I'm <strong>Rosalía Souza</strong>, creator of <em>Pasear por Pasear</em>.</p>
<p>Montevideo is my city and my passion. This project was born from the conviction that the best way to know a place is on foot, unhurried, letting yourself be surprised.</p>
<p>I'm a certified tour guide, amateur photographer, and an eternal curious about Montevideo's history.</p>",
                ContentPt = @"<p>Olá! Sou <strong>Rosalía Souza</strong>, criadora do <em>Pasear por Pasear</em>.</p>
<p>Montevidéu é minha cidade e minha paixão. Este projeto nasceu da convicção de que a melhor maneira de conhecer um lugar é a pé, sem pressa, deixando-se surpreender.</p>
<p>Sou guia de turismo certificada, fotógrafa amadora e eterna curiosa sobre a história de Montevidéu.</p>",
                ImagePath = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=600"
            });
        }

        // ── Club de Paseo ──
        if (!ctx.ClubDePaseoPages.Any())
        {
            ctx.ClubDePaseoPages.Add(new ClubDePaseoPage
            {
                Title = "Club de Paseo", TitleEn = "Walking Club", TitlePt = "Clube de Caminhada",
                Content = @"<p>El <strong>Club de Paseo</strong> es una comunidad para quienes disfrutan recorrer Montevideo a pie.</p>
<p>Cada semana organizamos caminatas grupales por diferentes barrios. ¡Es totalmente gratuito!</p>",
                ContentEn = @"<p>The <strong>Walking Club</strong> is a community for those who enjoy exploring Montevideo on foot.</p>
<p>Every week we organize group walks through different neighborhoods. It's completely free!</p>",
                ContentPt = @"<p>O <strong>Clube de Caminhada</strong> é uma comunidade para quem gosta de explorar Montevidéu a pé.</p>
<p>Toda semana organizamos caminhadas em grupo por diferentes bairros. É totalmente gratuito!</p>",
                ImagePath = "https://images.unsplash.com/photo-1581889470536-467bdbe30cd0?w=800"
            });
        }

        // ── Blog Posts ──
        if (!ctx.BlogPosts.Any())
        {
            ctx.BlogPosts.AddRange(
                new BlogPost
                {
                    Title = "Ciudad Vieja: Un paseo por la historia",
                    TitleEn = "Ciudad Vieja: A Walk Through History",
                    TitlePt = "Ciudad Vieja: Um Passeio pela História",
                    Slug = "ciudad-vieja-paseo-historia",
                    Content = @"<p>La <strong>Ciudad Vieja</strong> es el corazón histórico de Montevideo. Caminar por sus calles empedradas es como viajar en el tiempo.</p>
<p>El <strong>Mercado del Puerto</strong> es parada obligatoria. El aroma a parrilla se mezcla con el bullicio de los comensales.</p>",
                    ContentEn = @"<p><strong>Ciudad Vieja</strong> is the historic heart of Montevideo. Walking through its cobblestone streets is like traveling through time.</p>
<p>The <strong>Mercado del Puerto</strong> is a must-stop. The aroma of grilled meat mixes with the bustle of diners.</p>",
                    ContentPt = @"<p>A <strong>Ciudad Vieja</strong> é o coração histórico de Montevidéu. Caminhar por suas ruas de paralelepípedos é como viajar no tempo.</p>
<p>O <strong>Mercado del Puerto</strong> é parada obrigatória. O aroma da churrasqueira se mistura com o burburinho dos comensais.</p>",
                    Excerpt = "Recorrido por el casco histórico de Montevideo.",
                    ExcerptEn = "A tour through Montevideo's historic center.",
                    ExcerptPt = "Um passeio pelo centro histórico de Montevidéu.",
                    ImagePath = "https://images.unsplash.com/photo-1599413987323-b2b8c0d7d9c8?w=800",
                    PublishDate = DateTime.Now.AddDays(-10),
                    LocationName = "Ciudad Vieja, Montevideo",
                    LocationNameEn = "Old Town, Montevideo",
                    LocationNamePt = "Cidade Velha, Montevidéu",
                    MapEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3272.1!2d-56.2!3d-34.91!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zMzTCsDU0JzM2LjAiUyA1NsKwMTInMDAuMCJX!5e0!3m2!1ses!2suy!4v1"
                },
                new BlogPost
                {
                    Title = "Rambla de Montevideo al atardecer",
                    TitleEn = "Montevideo's Rambla at Sunset",
                    TitlePt = "Rambla de Montevidéu ao Pôr do Sol",
                    Slug = "rambla-montevideo-atardecer",
                    Content = @"<p>La <strong>Rambla de Montevideo</strong> es uno de los paseos costeros más largos del mundo, con más de 22 kilómetros.</p>",
                    ContentEn = @"<p>The <strong>Rambla de Montevideo</strong> is one of the longest coastal promenades in the world, with more than 22 kilometers.</p>",
                    ContentPt = @"<p>A <strong>Rambla de Montevidéu</strong> é um dos calçadões costeiros mais longos do mundo, com mais de 22 quilômetros.</p>",
                    Excerpt = "22 km de costa, mate y atardeceres inolvidables.",
                    ExcerptEn = "22 km of coastline, mate, and unforgettable sunsets.",
                    ExcerptPt = "22 km de costa, mate e pores do sol inesquecíveis.",
                    ImagePath = "https://images.unsplash.com/photo-1597933553032-2d264024ef41?w=800",
                    PublishDate = DateTime.Now.AddDays(-5),
                    LocationName = "Rambla de Pocitos",
                    LocationNameEn = "Pocitos Rambla",
                    LocationNamePt = "Rambla de Pocitos",
                    MapEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d6544.8!2d-56.16!3d-34.92!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zMzTCsDU1JzEyLjAiUyA1NsKwMDknMzYuMCJX!5e0!3m2!1ses!2suy!4v1"
                },
                new BlogPost
                {
                    Title = "Barrio Prado: Jardines y arquitectura",
                    TitleEn = "Prado Neighborhood: Gardens and Architecture",
                    TitlePt = "Bairro Prado: Jardins e Arquitetura",
                    Slug = "barrio-prado-jardines-arquitectura",
                    Content = @"<p>El <strong>Barrio Prado</strong> es el pulmón verde de Montevideo con parques, rosedales y casonas señoriales.</p>",
                    ContentEn = @"<p>The <strong>Prado Neighborhood</strong> is Montevideo's green lung with parks, rose gardens, and stately mansions.</p>",
                    ContentPt = @"<p>O <strong>Bairro Prado</strong> é o pulmão verde de Montevidéu com parques, roseirais e casarões senhoriais.</p>",
                    Excerpt = "El barrio más verde de Montevideo.",
                    ExcerptEn = "Montevideo's greenest neighborhood.",
                    ExcerptPt = "O bairro mais verde de Montevidéu.",
                    ImagePath = "https://images.unsplash.com/photo-1585320806297-9794b3e4eeae?w=800",
                    PublishDate = DateTime.Now.AddDays(-2),
                    LocationName = "Barrio Prado, Montevideo",
                    LocationNameEn = "Prado Neighborhood, Montevideo",
                    LocationNamePt = "Bairro Prado, Montevidéu",
                    MapEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d6543.5!2d-56.19!3d-34.87!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zMzTCsDUyJzEyLjAiUyA1NsKwMTEnMjQuMCJX!5e0!3m2!1ses!2suy!4v1"
                }
            );
        }

        // ── Tours ──
        if (!ctx.Tours.Any())
        {
            ctx.Tours.AddRange(
                new Tour
                {
                    Name = "Tour Ciudad Vieja Histórica", NameEn = "Historic Old Town Tour", NamePt = "Tour pela Cidade Velha Histórica",
                    Slug = "tour-ciudad-vieja",
                    Description = "Recorrido guiado por los principales puntos históricos.",
                    DescriptionEn = "Guided tour through the main historical landmarks.",
                    DescriptionPt = "Passeio guiado pelos principais pontos históricos.",
                    Duration = "3 horas", DurationEn = "3 hours", DurationPt = "3 horas",
                    Price = 800,
                    MeetingPoint = "Plaza Independencia", MeetingPointEn = "Plaza Independencia", MeetingPointPt = "Plaza Independencia",
                    MaxParticipants = 15,
                    ImagePath = "https://images.unsplash.com/photo-1599413987323-b2b8c0d7d9c8?w=800",
                },
                new Tour
                {
                    Name = "Atardeceres en la Rambla", NameEn = "Rambla Sunset Walk", NamePt = "Pôr do Sol na Rambla",
                    Slug = "atardeceres-rambla",
                    Description = "Caminata al atardecer por la Rambla de Pocitos.",
                    DescriptionEn = "Sunset walk along Pocitos Rambla.",
                    DescriptionPt = "Caminhada ao pôr do sol pela Rambla de Pocitos.",
                    Duration = "2 horas", DurationEn = "2 hours", DurationPt = "2 horas",
                    Price = 600,
                    MeetingPoint = "Playa de los Pocitos", MeetingPointEn = "Pocitos Beach", MeetingPointPt = "Praia de Pocitos",
                    MaxParticipants = 20,
                    ImagePath = "https://images.unsplash.com/photo-1597933553032-2d264024ef41?w=800",
                }
            );
        }

        // ── Itineraries ──
        if (!ctx.Itineraries.Any())
        {
            ctx.Itineraries.Add(new Itinerary
            {
                Title = "Montevideo en un día", TitleEn = "Montevideo in One Day", TitlePt = "Montevidéu em Um Dia",
                Slug = "montevideo-en-un-dia",
                Description = "El itinerario perfecto para conocer lo esencial.",
                DescriptionEn = "The perfect itinerary to see the essentials.",
                DescriptionPt = "O itinerário perfeito para conhecer o essencial.",
                Content = @"<h3>Mañana</h3><p><strong>9:00</strong> — Desayuno en la Ciudad Vieja.</p><p><strong>12:00</strong> — Almuerzo en el Mercado del Puerto.</p><h3>Tarde</h3><p><strong>14:00</strong> — Caminata por la Rambla.</p><h3>Noche</h3><p><strong>18:30</strong> — Atardecer en Pocitos.</p>",
                ContentEn = @"<h3>Morning</h3><p><strong>9:00</strong> — Breakfast in Old Town.</p><p><strong>12:00</strong> — Lunch at Mercado del Puerto.</p><h3>Afternoon</h3><p><strong>14:00</strong> — Walk along the Rambla.</p><h3>Evening</h3><p><strong>18:30</strong> — Sunset at Pocitos.</p>",
                ContentPt = @"<h3>Manhã</h3><p><strong>9:00</strong> — Café da manhã na Cidade Velha.</p><p><strong>12:00</strong> — Almoço no Mercado del Puerto.</p><h3>Tarde</h3><p><strong>14:00</strong> — Caminhada pela Rambla.</p><h3>Noite</h3><p><strong>18:30</strong> — Pôr do sol em Pocitos.</p>",
                Duration = "Todo el día", DurationEn = "Full day", DurationPt = "Dia inteiro",
                Distance = "~12 km",
                ImagePath = "https://images.unsplash.com/photo-1599413987323-b2b8c0d7d9c8?w=800",
            });
        }

        // ── Site Settings ──
        if (!ctx.SiteSettings.Any())
        {
            ctx.SiteSettings.AddRange(
                new SiteSetting { Key = "HomeHeroTitle", Value = "Pasear por Pasear", ValueEn = "Pasear por Pasear", ValuePt = "Pasear por Pasear" },
                new SiteSetting { Key = "HomeHeroSubtitle", Value = "Descubrí Montevideo a pie, sin prisa.", ValueEn = "Discover Montevideo on foot, unhurried.", ValuePt = "Descubra Montevidéu a pé, sem pressa." },
                new SiteSetting { Key = "HomeIntro", Value = "Bienvenidos a Pasear por Pasear, un espacio dedicado a explorar Montevideo paso a paso.", ValueEn = "Welcome to Pasear por Pasear, a space dedicated to exploring Montevideo step by step.", ValuePt = "Bem-vindos ao Pasear por Pasear, um espaço dedicado a explorar Montevidéu passo a passo." },
                new SiteSetting { Key = "MailchimpFormUrl", Value = "", ValueEn = "", ValuePt = "" }
            );
        }

        await ctx.SaveChangesAsync();
    }
}
