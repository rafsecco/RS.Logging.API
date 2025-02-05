using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.Logging.Domain.Log;

namespace RS.Logging.Infra.Mappings;

internal class LogMappings : IEntityTypeConfiguration<Log>
{
	public void Configure(EntityTypeBuilder<Log> builder)
	{
		builder.ToTable("TB_Log");

		builder.HasKey(key => key.Id);

		builder.Property(p => p.Id)
			.HasColumnName("id_Log")
			.HasColumnOrder(1)
			.HasColumnType("BIGINT UNSIGNED")
			.ValueGeneratedOnAdd();

		builder.Property(p => p.LogLevel)
			.HasColumnName("ie_LogLevel")
			.HasColumnOrder(2)
			.HasColumnType("SMALLINT UNSIGNED");
		//.HasColumnType("SMALLINT UNSIGNED");

		builder.Property(p => p.CreatedAt)
			.HasColumnName("dt_CreatedAt")
			.HasColumnOrder(3)
			.HasDefaultValueSql("NOW()");

		builder.Property(p => p.Message)
			.HasColumnName("ds_Message")
			.HasColumnOrder(4)
			.HasColumnType("VARCHAR")
			.HasMaxLength(255);

		builder.Property(p => p.StackTrace)
			.HasColumnName("ds_StackTrace")
			.HasColumnOrder(5);
		//.HasColumnType("VARCHAR");

		builder.HasIndex(i => i.CreatedAt, "idx_TB_Log_dt_CreatedAt");
		builder.HasIndex(i => new { i.CreatedAt, i.LogLevel }, "idx_TB_Log_dt_CreatedAt-ie_LogLevel");
	}
}

