using OracleEntityGenerator.Core.Generation;
using OracleEntityGenerator.Core.Metadata;
using OracleEntityGenerator.Core.Naming;
using OracleEntityGenerator.Core.TypeMapping;

namespace OracleEntityGenerator.CodeGeneration;

public sealed class EntityConfigurationCodeGenerator : IEntityConfigurationCodeGenerator
{
    private readonly INamingService _namingService;
    private readonly IOracleTypeMapper _typeMapper;

    public EntityConfigurationCodeGenerator()
        : this(new CSharpNamingService(), new OracleTypeMapper())
    {
    }

    public EntityConfigurationCodeGenerator(
        INamingService namingService,
        IOracleTypeMapper typeMapper)
    {
        _namingService = namingService;
        _typeMapper = typeMapper;
    }

    public GeneratedCodeFile GenerateConfiguration(
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

        if (table.PrimaryKey is null && !options.GenerateKeylessEntities)
        {
            throw new InvalidOperationException(
                $"Table '{table.SchemaName}.{table.Name}' has no primary key. Enable keyless entity generation to generate it.");
        }

        var className = _namingService.GetClassName(table.Name, options.Naming);
        var configurationClassName = $"{className.TrimStart('@')}Configuration";
        var properties = GetProperties(table, options).ToArray();
        EnsureUniquePropertyNames(table, properties);
        var writer = new CodeWriter();

        WriteFileHeader(writer, options.Template.FileHeader);

        var usings = GetConfigurationUsings(options);
        foreach (var @using in usings)
        {
            writer.WriteLine($"using {@using};");
        }

        if (!options.ConfigurationNamespace.Equals(options.EntityNamespace, StringComparison.Ordinal))
        {
            writer.WriteLine($"using {options.EntityNamespace};");
        }

        writer.WriteLine();
        writer.WriteLine($"namespace {options.ConfigurationNamespace};");
        writer.WriteLine();
        writer.WriteLine($"public sealed class {configurationClassName} : IEntityTypeConfiguration<{className}>");
        writer.WriteLine("{");
        writer.Indent();
        writer.WriteLine($"public void Configure(EntityTypeBuilder<{className}> builder)");
        writer.WriteLine("{");
        writer.Indent();

        writer.WriteLine($"builder.ToTable(\"{EscapeStringLiteral(table.Name)}\", \"{EscapeStringLiteral(table.SchemaName)}\");");
        writer.WriteLine();

        WriteKeyConfiguration(writer, table, options);

        for (var i = 0; i < properties.Length; i++)
        {
            WritePropertyConfiguration(writer, properties[i]);

            if (i < properties.Length - 1)
            {
                writer.WriteLine();
            }
        }

        writer.Unindent();
        writer.WriteLine("}");
        writer.Unindent();
        writer.WriteLine("}");

        return new GeneratedCodeFile(
            $"{configurationClassName}.cs",
            options.ConfigurationOutputDirectory ?? string.Empty,
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

    private void WriteKeyConfiguration(
        CodeWriter writer,
        OracleTableMetadata table,
        GenerationOptions options)
    {
        if (table.PrimaryKey is null)
        {
            writer.WriteLine("builder.HasNoKey();");
            writer.WriteLine();
            return;
        }

        var keyProperties = table.PrimaryKey.ColumnNames
            .Select(x => _namingService.GetPropertyName(x, options.Naming))
            .ToArray();

        var expression = keyProperties.Length == 1
            ? $"x.{keyProperties[0]}"
            : $"new {{ {string.Join(", ", keyProperties.Select(x => $"x.{x}"))} }}";

        writer.WriteLine($"builder.HasKey(x => {expression});");
        writer.WriteLine();
    }

    private static void WritePropertyConfiguration(
        CodeWriter writer,
        GeneratedProperty property)
    {
        writer.WriteLine($"builder.Property(x => x.{property.PropertyName})");
        writer.Indent();

        var calls = new List<string>
        {
            $".HasColumnName(\"{EscapeStringLiteral(property.Column.Name)}\")"
        };

        if (ShouldWriteMaxLength(property))
        {
            calls.Add($".HasMaxLength({property.Column.Length!.Value})");
        }

        if (ShouldWritePrecision(property))
        {
            calls.Add($".HasPrecision({property.Column.Precision!.Value}, {property.Column.Scale!.Value})");
        }

        if (!property.Column.IsNullable)
        {
            calls.Add(".IsRequired()");
        }

        for (var i = 0; i < calls.Count; i++)
        {
            var suffix = i == calls.Count - 1 ? ";" : string.Empty;
            writer.WriteLine($"{calls[i]}{suffix}");
        }

        writer.Unindent();
    }

    private static bool ShouldWriteMaxLength(GeneratedProperty property)
    {
        var dataType = property.Column.DataType.Trim().ToUpperInvariant();

        return property.IsString
            && property.Column.Length is > 0
            && dataType is not "CLOB"
            && dataType is not "NCLOB"
            && dataType is not "XMLTYPE";
    }

    private static bool ShouldWritePrecision(GeneratedProperty property)
    {
        return IsNumber(property.Column)
            && property.Column.Precision is > 0
            && property.Column.Scale is >= 0;
    }

    private static bool IsNumber(OracleColumnMetadata column)
    {
        return column.DataType.Trim().Equals("NUMBER", StringComparison.OrdinalIgnoreCase);
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

    private static string EscapeStringLiteral(string value)
    {
        return value.Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }

    private static IReadOnlyList<string> GetConfigurationUsings(GenerationOptions options)
    {
        var usings = new SortedSet<string>(StringComparer.Ordinal)
        {
            "Microsoft.EntityFrameworkCore",
            "Microsoft.EntityFrameworkCore.Metadata.Builders"
        };

        foreach (var @using in options.Template.ConfigurationUsings.Where(x => !string.IsNullOrWhiteSpace(x)))
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
}
