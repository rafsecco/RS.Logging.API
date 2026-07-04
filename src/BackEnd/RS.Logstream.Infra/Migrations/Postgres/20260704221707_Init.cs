using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RS.Logstream.Infra.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_ApiCallLog",
                columns: table => new
                {
                    id_ApiCallLog = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dt_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    fl_IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    ds_HttpMethod = table.Column<string>(type: "VARCHAR", maxLength: 10, nullable: false),
                    nr_ResponseStatusCode = table.Column<int>(type: "integer", nullable: true),
                    nr_DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    ds_Url = table.Column<string>(type: "TEXT", nullable: false),
                    ds_RequestBody = table.Column<string>(type: "TEXT", nullable: true),
                    ds_RequestHeaders = table.Column<string>(type: "TEXT", nullable: true),
                    ds_ResponseBody = table.Column<string>(type: "TEXT", nullable: true),
                    ds_ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    ds_TenantId = table.Column<string>(type: "VARCHAR", maxLength: 100, nullable: true),
                    ds_CorrelationId = table.Column<string>(type: "VARCHAR", maxLength: 64, nullable: true),
                    ds_TraceId = table.Column<string>(type: "VARCHAR", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_ApiCallLog", x => x.id_ApiCallLog);
                });

            migrationBuilder.CreateTable(
                name: "TB_Log",
                columns: table => new
                {
                    id_Log = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ie_LogLevel = table.Column<short>(type: "SMALLINT", nullable: false),
                    dt_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ds_Message = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    ds_StackTrace = table.Column<string>(type: "TEXT", nullable: true),
                    ds_TenantId = table.Column<string>(type: "VARCHAR", maxLength: 100, nullable: true),
                    ds_CorrelationId = table.Column<string>(type: "VARCHAR", maxLength: 64, nullable: true),
                    ds_TraceId = table.Column<string>(type: "VARCHAR", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Log", x => x.id_Log);
                });

            migrationBuilder.CreateTable(
                name: "TB_LogProcess",
                columns: table => new
                {
                    id_LogProcess = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_Process = table.Column<int>(type: "INT", nullable: false),
                    dt_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    nm_Process = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: true),
                    ds_TenantId = table.Column<string>(type: "VARCHAR", maxLength: 100, nullable: true),
                    ds_CorrelationId = table.Column<string>(type: "VARCHAR", maxLength: 64, nullable: true),
                    ds_TraceId = table.Column<string>(type: "VARCHAR", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LogProcess", x => x.id_LogProcess);
                });

            migrationBuilder.CreateTable(
                name: "TB_LogProcessDetail",
                columns: table => new
                {
                    id_LogProcessDetails = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cd_Process = table.Column<long>(type: "BIGINT", nullable: false),
                    ie_LogLevel = table.Column<short>(type: "SMALLINT", nullable: false),
                    dt_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ds_Message = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    ds_StackTrace = table.Column<string>(type: "TEXT", nullable: true),
                    ds_CorrelationId = table.Column<string>(type: "VARCHAR", maxLength: 64, nullable: true),
                    ds_TraceId = table.Column<string>(type: "VARCHAR", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LogProcessDetail", x => x.id_LogProcessDetails);
                    table.ForeignKey(
                        name: "FK-TB_LogProcessDetail_cd_Process-TB_LogProcess_id_LogProcess",
                        column: x => x.cd_Process,
                        principalTable: "TB_LogProcess",
                        principalColumn: "id_LogProcess",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_TB_ApiCallLog_ds_CorrelationId",
                table: "TB_ApiCallLog",
                column: "ds_CorrelationId");

            migrationBuilder.CreateIndex(
                name: "idx_TB_ApiCallLog_ds_TenantId",
                table: "TB_ApiCallLog",
                column: "ds_TenantId");

            migrationBuilder.CreateIndex(
                name: "idx_TB_ApiCallLog_ds_TraceId",
                table: "TB_ApiCallLog",
                column: "ds_TraceId");

            migrationBuilder.CreateIndex(
                name: "idx_TB_ApiCallLog_dt_CreatedAt",
                table: "TB_ApiCallLog",
                column: "dt_CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_TB_ApiCallLog_fl_IsSuccess",
                table: "TB_ApiCallLog",
                column: "fl_IsSuccess");

            migrationBuilder.CreateIndex(
                name: "idx_TB_ApiCallLog_nr_ResponseStatusCode",
                table: "TB_ApiCallLog",
                column: "nr_ResponseStatusCode");

            migrationBuilder.CreateIndex(
                name: "idx_TB_Log_ds_CorrelationId",
                table: "TB_Log",
                column: "ds_CorrelationId");

            migrationBuilder.CreateIndex(
                name: "idx_TB_Log_ds_TenantId",
                table: "TB_Log",
                column: "ds_TenantId");

            migrationBuilder.CreateIndex(
                name: "idx_TB_Log_ds_TraceId",
                table: "TB_Log",
                column: "ds_TraceId");

            migrationBuilder.CreateIndex(
                name: "idx_TB_Log_dt_CreatedAt",
                table: "TB_Log",
                column: "dt_CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_TB_Log_dt_CreatedAt-ie_LogLevel",
                table: "TB_Log",
                columns: new[] { "dt_CreatedAt", "ie_LogLevel" });

            migrationBuilder.CreateIndex(
                name: "IDX-TB_LogProcess_ds_CorrelationId",
                table: "TB_LogProcess",
                column: "ds_CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IDX-TB_LogProcess_ds_TenantId",
                table: "TB_LogProcess",
                column: "ds_TenantId");

            migrationBuilder.CreateIndex(
                name: "IDX-TB_LogProcess_ds_TraceId",
                table: "TB_LogProcess",
                column: "ds_TraceId");

            migrationBuilder.CreateIndex(
                name: "IDX-TB_LogProcess_dt_CreatedAt",
                table: "TB_LogProcess",
                column: "dt_CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IDX-TB_LogProcess_dt_CreatedAt-id_Process",
                table: "TB_LogProcess",
                columns: new[] { "dt_CreatedAt", "id_Process" });

            migrationBuilder.CreateIndex(
                name: "idx-TB_LogProcess_dt_CreatedAt",
                table: "TB_LogProcessDetail",
                column: "dt_CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx-TB_LogProcessDetail_ds_CorrelationId",
                table: "TB_LogProcessDetail",
                column: "ds_CorrelationId");

            migrationBuilder.CreateIndex(
                name: "idx-TB_LogProcessDetail_ds_TraceId",
                table: "TB_LogProcessDetail",
                column: "ds_TraceId");

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
                name: "TB_ApiCallLog");

            migrationBuilder.DropTable(
                name: "TB_Log");

            migrationBuilder.DropTable(
                name: "TB_LogProcessDetail");

            migrationBuilder.DropTable(
                name: "TB_LogProcess");
        }
    }
}
