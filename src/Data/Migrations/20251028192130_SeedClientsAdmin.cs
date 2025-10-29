using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace censudex_clients_service.src.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedClientsAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    FirstName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LastNames = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "clients",
                columns: new[] { "Id", "Address", "Birthdate", "CreatedAt", "Email", "FirstName", "IsActive", "LastNames", "PasswordHash", "PhoneNumber", "Role", "Username" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Headquarters", new DateOnly(1990, 1, 1), new DateOnly(2025, 10, 28), "admin@censudex.cl", "Admin", true, "Censudex", "$2a$11$Uc8ju29w6x8Z0dv6St3.De1BOELUrYXCLODoyxqpeBlrSZpCFGSS2", "+56900000000", 1, "admin" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Example Street 123", new DateOnly(1995, 5, 10), new DateOnly(2025, 10, 28), "john@censudex.cl", "John", true, "Doe", "$2a$11$NwiOOAh7imEqtZupJCN3A.HjxA5nLkhb2q8SpuHvyh0CeAHyLdzG6", "+56911111111", 0, "john" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Another Street 456", new DateOnly(2000, 3, 15), new DateOnly(2025, 10, 28), "jane@censudex.cl", "Jane", true, "Smith", "$2a$11$NwiOOAh7imEqtZupJCN3A.HjxA5nLkhb2q8SpuHvyh0CeAHyLdzG6", "+56922222222", 0, "jane" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Third Street 789", new DateOnly(1998, 10, 20), new DateOnly(2025, 10, 28), "mark@censudex.cl", "Mark", true, "Brown", "$2a$11$NwiOOAh7imEqtZupJCN3A.HjxA5nLkhb2q8SpuHvyh0CeAHyLdzG6", "+56933333333", 0, "mark" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_clients_Email",
                table: "clients",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clients_Username",
                table: "clients",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clients");
        }
    }
}
