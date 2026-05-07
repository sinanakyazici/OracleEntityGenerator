using OracleEntityGenerator.Core.Metadata;

namespace OracleEntityGenerator.Core.TypeMapping;

public sealed class OracleTypeMapper : IOracleTypeMapper
{
    public CSharpTypeMapping MapColumn(
        OracleColumnMetadata column,
        OracleTypeMappingOptions options)
    {
        if (column is null)
        {
            throw new ArgumentNullException(nameof(column));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var normalizedDataType = NormalizeDataType(column.DataType);
        var typeName = normalizedDataType switch
        {
            "VARCHAR2" or "NVARCHAR2" or "CHAR" or "NCHAR" or "CLOB" or "NCLOB" or "XMLTYPE" => "string",
            "BLOB" or "RAW" or "LONG RAW" => "byte[]",
            "FLOAT" or "BINARY_FLOAT" => "float",
            "BINARY_DOUBLE" => "double",
            "DATE" => "DateTime",
            "TIMESTAMP" or "TIMESTAMP WITH LOCAL TIME ZONE" => "DateTime",
            "TIMESTAMP WITH TIME ZONE" => "DateTimeOffset",
            "NUMBER" => MapNumber(column, options),
            _ => null
        };

        if (typeName is null)
        {
            return new CSharpTypeMapping
            {
                TypeName = "object",
                IsUnsupported = true,
                Warning = $"Unsupported Oracle data type '{column.DataType}'."
            };
        }

        var isReferenceType = typeName is "string" or "byte[]";

        return new CSharpTypeMapping
        {
            TypeName = typeName,
            IsReferenceType = isReferenceType,
            IsNullable = column.IsNullable,
        };
    }

    private static string MapNumber(
        OracleColumnMetadata column,
        OracleTypeMappingOptions options)
    {
        if (column.Scale > 0)
        {
            return "decimal";
        }

        return column.Precision switch
        {
            1 => options.NumberOneMapping == OracleNumberOneMapping.Bool ? "bool" : "byte",
            <= 3 => "byte",
            <= 5 => "short",
            <= 10 => "int",
            <= 19 => "long",
            _ => "decimal"
        };
    }

    private static string NormalizeDataType(string dataType)
    {
        var normalized = dataType.Trim().ToUpperInvariant();

        if (normalized.StartsWith("TIMESTAMP", StringComparison.Ordinal))
        {
            if (normalized.IndexOf("WITH LOCAL TIME ZONE", StringComparison.Ordinal) >= 0)
            {
                return "TIMESTAMP WITH LOCAL TIME ZONE";
            }

            if (normalized.IndexOf("WITH TIME ZONE", StringComparison.Ordinal) >= 0)
            {
                return "TIMESTAMP WITH TIME ZONE";
            }

            return "TIMESTAMP";
        }

        return normalized;
    }
}
