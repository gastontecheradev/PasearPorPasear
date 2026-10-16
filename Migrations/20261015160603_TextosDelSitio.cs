using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasearPorPasear.Migrations
{
    /// <inheritdoc />
    public partial class TextosDelSitio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TextosSitio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorPt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextosSitio", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TextosSitio_Clave",
                table: "TextosSitio",
                column: "Clave",
                unique: true);

            // Lo que ya estaba editado en los títulos de sección y en los
            // ajustes pasa a la tabla nueva con las claves del catálogo
            // (Services/CatalogoTextos.cs). Las tarjetas de pilares y el
            // encabezado del Clubcito no se copian: ya no se muestran.
            migrationBuilder.Sql(@"
INSERT INTO TextosSitio (Clave, Valor, ValorEn, ValorPt, ActualizadoEn)
SELECT m.Nueva,
       ISNULL(CASE m.Campo WHEN 'T' THEN e.Titulo   ELSE e.Bajada   END, ''),
       ISNULL(CASE m.Campo WHEN 'T' THEN e.TituloEn ELSE e.BajadaEn END, ''),
       ISNULL(CASE m.Campo WHEN 'T' THEN e.TituloPt ELSE e.BajadaPt END, ''),
       e.ActualizadoEn
FROM EncabezadosSeccion e
JOIN (VALUES
    ('portada',    'T', 'portada.frase'),
    ('portada',    'B', 'portada.entrada'),
    ('definicion', 'T', 'portada.definicion-titulo'),
    ('definicion', 'B', 'portada.definicion-texto'),
    ('paseos',     'T', 'paseos.titulo'),
    ('paseos',     'B', 'paseos.bajada'),
    ('archivo',    'T', 'archivo.titulo'),
    ('archivo',    'B', 'archivo.bajada'),
    ('productos',  'T', 'productos.titulo'),
    ('productos',  'B', 'productos.bajada'),
    ('cartelera',  'T', 'cartelera.titulo'),
    ('cartelera',  'B', 'cartelera.bajada'),
    ('contacto',   'T', 'contacto.titulo'),
    ('contacto',   'B', 'contacto.bajada')
) AS m(Clave, Campo, Nueva) ON m.Clave = e.Clave;

INSERT INTO TextosSitio (Clave, Valor, ValorEn, ValorPt, ActualizadoEn)
SELECT m.Nueva, a.Valor,
       CASE WHEN m.Traducible = 1 THEN a.ValorEn ELSE '' END,
       CASE WHEN m.Traducible = 1 THEN a.ValorPt ELSE '' END,
       SYSDATETIME()
FROM Ajustes a
JOIN (VALUES
    ('EmailContacto', 'general.correo',      0),
    ('CanalWhatsApp', 'general.whatsapp',    0),
    ('BuzonTexto',    'portada.buzon-texto', 1)
) AS m(Clave, Nueva, Traducible) ON m.Clave = a.Clave;
");

            migrationBuilder.DropTable(
                name: "Ajustes");

            migrationBuilder.DropTable(
                name: "EncabezadosSeccion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TextosSitio");

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
                name: "EncabezadosSeccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActualizadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Bajada = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BajadaEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BajadaPt = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Clave = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloPt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncabezadosSeccion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ajustes_Clave",
                table: "Ajustes",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EncabezadosSeccion_Clave",
                table: "EncabezadosSeccion",
                column: "Clave",
                unique: true);
        }
    }
}
