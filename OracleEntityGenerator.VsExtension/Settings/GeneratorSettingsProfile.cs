namespace OracleEntityGenerator.VsExtension.Settings;

public sealed record GeneratorSettingsProfile
{
    public string Host { get; init; } = string.Empty;

    public int Port { get; init; } = 1521;

    public string ServiceName { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public string FullConnectionString { get; init; } = string.Empty;

    public bool ShowSystemSchemas { get; init; }

    public string EntityNamespace { get; init; } = "MyProject.Domain.Entities";

    public string ConfigurationNamespace { get; init; } = "MyProject.Infrastructure.Persistence.Configurations";

    public string OutputDirectory { get; init; } = string.Empty;

    public string EntityOutputDirectory { get; init; } = "Entities";

    public string ConfigurationOutputDirectory { get; init; } = "Configurations";

    public string TablePrefixesToRemove { get; init; } = string.Empty;

    public string ColumnPrefixesToRemove { get; init; } = string.Empty;

    public bool OverwriteExistingFiles { get; init; }

    public bool NullableReferenceTypes { get; init; } = true;

    public bool GenerateXmlComments { get; init; }

    public bool GenerateColumnTypeMappings { get; init; }

    public bool CompactOutput { get; init; }

    public bool GenerateKeylessEntities { get; init; }

    public string FileHeader { get; init; } = string.Empty;

    public string EntityUsings { get; init; } = string.Empty;

    public string ConfigurationUsings { get; init; } = string.Empty;
}
