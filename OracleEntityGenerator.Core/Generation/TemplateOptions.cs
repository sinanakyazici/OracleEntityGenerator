namespace OracleEntityGenerator.Core.Generation;

public sealed record TemplateOptions
{
    public string? FileHeader { get; init; }

    public IReadOnlyList<string> EntityUsings { get; init; } = [];

    public IReadOnlyList<string> ConfigurationUsings { get; init; } = [];
}
