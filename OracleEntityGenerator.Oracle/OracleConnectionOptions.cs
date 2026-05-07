using Oracle.ManagedDataAccess.Client;

namespace OracleEntityGenerator.Oracle;

public sealed record OracleConnectionOptions
{
    public string? FullConnectionString { get; init; }

    public string? Host { get; init; }

    public int Port { get; init; } = 1521;

    public string? ServiceName { get; init; }

    public string? UserName { get; init; }

    public string? Password { get; init; }

    public string ToConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(FullConnectionString))
        {
            return FullConnectionString!;
        }

        if (string.IsNullOrWhiteSpace(Host))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(Host));
        }

        if (string.IsNullOrWhiteSpace(ServiceName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(ServiceName));
        }

        if (string.IsNullOrWhiteSpace(UserName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(UserName));
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(Password));
        }

        var builder = new OracleConnectionStringBuilder
        {
            DataSource = $"{Host}:{Port}/{ServiceName}",
            UserID = UserName,
            Password = Password,
            PersistSecurityInfo = false
        };

        return builder.ConnectionString;
    }
}
