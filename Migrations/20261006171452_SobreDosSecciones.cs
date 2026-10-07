using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasearPorPasear.Migrations
{
    /// <inheritdoc />
    public partial class SobreDosSecciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContenidoAutora",
                table: "PaginasSobre",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContenidoAutoraEn",
                table: "PaginasSobre",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContenidoAutoraPt",
                table: "PaginasSobre",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContenidoAutora",
                table: "PaginasSobre");

            migrationBuilder.DropColumn(
                name: "ContenidoAutoraEn",
                table: "PaginasSobre");

            migrationBuilder.DropColumn(
                name: "ContenidoAutoraPt",
                table: "PaginasSobre");
        }
    }
}
