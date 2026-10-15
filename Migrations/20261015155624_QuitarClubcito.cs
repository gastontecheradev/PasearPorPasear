using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasearPorPasear.Migrations
{
    /// <inheritdoc />
    public partial class QuitarClubcito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubcitoEncuentros");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClubcitoEncuentros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActualizadoEn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Asistentes = table.Column<int>(type: "int", nullable: true),
                    Barrio = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoPt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Extracto = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ExtractoEn = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ExtractoPt = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImagenAlt = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ImagenDatos = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImagenTipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImagenUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MapaEmbedUrl = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Publicado = table.Column<bool>(type: "bit", nullable: false),
                    PuntoEncuentro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PuntoEncuentroEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PuntoEncuentroPt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloPt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubcitoEncuentros", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubcitoEncuentros_Fecha",
                table: "ClubcitoEncuentros",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_ClubcitoEncuentros_Slug",
                table: "ClubcitoEncuentros",
                column: "Slug",
                unique: true);
        }
    }
}
