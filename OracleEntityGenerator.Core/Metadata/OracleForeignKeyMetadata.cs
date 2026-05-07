namespace OracleEntityGenerator.Core.Metadata;

public sealed record OracleForeignKeyMetadata
{
    public required string Name { get; init; }

    public required IReadOnlyList<string> ColumnNames { get; init; }

    public required string ReferencedSchemaName { get; init; }

    public required string ReferencedTableName { get; init; }

    public required IReadOnlyList<string> ReferencedColumnNames { get; init; }
}
