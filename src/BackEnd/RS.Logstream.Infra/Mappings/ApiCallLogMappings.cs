using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.Logstream.Domain.ApiCall;
using RS.Logstream.Infra.Providers;

namespace RS.Logstream.Infra.Mappings;

internal class ApiCallLogMappings : IEntityTypeConfiguration<ApiCallLog>
{
	private readonly IDbColumnTypes _t;
	public ApiCallLogMappings(IDbColumnTypes t) => _t = t;

	public void Configure(EntityTypeBuilder<ApiCallLog> builder)
	{
		builder.ToTable("TB_ApiCallLog");

		builder.HasKey(key => key.Id);

		builder.Property(p => p.Id)
			.HasColumnName("id_ApiCallLog")
			.HasColumnOrder(1)
			.HasColumnType(_t.BigInt)
			.ValueGeneratedOnAdd();

		builder.Property(p => p.CreatedAt)
			.HasColumnName("dt_CreatedAt")
			.HasColumnOrder(2)
			.HasDefaultValueSql(_t.NowSql);

		builder.Property(p => p.IsSuccess)
			.HasColumnName("fl_IsSuccess")
			.HasColumnOrder(3);

		builder.Property(p => p.HttpMethod)
			.HasColumnName("ds_HttpMethod")
			.HasColumnOrder(4)
			.HasColumnType(_t.VarChar)
			.HasMaxLength(10);

		builder.Property(p => p.ResponseStatusCode)
			.HasColumnName("nr_ResponseStatusCode")
			.HasColumnOrder(5);

		builder.Property(p => p.DurationMs)
			.HasColumnName("nr_DurationMs")
			.HasColumnOrder(6);

		builder.Property(p => p.Url)
			.HasColumnName("ds_Url")
			.HasColumnOrder(7)
			.HasColumnType(_t.LongText);

		builder.Property(p => p.RequestBody)
			.HasColumnName("ds_RequestBody")
			.HasColumnOrder(8)
			.HasColumnType(_t.LongText);

		builder.Property(p => p.RequestHeaders)
			.HasColumnName("ds_RequestHeaders")
			.HasColumnOrder(9)
			.HasColumnType(_t.LongText);

		builder.Property(p => p.ResponseBody)
			.HasColumnName("ds_ResponseBody")
			.HasColumnOrder(10)
			.HasColumnType(_t.LongText);

		builder.Property(p => p.ErrorMessage)
			.HasColumnName("ds_ErrorMessage")
			.HasColumnOrder(11)
			.HasColumnType(_t.LongText);

		builder.Property(p => p.TenantId)
			.HasColumnName("ds_TenantId")
			.HasColumnOrder(12)
			.HasColumnType(_t.VarChar)
			.HasMaxLength(100);

		builder.Property(p => p.CorrelationId)
			.HasColumnName("ds_CorrelationId")
			.HasColumnOrder(13)
			.HasColumnType(_t.VarChar)
			.HasMaxLength(64);

		builder.Property(p => p.TraceId)
			.HasColumnName("ds_TraceId")
			.HasColumnOrder(14)
			.HasColumnType(_t.VarChar)
			.HasMaxLength(64);

		builder.HasIndex(i => i.CreatedAt, "idx_TB_ApiCallLog_dt_CreatedAt");
		builder.HasIndex(i => i.IsSuccess, "idx_TB_ApiCallLog_fl_IsSuccess");
		builder.HasIndex(i => i.ResponseStatusCode, "idx_TB_ApiCallLog_nr_ResponseStatusCode");
		builder.HasIndex(i => i.TenantId, "idx_TB_ApiCallLog_ds_TenantId");
		builder.HasIndex(i => i.CorrelationId, "idx_TB_ApiCallLog_ds_CorrelationId");
		builder.HasIndex(i => i.TraceId, "idx_TB_ApiCallLog_ds_TraceId");
	}
}
