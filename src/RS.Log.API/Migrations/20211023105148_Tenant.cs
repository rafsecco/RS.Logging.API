using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RS.Log.API.Migrations
{
    public partial class Tenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Message = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StackTrace = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Logs",
                columns: new[] { "Id", "DateCreated", "Message", "StackTrace", "TenantId" },
                values: new object[] { 1, new DateTime(2021, 10, 23, 7, 51, 47, 750, DateTimeKind.Local).AddTicks(1211), "teste 1", null, "tenant-1" });

            migrationBuilder.InsertData(
                table: "Logs",
                columns: new[] { "Id", "DateCreated", "Message", "StackTrace", "TenantId" },
                values: new object[] { 2, new DateTime(2021, 10, 23, 7, 51, 47, 751, DateTimeKind.Local).AddTicks(1584), "teste 2", null, "tenant-2" });

            migrationBuilder.InsertData(
                table: "Logs",
                columns: new[] { "Id", "DateCreated", "Message", "StackTrace", "TenantId" },
                values: new object[] { 3, new DateTime(2021, 10, 23, 7, 51, 47, 751, DateTimeKind.Local).AddTicks(1602), "teste 3", null, "tenant-2" });

            migrationBuilder.CreateIndex(
                name: "idx_tenantid",
                table: "Logs",
                columns: new[] { "DateCreated", "TenantId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
