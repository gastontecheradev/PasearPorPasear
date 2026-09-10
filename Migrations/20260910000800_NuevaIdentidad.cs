using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasearPorPasear.Migrations
{
    /// <inheritdoc />
    public partial class NuevaIdentidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ajustes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorPt = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ajustes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarteleraAfiches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Barrio = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Texto = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Contacto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VigenteHasta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Color = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ImagenDatos = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImagenTipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImagenUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagenAlt = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EmailRemitente = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarteleraAfiches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClubcitoEncuentros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloPt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Barrio = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    PuntoEncuentro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PuntoEncuentroEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PuntoEncuentroPt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Extracto = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ExtractoEn = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ExtractoPt = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoPt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenDatos = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImagenTipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImagenUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagenAlt = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MapaEmbedUrl = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Asistentes = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Publicado = table.Column<bool>(type: "bit", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubcitoEncuentros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EncabezadosSeccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloPt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Bajada = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BajadaEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BajadaPt = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ActualizadoEn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncabezadosSeccion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fachadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloPt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Barrio = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Calle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Extracto = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ExtractoEn = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ExtractoPt = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoPt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenDatos = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImagenTipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImagenUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagenAlt = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    AnioConstruccion = table.Column<int>(type: "int", nullable: true),
                    MapaEmbedUrl = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    FechaEncontrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadaEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualizadaEn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Publicada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fachadas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MensajesContacto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Mensaje = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EnviadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Leido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesContacto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaginasSobre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TituloEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TituloPt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoPt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Firma = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ImagenDatos = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImagenTipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImagenUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagenAlt = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ActualizadaEn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaginasSobre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    TituloEn = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    TituloPt = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    DescripcionEn = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    DescripcionPt = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ImagenDatos = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImagenTipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImagenUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagenAlt = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ColorPanel = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ArchivoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Propuestas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloPt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Etiqueta = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    EtiquetaEn = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    EtiquetaPt = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescripcionEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescripcionPt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenDatos = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImagenTipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImagenUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagenAlt = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PendienteDeDefinir = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    CreadaEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualizadaEn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propuestas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuscriptoresBuzon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Origen = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    SuscritoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuscriptoresBuzon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsultasPaseo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropuestaId = table.Column<int>(type: "int", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaDeseada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CantidadPersonas = table.Column<int>(type: "int", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CreadaEn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultasPaseo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultasPaseo_Propuestas_PropuestaId",
                        column: x => x.PropuestaId,
                        principalTable: "Propuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PropuestaDatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropuestaId = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ValorEn = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ValorPt = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Etiqueta = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EtiquetaEn = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EtiquetaPt = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropuestaDatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropuestaDatos_Propuestas_PropuestaId",
                        column: x => x.PropuestaId,
                        principalTable: "Propuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ajustes_Clave",
                table: "Ajustes",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CarteleraAfiches_Estado_VigenteHasta",
                table: "CarteleraAfiches",
                columns: new[] { "Estado", "VigenteHasta" });

            migrationBuilder.CreateIndex(
                name: "IX_ClubcitoEncuentros_Fecha",
                table: "ClubcitoEncuentros",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_ClubcitoEncuentros_Slug",
                table: "ClubcitoEncuentros",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasPaseo_CreadaEn",
                table: "ConsultasPaseo",
                column: "CreadaEn");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasPaseo_PropuestaId",
                table: "ConsultasPaseo",
                column: "PropuestaId");

            migrationBuilder.CreateIndex(
                name: "IX_EncabezadosSeccion_Clave",
                table: "EncabezadosSeccion",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fachadas_Barrio_Numero",
                table: "Fachadas",
                columns: new[] { "Barrio", "Numero" });

            migrationBuilder.CreateIndex(
                name: "IX_Fachadas_Numero",
                table: "Fachadas",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fachadas_Publicada",
                table: "Fachadas",
                column: "Publicada");

            migrationBuilder.CreateIndex(
                name: "IX_Fachadas_Slug",
                table: "Fachadas",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MensajesContacto_Leido_EnviadoEn",
                table: "MensajesContacto",
                columns: new[] { "Leido", "EnviadoEn" });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Slug",
                table: "Productos",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropuestaDatos_PropuestaId_Orden",
                table: "PropuestaDatos",
                columns: new[] { "PropuestaId", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_Propuestas_Clave",
                table: "Propuestas",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Propuestas_Slug",
                table: "Propuestas",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SuscriptoresBuzon_Email",
                table: "SuscriptoresBuzon",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ajustes");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CarteleraAfiches");

            migrationBuilder.DropTable(
                name: "ClubcitoEncuentros");

            migrationBuilder.DropTable(
                name: "ConsultasPaseo");

            migrationBuilder.DropTable(
                name: "EncabezadosSeccion");

            migrationBuilder.DropTable(
                name: "Fachadas");

            migrationBuilder.DropTable(
                name: "MensajesContacto");

            migrationBuilder.DropTable(
                name: "PaginasSobre");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "PropuestaDatos");

            migrationBuilder.DropTable(
                name: "SuscriptoresBuzon");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Propuestas");
        }
    }
}
