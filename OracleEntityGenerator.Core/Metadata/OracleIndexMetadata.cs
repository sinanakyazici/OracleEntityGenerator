namespace OracleEntityGenerator.Core.Metadata;

public sealed record OracleIndexMetadata
{
    public required string Name { get; init; }

    public required IReadOnlyList<string> ColumnNames { get; init; }

    public bool IsUnique { get; init; }
}
