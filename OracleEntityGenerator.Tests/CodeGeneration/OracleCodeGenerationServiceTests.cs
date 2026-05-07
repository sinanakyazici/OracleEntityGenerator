using OracleEntityGenerator.CodeGeneration;
using OracleEntityGenerator.Core.Generation;

namespace OracleEntityGenerator.Tests.CodeGeneration;

public sealed class OracleCodeGenerationServiceTests
{
    [Fact]
    public void GenerateFiles_ReturnsEntityAndConfigurationForEachSelectedTable()
    {
        var service = new OracleCodeGenerationService();
        var options = new GenerationOptions
        {
            EntityNamespace = "MyProject.Domain.Entities",
            ConfigurationNamespace = "MyProject.Infrastructure.Persistence.Configurations"
        };

        var files = service.GenerateFiles(
            [TestTableFactory.CreateOrderLinesTable()],
            options);

        Assert.Equal(2, files.Count);
        Assert.Contains(files, x => x.FileName == "OrderLines.cs");
        Assert.Contains(files, x => x.FileName == "OrderLinesConfiguration.cs");
    }
}
