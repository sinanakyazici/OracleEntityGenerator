namespace OracleEntityGenerator.Core.Metadata;

public sealed record OraclePrimaryKeyMetadata
{
    public required string Name { get; init; }

    public required IReadOnlyList<string> ColumnNames { get; init; }
}
