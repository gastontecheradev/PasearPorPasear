namespace PasearPorPasear.Services;

/// <summary>Cómo se edita y se muestra un texto del sitio.</summary>
public enum TipoTexto
{
    /// <summary>Una línea, en los tres idiomas.</summary>
    Linea,
    /// <summary>Un párrafo, en los tres idiomas.</summary>
    Parrafo,
    /// <summary>Varios párrafos con HTML, en los tres idiomas.</summary>
    Html,
    /// <summary>Una línea que no se traduce (un nombre de usuario, una ciudad).</summary>
    Dato,
    /// <summary>Dirección https://, igual en los tres idiomas.</summary>
    Enlace,
    /// <summary>Correo electrónico, igual en los tres idiomas.</summary>
    Correo,
    /// <summary>Sí o no. Se guarda «si» o «no».</summary>
    Interruptor
}

/// <summary>
/// Un texto editable del sitio. La clave empieza por la página a la que
/// pertenece: «portada.frase» se edita en la pantalla de la Portada.
/// Si En o Pt quedan vacíos, el sitio muestra el español.
/// </summary>
public sealed record DefTexto(
    string Clave, string Grupo, string Nombre, TipoTexto Tipo,
    string Es, string En = "", string Pt = "",
    string? Pista = null, bool Opcional = false)
{
    public string Pagina => Clave[..Clave.IndexOf('.')];
    public bool Traducible => Tipo is TipoTexto.Linea or TipoTexto.Parrafo or TipoTexto.Html;
}

/// <summary>Una pantalla del panel: una página del sitio, o lo común a todas.</summary>
public sealed record PaginaPanel(
    string Clave, string Nombre, string Descripcion, string? UrlPublica,
    string? UrlListado = null, string? TextoListado = null)
{
    /// <summary>A dónde lleva el menú: al listado si la página tiene entradas.</summary>
    public string UrlPanel => UrlListado ?? UrlTextos;
    public string UrlTextos => Clave == "sobre" ? "/Admin/Paginas/Sobre" : $"/Admin/Paginas/{Clave}";
}

/// <summary>
/// Todos los textos fijos del sitio, con su valor de arranque. La base sólo
/// guarda lo que se cambia desde el panel (TextoSitio); mientras una clave no
/// tenga fila, se muestra el valor de acá.
/// Quedan afuera a propósito las etiquetas que sólo leen los lectores de
/// pantalla (títulos de iframes, nombres de menús) y los nombres de marca que
/// no cambian (Instagram, WhatsApp).
/// </summary>
public static class CatalogoTextos
{
    public static readonly IReadOnlyList<PaginaPanel> Paginas = new[]
    {
        new PaginaPanel("portada", "Portada", "Lo primero que se ve al entrar al sitio.", "/"),
        new PaginaPanel("sobre", "Sobre PPP", "La página donde contás de qué se trata.", "/Sobre"),
        new PaginaPanel("paseos", "¿Paseamos?", "Las cuatro propuestas para caminar acompañada.", "/Paseos",
            "/Admin/Propuestas", "Propuestas"),
        new PaginaPanel("archivo", "#ArchivoDeFachadas", "Las fichas de cada casa y la página del archivo.", "/Archivo",
            "/Admin/Fachadas", "Fichas"),
        new PaginaPanel("productos", "Productos", "El catálogo y el aviso de la tienda.", "/Productos",
            "/Admin/Productos", "Productos"),
        new PaginaPanel("cartelera", "Cartelera de barrio", "Los afiches del barrio y cómo mandar uno.", "/Cartelera",
            "/Admin/Cartelera", "Afiches"),
        new PaginaPanel("contacto", "Contacto", "La página de contacto y el formulario del buzón.", "/Contacto"),
        new PaginaPanel("general", "Menú, pie y enlaces", "Lo que se repite en todas las páginas: el menú, el pie, el correo y las redes.", null)
    };

    private const string Html = "Admite HTML: cada párrafo entre <p> y </p>.";

