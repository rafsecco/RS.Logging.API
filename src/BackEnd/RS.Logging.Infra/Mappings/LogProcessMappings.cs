using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.Logging.Domain.LogProcess;
using RS.Logging.Infra.Providers;

namespace RS.Logging.Infra.Mappings;

internal class LogProcessMappings : IEntityTypeConfiguration<LogProcess>
{
    private readonly IDbColumnTypes _t;
    public LogProcessMappings(IDbColumnTypes t) => _t = t;

    public void Configure(EntityTypeBuilder<LogProcess> builder)
    {
        builder.ToTable("TB_LogProcess");

        builder.HasKey(key => key.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id_LogProcess")
            .HasColumnOrder(1)
            .HasColumnType(_t.BigInt)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.ProcessId)
            .HasColumnName("id_Process")
            .HasColumnOrder(2)
            .HasColumnType(_t.IntUnsigned);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("dt_CreatedAt")
            .HasColumnOrder(3)
            .HasDefaultValueSql(_t.NowSql);

        builder.Property(p => p.Name)
            .HasColumnName("nm_Process")
            .HasColumnOrder(4)
            .HasColumnType(_t.VarChar)
            .HasMaxLength(255);

        builder.Property(p => p.TenantId)
            .HasColumnName("ds_TenantId")
            .HasColumnOrder(5)
            .HasColumnType(_t.VarChar)
            .HasMaxLength(100);

        builder.Property(p => p.CorrelationId)
            .HasColumnName("ds_CorrelationId")
            .HasColumnOrder(6)
            .HasColumnType(_t.VarChar)
            .HasMaxLength(64);

        builder.Property(p => p.TraceId)
            .HasColumnName("ds_TraceId")
            .HasColumnOrder(7)
            .HasColumnType(_t.VarChar)
            .HasMaxLength(64);

        builder.HasIndex(i => i.CreatedAt, "IDX-TB_LogProcess_dt_CreatedAt");
        builder.HasIndex(i => new { i.CreatedAt, i.ProcessId }, "IDX-TB_LogProcess_dt_CreatedAt-id_Process");
        builder.HasIndex(i => i.TenantId, "IDX-TB_LogProcess_ds_TenantId");
        builder.HasIndex(i => i.CorrelationId, "IDX-TB_LogProcess_ds_CorrelationId");
        builder.HasIndex(i => i.TraceId, "IDX-TB_LogProcess_ds_TraceId");

        builder
            .HasMany(e => e.LorProcessDetailList)
            .WithOne(e => e.LogProcess)
            .HasForeignKey(e => e.LogProcessId)
            .IsRequired();
    }
}
