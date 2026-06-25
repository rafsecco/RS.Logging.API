using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RS.Logstream.Infra.Migrations.MariaDB
{
	/// <inheritdoc />
	public partial class AddApiCallLog : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "TB_ApiCallLog",
				columns: table => new
				{
					id_ApiCallLog = table.Column<ulong>(type: "BIGINT UNSIGNED", nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					dt_CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "NOW()"),
					fl_IsSuccess = table.Column<bool>(type: "tinyint(1)", nullable: false),
					ds_HttpMethod = table.Column<string>(type: "VARCHAR(10)", maxLength: 10, nullable: false, collation: "utf8_general_ci"),
					nr_ResponseStatusCode = table.Column<int>(type: "int", nullable: true),
					nr_DurationMs = table.Column<long>(type: "bigint", nullable: true),
					ds_Url = table.Column<string>(type: "LONGTEXT", nullable: false, collation: "utf8_general_ci"),
					ds_RequestBody = table.Column<string>(type: "LONGTEXT", nullable: true, collation: "utf8_general_ci"),
					ds_RequestHeaders = table.Column<string>(type: "LONGTEXT", nullable: true, collation: "utf8_general_ci"),
					ds_ResponseBody = table.Column<string>(type: "LONGTEXT", nullable: true, collation: "utf8_general_ci"),
					ds_ErrorMessage = table.Column<string>(type: "LONGTEXT", nullable: true, collation: "utf8_general_ci"),
					ds_TenantId = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true, collation: "utf8_general_ci"),
					ds_CorrelationId = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true, collation: "utf8_general_ci"),
					ds_TraceId = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true, collation: "utf8_general_ci")
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_TB_ApiCallLog", x => x.id_ApiCallLog);
				})
				.Annotation("Relational:Collation", "utf8_general_ci");

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
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "TB_ApiCallLog");
		}
	}
}
