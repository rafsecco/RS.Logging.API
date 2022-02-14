using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RS.Logging.API.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdProcess = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    LogLevel = table.Column<short>(type: "SMALLINT", nullable: false),
                    Message = table.Column<string>(type: "VARCHAR(255)", nullable: false, collation: "utf8_general_ci"),
                    Info = table.Column<string>(type: "longtext", nullable: false, collation: "utf8_general_ci"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => new { x.Id, x.IdProcess });
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateIndex(
                name: "idx_Logs_DateCreated",
                table: "Logs",
                column: "DateCreated");

            migrationBuilder.CreateIndex(
                name: "idx_Logs_DateCreated-IdProcess",
                table: "Logs",
                columns: new[] { "DateCreated", "IdProcess" });

            migrationBuilder.CreateIndex(
                name: "idx_Logs_DateCreated-LogLevel",
                table: "Logs",
                columns: new[] { "DateCreated", "LogLevel" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
