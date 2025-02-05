using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.Logging.Domain.LogProcess;

namespace RS.Logging.Infra.Mappings;

internal class LogProcessMappings : IEntityTypeConfiguration<LogProcess>
{
	public void Configure(EntityTypeBuilder<LogProcess> builder)
	{
		builder.ToTable("TB_LogProcess");

		builder.HasKey(key => key.Id);

		builder.Property(p => p.Id)
			.HasColumnName("id_LogProcess")
			.HasColumnOrder(1)
			.HasColumnType("BIGINT UNSIGNED")
			.ValueGeneratedOnAdd();

		builder.Property(p => p.ProcessId)
			.HasColumnName("id_Process")
			.HasColumnOrder(2)
			.HasColumnType("INT UNSIGNED");

		builder.Property(p => p.CreatedAt)
			.HasColumnName("dt_CreatedAt")
			.HasColumnOrder(3)
			.HasDefaultValueSql("NOW()");

		builder.Property(p => p.Name)
			.HasColumnName("nm_Process")
			.HasColumnOrder(4)
			.HasColumnType("VARCHAR")
			.HasMaxLength(255);

		// Index
		builder.HasIndex(i => i.CreatedAt, "IDX-TB_LogProcess_dt_CreatedAt");
		builder.HasIndex(i => new { i.CreatedAt, i.ProcessId }, "IDX-TB_LogProcess_dt_CreatedAt-id_Process");

		// Relationship
		builder
			.HasMany(e => e.LorProcessDetailList)
			.WithOne(e => e.LogProcess)
			.HasForeignKey(e => e.LogProcessId)
			.IsRequired();
	}
}
