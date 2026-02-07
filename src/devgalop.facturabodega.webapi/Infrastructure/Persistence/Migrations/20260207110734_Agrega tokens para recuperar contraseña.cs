using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace devgalop.facturabodega.webapi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Agregatokenspararecuperarcontraseña : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "recover_password_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    expires_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recover_password_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_recover_password_tokens_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_recover_password_tokens_employee_id",
                table: "recover_password_tokens",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_recover_password_tokens_token",
                table: "recover_password_tokens",
                column: "token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recover_password_tokens");
        }
    }
}
