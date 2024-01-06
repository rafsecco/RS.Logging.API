using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.Logging.Domain.LogProcess;

namespace RS.Logging.Infra.Mappings;

internal class LogProcessDetailsMappings : IEntityTypeConfiguration<LogProcessDetail>
{
	public void Configure(EntityTypeBuilder<LogProcessDetail> builder)
	{
		builder.ToTable("TB_LogProcessDetail");

		builder.HasKey(key => key.Id);

		builder.Property(p => p.Id)
			.HasColumnName("id_LogProcessDetails")
			.HasColumnOrder(1)
			.HasColumnType("BIGINT UNSIGNED")
			.ValueGeneratedOnAdd();

		builder.Property(p => p.LogProcessId)
			.HasColumnName("cd_Process")
			.HasColumnOrder(2)
			.HasColumnType("BIGINT UNSIGNED");

		builder.Property(p => p.LogLevel)
			.HasColumnName("ie_LogLevel")
			.HasColumnOrder(3)
			.HasColumnType("SMALLINT UNSIGNED");
		//.HasColumnType("SMALLINT UNSIGNED");

		builder.Property(p => p.CreatedAt)
			.HasColumnName("dt_CreatedAt")
			.HasColumnOrder(4)
			.HasDefaultValueSql("NOW()");

		builder.Property(p => p.Message)
			.HasColumnName("ds_Message")
			.HasColumnOrder(5)
			.HasColumnType("VARCHAR")
			.HasMaxLength(255);

		builder.Property(p => p.StackTrace)
			.HasColumnName("ds_StackTrace")
			.HasColumnOrder(6);
		//.HasColumnType("VARCHAR");

		// Index
		builder.HasIndex(i => i.CreatedAt, "idx-TB_LogProcess_dt_CreatedAt");
		builder.HasIndex(i => new { i.CreatedAt, i.LogProcessId }, "idx-TB_LogProcessDetail_dt_CreatedAt-cd_Process");

		// Relationship
		builder
			.HasOne(e => e.LogProcess)
			.WithMany(e => e.LorProcessDetailList)
			.HasForeignKey(e => e.LogProcessId)
			.IsRequired();
	}
}
