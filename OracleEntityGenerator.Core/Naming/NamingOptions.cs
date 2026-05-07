namespace OracleEntityGenerator.Core.Naming;

public sealed record NamingOptions
{
    public bool PreserveOriginalNames { get; init; }

    public IReadOnlyList<string> TablePrefixesToRemove { get; init; } = [];

    public IReadOnlyList<string> ColumnPrefixesToRemove { get; init; } = [];
}
