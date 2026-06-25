namespace RS.Logstream.Infra.Providers;

public class PostgresColumnTypes : IDbColumnTypes
{
	public string BigInt      => "BIGINT";
	public string SmallInt    => "SMALLINT";
	public string IntUnsigned => "INT";
	public string LongText    => "TEXT";
	public string VarChar     => "VARCHAR";
	public string NowSql      => "NOW()";
}
