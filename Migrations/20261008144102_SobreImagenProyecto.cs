using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasearPorPasear.Migrations
{
    /// <inheritdoc />
    public partial class SobreImagenProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProyectoImagenAlt",
                table: "PaginasSobre",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProyectoImagenDatos",
                table: "PaginasSobre",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProyectoImagenTipo",
                table: "PaginasSobre",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProyectoImagenUrl",
                table: "PaginasSobre",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProyectoImagenAlt",
                table: "PaginasSobre");

            migrationBuilder.DropColumn(
                name: "ProyectoImagenDatos",
                table: "PaginasSobre");

            migrationBuilder.DropColumn(
                name: "ProyectoImagenTipo",
                table: "PaginasSobre");

            migrationBuilder.DropColumn(
                name: "ProyectoImagenUrl",
                table: "PaginasSobre");
        }
    }
}
