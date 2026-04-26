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

        await ctx.Database.EnsureCreatedAsync();

        if (!await roleMgr.RoleExistsAsync("Admin"))
            await roleMgr.CreateAsync(new IdentityRole("Admin"));

        const string adminEmail = "pasearporpasear@gmail.com";
        if (await userMgr.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await userMgr.CreateAsync(admin, "Pasear170593!");
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
                ImagePath = "/images/sobremi.jpg"
            });
        }

        // ── Club de Paseo ──
        if (!ctx.ClubDePaseoPages.Any())
        {
            ctx.ClubDePaseoPages.Add(new ClubDePaseoPage
            {
                Title = "Club de Paseo",
                TitleEn = "Walking Club",
                TitlePt = "Clube de Caminhada",
                Content = @"<p>El <strong>Club de Paseo</strong> es una comunidad para quienes disfrutan recorrer Montevideo a pie.</p>
                <p>Acá publicamos los recorridos que ya realizamos, con fotos, mapas y todo lo que descubrimos en cada caminata.</p>",
                ContentEn = @"<p>The <strong>Walking Club</strong> is a community for those who enjoy exploring Montevideo on foot.</p>
                <p>Here we publish the routes we've already completed, with photos, maps, and everything we discovered on each walk.</p>",
                ContentPt = @"<p>O <strong>Clube de Caminhada</strong> é uma comunidade para quem gosta de explorar Montevidéu a pé.</p>
                <p>Aqui publicamos os roteiros que já realizamos, com fotos, mapas e tudo o que descobrimos em cada caminhada.</p>",
                ImagePath = "/images/clubdepaseo.jpg"
            });
        }

        // ── Blog Posts ──
        if (!ctx.BlogPosts.Any())
        {
            ctx.BlogPosts.AddRange(
                new BlogPost
                {
                    Title = "Publicación 1",
                    TitleEn = "Post 1",
                    TitlePt = "publicação 1",
                    Slug = "post-1",
                    Content = @"<p>Contenido.</p>",
                    ContentEn = @"<p>Content.</p>",
                    ContentPt = @"<p>Conteúdo.</p>",
                    Excerpt = "Resumen.",
                    ExcerptEn = "Summary.",
                    ExcerptPt = "Resume.",
                    ImagePath = "/images/pasearporpasear.jpg",
                    PublishDate = DateTime.Now.AddDays(-2),
                    LocationName = "Locación",
                    LocationNameEn = "Location",
                    LocationNamePt = "localização",
                    MapEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d419185.17458825966!2d-56.52712040995068!3d-34.834004533391145!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x959f80ffc63bf7d3%3A0x6b321b2e355bec99!2sMontevideo%2C%20Departamento%20de%20Montevideo!5e0!3m2!1ses-419!2suy!4v1777158936633!5m2!1ses-419!2suy"
                },
                new BlogPost
                {
                    Title = "Publicación 2",
                    TitleEn = "Post 2",
                    TitlePt = "publicação 2",
                    Slug = "post-2",
                    Content = @"<p>Contenido.</p>",
                    ContentEn = @"<p>Content.</p>",
                    ContentPt = @"<p>Conteúdo.</p>",
                    Excerpt = "Resumen.",
                    ExcerptEn = "Summary.",
                    ExcerptPt = "Resume.",
                    ImagePath = "/images/pasearporpasear.jpg",
                    PublishDate = DateTime.Now.AddDays(-2),
                    LocationName = "Locación",
                    LocationNameEn = "Location",
                    LocationNamePt = "localização",
                    MapEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d419185.17458825966!2d-56.52712040995068!3d-34.834004533391145!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x959f80ffc63bf7d3%3A0x6b321b2e355bec99!2sMontevideo%2C%20Departamento%20de%20Montevideo!5e0!3m2!1ses-419!2suy!4v1777158936633!5m2!1ses-419!2suy"
                },
                new BlogPost
                {
                    Title = "Publicación 3",
                    TitleEn = "Post 3",
                    TitlePt = "publicação 3",
                    Slug = "post-3",
                    Content = @"<p>Contenido.</p>",
                    ContentEn = @"<p>Content.</p>",
                    ContentPt = @"<p>Conteúdo.</p>",
                    Excerpt = "Resumen.",
                    ExcerptEn = "Summary.",
                    ExcerptPt = "Resume.",
                    ImagePath = "/images/pasearporpasear.jpg",
                    PublishDate = DateTime.Now.AddDays(-2),
                    LocationName = "Locación",
                    LocationNameEn = "Location",
                    LocationNamePt = "localização",
                    MapEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d419185.17458825966!2d-56.52712040995068!3d-34.834004533391145!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x959f80ffc63bf7d3%3A0x6b321b2e355bec99!2sMontevideo%2C%20Departamento%20de%20Montevideo!5e0!3m2!1ses-419!2suy!4v1777158936633!5m2!1ses-419!2suy"
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
                    ImagePath = "/images/pasearporpasear.jpg",
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
                    ImagePath = "/images/pasearporpasear.jpg",
                }
            );
        }

        // ── Club de Paseo Entries ──
        if (!ctx.ClubDePaseoEntries.Any())
        {
            ctx.ClubDePaseoEntries.Add(new ClubDePaseoEntry
            {
                Title = "Prueba",
                TitleEn = "Test",
                TitlePt = "Prova",
                Slug = "prueba",
                Excerpt = "Entrada de pruebe.",
                ExcerptEn = "Test entry.",
                ExcerptPt = "Entrada de teste.",
                Content = @"<h3>Introducción</h3><p>Este es un <strong>contenido de prueba</strong> en español. Sirve para validar que el editor, los estilos y la traducción funcionan como esperamos.</p><h3>Detalles</h3><ul><li>Primer punto de prueba</li><li>Segundo punto de prueba</li><li>Tercer punto de prueba</li></ul><p>Fin de la prueba.</p>",
                ContentEn = @"<h3>Introduction</h3><p>This is a <strong>test content</strong> in English. It is used to validate that the editor, styles, and translation work as expected.</p><h3>Details</h3><ul><li>First test point</li><li>Second test point</li><li>Third test point</li></ul><p>End of test.</p>",
                ContentPt = @"<h3>Introdução</h3><p>Este é um <strong>conteúdo de teste</strong> em português. Serve para validar que o editor, os estilos e a tradução funcionam como esperamos.</p><h3>Detalhes</h3><ul><li>Primeiro ponto de teste</li><li>Segundo ponto de teste</li><li>Terceiro ponto de teste</li></ul><p>Fim do teste.</p>",
                Category = "personal",
                LocationName = "Montevideo",
                LocationNameEn = "Montevideo",
                LocationNamePt = "Montevidéu",
                MapEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d104769.34!2d-56.2156!3d-34.9011!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x959f80ffc63bf7d3%3A0xc63b2110d426a3ae!2sMontevideo!5e0!3m2!1ses!2suy!4v1700000000000",
                PublishDate = DateTime.Now,
                IsPublished = true,
                ImagePath = "/images/pasearporpasear.jpg",
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
