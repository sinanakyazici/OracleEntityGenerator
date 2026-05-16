using OracleEntityGenerator.CodeGeneration;
using OracleEntityGenerator.Core.Generation;

namespace OracleEntityGenerator.Tests.CodeGeneration;

public sealed class EntityConfigurationCodeGeneratorTests
{
    [Fact]
    public void GenerateConfiguration_UsesOracleNamesInMapping()
    {
        var table = TestTableFactory.CreateOrderLinesTable();
        var generator = new EntityConfigurationCodeGenerator();

        var file = generator.GenerateConfiguration(table, CreateOptions());

        Assert.Equal("OrderLinesConfiguration.cs", file.FileName);
        Assert.Contains("public sealed class OrderLinesConfiguration : IEntityTypeConfiguration<OrderLines>", file.SourceText);
        Assert.Contains("builder.ToTable(\"ORDER_LINES\", \"CRM\");", file.SourceText);
        Assert.Contains("builder.HasKey(x => new { x.OrderId, x.LineNo });", file.SourceText);
        Assert.Contains(".HasColumnName(\"ORDER_NO\")", file.SourceText);
        Assert.Contains(".HasMaxLength(30)", file.SourceText);
        Assert.Contains(".HasPrecision(10, 0)", file.SourceText);
        Assert.Contains(".HasPrecision(12, 2)", file.SourceText);
    }

    [Fact]
    public void GenerateConfiguration_RejectsKeylessTableUnlessEnabled()
    {
        var table = TestTableFactory.CreateKeylessCustomersTable();
        var generator = new EntityConfigurationCodeGenerator();

        var exception = Assert.Throws<InvalidOperationException>(
            () => generator.GenerateConfiguration(
                TestTableFactory.CreateKeylessCustomersTable(),
                CreateOptions()));

        Assert.Contains("has no primary key", exception.Message);
        Assert.Null(table.PrimaryKey);
    }

    [Fact]
    public void GenerateConfiguration_CanGenerateKeylessEntityWhenEnabled()
    {
        var table = TestTableFactory.CreateKeylessCustomersTable();
        var generator = new EntityConfigurationCodeGenerator();
        var options = CreateOptions() with { GenerateKeylessEntities = true };

        var file = generator.GenerateConfiguration(table, options);

        Assert.Contains("public sealed class CustomersConfiguration", file.SourceText);
        Assert.Contains("builder.HasNoKey();", file.SourceText);
        Assert.DoesNotContain("CustomerConfiguration", file.SourceText);
    }

    [Fact]
    public void GenerateConfiguration_DoesNotWriteMaxLengthForClob()
    {
        var table = SmokeTests.SmokeTestData.CreateIdentifierEdgeCaseTable();
        var generator = new EntityConfigurationCodeGenerator();

        var file = generator.GenerateConfiguration(table, CreateOptions());

        Assert.Contains(".HasColumnName(\"DESCRIPTION_TEXT\");", file.SourceText);
        Assert.DoesNotContain(".HasMaxLength(4000)", file.SourceText);
    }

    [Fact]
    public void GenerateConfiguration_WritesPrecisionOnlyWhenNumberScaleAndPrecisionAreAvailable()
    {
        var table = TestTableFactory.CreateNumberMetadataEdgeCaseTable();
        var generator = new EntityConfigurationCodeGenerator();

        var file = generator.GenerateConfiguration(table, CreateOptions());
        var source = NormalizeLineEndings(file.SourceText);

        Assert.Contains(".HasPrecision(10, 0)", source);
        Assert.Contains(".HasPrecision(12, 2)", source);
        Assert.Contains(".HasColumnName(\"UNKNOWN_SCALE_VALUE\")\n            .IsRequired();", source);
        Assert.Contains(".HasColumnName(\"NO_PRECISION_VALUE\")\n            .IsRequired();", source);
    }

