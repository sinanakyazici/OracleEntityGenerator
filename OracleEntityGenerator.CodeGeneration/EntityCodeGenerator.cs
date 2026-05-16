using System.Security;
using OracleEntityGenerator.Core.Generation;
using OracleEntityGenerator.Core.Metadata;
using OracleEntityGenerator.Core.Naming;
using OracleEntityGenerator.Core.TypeMapping;

namespace OracleEntityGenerator.CodeGeneration;

public sealed class EntityCodeGenerator : IEntityCodeGenerator
{
    private readonly INamingService _namingService;
    private readonly IOracleTypeMapper _typeMapper;

    public EntityCodeGenerator()
        : this(new CSharpNamingService(), new OracleTypeMapper())
    {
    }

    public EntityCodeGenerator(
        INamingService namingService,
        IOracleTypeMapper typeMapper)
    {
        _namingService = namingService;
        _typeMapper = typeMapper;
    }

    public GeneratedCodeFile GenerateEntity(
        OracleTableMetadata table,
        GenerationOptions options)
    {
        if (table is null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var className = _namingService.GetClassName(table.Name, options.Naming);
        var properties = GetProperties(table, options).ToArray();
        EnsureUniquePropertyNames(table, properties);
        var writer = new CodeWriter();

        WriteFileHeader(writer, options.Template.FileHeader);

        if (!options.NullableReferenceTypes)
        {
            writer.WriteLine("#nullable disable");
            WriteBlankLine(writer, options);
        }

        var usings = GetEntityUsings(properties, options);
        foreach (var @using in usings)
        {
            writer.WriteLine($"using {@using};");
        }

        if (usings.Count > 0)
        {
            WriteBlankLine(writer, options);
        }

        writer.WriteLine($"namespace {options.EntityNamespace};");
        WriteBlankLine(writer, options);
        WriteXmlComment(writer, table.Comment, options.GenerateXmlComments);
        writer.WriteLine($"public class {className}");
        writer.WriteLine("{");
        writer.Indent();

        for (var i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            WriteXmlComment(writer, property.Column.Comment, options.GenerateXmlComments);
            writer.WriteLine(GetPropertyDeclaration(property, options));

            if (!options.CompactOutput && i < properties.Length - 1)
            {
                writer.WriteLine();
            }
        }

        writer.Unindent();
        writer.WriteLine("}");

        if (!options.NullableReferenceTypes)
        {
            WriteBlankLine(writer, options);
            writer.WriteLine("#nullable restore");
        }

        return new GeneratedCodeFile(
            $"{GetSafeFileName(className)}.cs",
            options.EntityOutputDirectory ?? string.Empty,
            writer.ToString());
    }

    private IEnumerable<GeneratedProperty> GetProperties(
        OracleTableMetadata table,
        GenerationOptions options)
    {
        foreach (var column in table.Columns.OrderBy(x => x.Ordinal))
        {
            yield return new GeneratedProperty(
                column,
                _namingService.GetPropertyName(column.Name, options.Naming),
                _typeMapper.MapColumn(column, options.TypeMapping));
        }
    }

    private static string GetPropertyDeclaration(
        GeneratedProperty property,
        GenerationOptions options)
    {
        var typeName = property.GetPropertyTypeName(options.NullableReferenceTypes);
        var defaultValue = GetDefaultValue(property, options);

        return string.IsNullOrEmpty(defaultValue)
            ? $"public {typeName} {property.PropertyName} {{ get; set; }}"
            : $"public {typeName} {property.PropertyName} {{ get; set; }}{defaultValue};";
    }

    private static string GetDefaultValue(
        GeneratedProperty property,
        GenerationOptions options)
    {
        return options.NullableReferenceTypes
            && property.TypeMapping.IsReferenceType
            && !property.TypeMapping.IsNullable
            ? " = null!"
            : string.Empty;
    }

    private static bool RequiresSystemNamespace(GeneratedProperty property)
    {
        return property.TypeMapping.TypeName is "DateTime" or "DateTimeOffset";
    }

    private static IReadOnlyList<string> GetEntityUsings(
        IReadOnlyList<GeneratedProperty> properties,
        GenerationOptions options)
    {
        var usings = new SortedSet<string>(StringComparer.Ordinal);

        if (properties.Any(RequiresSystemNamespace))
        {
            usings.Add("System");
        }

        foreach (var @using in options.Template.EntityUsings.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            usings.Add(@using.Trim().TrimEnd(';'));
        }

        return usings.ToArray();
    }

    private static void WriteFileHeader(CodeWriter writer, string? fileHeader)
    {
        var header = fileHeader;
        if (header is null || header.Trim().Length == 0)
        {
            return;
        }

        foreach (var line in header.Replace("\r\n", "\n").Split('\n'))
        {
            writer.WriteLine(line);
        }

        writer.WriteLine();
    }

    private static void WriteBlankLine(
        CodeWriter writer,
        GenerationOptions options)
    {
        if (!options.CompactOutput)
        {
            writer.WriteLine();
        }
    }

    private static void EnsureUniquePropertyNames(
        OracleTableMetadata table,
        IReadOnlyList<GeneratedProperty> properties)
    {
        var duplicate = properties
            .GroupBy(x => x.PropertyName, StringComparer.Ordinal)
            .FirstOrDefault(x => x.Count() > 1);

        if (duplicate is null)
        {
            return;
        }

        var columnNames = string.Join(", ", duplicate.Select(x => x.Column.Name));
        throw new InvalidOperationException(
            $"Table '{table.SchemaName}.{table.Name}' has multiple columns that map to C# property '{duplicate.Key}': {columnNames}.");
    }

    private static void WriteXmlComment(
        CodeWriter writer,
        string? comment,
        bool enabled)
    {
        if (!enabled || comment is null)
        {
            return;
        }

        var trimmedComment = comment.Trim();
        if (trimmedComment.Length == 0)
        {
            return;
        }

        writer.WriteLine("/// <summary>");
        writer.WriteLine($"/// {SecurityElement.Escape(trimmedComment) ?? string.Empty}");
        writer.WriteLine("/// </summary>");
    }

    private static string GetSafeFileName(string className)
    {
        return className.TrimStart('@');
    }
}
