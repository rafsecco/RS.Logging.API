#pragma warning disable EF1001 // MigrationsAssembly is internal EF Core API — intentional for namespace filtering
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace RS.Logstream.Infra.Providers;

public abstract class NamespacedMigrationsAssembly : MigrationsAssembly
{
    private readonly string _namespace;
    private IReadOnlyDictionary<string, TypeInfo>? _filtered;

    protected NamespacedMigrationsAssembly(
        ICurrentDbContext currentContext,
        IDbContextOptions options,
        IMigrationsIdGenerator idGenerator,
        IDiagnosticsLogger<DbLoggerCategory.Migrations> logger,
        string @namespace)
        : base(currentContext, options, idGenerator, logger)
    {
        _namespace = @namespace;
    }

    public override IReadOnlyDictionary<string, TypeInfo> Migrations =>
        _filtered ??= base.Migrations
            .Where(kv => kv.Value.Namespace == _namespace)
            .ToDictionary(kv => kv.Key, kv => kv.Value);
}

public sealed class MariaDbMigrationsAssembly(
    ICurrentDbContext c, IDbContextOptions o,
    IMigrationsIdGenerator g, IDiagnosticsLogger<DbLoggerCategory.Migrations> l)
    : NamespacedMigrationsAssembly(c, o, g, l, "RS.Logstream.Infra.Migrations.MariaDB");

public sealed class PostgresMigrationsAssembly(
    ICurrentDbContext c, IDbContextOptions o,
    IMigrationsIdGenerator g, IDiagnosticsLogger<DbLoggerCategory.Migrations> l)
    : NamespacedMigrationsAssembly(c, o, g, l, "RS.Logstream.Infra.Migrations.Postgres");

public sealed class SqlServerMigrationsAssembly(
    ICurrentDbContext c, IDbContextOptions o,
    IMigrationsIdGenerator g, IDiagnosticsLogger<DbLoggerCategory.Migrations> l)
    : NamespacedMigrationsAssembly(c, o, g, l, "RS.Logstream.Infra.Migrations.SqlServer");