    public static readonly IReadOnlyList<DefTexto> Todos = new[]
    {
        // ── Portada ───────────────────────────────────────────
        new DefTexto("portada.frase", "Arriba de todo", "Frase de entrada", TipoTexto.Parrafo,
            "Salir a caminar sin destino. Ese es todo el plan.",
            "Going out to walk with nowhere to be. That is the whole plan.",
            "Sair para caminhar sem destino. É esse o plano inteiro.",
            "La frase grande debajo del logo."),
        new DefTexto("portada.entrada", "Arriba de todo", "Texto de entrada", TipoTexto.Parrafo,
            "Un recorrido a pie por los barrios de Montevideo: las casas bajas, las ventanas con plantas y todo lo que aparece cuando una va sin apuro.",
            "A walk through the neighbourhoods of Montevideo: the low houses, the windows full of plants and everything that shows up when you are in no hurry.",
            "Um percurso a pé pelos bairros de Montevidéu: as casas baixas, as janelas com plantas e tudo o que aparece quando se vai sem pressa.",
            Opcional: true),
        new DefTexto("portada.boton-paseos", "Arriba de todo", "Botón a ¿Paseamos?", TipoTexto.Linea,
            "¿Paseamos?", "Shall we walk?", "Vamos passear?"),
        new DefTexto("portada.boton-archivo", "Arriba de todo", "Botón al archivo", TipoTexto.Linea,
            "Ver el archivo", "See the archive", "Ver o arquivo"),

        new DefTexto("portada.definicion-titulo", "Qué es Pasear por Pasear", "Título", TipoTexto.Linea,
            "Qué es Pasear por Pasear", "What Pasear por Pasear is", "O que é Pasear por Pasear"),
        new DefTexto("portada.definicion-texto", "Qué es Pasear por Pasear", "Texto", TipoTexto.Html,
            "<p>Esto empezó como una costumbre: salir a caminar sin rumbo por Montevideo y prestarle atención a lo que siempre estuvo ahí. Una fachada amarilla que aguanta desde hace ochenta años. Una ventana con geranios. Un gorrión metido en un nido arriba de un cable.</p><p>No es turismo y no hay que ir muy lejos. Es mirar el barrio de uno como si fuera la primera vez.</p>",
            "<p>This started as a habit: going out to walk Montevideo with nowhere to be and paying attention to what was always there. A yellow facade that has held up for eighty years. A window with geraniums. A sparrow tucked into a nest above a cable.</p><p>It is not tourism and you do not have to go far. It is looking at your own neighbourhood as if for the first time.</p>",
            "<p>Isto começou como um costume: sair para caminhar sem rumo por Montevidéu e prestar atenção ao que sempre esteve ali. Uma fachada amarela que aguenta há oitenta anos. Uma janela com gerânios. Um pardal metido num ninho em cima de um cabo.</p><p>Não é turismo e não é preciso ir longe. É olhar o próprio bairro como se fosse a primeira vez.</p>",
            Html, Opcional: true),
        new DefTexto("portada.boton-sobre", "Qué es Pasear por Pasear", "Botón a Sobre PPP", TipoTexto.Linea,
            "Sobre PPP", "About PPP", "Sobre PPP"),

        new DefTexto("portada.buzon-titulo", "Buzón de la Casita", "Título", TipoTexto.Linea,
            "El buzón de la Casita", "The Casita mailbox", "A caixa de correio da Casita"),
        new DefTexto("portada.buzon-texto", "Buzón de la Casita", "Texto", TipoTexto.Parrafo,
            "Una carta por mes: la próxima salida del Clubcito, tres fachadas nuevas del archivo y alguna recomendación suelta.",
            "One letter a month: the next Clubcito walk, three new facades from the archive and the odd recommendation.",
            "Uma carta por mês: a próxima saída do Clubcito, três fachadas novas do arquivo e alguma recomendação solta.",
            Opcional: true),
        new DefTexto("portada.buzon-boton", "Buzón de la Casita", "Botón", TipoTexto.Linea,
            "Suscribirme", "Subscribe", "Inscrever-me", "Lleva al formulario de Contacto."),
        new DefTexto("portada.buzon-nota", "Buzón de la Casita", "Nota del canal de WhatsApp", TipoTexto.Parrafo,
            "También hay canal de WhatsApp, si preferís que te llegue por ahí.",
            "There is also a WhatsApp channel, if you would rather get it there.",
            "Também há canal de WhatsApp, se preferir receber por lá.",
            "Sólo se muestra si hay un canal de WhatsApp cargado en «Menú, pie y enlaces».", Opcional: true),

        // ── Sobre PPP (el resto de la página está en su propia ficha) ──
        new DefTexto("sobre.colaborar-titulo", "Colaborar al sobre virtual", "Título", TipoTexto.Parrafo,
            "¿Te gusta el proyecto? Podés colaborar “al sobre virtual”.",
            "Do you like the project? You can chip in to the “virtual envelope”.",
            "Gosta do projeto? Você pode colaborar com o “envelope virtual”."),
        new DefTexto("sobre.colaborar-texto", "Colaborar al sobre virtual", "Texto", TipoTexto.Parrafo,
            "Este proyecto se sostiene con tiempo, cafecito y mucho amor. Si querés colaborar con lo que puedas, quieras, podés hacerlo acá:",
            "This project runs on time, coffee and a lot of love. If you'd like to chip in whatever you can, you can do it here:",
            "Este projeto se sustenta com tempo, cafezinho e muito amor. Se quiser colaborar com o que puder e quiser, pode fazer isso aqui:",
            Opcional: true),
        new DefTexto("sobre.colaborar-boton", "Colaborar al sobre virtual", "Botón", TipoTexto.Linea,
            "Dejar una propina", "Leave a tip", "Deixar uma gorjeta"),
        new DefTexto("sobre.colaborar-enlace", "Colaborar al sobre virtual", "A dónde lleva el botón", TipoTexto.Enlace,
            "https://link.mercadopago.com.uy/pasearporpasear", Pista: "El enlace de pago de Mercado Pago."),

        // ── ¿Paseamos? ────────────────────────────────────────
        new DefTexto("paseos.titulo", "Encabezado", "Título", TipoTexto.Linea,
            "¿Paseamos?", "Shall we walk?", "Vamos passear?"),
        new DefTexto("paseos.bajada", "Encabezado", "Bajada", TipoTexto.Parrafo,
            "Caminar Montevideo acompañada. Hay cuatro maneras de hacerlo, según cuánto quieras comprometerte y con quién quieras ir.",
            "Walking Montevideo with company. There are four ways to do it, depending on how much you want to commit and who you want to go with.",
            "Caminhar Montevidéu acompanhada. Há quatro maneiras de fazê-lo, conforme o quanto queira se comprometer e com quem queira ir.",
            Opcional: true),
        new DefTexto("paseos.falta-definir", "Tarjetas", "Aviso de propuesta sin definir", TipoTexto.Linea,
            "Falta definir el contenido", "Content still to be defined", "Falta definir o conteúdo"),
        new DefTexto("paseos.boton-consultar", "Tarjetas", "Botón de las propuestas", TipoTexto.Linea,
            "Consultar", "Get in touch", "Consultar", "Lleva a Contacto."),
        new DefTexto("paseos.boton-clubcito", "Tarjetas", "Botón de El Clubcito", TipoTexto.Linea,
            "Recibir novedades", "Get updates", "Receber novidades"),
        new DefTexto("paseos.enlace-clubcito", "Tarjetas", "A dónde lleva el botón de El Clubcito", TipoTexto.Enlace,
            "https://preview.mailerlite.io/forms/2403548/194248063120835606/share",
            Pista: "El formulario de novedades del Clubcito en MailerLite."),

        // ── #ArchivoDeFachadas ────────────────────────────────
        new DefTexto("archivo.titulo", "Encabezado", "Título", TipoTexto.Dato, "#ArchivoDeFachadas",
            Pista: "Es parte de la marca: no se traduce."),
        new DefTexto("archivo.bajada", "Encabezado", "Bajada", TipoTexto.Parrafo,
            "Cada casa de Montevideo que me hizo frenar en la vereda, con su calle, su barrio y la fecha en que la encontré. Se agrega algo nuevo todas las semanas.",
            "Every house in Montevideo that made me stop on the pavement, with its street, its neighbourhood and the date I found it. Something new is added every week.",
            "Cada casa de Montevidéu que me fez parar na calçada, com sua rua, seu bairro e a data em que a encontrei. Algo novo é acrescentado toda semana.",
            Opcional: true),
        new DefTexto("archivo.filtro-todos", "Listado", "Filtro de todos los barrios", TipoTexto.Linea,
            "Todos", "All", "Todos"),
        new DefTexto("archivo.vacio", "Listado", "Barrio sin fachadas", TipoTexto.Parrafo,
            "Todavía no hay fachadas en este barrio.", "No facades in this neighbourhood yet.", "Ainda não há fachadas neste bairro."),
        new DefTexto("archivo.siguiente", "Listado", "Página siguiente", TipoTexto.Linea,
            "Siguiente", "Next", "Seguinte"),
        new DefTexto("archivo.mapa-titulo", "Mapa", "Título", TipoTexto.Linea,
            "Las fachadas en el mapa", "The facades on the map", "As fachadas no mapa"),
        new DefTexto("archivo.mapa-bajada", "Mapa", "Bajada", TipoTexto.Parrafo, "", Opcional: true),
        new DefTexto("archivo.mapa", "Mapa", "Dirección del mapa", TipoTexto.Enlace,
            "https://www.google.com/maps/d/embed?mid=1iuF8y_VcJXHIqgO80J7WfNwcECNli1c&hl=es&ehbc=2E312F",
            Pista: "En Google My Maps: Compartir, Insertar en mi sitio, y copiar sólo la dirección que está dentro de src=\"…\". Si lo dejás vacío, el mapa no se muestra.",
            Opcional: true),
        new DefTexto("archivo.ficha", "Cada ficha", "Antes del número", TipoTexto.Linea,
            "Ficha", "Entry", "Ficha", "Queda «Ficha N.º 347»."),
        new DefTexto("archivo.casa-de", "Cada ficha", "Antes del año de construcción", TipoTexto.Linea,
            "Casa de", "House from", "Casa de", "Queda «Casa de 1920»."),
        new DefTexto("archivo.cercanas", "Cada ficha", "Título de las fachadas del mismo barrio", TipoTexto.Linea,
            "Otras fachadas del mismo barrio", "Other facades in the same neighbourhood", "Outras fachadas do mesmo bairro"),
        new DefTexto("archivo.anterior", "Cada ficha", "Enlace a la ficha anterior", TipoTexto.Linea,
            "Ficha anterior", "Previous entry", "Ficha anterior"),
        new DefTexto("archivo.posterior", "Cada ficha", "Enlace a la ficha siguiente", TipoTexto.Linea,
            "Ficha siguiente", "Next entry", "Ficha seguinte"),

        // ── Productos ─────────────────────────────────────────
        new DefTexto("productos.titulo", "Encabezado", "Título", TipoTexto.Linea,
            "Productos", "Shop", "Produtos"),
        new DefTexto("productos.bajada", "Encabezado", "Bajada", TipoTexto.Parrafo,
            "Las ilustraciones de Pasear por Pasear, impresas en Montevideo en tiradas chicas. Todavía no están a la venta: esto es lo que se viene.",
            "The Pasear por Pasear illustrations, printed in Montevideo in small runs. Not on sale yet — this is what is coming.",
            "As ilustrações do Pasear por Pasear, impressas em Montevidéu em tiragens pequenas. Ainda não estão à venda: isto é o que vem por aí.",
            "No se muestra mientras la página esté en construcción.", Opcional: true),
        new DefTexto("productos.en-construccion", "En construcción", "¿La página está en construcción?", TipoTexto.Interruptor,
            "si", Pista: "Mientras diga «Sí», en lugar de los productos se ve el aviso de abajo."),
        new DefTexto("productos.construccion-titulo", "En construcción", "Título del aviso", TipoTexto.Linea,
            "Esta sección está en construcción", "This section is under construction", "Esta seção está em construção"),
        new DefTexto("productos.construccion-texto", "En construcción", "Texto del aviso", TipoTexto.Parrafo,
            "Estamos preparando los productos de la Casita. Muy pronto van a estar acá.",
            "We are getting the Casita products ready. They will be here soon.",
            "Estamos preparando os produtos da Casita. Em breve estarão aqui.", Opcional: true),
        new DefTexto("productos.gratis", "Tarjetas", "Precio de lo gratis", TipoTexto.Linea, "Gratis", "Free", "Grátis"),
        new DefTexto("productos.descargar", "Tarjetas", "Botón de descarga", TipoTexto.Linea, "Descargar", "Download", "Baixar"),
        new DefTexto("productos.pedir", "Tarjetas", "Botón de lo que está a la venta", TipoTexto.Linea, "Pedir", "Order", "Pedir",
            "Lleva a Contacto."),
        new DefTexto("productos.pronto", "Tarjetas", "Precio de lo que está en preparación", TipoTexto.Linea, "Pronto", "Soon", "Em breve"),
        new DefTexto("productos.en-preparacion", "Tarjetas", "Pastilla de lo que está en preparación", TipoTexto.Linea,
            "En preparación", "In the works", "Em preparação"),
        new DefTexto("productos.buzon-titulo", "Aviso por el buzón", "Título", TipoTexto.Linea,
            "Te avisamos cuando salgan", "We'll let you know when they're out", "Avisamos quando saírem"),
        new DefTexto("productos.buzon-texto", "Aviso por el buzón", "Texto", TipoTexto.Parrafo,
            "Todavía estamos con las pruebas de impresión. Cuando estén listos, el aviso sale primero por el buzón de la Casita.",
            "We are still on the printing tests. When they are ready, the news goes out first through the Casita mailbox.",
            "Ainda estamos nos testes de impressão. Quando estiverem prontos, o aviso sai primeiro pela caixa de correio da Casita.",
            Opcional: true),
        new DefTexto("productos.buzon-boton", "Aviso por el buzón", "Botón", TipoTexto.Linea,
            "Avisame", "Notify me", "Avise-me", "Lleva al formulario del buzón."),

        // ── Cartelera de barrio ───────────────────────────────
        new DefTexto("cartelera.titulo", "Encabezado", "Título", TipoTexto.Linea,
            "La cartelera de barrio", "The neighbourhood board", "O mural do bairro"),
        new DefTexto("cartelera.bajada", "Encabezado", "Bajada", TipoTexto.Parrafo,
            "Como la cartelera del almacén de la esquina, pero acá. Eventos, emprendimientos chicos y pedidos de ayuda del barrio.",
            "Like the board at the corner shop, but here. Events, small ventures and neighbours asking for a hand.",
            "Como o mural do armazém da esquina, mas aqui. Eventos, pequenos empreendimentos e pedidos de ajuda do bairro.",
            Opcional: true),
        new DefTexto("cartelera.como-titulo", "Cómo pegar tu afiche", "Título", TipoTexto.Linea,
            "Cómo pegar tu afiche", "How to put your poster up", "Como colar seu cartaz"),
        new DefTexto("cartelera.paso1-titulo", "Cómo pegar tu afiche", "Paso 1, título", TipoTexto.Linea,
            "Escribinos", "Write to us", "Escreva"),
        new DefTexto("cartelera.paso1-texto", "Cómo pegar tu afiche", "Paso 1, texto", TipoTexto.Parrafo,
            "Mandá un correo a {correo} con el asunto «cartelera de barrio».",
            "Send an email to {correo} with the subject «cartelera de barrio».",
            "Mande um e-mail para {correo} com o assunto «cartelera de barrio».",
            "{correo} se reemplaza por el correo de contacto, en negrita."),
        new DefTexto("cartelera.paso2-titulo", "Cómo pegar tu afiche", "Paso 2, título", TipoTexto.Linea,
            "Contanos qué es", "Tell us what it is", "Conte o que é"),
        new DefTexto("cartelera.paso2-texto", "Cómo pegar tu afiche", "Paso 2, texto", TipoTexto.Parrafo,
            "Qué pasa, cuándo, dónde y cómo te contactan. Si tenés una foto o un afiche hecho, mandalo también.",
            "What is happening, when, where and how to reach you. If you have a photo or a finished poster, send that too.",
            "O que acontece, quando, onde e como entram em contato. Se tiver uma foto ou um cartaz pronto, mande também."),
        new DefTexto("cartelera.paso3-titulo", "Cómo pegar tu afiche", "Paso 3, título", TipoTexto.Linea,
            "Lo pegamos", "We put it up", "A gente cola"),
        new DefTexto("cartelera.paso3-texto", "Cómo pegar tu afiche", "Paso 3, texto", TipoTexto.Parrafo,
            "Lo publicamos acá y queda hasta que pase la fecha. Es gratis y no hay que ser nadie en particular.",
            "We publish it here and it stays until the date passes. It is free and you do not need to be anyone in particular.",
            "Publicamos aqui e fica até passar a data. É grátis e não é preciso ser ninguém em especial."),
        new DefTexto("cartelera.boton-mandar", "Cómo pegar tu afiche", "Botón", TipoTexto.Linea,
            "Mandar mi afiche", "Send my poster", "Enviar meu cartaz", "Abre un correo nuevo al correo de contacto."),
        new DefTexto("cartelera.muro-titulo", "El muro", "Título", TipoTexto.Linea,
            "Lo que está pegado ahora", "What is up right now", "O que está colado agora"),
        new DefTexto("cartelera.vacia", "El muro", "Qué dice la cartelera vacía", TipoTexto.Parrafo,
            "Cartelera vacía de Pasear por Pasear. Si querés pegar tu afiche, escribí a {correo} con el asunto «cartelera de barrio» y la información sobre tu evento, pedido de ayuda o emprendimiento.",
            "Empty Pasear por Pasear board. To put your poster up, write to {correo} with the subject «cartelera de barrio» and the details of your event, request for help or small business.",
            "Mural vazio do Pasear por Pasear. Para colar seu cartaz, escreva para {correo} com o assunto «cartelera de barrio» e as informações do seu evento, pedido de ajuda ou empreendimento.",
            "Cuando no hay afiches se ve la plantilla vacía. Esto lo leen quienes usan lector de pantalla. {correo} se reemplaza por el correo de contacto."),
        new DefTexto("cartelera.cerrar", "El muro", "Botón para cerrar el afiche en grande", TipoTexto.Linea,
            "Cerrar", "Close", "Fechar"),

        // ── Contacto ──────────────────────────────────────────
        new DefTexto("contacto.titulo", "Encabezado", "Título", TipoTexto.Linea,
            "Contacto", "Contact", "Contato"),
        new DefTexto("contacto.bajada", "Encabezado", "Bajada", TipoTexto.Parrafo,
            "Para proponer un barrio, sumarte a una salida, hacer un pedido o mandar tu afiche a la cartelera.",
            "To suggest a neighbourhood, join a walk, place an order or send your poster to the board.",
            "Para propor um bairro, entrar num passeio, fazer um pedido ou enviar seu cartaz ao mural.",
            Opcional: true),
        new DefTexto("contacto.etiqueta-correo", "Datos", "Etiqueta del correo", TipoTexto.Linea,
            "Correo", "Email", "E-mail"),
        new DefTexto("contacto.etiqueta-donde", "Datos", "Etiqueta de dónde están", TipoTexto.Linea,
            "Dónde estamos", "Where we are", "Onde estamos"),
        new DefTexto("contacto.donde", "Datos", "Dónde están", TipoTexto.Dato, "Montevideo, Uruguay"),

        // ── Menú, pie y enlaces ───────────────────────────────
        new DefTexto("general.menu-inicio", "Menú", "Inicio", TipoTexto.Linea, "Inicio", "Home", "Início"),
        new DefTexto("general.menu-sobre", "Menú", "Sobre PPP", TipoTexto.Linea, "Sobre PPP", "About PPP", "Sobre PPP"),
        new DefTexto("general.menu-paseos", "Menú", "¿Paseamos?", TipoTexto.Linea, "¿Paseamos?", "Shall we walk?", "Vamos passear?"),
        new DefTexto("general.menu-archivo", "Menú", "#ArchivoDeFachadas", TipoTexto.Dato, "#ArchivoDeFachadas",
            Pista: "Es parte de la marca: no se traduce."),
        new DefTexto("general.menu-productos", "Menú", "Productos", TipoTexto.Linea, "Productos", "Shop", "Produtos"),
        new DefTexto("general.menu-cartelera", "Menú", "Cartelera", TipoTexto.Linea, "Cartelera", "Neighbourhood board", "Mural do bairro"),
        new DefTexto("general.menu-contacto", "Menú", "Contacto", TipoTexto.Linea, "Contacto", "Contact", "Contato"),

        new DefTexto("general.pie-lema", "Pie", "Frase debajo del logo", TipoTexto.Parrafo,
            "Caminar Montevideo sin apuro y contarlo. Barrio por barrio, vereda por vereda.",
            "Walking Montevideo slowly, and writing it down. Block by block, pavement by pavement.",
            "Caminhar Montevidéu sem pressa e contar. Bairro por bairro, calçada por calçada.", Opcional: true),
        new DefTexto("general.pie-recorrer", "Pie", "Título de la primera columna", TipoTexto.Linea, "Recorrer", "Explore", "Percorrer"),
        new DefTexto("general.pie-cartelera", "Pie", "Enlace a la cartelera", TipoTexto.Linea,
            "Cartelera de barrio", "Neighbourhood board", "Mural do bairro"),
        new DefTexto("general.pie-escribir", "Pie", "Título de la segunda columna", TipoTexto.Linea, "Escribir", "Write", "Escrever"),
        new DefTexto("general.pie-afiche", "Pie", "Enlace para mandar un afiche", TipoTexto.Linea,
            "Mandar un afiche", "Send a poster", "Enviar um cartaz"),
        new DefTexto("general.pie-buzon", "Pie", "Enlace al buzón", TipoTexto.Linea,
            "El buzón de la Casita", "The Casita mailbox", "A caixa de correio da Casita"),
        new DefTexto("general.pie-seguir", "Pie", "Título de la tercera columna", TipoTexto.Linea, "Seguir", "Follow", "Seguir"),
        new DefTexto("general.pie-firma", "Pie", "Firma", TipoTexto.Dato,
            "Pasear por Pasear · Rosalía Souza · Montevideo, Uruguay",
            Pista: "Va después de «©» y el año, que se ponen solos."),
        new DefTexto("general.pie-identidad", "Pie", "Crédito del diseño", TipoTexto.Linea,
            "Identidad visual: Mechi Barrón", "Visual identity: Mechi Barrón", "Identidade visual: Mechi Barrón", Opcional: true),
        new DefTexto("general.pie-whatsapp", "Pie", "Enlace al canal de WhatsApp", TipoTexto.Linea,
            "Canal de WhatsApp", "WhatsApp channel", "Canal do WhatsApp", "También es el texto del enlace en Contacto."),

        new DefTexto("general.correo", "Correo y redes", "Correo de contacto", TipoTexto.Correo,
            "pasearporpasear@gmail.com", Pista: "Aparece en Contacto, en la Cartelera y en el pie."),
        new DefTexto("general.instagram", "Correo y redes", "Instagram", TipoTexto.Enlace,
            "https://www.instagram.com/pasearporpasear/"),
        new DefTexto("general.instagram-usuario", "Correo y redes", "Usuario de Instagram", TipoTexto.Dato,
            "@pasearporpasear", Pista: "Cómo se muestra el enlace en Contacto."),
        new DefTexto("general.whatsapp", "Correo y redes", "Canal de WhatsApp", TipoTexto.Enlace,
            "https://www.whatsapp.com/channel/0029Va9ciekKWEKxelyurH08",
            Pista: "Si queda vacío, el sitio no menciona el canal.", Opcional: true),
        new DefTexto("general.buzon-formulario", "Correo y redes", "Formulario del buzón (MailerLite)", TipoTexto.Enlace,
            "https://preview.mailerlite.io/forms/2403548/193692652342872057/share",
            Pista: "Se muestra en Contacto y lo abren los botones de suscripción."),

        new DefTexto("general.atributo-1", "Marca", "Primera palabra de la marca", TipoTexto.Linea, "Movimiento", "Movement", "Movimento",
            "Las tres palabras separadas por flores, en la portada y en Sobre PPP."),
        new DefTexto("general.atributo-2", "Marca", "Segunda palabra", TipoTexto.Linea, "Curiosidad", "Curiosity", "Curiosidade"),
        new DefTexto("general.atributo-3", "Marca", "Tercera palabra", TipoTexto.Linea, "Identidad", "Identity", "Identidade"),
        new DefTexto("general.descripcion", "Marca", "Descripción para redes y buscadores", TipoTexto.Parrafo,
            "Caminar Montevideo sin apuro y contarlo. Barrio por barrio, vereda por vereda.",
            "Walking Montevideo slowly, and writing it down. Block by block.",
            "Caminhar Montevidéu sem pressa e contar. Bairro por bairro.",
            "La que aparece al compartir un enlace del sitio.")
    };

    private static readonly Dictionary<string, DefTexto> PorClave =
        Todos.ToDictionary(t => t.Clave, StringComparer.OrdinalIgnoreCase);

    public static DefTexto? Buscar(string clave) => PorClave.GetValueOrDefault(clave);

    public static PaginaPanel? Pagina(string clave) =>
        Paginas.FirstOrDefault(p => string.Equals(p.Clave, clave, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<DefTexto> DePagina(string pagina) =>
        Todos.Where(t => string.Equals(t.Pagina, pagina, StringComparison.OrdinalIgnoreCase));
}
