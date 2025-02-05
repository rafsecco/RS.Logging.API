using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RS.Logging.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_Log",
                columns: table => new
                {
                    id_Log = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ie_LogLevel = table.Column<short>(type: "SMALLINT", nullable: false),
                    dt_CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "NOW()"),
                    ds_Message = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false, collation: "utf8_general_ci"),
                    ds_StackTrace = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Log", x => x.id_Log);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateIndex(
                name: "idx_Logs_CreatedAt",
                table: "TB_Log",
                column: "dt_CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_Logs_CreatedAt-LogLevel",
                table: "TB_Log",
                columns: new[] { "dt_CreatedAt", "ie_LogLevel" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_Log");
        }
    }
}
