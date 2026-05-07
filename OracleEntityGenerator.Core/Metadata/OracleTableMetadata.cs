namespace OracleEntityGenerator.Core.Metadata;

public sealed record OracleTableMetadata
{
    public required string SchemaName { get; init; }

    public required string Name { get; init; }

    public string? Comment { get; init; }

    public required IReadOnlyList<OracleColumnMetadata> Columns { get; init; }

    public OraclePrimaryKeyMetadata? PrimaryKey { get; init; }

    public IReadOnlyList<OracleForeignKeyMetadata> ForeignKeys { get; init; } = [];

    public IReadOnlyList<OracleUniqueConstraintMetadata> UniqueConstraints { get; init; } = [];

    public IReadOnlyList<OracleIndexMetadata> Indexes { get; init; } = [];
}
