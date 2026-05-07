using OracleEntityGenerator.Core.Metadata;

namespace OracleEntityGenerator.Core.TypeMapping;

public interface IOracleTypeMapper
{
    CSharpTypeMapping MapColumn(
        OracleColumnMetadata column,
        OracleTypeMappingOptions options);
}
