using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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

        // Create the schema (or no-op if DB already exists at the level of EnsureCreated).
        await ctx.Database.EnsureCreatedAsync();

        // Idempotently add new columns for in-DB image storage on existing databases.
        // This is critical because EnsureCreated does NOT alter existing tables.
        await EnsureImageColumnsAsync(ctx);

        // One-time cleanup: orphaned ImagePaths pointing at /uploads/* (filesystem-stored
        // images that may not exist on the host). The next admin re-upload will store
        // the image in the DB instead.
        await CleanOrphanedUploadPathsAsync(ctx);

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
<p>Este proyecto nació de la convicción de que la mejor manera de conocer un lugar es a pie, sin prisa, dejándose sorprender.</p>
<p>Soy una eterna curiosa de la historia montevideana.</p>",
                ContentEn = @"<p>Hi! I'm <strong>Rosalía Souza</strong>, creator of <em>Pasear por Pasear</em>.</p>
<p>This project was born from the conviction that the best way to know a place is on foot, unhurried, letting yourself be surprised.</p>
<p>I'm an eternal curious about Montevideo's history.</p>",
                ContentPt = @"<p>Olá! Sou <strong>Rosalía Souza</strong>, criadora do <em>Pasear por Pasear</em>.</p>
<p>Este projeto nasceu da convicção de que a melhor maneira de conhecer um lugar é a pé, sem pressa, deixando-se surpreender.</p>
<p>Sou uma eterna curiosa sobre a história de Montevidéu.</p>",
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

        // ── Club de Paseo Entries ──
        if (!ctx.ClubDePaseoEntries.Any())
        {
            ctx.ClubDePaseoEntries.AddRange(
                new ClubDePaseoEntry
                {
                    Title = "Prueba 1",
                    TitleEn = "Test 1",
                    TitlePt = "Prova 1",
                    Slug = "prueba-1",
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
                    ImagePath = "/images/prueba-1.jpg",
                },
                new ClubDePaseoEntry
                {
                    Title = "Prueba 2",
                    TitleEn = "Test 2",
                    TitlePt = "Prova 2",
                    Slug = "prueba-2",
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
                    ImagePath = "/images/prueba-2.jpg",
                },
                new ClubDePaseoEntry
                {
                    Title = "Prueba 3",
                    TitleEn = "Test 3",
                    TitlePt = "Prova 3",
                    Slug = "prueba-3",
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
                    ImagePath = "/images/prueba-3.jpg",
                }
            );
        }

        // ── Site Settings ──
        if (!ctx.SiteSettings.Any())
        {
            ctx.SiteSettings.AddRange(
                new SiteSetting { Key = "HomeHeroTitle", Value = "Pasear por Pasear", ValueEn = "Pasear por Pasear", ValuePt = "Pasear por Pasear" },
                new SiteSetting { Key = "HomeHeroSubtitle", Value = "Aprendiendo y compartiendo sobre mirar Montevideo.", ValueEn = "Learning and sharing perspectives on Montevideo.", ValuePt = "Aprendendo e compartilhando olhares sobre Montevidéu." },
                new SiteSetting { Key = "HomeIntro", Value = "Bienvenidos a Pasear por Pasear, un espacio dedicado a explorar Montevideo paso a paso.", ValueEn = "Welcome to Pasear por Pasear, a space dedicated to exploring Montevideo step by step.", ValuePt = "Bem-vindos ao Pasear por Pasear, um espaço dedicado a explorar Montevidéu passo a passo." },
                new SiteSetting { Key = "MailchimpFormUrl", Value = "", ValueEn = "", ValuePt = "" }
            );
        }

        await ctx.SaveChangesAsync();
    }

    // Adds ImageData / ImageContentType columns to existing tables if they're missing.
    // Uses raw SQL via SQLite's PRAGMA so it works even when EnsureCreated decides "DB exists, no-op".
    private static async Task EnsureImageColumnsAsync(ApplicationDbContext ctx)
    {
        var connection = ctx.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        var tables = new (string Table, string Column, string Type)[]
        {
            ("BlogPosts",          "ImageData",        "BLOB"),
            ("BlogPosts",          "ImageContentType", "TEXT"),
            ("ClubDePaseoEntries", "ImageData",        "BLOB"),
            ("ClubDePaseoEntries", "ImageContentType", "TEXT"),
            ("ClubDePaseoPages",   "ImageData",        "BLOB"),
            ("ClubDePaseoPages",   "ImageContentType", "TEXT"),
            ("AboutPages",         "ImageData",        "BLOB"),
            ("AboutPages",         "ImageContentType", "TEXT"),
            ("Tours",              "ImageData",        "BLOB"),
            ("Tours",              "ImageContentType", "TEXT"),
        };

        foreach (var (table, column, type) in tables)
        {
            if (!await ColumnExistsAsync(connection, table, column))
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = $"ALTER TABLE \"{table}\" ADD COLUMN \"{column}\" {type} NULL;";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    private static async Task<bool> ColumnExistsAsync(System.Data.Common.DbConnection conn, string table, string column)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"PRAGMA table_info(\"{table}\");";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            // PRAGMA columns: cid, name, type, notnull, dflt_value, pk
            var name = reader.GetString(1);
            if (string.Equals(name, column, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    // Reset ImagePath to NULL on rows that point to /uploads/ but have no ImageData blob.
    // These are the "broken image" rows from the filesystem-storage era.
    private static async Task CleanOrphanedUploadPathsAsync(ApplicationDbContext ctx)
    {
        var connection = ctx.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        var tables = new[] { "BlogPosts", "ClubDePaseoEntries", "ClubDePaseoPages", "AboutPages", "Tours" };
        foreach (var t in tables)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText =
                $"UPDATE \"{t}\" SET \"ImagePath\" = NULL " +
                $"WHERE \"ImagePath\" LIKE '/uploads/%' AND (\"ImageData\" IS NULL OR length(\"ImageData\") = 0);";
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
