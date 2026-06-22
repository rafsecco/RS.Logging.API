using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RS.Logging.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantCorrelationTraceFullText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_LogProcessDetail_TB_LogProcess_cd_Process",
                table: "TB_LogProcessDetail");

            migrationBuilder.RenameIndex(
                name: "idx-TB_LogProcess_dt_CreatedAt1",
                table: "TB_LogProcessDetail",
                newName: "idx-TB_LogProcess_dt_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "idx-TB_LogProcess_dt_CreatedAt-id_Process",
                table: "TB_LogProcess",
                newName: "IDX-TB_LogProcess_dt_CreatedAt-id_Process");

            migrationBuilder.RenameIndex(
                name: "idx-TB_LogProcess_dt_CreatedAt",
                table: "TB_LogProcess",
                newName: "IDX-TB_LogProcess_dt_CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "ds_StackTrace",
                table: "TB_LogProcessDetail",
                type: "LONGTEXT",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<ulong>(
                name: "id_LogProcessDetails",
                table: "TB_LogProcessDetail",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIGINT UNSIGNED")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "ds_CorrelationId",
                table: "TB_LogProcessDetail",
                type: "VARCHAR(64)",
                maxLength: 64,
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("Relational:ColumnOrder", 7);

            migrationBuilder.AddColumn<string>(
                name: "ds_TraceId",
                table: "TB_LogProcessDetail",
                type: "VARCHAR(64)",
                maxLength: 64,
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<ulong>(
                name: "id_LogProcess",
                table: "TB_LogProcess",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIGINT UNSIGNED")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "ds_CorrelationId",
                table: "TB_LogProcess",
                type: "VARCHAR(64)",
                maxLength: 64,
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("Relational:ColumnOrder", 6);

            migrationBuilder.AddColumn<string>(
                name: "ds_TenantId",
                table: "TB_LogProcess",
                type: "VARCHAR(100)",
                maxLength: 100,
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<string>(
                name: "ds_TraceId",
                table: "TB_LogProcess",
                type: "VARCHAR(64)",
                maxLength: 64,
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<string>(
                name: "ds_StackTrace",
                table: "TB_Log",
                type: "LONGTEXT",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<ulong>(
                name: "id_Log",
                table: "TB_Log",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIGINT UNSIGNED")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "ds_CorrelationId",
                table: "TB_Log",
                type: "VARCHAR(64)",
                maxLength: 64,
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("Relational:ColumnOrder", 7);

            migrationBuilder.AddColumn<string>(
                name: "ds_TenantId",
                table: "TB_Log",
                type: "VARCHAR(100)",
                maxLength: 100,
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("Relational:ColumnOrder", 6);

            migrationBuilder.AddColumn<string>(
                name: "ds_TraceId",
                table: "TB_Log",
                type: "VARCHAR(64)",
                maxLength: 64,
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("Relational:ColumnOrder", 8);

            migrationBuilder.CreateIndex(
                name: "ft_TB_LogProcessDetail_ds_Message",
                table: "TB_LogProcessDetail",
                column: "ds_Message")
                .Annotation("MySql:FullTextIndex", true);

            migrationBuilder.CreateIndex(
                name: "ft_TB_LogProcessDetail_ds_StackTrace",
                table: "TB_LogProcessDetail",
                column: "ds_StackTrace")
                .Annotation("MySql:FullTextIndex", true);

            migrationBuilder.CreateIndex(
                name: "idx-TB_LogProcessDetail_ds_CorrelationId",
                table: "TB_LogProcessDetail",
                column: "ds_CorrelationId");

            migrationBuilder.CreateIndex(
                name: "idx-TB_LogProcessDetail_ds_TraceId",
                table: "TB_LogProcessDetail",
                column: "ds_TraceId");

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
                name: "ft_TB_Log_ds_Message",
                table: "TB_Log",
                column: "ds_Message")
                .Annotation("MySql:FullTextIndex", true);

            migrationBuilder.CreateIndex(
                name: "ft_TB_Log_ds_StackTrace",
                table: "TB_Log",
                column: "ds_StackTrace")
                .Annotation("MySql:FullTextIndex", true);

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

            migrationBuilder.AddForeignKey(
                name: "FK-TB_LogProcessDetail_cd_Process-TB_LogProcess_id_LogProcess",
                table: "TB_LogProcessDetail",
                column: "cd_Process",
                principalTable: "TB_LogProcess",
                principalColumn: "id_LogProcess",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK-TB_LogProcessDetail_cd_Process-TB_LogProcess_id_LogProcess",
                table: "TB_LogProcessDetail");

            migrationBuilder.DropIndex(
                name: "ft_TB_LogProcessDetail_ds_Message",
                table: "TB_LogProcessDetail");

            migrationBuilder.DropIndex(
                name: "ft_TB_LogProcessDetail_ds_StackTrace",
                table: "TB_LogProcessDetail");

            migrationBuilder.DropIndex(
                name: "idx-TB_LogProcessDetail_ds_CorrelationId",
                table: "TB_LogProcessDetail");

            migrationBuilder.DropIndex(
                name: "idx-TB_LogProcessDetail_ds_TraceId",
                table: "TB_LogProcessDetail");

            migrationBuilder.DropIndex(
                name: "IDX-TB_LogProcess_ds_CorrelationId",
                table: "TB_LogProcess");

            migrationBuilder.DropIndex(
                name: "IDX-TB_LogProcess_ds_TenantId",
                table: "TB_LogProcess");

            migrationBuilder.DropIndex(
                name: "IDX-TB_LogProcess_ds_TraceId",
                table: "TB_LogProcess");

            migrationBuilder.DropIndex(
                name: "ft_TB_Log_ds_Message",
                table: "TB_Log");

            migrationBuilder.DropIndex(
                name: "ft_TB_Log_ds_StackTrace",
                table: "TB_Log");

            migrationBuilder.DropIndex(
                name: "idx_TB_Log_ds_CorrelationId",
                table: "TB_Log");

            migrationBuilder.DropIndex(
                name: "idx_TB_Log_ds_TenantId",
                table: "TB_Log");

            migrationBuilder.DropIndex(
                name: "idx_TB_Log_ds_TraceId",
                table: "TB_Log");

            migrationBuilder.DropColumn(
                name: "ds_CorrelationId",
                table: "TB_LogProcessDetail");

            migrationBuilder.DropColumn(
                name: "ds_TraceId",
                table: "TB_LogProcessDetail");

            migrationBuilder.DropColumn(
                name: "ds_CorrelationId",
                table: "TB_LogProcess");

            migrationBuilder.DropColumn(
                name: "ds_TenantId",
                table: "TB_LogProcess");

            migrationBuilder.DropColumn(
                name: "ds_TraceId",
                table: "TB_LogProcess");

            migrationBuilder.DropColumn(
                name: "ds_CorrelationId",
                table: "TB_Log");

            migrationBuilder.DropColumn(
                name: "ds_TenantId",
                table: "TB_Log");

            migrationBuilder.DropColumn(
                name: "ds_TraceId",
                table: "TB_Log");

            migrationBuilder.RenameIndex(
                name: "idx-TB_LogProcess_dt_CreatedAt",
                table: "TB_LogProcessDetail",
                newName: "idx-TB_LogProcess_dt_CreatedAt1");

            migrationBuilder.RenameIndex(
                name: "IDX-TB_LogProcess_dt_CreatedAt-id_Process",
                table: "TB_LogProcess",
                newName: "idx-TB_LogProcess_dt_CreatedAt-id_Process");

            migrationBuilder.RenameIndex(
                name: "IDX-TB_LogProcess_dt_CreatedAt",
                table: "TB_LogProcess",
                newName: "idx-TB_LogProcess_dt_CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "ds_StackTrace",
                table: "TB_LogProcessDetail",
                type: "longtext",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "LONGTEXT",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<ulong>(
                name: "id_LogProcessDetails",
                table: "TB_LogProcessDetail",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIGINT UNSIGNED")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<ulong>(
                name: "id_LogProcess",
                table: "TB_LogProcess",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIGINT UNSIGNED")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "ds_StackTrace",
                table: "TB_Log",
                type: "longtext",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "LONGTEXT",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<ulong>(
                name: "id_Log",
                table: "TB_Log",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIGINT UNSIGNED")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_TB_LogProcessDetail_TB_LogProcess_cd_Process",
                table: "TB_LogProcessDetail",
                column: "cd_Process",
                principalTable: "TB_LogProcess",
                principalColumn: "id_LogProcess",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
