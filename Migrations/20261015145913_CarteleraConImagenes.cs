using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasearPorPasear.Migrations
{
    /// <inheritdoc />
    public partial class CarteleraConImagenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Los afiches pasan a ser imágenes. Los de texto (los del seed) no
            // tienen imagen y no se pueden mostrar: se borran.
            migrationBuilder.Sql("DELETE FROM CarteleraAfiches WHERE ImagenDatos IS NULL;");

            // «PorRevisar» (0) dejó de existir: lo que quede así pasa a archivado.
            migrationBuilder.Sql("UPDATE CarteleraAfiches SET Estado = 2 WHERE Estado = 0;");

            migrationBuilder.DropColumn(
                name: "Barrio",
                table: "CarteleraAfiches");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "CarteleraAfiches");

            migrationBuilder.DropColumn(
                name: "Contacto",
                table: "CarteleraAfiches");

            migrationBuilder.DropColumn(
                name: "EmailRemitente",
                table: "CarteleraAfiches");

            migrationBuilder.DropColumn(
                name: "Texto",
                table: "CarteleraAfiches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barrio",
                table: "CarteleraAfiches",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "CarteleraAfiches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Contacto",
                table: "CarteleraAfiches",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailRemitente",
                table: "CarteleraAfiches",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Texto",
                table: "CarteleraAfiches",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "");
        }
    }
}
