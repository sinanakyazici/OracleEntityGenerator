namespace OracleEntityGenerator.Core.Metadata;

public sealed record OracleColumnMetadata
{
    public required string Name { get; init; }

    public required string DataType { get; init; }

    public int? Length { get; init; }

    public int? Precision { get; init; }

    public int? Scale { get; init; }

    public bool IsNullable { get; init; }

    public string? DefaultValue { get; init; }

    public string? Comment { get; init; }

    public int Ordinal { get; init; }
}
