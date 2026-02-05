using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace devgalop.facturabodega.webapi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Agregaindicepornombreparaproducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_products_name",
                table: "products",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_products_name",
                table: "products");
        }
    }
}
