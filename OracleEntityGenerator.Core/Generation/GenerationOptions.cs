using OracleEntityGenerator.Core.Naming;
using OracleEntityGenerator.Core.TypeMapping;

namespace OracleEntityGenerator.Core.Generation;

public sealed record GenerationOptions
{
    public required string EntityNamespace { get; init; }

    public required string ConfigurationNamespace { get; init; }

    public string? EntityOutputDirectory { get; init; }

    public string? ConfigurationOutputDirectory { get; init; }

    public bool NullableReferenceTypes { get; init; } = true;

    public bool GenerateXmlComments { get; init; }

    public bool OverwriteExistingFiles { get; init; }

    public bool GenerateKeylessEntities { get; init; }

    public NamingOptions Naming { get; init; } = new();

    public OracleTypeMappingOptions TypeMapping { get; init; } = new();

    public TemplateOptions Template { get; init; } = new();
}
