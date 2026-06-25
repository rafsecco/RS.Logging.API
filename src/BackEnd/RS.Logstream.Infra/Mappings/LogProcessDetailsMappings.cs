using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.Logstream.Domain.LogProcess;
using RS.Logstream.Infra.Providers;

namespace RS.Logstream.Infra.Mappings;

internal class LogProcessDetailsMappings : IEntityTypeConfiguration<LogProcessDetail>
{
	private readonly IDbColumnTypes _t;
	public LogProcessDetailsMappings(IDbColumnTypes t) => _t = t;

	public void Configure(EntityTypeBuilder<LogProcessDetail> builder)
	{
		builder.ToTable("TB_LogProcessDetail");

		builder.HasKey(key => key.Id);

		builder.Property(p => p.Id)
			.HasColumnName("id_LogProcessDetails")
			.HasColumnOrder(1)
			.HasColumnType(_t.BigInt)
			.ValueGeneratedOnAdd();

		builder.Property(p => p.LogProcessId)
			.HasColumnName("cd_Process")
			.HasColumnOrder(2)
			.HasColumnType(_t.BigInt);

		builder.Property(p => p.LogLevel)
			.HasColumnName("ie_LogLevel")
			.HasColumnOrder(3)
			.HasColumnType(_t.SmallInt);

		builder.Property(p => p.CreatedAt)
			.HasColumnName("dt_CreatedAt")
			.HasColumnOrder(4)
			.HasDefaultValueSql(_t.NowSql);

		builder.Property(p => p.Message)
			.HasColumnName("ds_Message")
			.HasColumnOrder(5)
			.HasColumnType(_t.VarChar)
			.HasMaxLength(255);

		builder.Property(p => p.StackTrace)
			.HasColumnName("ds_StackTrace")
			.HasColumnOrder(6)
			.HasColumnType(_t.LongText);

		builder.Property(p => p.CorrelationId)
			.HasColumnName("ds_CorrelationId")
			.HasColumnOrder(7)
			.HasColumnType(_t.VarChar)
			.HasMaxLength(64);

		builder.Property(p => p.TraceId)
			.HasColumnName("ds_TraceId")
			.HasColumnOrder(8)
			.HasColumnType(_t.VarChar)
			.HasMaxLength(64);

		builder.HasIndex(i => i.CreatedAt, "idx-TB_LogProcess_dt_CreatedAt");
		builder.HasIndex(i => new { i.CreatedAt, i.LogProcessId }, "idx-TB_LogProcessDetail_dt_CreatedAt-cd_Process");
		builder.HasIndex(i => i.CorrelationId, "idx-TB_LogProcessDetail_ds_CorrelationId");
		builder.HasIndex(i => i.TraceId, "idx-TB_LogProcessDetail_ds_TraceId");

		if (_t is MariaDbColumnTypes)
		{
			builder.HasIndex(i => i.Message, "ft_TB_LogProcessDetail_ds_Message").IsFullText();
			builder.HasIndex(i => i.StackTrace, "ft_TB_LogProcessDetail_ds_StackTrace").IsFullText();
		}

		builder
			.HasOne(e => e.LogProcess)
			.WithMany(e => e.LorProcessDetailList)
			.HasForeignKey(e => e.LogProcessId)
			.HasConstraintName("FK-TB_LogProcessDetail_cd_Process-TB_LogProcess_id_LogProcess")
			.IsRequired();
	}
}