    [Fact]
    public void GenerateConfiguration_CanWriteOracleColumnTypeMappings()
    {
        var table = TestTableFactory.CreateOrderLinesTable();
        var generator = new EntityConfigurationCodeGenerator();
        var options = CreateOptions() with { GenerateColumnTypeMappings = true };

        var file = generator.GenerateConfiguration(table, options);

        Assert.Contains(".HasColumnType(\"NUMBER(10,0)\")", file.SourceText);
        Assert.Contains(".HasColumnType(\"VARCHAR2(30)\")", file.SourceText);
        Assert.Contains(".HasColumnType(\"DATE\")", file.SourceText);
    }

    [Fact]
    public void GenerateConfiguration_OmitsOracleColumnTypeMappingsByDefault()
    {
        var table = TestTableFactory.CreateOrderLinesTable();
        var generator = new EntityConfigurationCodeGenerator();

        var file = generator.GenerateConfiguration(table, CreateOptions());

        Assert.DoesNotContain("HasColumnType", file.SourceText);
    }

    [Fact]
    public void GenerateConfiguration_CanWriteCompactInlinePropertyMappings()
    {
        var table = TestTableFactory.CreateOrderLinesTable();
        var generator = new EntityConfigurationCodeGenerator();
        var options = CreateOptions() with
        {
            CompactOutput = true,
            GenerateColumnTypeMappings = true
        };

        var file = generator.GenerateConfiguration(table, options);
        var source = NormalizeLineEndings(file.SourceText);

        Assert.Contains(
            "builder.Property(x => x.OrderNo).HasColumnName(\"ORDER_NO\").HasColumnType(\"VARCHAR2(30)\").HasMaxLength(30).IsRequired();",
            source);
        Assert.DoesNotContain("builder.Property(x => x.OrderNo)\n", source);
        Assert.DoesNotContain("\n\n", source);
    }

    [Fact]
    public void GenerateConfiguration_EscapesSchemaAndTableStringLiterals()
    {
        var table = SmokeTests.SmokeTestData.CreateIdentifierEdgeCaseTable();
        var generator = new EntityConfigurationCodeGenerator();

        var file = generator.GenerateConfiguration(table, CreateOptions());

        Assert.Contains("builder.ToTable(\"123_ORDER DETAILS\", \"CRM\\\"EDGE\");", file.SourceText);
    }

    [Fact]
    public void GenerateConfiguration_RejectsDuplicatePropertyNames()
    {
        var table = SmokeTests.SmokeTestData.CreateDuplicatePropertyNameTable();
        var generator = new EntityConfigurationCodeGenerator();

        var exception = Assert.Throws<InvalidOperationException>(
            () => generator.GenerateConfiguration(table, CreateOptions()));

        Assert.Contains("multiple columns that map to C# property 'CustomerId'", exception.Message);
    }

    [Fact]
    public void GenerateConfiguration_WritesTemplateHeaderAndCustomUsings()
    {
        var table = TestTableFactory.CreateOrderLinesTable();
        var generator = new EntityConfigurationCodeGenerator();
        var options = CreateOptions() with
        {
            Template = new TemplateOptions
            {
                FileHeader = "// config header",
                ConfigurationUsings = ["System.Diagnostics.CodeAnalysis"]
            }
        };

        var file = generator.GenerateConfiguration(table, options);

        Assert.StartsWith("// config header", file.SourceText);
        Assert.Contains("using Microsoft.EntityFrameworkCore;", file.SourceText);
        Assert.Contains("using System.Diagnostics.CodeAnalysis;", file.SourceText);
    }

    private static GenerationOptions CreateOptions()
    {
        return new GenerationOptions
        {
            EntityNamespace = "MyProject.Domain.Entities",
            ConfigurationNamespace = "MyProject.Infrastructure.Persistence.Configurations"
        };
    }

    private static string NormalizeLineEndings(string value)
    {
        return value.Replace("\r\n", "\n", StringComparison.Ordinal);
    }
}
