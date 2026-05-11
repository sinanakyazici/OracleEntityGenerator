using OracleEntityGenerator.Oracle;

namespace OracleEntityGenerator.Tests.Oracle;

public sealed class OracleConnectionOptionsTests
{
    [Fact]
    public void ToConnectionString_ReturnsFullConnectionStringWhenProvided()
    {
        var options = new OracleConnectionOptions
        {
            FullConnectionString = "Data Source=localhost:1521/FREEPDB1;User Id=APP;Password=secret;"
        };

        var connectionString = options.ToConnectionString();

        Assert.Equal(options.FullConnectionString, connectionString);
    }

    [Fact]
    public void ToConnectionString_BuildsConnectionStringFromHostPortServiceAndCredentials()
    {
        var options = new OracleConnectionOptions
        {
            Host = "localhost",
            Port = 1521,
            ServiceName = "FREEPDB1",
            UserName = "APP",
            Password = "secret"
        };

        var connectionString = options.ToConnectionString();

        Assert.Contains("DATA SOURCE=localhost:1521/FREEPDB1", connectionString, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("USER ID=APP", connectionString, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("PASSWORD=secret", connectionString, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("PERSIST SECURITY INFO=False", connectionString, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null, "FREEPDB1", "APP", "secret", "Host")]
    [InlineData("localhost", null, "APP", "secret", "ServiceName")]
    [InlineData("localhost", "FREEPDB1", null, "secret", "UserName")]
    [InlineData("localhost", "FREEPDB1", "APP", null, "Password")]
    public void ToConnectionString_RejectsMissingConnectionParts(
        string? host,
        string? serviceName,
        string? userName,
        string? password,
        string expectedParameterName)
    {
        var options = new OracleConnectionOptions
        {
            Host = host,
            ServiceName = serviceName,
            UserName = userName,
            Password = password
        };

        var exception = Assert.Throws<ArgumentException>(options.ToConnectionString);

        Assert.Equal(expectedParameterName, exception.ParamName);
    }
}
