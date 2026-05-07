using OracleEntityGenerator.Core.Generation;
using OracleEntityGenerator.Core.Metadata;

namespace OracleEntityGenerator.CodeGeneration;

public sealed class OracleCodeGenerationService
{
    private readonly IEntityCodeGenerator _entityCodeGenerator;
    private readonly IEntityConfigurationCodeGenerator _configurationCodeGenerator;

    public OracleCodeGenerationService()
        : this(new EntityCodeGenerator(), new EntityConfigurationCodeGenerator())
    {
    }

    public OracleCodeGenerationService(
        IEntityCodeGenerator entityCodeGenerator,
        IEntityConfigurationCodeGenerator configurationCodeGenerator)
    {
        _entityCodeGenerator = entityCodeGenerator;
        _configurationCodeGenerator = configurationCodeGenerator;
    }

    public IReadOnlyList<GeneratedCodeFile> GenerateFiles(
        IEnumerable<OracleTableMetadata> selectedTables,
        GenerationOptions options)
    {
        if (selectedTables is null)
        {
            throw new ArgumentNullException(nameof(selectedTables));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var files = new List<GeneratedCodeFile>();

        foreach (var table in selectedTables)
        {
            files.Add(_entityCodeGenerator.GenerateEntity(table, options));
            files.Add(_configurationCodeGenerator.GenerateConfiguration(table, options));
        }

        return files;
    }
}
