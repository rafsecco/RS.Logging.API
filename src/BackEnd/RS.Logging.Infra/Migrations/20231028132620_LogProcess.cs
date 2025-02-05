using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RS.Logging.Infra.Migrations
{
    /// <inheritdoc />
    public partial class LogProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "idx_Logs_CreatedAt-LogLevel",
                table: "TB_Log",
                newName: "idx_TB_Log_dt_CreatedAt-ie_LogLevel");

            migrationBuilder.RenameIndex(
                name: "idx_Logs_CreatedAt",
                table: "TB_Log",
                newName: "idx_TB_Log_dt_CreatedAt");

            migrationBuilder.AlterColumn<ushort>(
                name: "ie_LogLevel",
                table: "TB_Log",
                type: "SMALLINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "SMALLINT");

            migrationBuilder.AlterColumn<ulong>(
                name: "id_Log",
                table: "TB_Log",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateTable(
                name: "TB_LogProcess",
                columns: table => new
                {
                    id_LogProcess = table.Column<ulong>(type: "BIGINT UNSIGNED", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_Process = table.Column<uint>(type: "INT UNSIGNED", nullable: false),
                    dt_CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "NOW()"),
                    nm_Process = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true, collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LogProcess", x => x.id_LogProcess);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "TB_LogProcessDetail",
                columns: table => new
                {
                    id_LogProcessDetails = table.Column<ulong>(type: "BIGINT UNSIGNED", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cd_Process = table.Column<ulong>(type: "BIGINT UNSIGNED", nullable: false),
                    ie_LogLevel = table.Column<ushort>(type: "SMALLINT UNSIGNED", nullable: false),
                    dt_CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "NOW()"),
                    ds_Message = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false, collation: "utf8_general_ci"),
                    ds_StackTrace = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LogProcessDetail", x => x.id_LogProcessDetails);
                    table.ForeignKey(
                        name: "FK_TB_LogProcessDetail_TB_LogProcess_cd_Process",
                        column: x => x.cd_Process,
                        principalTable: "TB_LogProcess",
                        principalColumn: "id_LogProcess",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateIndex(
                name: "idx-TB_LogProcess_dt_CreatedAt",
                table: "TB_LogProcess",
                column: "dt_CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx-TB_LogProcess_dt_CreatedAt-id_Process",
                table: "TB_LogProcess",
                columns: new[] { "dt_CreatedAt", "id_Process" });

            migrationBuilder.CreateIndex(
                name: "idx-TB_LogProcess_dt_CreatedAt1",
                table: "TB_LogProcessDetail",
                column: "dt_CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx-TB_LogProcessDetail_dt_CreatedAt-cd_Process",
                table: "TB_LogProcessDetail",
                columns: new[] { "dt_CreatedAt", "cd_Process" });

            migrationBuilder.CreateIndex(
                name: "IX_TB_LogProcessDetail_cd_Process",
                table: "TB_LogProcessDetail",
                column: "cd_Process");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_LogProcessDetail");

            migrationBuilder.DropTable(
                name: "TB_LogProcess");

            migrationBuilder.RenameIndex(
                name: "idx_TB_Log_dt_CreatedAt-ie_LogLevel",
                table: "TB_Log",
                newName: "idx_Logs_CreatedAt-LogLevel");

            migrationBuilder.RenameIndex(
                name: "idx_TB_Log_dt_CreatedAt",
                table: "TB_Log",
                newName: "idx_Logs_CreatedAt");

            migrationBuilder.AlterColumn<short>(
                name: "ie_LogLevel",
                table: "TB_Log",
                type: "SMALLINT",
                nullable: false,
                oldClrType: typeof(ushort),
                oldType: "SMALLINT UNSIGNED");

            migrationBuilder.AlterColumn<ulong>(
                name: "id_Log",
                table: "TB_Log",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIGINT UNSIGNED")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
