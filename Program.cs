using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Data;
using PasearPorPasear.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ── SQL Server ────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'DefaultConnection'. " +
        "Configurala en los secretos de usuario (local) o en Azure App Service > Configuración.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sql =>
    {
        // Reintentos ante fallos transitorios: imprescindible en Azure SQL.
        sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
        sql.CommandTimeout(60);
    }));

// ── Identity ──────────────────────────────────────────────────
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

// ── Localización ──────────────────────────────────────────────
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// ── Imágenes ──────────────────────────────────────────────────
// Las imágenes que sube Rosalía se guardan en la base, no en el disco:
// en App Service el contenido de wwwroot se pisa en cada despliegue.
builder.Services.AddScoped<IServicioImagenes, ServicioImagenes>();

// El límite del formulario acompaña al del servicio, con algo de margen
// para el resto de los campos.
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = ServicioImagenes.TamanoMaximo + 512 * 1024;
});

var app = builder.Build();

// ── Localización: middleware ──────────────────────────────────
var culturasSoportadas = new[] { new CultureInfo("es"), new CultureInfo("en"), new CultureInfo("pt") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("es"),
    SupportedCultures = culturasSoportadas,
    SupportedUICultures = culturasSoportadas,
    RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider()
    }
});

// ── Pipeline ──────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Tipos MIME que App Service sobre Linux no siempre trae de fábrica,
// más caché larga para fuentes e imágenes de marca, que no cambian.
var tiposDeArchivo = new FileExtensionContentTypeProvider();
tiposDeArchivo.Mappings[".woff2"] = "font/woff2";
tiposDeArchivo.Mappings[".webp"] = "image/webp";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = tiposDeArchivo,
    OnPrepareResponse = ctx =>
    {
        var ruta = ctx.File.Name;
        if (ruta.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase) ||
            ctx.Context.Request.Path.StartsWithSegments("/img/marca"))
        {
            ctx.Context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
        }
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// ── Seed ──────────────────────────────────────────────────────
await DbSeeder.SeedAsync(app.Services);

app.Run();
