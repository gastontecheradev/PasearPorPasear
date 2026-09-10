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
        SeedEncabezados(ctx);
        SeedPropuestas(ctx);
        SeedClubcito(ctx);
        SeedFachadas(ctx);
        SeedProductos(ctx);
        SeedCartelera(ctx);
        SeedAjustes(ctx);

        await ctx.SaveChangesAsync();
    }

    // ══════════════════════════════════════════════════════════
    //  Sobre PPP
    // ══════════════════════════════════════════════════════════
    private static void SeedSobre(ApplicationDbContext ctx)
    {
        if (ctx.PaginasSobre.Any()) return;

        ctx.PaginasSobre.Add(new PaginaSobre
        {
            Titulo = "Sobre Pasear por Pasear",
            TituloEn = "About Pasear por Pasear",
            TituloPt = "Sobre Pasear por Pasear",
            Contenido =
                "<p>Me llamo Rosalía, nací en Montevideo y me pasé media vida caminándola sin darme cuenta. " +
                "Un día empecé a sacarle fotos a las casas que me gustaban —las bajas, las descascaradas, las " +
                "que tienen una ventana con plantas— y las empecé a subir sin más plan que ese.</p>" +
                "<p>Resultó que a mucha gente le pasaba lo mismo. Que hay una ciudad entera que vemos todos los " +
                "días y no miramos nunca. Que el alma de Montevideo no está en el Centro ni en la rambla: está " +
                "en el barrio, en la vereda rota y en el kiosco de la esquina.</p>" +
                "<p>Pasear por Pasear es eso, un poco más ordenado. Un archivo de fachadas que crece cada semana, " +
                "paseos para quien quiera venir, una cartelera para que el barrio publique lo suyo y una carta " +
                "por mes para quien quiera recibirla.</p>",
            ContenidoEn =
                "<p>My name is Rosalía. I was born in Montevideo and spent half my life walking it without " +
                "noticing. One day I started photographing the houses I liked —the low ones, the peeling ones, " +
                "the ones with a plant in the window— and posting them with no plan beyond that.</p>" +
                "<p>It turned out plenty of people felt the same. That there is a whole city we see every day and " +
                "never look at. That the soul of Montevideo is not downtown or on the waterfront: it is in the " +
                "neighbourhood, on the cracked pavement, at the corner kiosk.</p>" +
                "<p>Pasear por Pasear is that, slightly better organised. A facade archive that grows every week, " +
                "walks for anyone who wants to come along, a board for the neighbourhood to post on, and one " +
                "letter a month for whoever wants it.</p>",
            ContenidoPt =
                "<p>Meu nome é Rosalía, nasci em Montevidéu e passei metade da vida caminhando por ela sem " +
                "perceber. Um dia comecei a fotografar as casas de que gostava —as baixas, as descascadas, as " +
                "que têm uma janela com plantas— e a publicá-las sem mais plano que esse.</p>" +
                "<p>Descobri que muita gente sentia o mesmo. Que há uma cidade inteira que vemos todos os dias e " +
                "nunca olhamos. Que a alma de Montevidéu não está no Centro nem na orla: está no bairro, na " +
                "calçada quebrada e no quiosque da esquina.</p>" +
                "<p>Pasear por Pasear é isso, um pouco mais organizado. Um arquivo de fachadas que cresce toda " +
                "semana, passeios para quem quiser vir, um mural para o bairro publicar o seu e uma carta por " +
                "mês para quem quiser recebê-la.</p>",
            Firma = "Rosalía Souza",
            ImagenUrl = Demo + "rosalia.webp",
            ImagenAlt = "Rosalía caminando por la rambla de Montevideo al atardecer"
        });
    }

    // ══════════════════════════════════════════════════════════
    //  Encabezados de sección
    // ══════════════════════════════════════════════════════════
    private static void SeedEncabezados(ApplicationDbContext ctx)
    {
        if (ctx.EncabezadosSeccion.Any()) return;

        ctx.EncabezadosSeccion.AddRange(
            new EncabezadoSeccion
            {
                Clave = "portada",
                Titulo = "Salir a caminar sin destino. Ese es todo el plan.",
                TituloEn = "Going out to walk with nowhere to be. That is the whole plan.",
                TituloPt = "Sair para caminhar sem destino. É esse o plano inteiro.",
                Bajada = "Un recorrido a pie por los barrios de Montevideo: las casas bajas, las ventanas con plantas y todo lo que aparece cuando una va sin apuro.",
                BajadaEn = "A walk through the neighbourhoods of Montevideo: the low houses, the windows full of plants and everything that shows up when you are in no hurry.",
                BajadaPt = "Um percurso a pé pelos bairros de Montevidéu: as casas baixas, as janelas com plantas e tudo o que aparece quando se vai sem pressa."
            },
            new EncabezadoSeccion
            {
                Clave = "definicion",
                Titulo = "Qué es Pasear por Pasear",
                TituloEn = "What Pasear por Pasear is",
                TituloPt = "O que é Pasear por Pasear",
                Bajada = "<p>Esto empezó como una costumbre: salir a caminar sin rumbo por Montevideo y prestarle atención a lo que siempre estuvo ahí. Una fachada amarilla que aguanta desde hace ochenta años. Una ventana con geranios. Un gorrión metido en un nido arriba de un cable.</p><p>No es turismo y no hay que ir muy lejos. Es mirar el barrio de uno como si fuera la primera vez.</p>",
                BajadaEn = "<p>This started as a habit: going out to walk Montevideo with nowhere to be and paying attention to what was always there. A yellow facade that has held up for eighty years. A window with geraniums. A sparrow tucked into a nest above a cable.</p><p>It is not tourism and you do not have to go far. It is looking at your own neighbourhood as if for the first time.</p>",
                BajadaPt = "<p>Isto começou como um costume: sair para caminhar sem rumo por Montevidéu e prestar atenção ao que sempre esteve ali. Uma fachada amarela que aguenta há oitenta anos. Uma janela com gerânios. Um pardal metido num ninho em cima de um cabo.</p><p>Não é turismo e não é preciso ir longe. É olhar o próprio bairro como se fosse a primeira vez.</p>"
            },
            new EncabezadoSeccion
            {
                Clave = "paseos",
                Titulo = "¿Paseás conmigo?",
                TituloEn = "Walk with me?",
                TituloPt = "Passeia comigo?",
                Bajada = "Caminar Montevideo acompañada. Hay cuatro maneras de hacerlo, según cuánto quieras comprometerte y con quién quieras ir.",
                BajadaEn = "Walking Montevideo with company. There are four ways to do it, depending on how much you want to commit and who you want to go with.",
                BajadaPt = "Caminhar Montevidéu acompanhada. Há quatro maneiras de fazê-lo, conforme o quanto queira se comprometer e com quem queira ir."
            },
            new EncabezadoSeccion
            {
                Clave = "clubcito",
                Titulo = "El Clubcito",
                TituloEn = "El Clubcito",
                TituloPt = "El Clubcito",
                Bajada = "Una vez por mes salimos a caminar un barrio. Es gratis, no hay que anotarse en ningún lado y no hace falta conocer a nadie. Se avisa por el buzón de la Casita y se aparece.",
                BajadaEn = "Once a month we go out and walk a neighbourhood. It is free, there is no sign-up list and you do not need to know anyone. We announce it through the Casita mailbox and you just show up.",
                BajadaPt = "Uma vez por mês saímos para caminhar um bairro. É grátis, não é preciso se inscrever em lugar nenhum nem conhecer ninguém. Avisamos pela caixa de correio da Casita e é só aparecer."
            },
            new EncabezadoSeccion
            {
                Clave = "archivo",
                Titulo = "#ArchivoDeFachadas",
                TituloEn = "#ArchivoDeFachadas",
                TituloPt = "#ArchivoDeFachadas",
                Bajada = "Cada casa de Montevideo que me hizo frenar en la vereda, con su calle, su barrio y la fecha en que la encontré. Se agrega algo nuevo todas las semanas.",
                BajadaEn = "Every house in Montevideo that made me stop on the pavement, with its street, its neighbourhood and the date I found it. Something new is added every week.",
                BajadaPt = "Cada casa de Montevidéu que me fez parar na calçada, com sua rua, seu bairro e a data em que a encontrei. Algo novo é acrescentado toda semana."
            },
            new EncabezadoSeccion
            {
                Clave = "productos",
                Titulo = "Productos",
                TituloEn = "Shop",
                TituloPt = "Produtos",
                Bajada = "Las ilustraciones de Pasear por Pasear, impresas en Montevideo en tiradas chicas. Todavía no están a la venta: esto es lo que se viene.",
                BajadaEn = "The Pasear por Pasear illustrations, printed in Montevideo in small runs. Not on sale yet — this is what is coming.",
                BajadaPt = "As ilustrações do Pasear por Pasear, impressas em Montevidéu em tiragens pequenas. Ainda não estão à venda: isto é o que vem por aí."
            },
            new EncabezadoSeccion
            {
                Clave = "cartelera",
                Titulo = "La cartelera de barrio",
                TituloEn = "The neighbourhood board",
                TituloPt = "O mural do bairro",
                Bajada = "Como la cartelera del almacén de la esquina, pero acá. Eventos, emprendimientos chicos y pedidos de ayuda del barrio.",
                BajadaEn = "Like the board at the corner shop, but here. Events, small ventures and neighbours asking for a hand.",
                BajadaPt = "Como o mural do armazém da esquina, mas aqui. Eventos, pequenos empreendimentos e pedidos de ajuda do bairro."
            },
            new EncabezadoSeccion
            {
                Clave = "contacto",
                Titulo = "Contacto",
                TituloEn = "Contact",
                TituloPt = "Contato",
                Bajada = "Para proponer un barrio, sumarte a una salida, hacer un pedido o mandar tu afiche a la cartelera.",
                BajadaEn = "To suggest a neighbourhood, join a walk, place an order or send your poster to the board.",
                BajadaPt = "Para propor um bairro, entrar num passeio, fazer um pedido ou enviar seu cartaz ao mural."
            });
    }

    // ══════════════════════════════════════════════════════════
    //  ¿Paseás conmigo? — las cuatro propuestas
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
    //  El Clubcito
    // ══════════════════════════════════════════════════════════
    private static void SeedClubcito(ApplicationDbContext ctx)
    {
        if (ctx.ClubcitoEncuentros.Any()) return;

        // Fechas relativas a hoy para que el seed no envejezca.
        var hoy = DateTime.Today;

        ctx.ClubcitoEncuentros.AddRange(
            new ClubcitoEncuentro
            {
                Titulo = "Reus al Norte", TituloEn = "Reus al Norte", TituloPt = "Reus al Norte",
                Slug = "reus-al-norte",
                Fecha = hoy.AddDays(12),
                Barrio = "Aguada",
                PuntoEncuentro = "Nicaragua y Galicia",
                PuntoEncuentroEn = "Nicaragua and Galicia", PuntoEncuentroPt = "Nicaragua e Galicia",
                Extracto = "Las casas de colores, la historia de Emilio Reus y por qué las pintaron así.",
                ExtractoEn = "The coloured houses, the story of Emilio Reus and why they were painted that way.",
                ExtractoPt = "As casas coloridas, a história de Emilio Reus e por que as pintaram assim.",
                Estado = EstadoEncuentro.Proximo
            },
            new ClubcitoEncuentro
            {
                Titulo = "Villa Muñoz y el Goes", TituloEn = "Villa Muñoz and Goes", TituloPt = "Villa Muñoz e o Goes",
                Slug = "villa-munoz-y-el-goes",
                Fecha = hoy.AddDays(40),
                Barrio = "Villa Muñoz",
                PuntoEncuentro = "Mercado Agrícola",
                PuntoEncuentroEn = "Mercado Agrícola", PuntoEncuentroPt = "Mercado Agrícola",
                Extracto = "Barrio de talleres, telas y ferias. Terminamos en el mercado, que es donde hay que terminar.",
                ExtractoEn = "A neighbourhood of workshops, fabric shops and street markets. We finish at the market, which is where you finish.",
                ExtractoPt = "Bairro de oficinas, tecidos e feiras. Terminamos no mercado, que é onde se deve terminar.",
                Estado = EstadoEncuentro.Proximo
            },
            new ClubcitoEncuentro
            {
                Titulo = "Todavía sin definir", TituloEn = "Still to be decided", TituloPt = "Ainda por definir",
                Slug = "sin-definir-" + hoy.AddDays(68).ToString("yyyy-MM"),
                Fecha = hoy.AddDays(68),
                Extracto = "El barrio lo elige el Clubcito. Se vota por el buzón unas semanas antes.",
                ExtractoEn = "The Clubcito picks the neighbourhood. We vote through the mailbox a few weeks ahead.",
                ExtractoPt = "O bairro é escolhido pelo Clubcito. Vota-se pela caixa de correio algumas semanas antes.",
                Estado = EstadoEncuentro.SinDefinir
            },
            new ClubcitoEncuentro
            {
                Titulo = "Jardines de vereda", TituloEn = "Pavement gardens", TituloPt = "Jardins de calçada",
                Slug = "jardines-de-vereda",
                Fecha = hoy.AddDays(-26),
                Barrio = "La Blanqueada",
                Extracto = "Cuando el cantero de la vereda se convierte en jardín. Doce cuadras y ni un metro repetido.",
                ExtractoEn = "When the strip of soil by the kerb turns into a garden. Twelve blocks and not one metre repeated.",
                ExtractoPt = "Quando o canteiro da calçada vira jardim. Doze quarteirões e nem um metro repetido.",
                ImagenUrl = Demo + "casa-arbol.webp",
                ImagenAlt = "Casa con árbol podado y alambrado",
                Asistentes = 22,
                Estado = EstadoEncuentro.Realizado
            },
            new ClubcitoEncuentro
            {
                Titulo = "Techos de chapa", TituloEn = "Tin roofs", TituloPt = "Telhados de chapa",
                Slug = "techos-de-chapa",
                Fecha = hoy.AddDays(-54),
                Barrio = "Malvín",
                Extracto = "El barrio antes de que llegaran las torres. Todavía quedan cuadras enteras como eran.",
                ExtractoEn = "The neighbourhood before the towers arrived. Whole blocks are still as they were.",
                ExtractoPt = "O bairro antes de chegarem as torres. Ainda há quarteirões inteiros como eram.",
                ImagenUrl = Demo + "casa-techo.webp",
                ImagenAlt = "Cuadra con techos de chapa rojos",
                Asistentes = 17,
                Estado = EstadoEncuentro.Realizado
            },
            new ClubcitoEncuentro
            {
                Titulo = "De esquina a esquina", TituloEn = "Corner to corner", TituloPt = "De esquina a esquina",
                Slug = "de-esquina-a-esquina",
                Fecha = hoy.AddDays(-84),
                Barrio = "Buceo",
                Extracto = "Una vuelta corta con lluvia amenazando: seis esquinas y qué pasó en cada una.",
                ExtractoEn = "A short loop with rain threatening: six corners and what happened at each one.",
                ExtractoPt = "Uma volta curta com chuva ameaçando: seis esquinas e o que aconteceu em cada uma.",
                ImagenUrl = Demo + "vereda-flores.webp",
                ImagenAlt = "Cantero de vereda lleno de flores",
                Asistentes = 31,
                Estado = EstadoEncuentro.Realizado
            });
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
    //  Cartelera de barrio
    // ══════════════════════════════════════════════════════════
    private static void SeedCartelera(ApplicationDbContext ctx)
    {
        if (ctx.CarteleraAfiches.Any()) return;

        var hoy = DateTime.Today;

        ctx.CarteleraAfiches.AddRange(
            new CarteleraAfiche
            {
                Titulo = "Feria de las artesanas", Barrio = "Parque Batlle",
                Texto = "Cerámica, tejido y encuadernación. Con feria de plantas y algo para comer.",
                Contacto = "Sáb 14 · Plaza de los Olímpicos · 10 a 18 h",
                VigenteHasta = hoy.AddDays(20),
                Color = ColorAfiche.Crema, Estado = EstadoAfiche.Publicado, Orden = 1
            },
            new CarteleraAfiche
            {
                Titulo = "Se busca gato atigrado", Barrio = "Punta Carretas",
                Texto = "Se llama Nabo, es gris con rayas y bastante miedoso. Se perdió el domingo cerca de la rambla.",
                Contacto = "099 000 000",
                VigenteHasta = hoy.AddDays(30),
                Color = ColorAfiche.Petroleo, Estado = EstadoAfiche.Publicado, Orden = 2
            },
            new CarteleraAfiche
            {
                Titulo = "Taller de cerámica", Barrio = "Parque Rodó",
                Texto = "Ocho encuentros, materiales incluidos y horno propio. Quedan tres lugares.",
                Contacto = "Miércoles 18 h",
                VigenteHasta = hoy.AddDays(45),
                Color = ColorAfiche.Coral, Estado = EstadoAfiche.Publicado, Orden = 3
            },
            new CarteleraAfiche
            {
                Titulo = "Arreglos de costura", Barrio = "Malvín",
                Texto = "Ruedos, cierres, ajustes y ropa a medida. Trabajo en casa y entrego rápido.",
                Contacto = "Marta",
                Color = ColorAfiche.Rayas, Estado = EstadoAfiche.Publicado, Orden = 4
            },
            new CarteleraAfiche
            {
                Titulo = "Clases de guitarra", Barrio = "La Blanqueada",
                Texto = "A domicilio o en mi casa. Para empezar de cero, sin saber leer música. La primera es gratis.",
                Contacto = "Diego",
                Color = ColorAfiche.Durazno, Estado = EstadoAfiche.Publicado, Orden = 5
            },
            new CarteleraAfiche
            {
                Titulo = "Merienda compartida", Barrio = "Buceo",
                Texto = "Se junta el club de la esquina todos los últimos viernes. Cada uno lleva algo. Vengan.",
                Contacto = "Club Atlético Buceo · 17 h",
                VigenteHasta = hoy.AddDays(12),
                Color = ColorAfiche.Mostaza, Estado = EstadoAfiche.Publicado, Orden = 6
            },
            new CarteleraAfiche
            {
                Titulo = "Coro de barrio abierto", Barrio = "Villa Dolores",
                Texto = "Ensayamos los jueves en el salón de la parroquia. No hace falta saber cantar.",
                Contacto = "coro.villadolores@ejemplo.com",
                VigenteHasta = hoy.AddDays(50),
                Color = ColorAfiche.Crema, Estado = EstadoAfiche.PorRevisar, Orden = 7
            });
    }

    // ══════════════════════════════════════════════════════════
    //  Ajustes
    // ══════════════════════════════════════════════════════════
    private static void SeedAjustes(ApplicationDbContext ctx)
    {
        if (ctx.Ajustes.Any()) return;

        ctx.Ajustes.AddRange(
            new Ajuste { Clave = "MailchimpFormUrl", Valor = "", ValorEn = "", ValorPt = "" },
            new Ajuste { Clave = "EmailContacto", Valor = "pasearporpasear@gmail.com", ValorEn = "pasearporpasear@gmail.com", ValorPt = "pasearporpasear@gmail.com" },
            new Ajuste { Clave = "CanalWhatsApp", Valor = "https://www.whatsapp.com/channel/0029Va9ciekKWEKxelyurH08", ValorEn = "https://www.whatsapp.com/channel/0029Va9ciekKWEKxelyurH08", ValorPt = "https://www.whatsapp.com/channel/0029Va9ciekKWEKxelyurH08" },
            new Ajuste
            {
                Clave = "BuzonTexto",
                Valor = "Una carta por mes: la próxima salida del Clubcito, tres fachadas nuevas del archivo y alguna recomendación suelta.",
                ValorEn = "One letter a month: the next Clubcito walk, three new facades from the archive and the odd recommendation.",
                ValorPt = "Uma carta por mês: a próxima saída do Clubcito, três fachadas novas do arquivo e alguma recomendação solta."
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
