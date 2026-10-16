using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasearPorPasear.Migrations
{
    /// <inheritdoc />
    public partial class CategoriaAfiches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Categoria",
                table: "CarteleraAfiches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "CarteleraAfiches");
        }
    }
}
