using OracleEntityGenerator.Core.Metadata;

namespace OracleEntityGenerator.Core.Generation;

public interface IEntityConfigurationCodeGenerator
{
    GeneratedCodeFile GenerateConfiguration(
        OracleTableMetadata table,
        GenerationOptions options);
}
