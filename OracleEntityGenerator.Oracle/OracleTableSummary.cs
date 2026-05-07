namespace OracleEntityGenerator.Oracle;

public sealed record OracleTableSummary(
    string SchemaName,
    string TableName,
    string? Comment,
    int ColumnCount,
    bool HasPrimaryKey);
