using OracleEntityGenerator.CodeGeneration;
using OracleEntityGenerator.Core.Generation;
using OracleEntityGenerator.Core.Metadata;

namespace OracleEntityGenerator.Tests.CodeGeneration;

public sealed class EntityCodeGeneratorTests
{
    [Fact]
    public void GenerateEntity_UsesOracleTableNameWithoutSingularizing()
    {
        var table = TestTableFactory.CreateOrderLinesTable();
        var generator = new EntityCodeGenerator();

        var file = generator.GenerateEntity(table, CreateOptions());

        Assert.Equal("OrderLines.cs", file.FileName);
        Assert.Contains("public class OrderLines", file.SourceText);
        Assert.DoesNotContain("public class OrderLine\r\n", file.SourceText);
    }

    [Fact]
    public void GenerateEntity_WritesNullableReferenceTypesAndRequiredInitializers()
    {
        var table = TestTableFactory.CreateOrderLinesTable();
        var generator = new EntityCodeGenerator();

        var file = generator.GenerateEntity(table, CreateOptions());

        Assert.Contains("public string OrderNo { get; set; } = null!;", file.SourceText);
        Assert.Contains("public string? Note { get; set; }", file.SourceText);
        Assert.Contains("public DateTime CreatedAt { get; set; }", file.SourceText);
    }

    [Fact]
    public void GenerateEntity_CanWriteXmlCommentsFromOracleComments()
    {
        var table = TestTableFactory.CreateOrderLinesTable();
        var generator = new EntityCodeGenerator();
        var options = CreateOptions() with { GenerateXmlComments = true };

        var file = generator.GenerateEntity(table, options);

        Assert.Contains("/// Order lines table", file.SourceText);
        Assert.Contains("/// Line note", file.SourceText);
    }

    [Fact]
    public void GenerateEntity_RejectsDuplicatePropertyNames()
    {
        var table = SmokeTests.SmokeTestData.CreateDuplicatePropertyNameTable();
        var generator = new EntityCodeGenerator();

        var exception = Assert.Throws<InvalidOperationException>(
            () => generator.GenerateEntity(table, CreateOptions()));

        Assert.Contains("multiple columns that map to C# property 'CustomerId'", exception.Message);
        Assert.Contains("CUSTOMER_ID, CUSTOMER-ID", exception.Message);
    }

    [Fact]
    public void GenerateEntity_WritesTemplateHeaderAndCustomUsings()
    {
        var table = TestTableFactory.CreateOrderLinesTable();
        var generator = new EntityCodeGenerator();
        var options = CreateOptions() with
        {
            Template = new TemplateOptions
            {
                FileHeader = "// custom header",
                EntityUsings = ["System.ComponentModel.DataAnnotations"]
            }
        };

        var file = generator.GenerateEntity(table, options);

        Assert.StartsWith("// custom header", file.SourceText);
        Assert.Contains("using System;", file.SourceText);
        Assert.Contains("using System.ComponentModel.DataAnnotations;", file.SourceText);
    }

    private static GenerationOptions CreateOptions()
    {
        return new GenerationOptions
        {
            EntityNamespace = "MyProject.Domain.Entities",
            ConfigurationNamespace = "MyProject.Infrastructure.Persistence.Configurations"
        };
    }
}
