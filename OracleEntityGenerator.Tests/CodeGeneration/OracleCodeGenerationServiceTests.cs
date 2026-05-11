using OracleEntityGenerator.CodeGeneration;
using OracleEntityGenerator.Core.Generation;

namespace OracleEntityGenerator.Tests.CodeGeneration;

public sealed class OracleCodeGenerationServiceTests
{
    [Fact]
    public void GenerateFiles_RejectsNullSelectedTables()
    {
        var service = new OracleCodeGenerationService();

        Assert.Throws<ArgumentNullException>(
            () => service.GenerateFiles(null!, CreateOptions()));
    }

    [Fact]
    public void GenerateFiles_RejectsNullOptions()
    {
        var service = new OracleCodeGenerationService();

        Assert.Throws<ArgumentNullException>(
            () => service.GenerateFiles([], null!));
    }

    [Fact]
    public void GenerateFiles_ReturnsEmptyListWhenNoTablesAreSelected()
    {
        var service = new OracleCodeGenerationService();

        var files = service.GenerateFiles([], CreateOptions());

        Assert.Empty(files);
    }

    [Fact]
    public void GenerateFiles_ReturnsEntityAndConfigurationForEachSelectedTable()
    {
        var service = new OracleCodeGenerationService();

        var files = service.GenerateFiles(
            [TestTableFactory.CreateOrderLinesTable()],
            CreateOptions());

        Assert.Equal(2, files.Count);
        Assert.Contains(files, x => x.FileName == "OrderLines.cs");
        Assert.Contains(files, x => x.FileName == "OrderLinesConfiguration.cs");
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
