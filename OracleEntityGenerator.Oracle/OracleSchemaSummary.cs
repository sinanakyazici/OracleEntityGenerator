namespace OracleEntityGenerator.Oracle;

public sealed record OracleSchemaSummary(
    string SchemaName,
    int TableCount);
