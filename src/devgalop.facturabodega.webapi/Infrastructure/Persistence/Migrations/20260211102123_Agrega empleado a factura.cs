using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace devgalop.facturabodega.webapi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Agregaempleadoafactura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "employee_id",
                table: "invoices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_invoices_employee_id",
                table: "invoices",
                column: "employee_id");

            migrationBuilder.AddForeignKey(
                name: "fk_invoices_employees_employee_id",
                table: "invoices",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_invoices_employees_employee_id",
                table: "invoices");

            migrationBuilder.DropIndex(
                name: "ix_invoices_employee_id",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "employee_id",
                table: "invoices");
        }
    }
}
