using OracleEntityGenerator.Oracle;

namespace OracleEntityGenerator.Tests.Oracle;

public sealed class OracleMetadataReaderTests
{
    [Fact]
    public async Task TestConnectionAsync_RejectsNullConnectionOptions()
    {
        var reader = new OracleMetadataReader();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => reader.TestConnectionAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task GetAccessibleSchemasAsync_RejectsNullConnectionOptions()
    {
        var reader = new OracleMetadataReader();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => reader.GetAccessibleSchemasAsync(null!, new OracleMetadataReaderOptions(), CancellationToken.None));
    }

    [Fact]
    public async Task GetAccessibleSchemasAsync_RejectsNullOptions()
    {
        var reader = new OracleMetadataReader();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => reader.GetAccessibleSchemasAsync(CreateConnectionOptions(), null!, CancellationToken.None));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetTablesAsync_RejectsBlankSchemaName(string schemaName)
    {
        var reader = new OracleMetadataReader();

        await Assert.ThrowsAsync<ArgumentException>(
            () => reader.GetTablesAsync(CreateConnectionOptions(), schemaName, new OracleMetadataReaderOptions(), CancellationToken.None));
    }

    [Fact]
    public async Task GetTablesAsync_ReturnsEmptyListForExcludedSystemSchemaWithoutOpeningConnection()
    {
        var reader = new OracleMetadataReader();

        var tables = await reader.GetTablesAsync(
            CreateConnectionOptions(),
            "SYS",
            new OracleMetadataReaderOptions(),
            CancellationToken.None);

        Assert.Empty(tables);
    }

    [Theory]
    [InlineData("", "CUSTOMERS")]
    [InlineData("   ", "CUSTOMERS")]
    [InlineData("CRM", "")]
    [InlineData("CRM", "   ")]
    public async Task GetTableMetadataAsync_RejectsBlankSchemaOrTableName(
        string schemaName,
        string tableName)
    {
        var reader = new OracleMetadataReader();

        await Assert.ThrowsAsync<ArgumentException>(
            () => reader.GetTableMetadataAsync(CreateConnectionOptions(), schemaName, tableName, CancellationToken.None));
    }

    private static OracleConnectionOptions CreateConnectionOptions()
    {
        return new OracleConnectionOptions
        {
            Host = "localhost",
            ServiceName = "FREEPDB1",
            UserName = "APP",
            Password = "secret"
        };
    }
}
