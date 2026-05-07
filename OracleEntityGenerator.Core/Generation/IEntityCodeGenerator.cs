using OracleEntityGenerator.Core.Metadata;

namespace OracleEntityGenerator.Core.Generation;

public interface IEntityCodeGenerator
{
    GeneratedCodeFile GenerateEntity(
        OracleTableMetadata table,
        GenerationOptions options);
}
