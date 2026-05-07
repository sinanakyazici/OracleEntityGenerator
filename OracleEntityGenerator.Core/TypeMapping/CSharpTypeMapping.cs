namespace OracleEntityGenerator.Core.TypeMapping;

public sealed record CSharpTypeMapping
{
    public required string TypeName { get; init; }

    public bool IsReferenceType { get; init; }

    public bool IsNullable { get; init; }

    public bool IsUnsupported { get; init; }

    public string? Warning { get; init; }
}
