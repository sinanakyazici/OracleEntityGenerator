using OracleEntityGenerator.Core.Naming;

namespace OracleEntityGenerator.Tests.Naming;

public sealed class CSharpNamingServiceTests
{
    [Theory]
    [InlineData("CUSTOMERS", "Customers")]
    [InlineData("CUSTOMER_ORDER", "CustomerOrder")]
    [InlineData("ORDER_LINES", "OrderLines")]
    [InlineData("IS_ACTIVE", "IsActive")]
    [InlineData("CREATED_AT", "CreatedAt")]
    public void GetClassName_ConvertsOracleNameToPascalCaseWithoutSingularizing(
        string oracleName,
        string expectedName)
    {
        var service = new CSharpNamingService();

        var actualName = service.GetClassName(oracleName, new NamingOptions());

        Assert.Equal(expectedName, actualName);
    }

    [Fact]
    public void GetPropertyName_RemovesPrefixOnlyWhenConfigured()
    {
        var service = new CSharpNamingService();
        var options = new NamingOptions
        {
            ColumnPrefixesToRemove = ["COL_"]
        };

        var actualName = service.GetPropertyName("COL_CUSTOMER_ID", options);

        Assert.Equal("CustomerId", actualName);
    }

    [Fact]
    public void GetPropertyName_EscapesReservedKeyword()
    {
        var service = new CSharpNamingService();
        var options = new NamingOptions
        {
            PreserveOriginalNames = true
        };

        var actualName = service.GetPropertyName("class", options);

        Assert.Equal("@class", actualName);
    }

    [Fact]
    public void GetClassName_PrefixRemovalDoesNotApplyPluralization()
    {
        var service = new CSharpNamingService();
        var options = new NamingOptions
        {
            TablePrefixesToRemove = ["TB_"]
        };

        var actualName = service.GetClassName("TB_CUSTOMERS", options);

        Assert.Equal("Customers", actualName);
        Assert.NotEqual("Customer", actualName);
    }

    [Theory]
    [InlineData("123_TABLE", "_123Table")]
    [InlineData("CUSTOMER-ID", "CustomerId")]
    [InlineData("CUSTOMER ID", "CustomerId")]
    [InlineData("$", "_")]
    public void GetClassName_SanitizesInvalidCSharpIdentifierCharacters(
        string oracleName,
        string expectedName)
    {
        var service = new CSharpNamingService();

        var actualName = service.GetClassName(oracleName, new NamingOptions());

        Assert.Equal(expectedName, actualName);
    }

    [Fact]
    public void GetPropertyName_IgnoresEmptyConfiguredPrefixes()
    {
        var service = new CSharpNamingService();
        var options = new NamingOptions
        {
            ColumnPrefixesToRemove = ["", "   ", "COL_"]
        };

        var actualName = service.GetPropertyName("COL_CREATED_AT", options);

        Assert.Equal("CreatedAt", actualName);
    }

    [Fact]
    public void GetPropertyName_PrefixRemovalCanProduceFallbackIdentifier()
    {
        var service = new CSharpNamingService();
        var options = new NamingOptions
        {
            ColumnPrefixesToRemove = ["COL_"]
        };

        var actualName = service.GetPropertyName("COL_", options);

        Assert.Equal("_", actualName);
    }
}
