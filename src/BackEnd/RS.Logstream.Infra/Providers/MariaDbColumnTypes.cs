namespace RS.Logstream.Infra.Providers;

public class MariaDbColumnTypes : IDbColumnTypes
{
	public string BigInt      => "BIGINT";
	public string SmallInt    => "SMALLINT UNSIGNED";
	public string IntUnsigned => "INT UNSIGNED";
	public string LongText    => "LONGTEXT";
	public string VarChar     => "VARCHAR";
	public string NowSql      => "NOW()";
}
