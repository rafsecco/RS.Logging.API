namespace RS.Logstream.Infra.Providers;

public class SqlServerColumnTypes : IDbColumnTypes
{
	public string BigInt      => "BIGINT";
	public string SmallInt    => "SMALLINT";
	public string IntUnsigned => "INT";
	public string LongText    => "NVARCHAR(MAX)";
	public string VarChar     => "NVARCHAR";
	public string NowSql      => "GETDATE()";
}
