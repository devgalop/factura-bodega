using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace devgalop.facturabodega.webapi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarelaciondeproductoconStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_stocks_products_product_id",
                table: "product_stocks");

            migrationBuilder.DropIndex(
                name: "ix_product_stocks_product_id",
                table: "product_stocks");

            migrationBuilder.AddColumn<Guid>(
                name: "stock_id",
                table: "products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_products_stock_id",
                table: "products",
                column: "stock_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_products_product_stocks_stock_id",
                table: "products",
                column: "stock_id",
                principalTable: "product_stocks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_product_stocks_stock_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "ix_products_stock_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "stock_id",
                table: "products");

            migrationBuilder.CreateIndex(
                name: "ix_product_stocks_product_id",
                table: "product_stocks",
                column: "product_id");

            migrationBuilder.AddForeignKey(
                name: "fk_product_stocks_products_product_id",
                table: "product_stocks",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
