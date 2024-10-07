using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasearPorPasear.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracionNueva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenPortada",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImagenesTexto",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "MapaUbicacion",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenPortada",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagenesTexto",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MapaUbicacion",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
