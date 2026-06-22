using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.Logging.Domain.Log;
using RS.Logging.Infra.Providers;

namespace RS.Logging.Infra.Mappings;

internal class LogMappings : IEntityTypeConfiguration<Log>
{
    private readonly IDbColumnTypes _t;
    public LogMappings(IDbColumnTypes t) => _t = t;

    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.ToTable("TB_Log");

        builder.HasKey(key => key.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id_Log")
            .HasColumnOrder(1)
            .HasColumnType(_t.BigInt)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.LogLevel)
            .HasColumnName("ie_LogLevel")
            .HasColumnOrder(2)
            .HasColumnType(_t.SmallInt);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("dt_CreatedAt")
            .HasColumnOrder(3)
            .HasDefaultValueSql(_t.NowSql);

        builder.Property(p => p.Message)
            .HasColumnName("ds_Message")
            .HasColumnOrder(4)
            .HasColumnType(_t.VarChar)
            .HasMaxLength(255);

        builder.Property(p => p.StackTrace)
            .HasColumnName("ds_StackTrace")
            .HasColumnOrder(5)
            .HasColumnType(_t.LongText);

        builder.Property(p => p.TenantId)
            .HasColumnName("ds_TenantId")
            .HasColumnOrder(6)
            .HasColumnType(_t.VarChar)
            .HasMaxLength(100);

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

        builder.HasIndex(i => i.CreatedAt, "idx_TB_Log_dt_CreatedAt");
        builder.HasIndex(i => new { i.CreatedAt, i.LogLevel }, "idx_TB_Log_dt_CreatedAt-ie_LogLevel");
        builder.HasIndex(i => i.TenantId, "idx_TB_Log_ds_TenantId");
        builder.HasIndex(i => i.CorrelationId, "idx_TB_Log_ds_CorrelationId");
        builder.HasIndex(i => i.TraceId, "idx_TB_Log_ds_TraceId");

        if (_t is MariaDbColumnTypes)
        {
            builder.HasIndex(i => i.Message, "ft_TB_Log_ds_Message").IsFullText();
            builder.HasIndex(i => i.StackTrace, "ft_TB_Log_ds_StackTrace").IsFullText();
        }
    }
}
