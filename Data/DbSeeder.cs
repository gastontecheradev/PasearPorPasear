using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PasearPorPasear.Models;
using PasearPorPasear.Services;

namespace PasearPorPasear.Data;

/// <summary>
/// Crea el esquema, el usuario administrador y el contenido de arranque.
/// Cada bloque es idempotente: sólo escribe si la tabla está vacía, así que
/// se puede correr en cada arranque sin duplicar nada.
/// </summary>
public static class DbSeeder
{
    private const string AdminRole = "Admin";

    // Las fotos del seed son archivos estáticos de wwwroot, no imágenes cargadas.
    // Van en ImagenUrl y dejan ImagenDatos en null; en cuanto Rosalía suba una
    // desde el panel, esa gana y la estática deja de usarse.
    private const string Demo = "/img/demo/";

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

        // Aplica las migraciones pendientes y crea el esquema en una base nueva.
        await ctx.Database.MigrateAsync();

        await SeedAdminAsync(userMgr, roleMgr, config, logger);

        SeedSobre(ctx);
        SeedPropuestas(ctx);
        SeedFachadas(ctx);
        SeedProductos(ctx);

        await ctx.SaveChangesAsync();
    }

    // ══════════════════════════════════════════════════════════
    //  Sobre PPP
    // ══════════════════════════════════════════════════════════
    private static void SeedSobre(ApplicationDbContext ctx)
    {
        var existente = ctx.PaginasSobre.FirstOrDefault();
        if (existente is not null)
        {
            SepararSobre(existente);

            // La foto del proyecto llegó después: las páginas ya creadas arrancan con
            // la demo.
            if (existente.ProyectoImagenUrl is null && existente.ProyectoImagenDatos is null)
            {
                existente.ProyectoImagenUrl = ProyectoImagenDemo;
                existente.ProyectoImagenAlt = ProyectoImagenAltDemo;
            }

            // «Por» pasó a ir con mayúscula. Sólo si sigue el título del seed.
            if (existente.Titulo == "Sobre Pasear por Pasear") existente.Titulo = SobreTitulo;
            if (existente.TituloPt == "Sobre Pasear por Pasear") existente.TituloPt = SobreTitulo;
            return;
        }

        ctx.PaginasSobre.Add(new PaginaSobre
        {
            Titulo = SobreTitulo,
            TituloEn = "About Pasear por Pasear",
            TituloPt = SobreTitulo,
            Contenido = SobreProyectoEs,
            ContenidoEn = SobreProyectoEn,
            ContenidoPt = SobreProyectoPt,
            ProyectoImagenUrl = ProyectoImagenDemo,
            ProyectoImagenAlt = ProyectoImagenAltDemo,
            Firma = "Rosalía Souza",
            ContenidoAutora = SobreAutoraEs,
            ContenidoAutoraEn = SobreAutoraEn,
            ContenidoAutoraPt = SobreAutoraPt,
            ImagenUrl = Demo + "rosalia.webp",
            ImagenAlt = "Rosalía caminando por la rambla de Montevideo al atardecer"
        });
    }

    private const string SobreTitulo = "Sobre Pasear Por Pasear";
    private const string ProyectoImagenDemo = Demo + "fachada-2.webp";
    private const string ProyectoImagenAltDemo = "Casa con buganvilla y techo rojo";

    // La página pasó de un solo texto a dos secciones: el proyecto y Rosalía.
    // Sólo se reparte el idioma que conserva el texto original del seed; si
    // Rosalía ya lo editó desde el panel, no se toca.
    private static void SepararSobre(PaginaSobre p)
    {
        var cambio = false;
        if (p.ContenidoAutora.Length == 0 && p.Contenido == SobreOriginalEs)
        {
            p.Contenido = SobreProyectoEs; p.ContenidoAutora = SobreAutoraEs; cambio = true;
        }
        if (p.ContenidoAutoraEn.Length == 0 && p.ContenidoEn == SobreOriginalEn)
        {
            p.ContenidoEn = SobreProyectoEn; p.ContenidoAutoraEn = SobreAutoraEn; cambio = true;
        }
        if (p.ContenidoAutoraPt.Length == 0 && p.ContenidoPt == SobreOriginalPt)
        {
            p.ContenidoPt = SobreProyectoPt; p.ContenidoAutoraPt = SobreAutoraPt; cambio = true;
        }
        if (cambio) p.ActualizadaEn = DateTime.Now;
    }

    private const string SobreProyectoEs =
        "<p>Hay una ciudad entera que vemos todos los días y no miramos nunca. El alma de Montevideo no " +
        "está en el Centro ni en la rambla: está en el barrio, en la vereda rota y en el kiosco de la esquina.</p>" +
        "<p>Pasear por Pasear es un archivo de fachadas que crece cada semana, paseos para quien quiera venir, " +
        "una cartelera para que el barrio publique lo suyo y una carta por mes para quien quiera recibirla.</p>";
    private const string SobreProyectoEn =
        "<p>There is a whole city we see every day and never look at. The soul of Montevideo is not downtown " +
        "or on the waterfront: it is in the neighbourhood, on the cracked pavement, at the corner kiosk.</p>" +
        "<p>Pasear por Pasear is a facade archive that grows every week, walks for anyone who wants to come " +
        "along, a board for the neighbourhood to post on, and one letter a month for whoever wants it.</p>";
    private const string SobreProyectoPt =
        "<p>Há uma cidade inteira que vemos todos os dias e nunca olhamos. A alma de Montevidéu não está no " +
        "Centro nem na orla: está no bairro, na calçada quebrada e no quiosque da esquina.</p>" +
        "<p>Pasear por Pasear é um arquivo de fachadas que cresce toda semana, passeios para quem quiser vir, " +
        "um mural para o bairro publicar o seu e uma carta por mês para quem quiser recebê-la.</p>";

    private const string SobreAutoraEs =
        "<p>Me llamo Rosalía, nací en Montevideo y me pasé media vida caminándola sin darme cuenta. " +
        "Un día empecé a sacarle fotos a las casas que me gustaban —las bajas, las descascaradas, las " +
        "que tienen una ventana con plantas— y las empecé a subir sin más plan que ese.</p>" +
        "<p>Resultó que a mucha gente le pasaba lo mismo. De esas fotos, y de esa gente, salió Pasear por " +
        "Pasear: lo mismo que hacía sola, un poco más ordenado.</p>";
    private const string SobreAutoraEn =
        "<p>My name is Rosalía. I was born in Montevideo and spent half my life walking it without " +
        "noticing. One day I started photographing the houses I liked —the low ones, the peeling ones, " +
        "the ones with a plant in the window— and posting them with no plan beyond that.</p>" +
        "<p>It turned out plenty of people felt the same. Those photos, and those people, became Pasear " +
        "por Pasear: what I used to do on my own, slightly better organised.</p>";
    private const string SobreAutoraPt =
        "<p>Meu nome é Rosalía, nasci em Montevidéu e passei metade da vida caminhando por ela sem " +
        "perceber. Um dia comecei a fotografar as casas de que gostava —as baixas, as descascadas, as " +
        "que têm uma janela com plantas— e a publicá-las sem mais plano que esse.</p>" +
        "<p>Descobri que muita gente sentia o mesmo. Dessas fotos, e dessas pessoas, nasceu Pasear por " +
        "Pasear: o que eu fazia sozinha, um pouco mais organizado.</p>";

    // El texto único que traía el seed antes de separar la página en dos.
    private const string SobreOriginalEs =
        "<p>Me llamo Rosalía, nací en Montevideo y me pasé media vida caminándola sin darme cuenta. " +
        "Un día empecé a sacarle fotos a las casas que me gustaban —las bajas, las descascaradas, las " +
        "que tienen una ventana con plantas— y las empecé a subir sin más plan que ese.</p>" +
        "<p>Resultó que a mucha gente le pasaba lo mismo. Que hay una ciudad entera que vemos todos los " +
        "días y no miramos nunca. Que el alma de Montevideo no está en el Centro ni en la rambla: está " +
        "en el barrio, en la vereda rota y en el kiosco de la esquina.</p>" +
        "<p>Pasear por Pasear es eso, un poco más ordenado. Un archivo de fachadas que crece cada semana, " +
        "paseos para quien quiera venir, una cartelera para que el barrio publique lo suyo y una carta " +
        "por mes para quien quiera recibirla.</p>";
    private const string SobreOriginalEn =
        "<p>My name is Rosalía. I was born in Montevideo and spent half my life walking it without " +
        "noticing. One day I started photographing the houses I liked —the low ones, the peeling ones, " +
        "the ones with a plant in the window— and posting them with no plan beyond that.</p>" +
        "<p>It turned out plenty of people felt the same. That there is a whole city we see every day and " +
        "never look at. That the soul of Montevideo is not downtown or on the waterfront: it is in the " +
        "neighbourhood, on the cracked pavement, at the corner kiosk.</p>" +
        "<p>Pasear por Pasear is that, slightly better organised. A facade archive that grows every week, " +
        "walks for anyone who wants to come along, a board for the neighbourhood to post on, and one " +
        "letter a month for whoever wants it.</p>";
    private const string SobreOriginalPt =
        "<p>Meu nome é Rosalía, nasci em Montevidéu e passei metade da vida caminhando por ela sem " +
        "perceber. Um dia comecei a fotografar as casas de que gostava —as baixas, as descascadas, as " +
        "que têm uma janela com plantas— e a publicá-las sem mais plano que esse.</p>" +
        "<p>Descobri que muita gente sentia o mesmo. Que há uma cidade inteira que vemos todos os dias e " +
        "nunca olhamos. Que a alma de Montevidéu não está no Centro nem na orla: está no bairro, na " +
        "calçada quebrada e no quiosque da esquina.</p>" +
        "<p>Pasear por Pasear é isso, um pouco mais organizado. Um arquivo de fachadas que cresce toda " +
        "semana, passeios para quem quiser vir, um mural para o bairro publicar o seu e uma carta por " +
        "mês para quem quiser recebê-la.</p>";

    // ══════════════════════════════════════════════════════════
    //  ¿Paseamos? — las cuatro propuestas
    // ══════════════════════════════════════════════════════════
    private static void SeedPropuestas(ApplicationDbContext ctx)
    {
        if (ctx.Propuestas.Any()) return;

        var clubcito = new Propuesta
        {
            Clave = ClavePropuesta.Clubcito,
            Titulo = "El Clubcito", TituloEn = "El Clubcito", TituloPt = "El Clubcito",
            Slug = "el-clubcito",
            Etiqueta = "Todos los meses", EtiquetaEn = "Every month", EtiquetaPt = "Todos os meses",
            Descripcion =
                "<p>El grupo que sale a caminar una vez por mes. Se avisa por el buzón de la Casita, se anota " +
                "quien quiere y cada vez se camina un barrio distinto. No hay que saber nada ni traer nada.</p>",
            DescripcionEn =
                "<p>The group that goes out walking once a month. We announce it through the Casita mailbox, " +
                "whoever wants signs up, and each time we walk a different neighbourhood. No knowledge or gear required.</p>",
            DescripcionPt =
                "<p>O grupo que sai para caminhar uma vez por mês. Avisamos pela caixa de correio da Casita, " +
                "inscreve-se quem quiser e a cada vez caminhamos um bairro diferente. Não é preciso saber nada nem levar nada.</p>",
            ImagenUrl = Demo + "casa-amarilla.webp",
            ImagenAlt = "Grupo caminando por una calle de casas bajas",
            Orden = 1,
            Datos =
            {
                new PropuestaDato { Orden = 1, Valor = "1 vez al mes", ValorEn = "Once a month", ValorPt = "1 vez por mês", Etiqueta = "frecuencia", EtiquetaEn = "how often", EtiquetaPt = "frequência" },
                new PropuestaDato { Orden = 2, Valor = "Gratis", ValorEn = "Free", ValorPt = "Grátis", Etiqueta = "siempre", EtiquetaEn = "always", EtiquetaPt = "sempre" },
                new PropuestaDato { Orden = 3, Valor = "Sin cupo fijo", ValorEn = "No fixed cap", ValorPt = "Sem vagas fixas", Etiqueta = "viene quien quiere", EtiquetaEn = "all welcome", EtiquetaPt = "vem quem quiser" }
            }
        };

        // «Al sobre» y «Recorridos creativos» quedan marcadas como pendientes:
        // la vista muestra el aviso de «falta definir» hasta que se complete el texto.
        var alSobre = new Propuesta
        {
            Clave = ClavePropuesta.AlSobre,
            Titulo = "Al sobre", TituloEn = "Al sobre", TituloPt = "Al sobre",
            Slug = "al-sobre",
            Etiqueta = "Propuesta", EtiquetaEn = "Offering", EtiquetaPt = "Proposta",
            Descripcion = "<p>Acá va la descripción de esta propuesta: qué es, cómo se accede y qué recibe la persona.</p>",
            ImagenUrl = Demo + "casa-flores.webp",
            ImagenAlt = "Fachada con enredadera y flores",
            PendienteDeDefinir = true,
            Orden = 2
        };

        var creativos = new Propuesta
        {
            Clave = ClavePropuesta.RecorridosCreativos,
            Titulo = "Recorridos creativos", TituloEn = "Creative walks", TituloPt = "Percursos criativos",
            Slug = "recorridos-creativos",
            Etiqueta = "Propuesta", EtiquetaEn = "Offering", EtiquetaPt = "Proposta",
            Descripcion = "<p>Acá va la descripción de esta propuesta: qué se hace durante el recorrido, cuánto dura y qué hay que llevar.</p>",
            ImagenUrl = Demo + "kiosco-plaza.webp",
            ImagenAlt = "Kiosco de fundición en una plaza",
            PendienteDeDefinir = true,
            Orden = 3
        };

        var personalizados = new Propuesta
        {
            Clave = ClavePropuesta.Personalizados,
            Titulo = "Personalizados", TituloEn = "Private walks", TituloPt = "Personalizados",
            Slug = "personalizados",
            Etiqueta = "A pedido", EtiquetaEn = "On request", EtiquetaPt = "Sob encomenda",
            Descripcion =
                "<p>Un recorrido armado para tu grupo, en el barrio que quieran y el día que les sirva. Sirve " +
                "para cumpleaños, para visitas de afuera o para una salida de la oficina que no sea otra vez un bar.</p>",
            DescripcionEn =
                "<p>A walk built for your group, in the neighbourhood you choose and on the day that suits you. " +
                "Good for birthdays, for visitors from out of town, or for a work outing that is not another bar.</p>",
            DescripcionPt =
                "<p>Um percurso montado para o seu grupo, no bairro que quiserem e no dia que servir. Serve para " +
                "aniversários, para visitas de fora ou para uma saída do escritório que não seja outro bar.</p>",
            ImagenUrl = Demo + "esquina.webp",
            ImagenAlt = "Casa de esquina en Malvín",
            Orden = 4,
            Datos =
            {
                new PropuestaDato { Orden = 1, Valor = "A convenir", ValorEn = "To agree", ValorPt = "A combinar", Etiqueta = "duración", EtiquetaEn = "length", EtiquetaPt = "duração" },
                new PropuestaDato { Orden = 2, Valor = "Desde 4", ValorEn = "From 4", ValorPt = "A partir de 4", Etiqueta = "personas", EtiquetaEn = "people", EtiquetaPt = "pessoas" },
                new PropuestaDato { Orden = 3, Valor = "Cualquier día", ValorEn = "Any day", ValorPt = "Qualquer dia", Etiqueta = "disponibilidad", EtiquetaEn = "availability", EtiquetaPt = "disponibilidade" }
            }
        };

        ctx.Propuestas.AddRange(clubcito, alSobre, creativos, personalizados);
    }

    // ══════════════════════════════════════════════════════════
    //  #ArchivoDeFachadas
    // ══════════════════════════════════════════════════════════
    private static void SeedFachadas(ApplicationDbContext ctx)
    {
        if (ctx.Fachadas.Any()) return;

        var hoy = DateTime.Today;

        (int Numero, string Titulo, string Barrio, string Foto, int Dias, string Alt)[] fichas =
        {
            (347, "Rivera casi Comercio",       "Buceo",         "fachada-1", 3,  "Casa baja amarilla con árbol adelante"),
            (346, "Lucas Obes y Suárez",        "Prado",         "fachada-2", 6,  "Casa con buganvilla y techo rojo"),
            (345, "Michigan y Amazonas",        "Malvín",        "fachada-3", 9,  "Casa naranja de esquina con cactus"),
            (344, "Larrañaga y Pan de Azúcar",  "La Blanqueada", "fachada-4", 13, "Casa con árbol podado y alambrado"),
            (343, "Bulevar Artigas al 1800",    "Villa Dolores", "fachada-5", 18, "Fachada con reja y árbol seco"),
            (342, "Orinoco esquina Concepción", "Malvín",        "fachada-6", 22, "Cuadra con techos de chapa rojos"),
            (341, "Comercio al 2200",           "La Blanqueada", "fachada-7", 25, "Cantero de vereda lleno de flores"),
            (340, "Plaza de los Treinta y Tres","Centro",        "fachada-8", 29, "Kiosco de fundición en una plaza"),
            (339, "Plaza Independencia",        "Centro",        "fachada-9", 32, "Kiosco verde con las torres al fondo")
        };

        foreach (var f in fichas)
        {
            var fecha = hoy.AddDays(-f.Dias);
            ctx.Fachadas.Add(new Fachada
            {
                Numero = f.Numero,
                Titulo = f.Titulo, TituloEn = f.Titulo, TituloPt = f.Titulo,
                Slug = SlugHelper.GenerateSlug($"{f.Numero}-{f.Titulo}"),
                Barrio = f.Barrio,
                Calle = f.Titulo,
                Extracto = "Ficha del archivo. El texto largo se escribe desde el panel.",
                Contenido = "<p>Acá va la ficha completa de esta fachada.</p>",
                ImagenUrl = Demo + f.Foto + ".webp",
                ImagenAlt = f.Alt,
                FechaEncontrada = fecha,
                FechaPublicacion = fecha,
                Publicada = true
            });
        }
    }

    // ══════════════════════════════════════════════════════════
    //  Productos
    // ══════════════════════════════════════════════════════════
    private static void SeedProductos(ApplicationDbContext ctx)
    {
        if (ctx.Productos.Any()) return;

        ctx.Productos.AddRange(
            new Producto
            {
                Titulo = "Sticker Gorrión", TituloEn = "Sparrow sticker", TituloPt = "Adesivo Pardal",
                Slug = "sticker-gorrion",
                Descripcion = "Vinilo resistente al agua, 6 cm. Para la notebook, la botella o la ventana del bondi.",
                DescripcionEn = "Waterproof vinyl, 6 cm. For the laptop, the water bottle or the bus window.",
                DescripcionPt = "Vinil resistente à água, 6 cm. Para o notebook, a garrafa ou a janela do ônibus.",
                ImagenUrl = "/img/marca/gorrion.webp", ImagenAlt = "Sticker del gorrión en su nido",
                ColorPanel = ColorAfiche.Mostaza, Estado = EstadoProducto.EnPreparacion, Orden = 1
            },
            new Producto
            {
                Titulo = "Sticker Kiosquito", TituloEn = "Kiosk sticker", TituloPt = "Adesivo Quiosque",
                Slug = "sticker-kiosquito",
                Descripcion = "El kiosco de fundición de la plaza, en vinilo de 6 cm. El más pedido de todos.",
                DescripcionEn = "The cast-iron kiosk from the square, 6 cm vinyl. The most requested of all.",
                DescripcionPt = "O quiosque de ferro fundido da praça, em vinil de 6 cm. O mais pedido de todos.",
                ImagenUrl = "/img/marca/kiosquito.webp", ImagenAlt = "Sticker del kiosco de fundición",
                ColorPanel = ColorAfiche.Rayas, Estado = EstadoProducto.EnPreparacion, Orden = 2
            },
            new Producto
            {
                Titulo = "Set de ventanas", TituloEn = "Window set", TituloPt = "Conjunto de janelas",
                Slug = "set-de-ventanas",
                Descripcion = "Las tres ventanas de la colección, juntas. Vinilo de 6 cm cada una.",
                DescripcionEn = "The three windows of the collection, together. 6 cm vinyl each.",
                DescripcionPt = "As três janelas da coleção, juntas. Vinil de 6 cm cada uma.",
                ImagenUrl = "/img/marca/ventana-2.webp", ImagenAlt = "Set de stickers de ventanas",
                ColorPanel = ColorAfiche.Durazno, Estado = EstadoProducto.EnPreparacion, Orden = 3
            },
            new Producto
            {
                Titulo = "Set completo", TituloEn = "Complete set", TituloPt = "Conjunto completo",
                Slug = "set-completo",
                Descripcion = "Los cinco stickers de la colección: el gorrión, el kiosquito y las tres ventanas.",
                DescripcionEn = "All five stickers: the sparrow, the kiosk and the three windows.",
                DescripcionPt = "Os cinco adesivos da coleção: o pardal, o quiosque e as três janelas.",
                ImagenUrl = "/img/marca/ventana-3.webp", ImagenAlt = "Set completo de los cinco stickers",
                ColorPanel = ColorAfiche.Crema, Estado = EstadoProducto.EnPreparacion, Orden = 4
            },
            new Producto
            {
                Titulo = "Lámina La Casita", TituloEn = "La Casita print", TituloPt = "Lâmina La Casita",
                Slug = "lamina-la-casita",
                Descripcion = "Impresión A4 en papel de 300 g, firmada al dorso. Va sin marco.",
                DescripcionEn = "A4 print on 300 gsm paper, signed on the back. Frame not included.",
                DescripcionPt = "Impressão A4 em papel de 300 g, assinada no verso. Sem moldura.",
                ImagenUrl = "/img/marca/casita.webp", ImagenAlt = "Lámina de la casita",
                ColorPanel = ColorAfiche.Mostaza, Estado = EstadoProducto.EnPreparacion, Orden = 5
            },
            new Producto
            {
                Titulo = "Cartelera imprimible", TituloEn = "Printable board", TituloPt = "Mural imprimível",
                Slug = "cartelera-imprimible",
                Descripcion = "Los seis diseños de afiche en A5, listos para imprimir y pegar en tu barrio.",
                DescripcionEn = "All six A5 poster designs, ready to print and put up in your neighbourhood.",
                DescripcionPt = "Os seis desenhos de cartaz em A5, prontos para imprimir e colar no seu bairro.",
                ImagenUrl = "/img/marca/ventana-1.webp", ImagenAlt = "Afiche imprimible de la cartelera de barrio",
                ColorPanel = ColorAfiche.Petroleo, Estado = EstadoProducto.DescargaGratis, Orden = 6
            });
    }

    // ══════════════════════════════════════════════════════════
    //  Usuario administrador
    //  Sin cambios respecto de la versión anterior: las credenciales
    //  vienen de configuración y nunca del código fuente.
    // ══════════════════════════════════════════════════════════
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
            // La cuenta ya existe: nunca se le pisa la contraseña desde configuración.
            // Sólo se verifica que siga teniendo el rol.
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
            // Causa más común: la contraseña no cumple la política declarada en
            // Program.cs (8 o más caracteres, dígito, mayúscula y símbolo).
            logger.LogError(
                "No se pudo crear el usuario administrador '{Email}'. Errores: {Errors}",
                adminEmail,
                string.Join(" | ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
        }
    }
}
