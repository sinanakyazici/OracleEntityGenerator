namespace OracleEntityGenerator.Core.Metadata;

public sealed record OracleUniqueConstraintMetadata
{
    public required string Name { get; init; }

    public required IReadOnlyList<string> ColumnNames { get; init; }
}
