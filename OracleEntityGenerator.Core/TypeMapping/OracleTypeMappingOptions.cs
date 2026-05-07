namespace OracleEntityGenerator.Core.TypeMapping;

public sealed record OracleTypeMappingOptions
{
    public OracleNumberOneMapping NumberOneMapping { get; init; } = OracleNumberOneMapping.Bool;
}
