using OracleEntityGenerator.Core.Metadata;
using OracleEntityGenerator.Core.TypeMapping;

namespace OracleEntityGenerator.CodeGeneration;

internal sealed record GeneratedProperty(
    OracleColumnMetadata Column,
    string PropertyName,
    CSharpTypeMapping TypeMapping)
{
    public bool IsString => TypeMapping.TypeName == "string";

    public bool IsDecimal => TypeMapping.TypeName == "decimal";

    public string GetPropertyTypeName(bool nullableReferenceTypes)
    {
        if (!TypeMapping.IsNullable)
        {
            return TypeMapping.TypeName;
        }

        if (TypeMapping.IsReferenceType)
        {
            return nullableReferenceTypes ? $"{TypeMapping.TypeName}?" : TypeMapping.TypeName;
        }

        return $"{TypeMapping.TypeName}?";
    }
}
