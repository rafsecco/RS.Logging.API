namespace RS.Logstream.Infra.Providers;

public interface IDbColumnTypes
{
	string BigInt      { get; }  // "BIGINT UNSIGNED"  | "BIGINT"        | "BIGINT"
	string SmallInt    { get; }  // "SMALLINT UNSIGNED" | "SMALLINT"      | "SMALLINT"
	string IntUnsigned { get; }  // "INT UNSIGNED"      | "INT"           | "INT"
	string LongText    { get; }  // "LONGTEXT"          | "NVARCHAR(MAX)" | "TEXT"
	string VarChar     { get; }  // "VARCHAR"           | "NVARCHAR"      | "VARCHAR"
	string NowSql      { get; }  // "NOW()"             | "GETDATE()"     | "NOW()"
}
