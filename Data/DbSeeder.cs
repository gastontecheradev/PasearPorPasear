using Microsoft.AspNetCore.Identity;
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
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("PasearPorPasear.Data.DbSeeder");

        // Apply pending EF Core migrations (creates the schema on a brand-new database).
        // NOTE: with SQL Server we use Migrate instead of EnsureCreated, so that the
        // __EFMigrationsHistory table is written and Update-Database stays in sync.
        await ctx.Database.MigrateAsync();

        // Idempotently add new columns for in-DB image storage on existing databases.
        // Defensive no-op once the migrations above have run.
        await EnsureImageColumnsAsync(ctx);

        // One-time cleanup: orphaned ImagePaths pointing at /uploads/* (filesystem-stored
        // images that may not exist on the host). The next admin re-upload will store
        // the image in the DB instead.
        await CleanOrphanedUploadPathsAsync(ctx);

        // ── Admin role + user (credentials come from configuration, never from source) ──
        await SeedAdminAsync(userMgr, roleMgr, config, logger);

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

    private const string AdminRole = "Admin";

    // Creates the Admin role and the administrator account using credentials taken from
    // configuration (User Secrets in local development, App Settings in Azure).
    // Nothing is hardcoded here on purpose: this file is committed to source control.
    private static async Task SeedAdminAsync(
        UserManager<IdentityUser> userMgr,
        RoleManager<IdentityRole> roleMgr,
        IConfiguration config,
        ILogger logger)
    {
        if (!await roleMgr.RoleExistsAsync(AdminRole))
            await roleMgr.CreateAsync(new IdentityRole(AdminRole));

        var adminEmail = config["AdminSettings:Email"];
        var adminPassword = config["AdminSettings:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning(
                "No se creó el usuario administrador porque falta 'AdminSettings:Email' " +
                "y/o 'AdminSettings:Password'. En local: clic derecho sobre el proyecto > " +
                "Administrar secretos de usuario. En Azure: App Service > Configuración > " +
                "Configuración de la aplicación (AdminSettings__Email / AdminSettings__Password).");
            return;
        }

        var existing = await userMgr.FindByEmailAsync(adminEmail);
        if (existing is not null)
        {
            // The account already exists: its password is never overwritten from configuration.
            // We only make sure the Admin role is still assigned.
            if (!await userMgr.IsInRoleAsync(existing, AdminRole))
                await userMgr.AddToRoleAsync(existing, AdminRole);
            return;
        }

        var admin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userMgr.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userMgr.AddToRoleAsync(admin, AdminRole);
            logger.LogInformation("Usuario administrador creado: {Email}", adminEmail);
        }
        else
        {
            // Most common cause: the configured password does not satisfy the Identity
            // policy declared in Program.cs (8+ chars, digit, uppercase, symbol).
            logger.LogError(
                "No se pudo crear el usuario administrador '{Email}'. Errores: {Errors}",
                adminEmail,
                string.Join(" | ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
        }
    }

    // Adds ImageData / ImageContentType columns to existing tables if they're missing.
    // Uses INFORMATION_SCHEMA (SQL Server) so it is safe to run against a database that
    // was created before these columns existed.
    private static async Task EnsureImageColumnsAsync(ApplicationDbContext ctx)
    {
        var connection = ctx.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        var tables = new (string Table, string Column, string Type)[]
        {
            ("BlogPosts",          "ImageData",        "varbinary(max)"),
            ("BlogPosts",          "ImageContentType", "nvarchar(100)"),
            ("ClubDePaseoEntries", "ImageData",        "varbinary(max)"),
            ("ClubDePaseoEntries", "ImageContentType", "nvarchar(100)"),
            ("ClubDePaseoPages",   "ImageData",        "varbinary(max)"),
            ("ClubDePaseoPages",   "ImageContentType", "nvarchar(100)"),
            ("AboutPages",         "ImageData",        "varbinary(max)"),
            ("AboutPages",         "ImageContentType", "nvarchar(100)"),
            ("Tours",              "ImageData",        "varbinary(max)"),
            ("Tours",              "ImageContentType", "nvarchar(100)"),
        };

        foreach (var (table, column, type) in tables)
        {
            if (!await TableExistsAsync(connection, table)) continue;

            if (!await ColumnExistsAsync(connection, table, column))
            {
                using var cmd = connection.CreateCommand();
                // ADD (not ADD COLUMN) is the T-SQL syntax.
                cmd.CommandText = $"ALTER TABLE [{table}] ADD [{column}] {type} NULL;";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    private static async Task<bool> TableExistsAsync(System.Data.Common.DbConnection conn, string table)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES " +
            "WHERE TABLE_NAME = @table AND TABLE_SCHEMA = SCHEMA_NAME();";
        AddParam(cmd, "@table", table);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }

    private static async Task<bool> ColumnExistsAsync(System.Data.Common.DbConnection conn, string table, string column)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS " +
            "WHERE TABLE_NAME = @table AND COLUMN_NAME = @column AND TABLE_SCHEMA = SCHEMA_NAME();";
        AddParam(cmd, "@table", table);
        AddParam(cmd, "@column", column);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }

    private static void AddParam(System.Data.Common.DbCommand cmd, string name, string value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value;
        cmd.Parameters.Add(p);
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
            if (!await TableExistsAsync(connection, t)) continue;

            using var cmd = connection.CreateCommand();
            // T-SQL: DATALENGTH is the equivalent of SQLite's length() over a blob.
            cmd.CommandText =
                $"UPDATE [{t}] SET [ImagePath] = NULL " +
                $"WHERE [ImagePath] LIKE '/uploads/%' AND ([ImageData] IS NULL OR DATALENGTH([ImageData]) = 0);";
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
