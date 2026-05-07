using OracleEntityGenerator.Core.Metadata;

namespace OracleEntityGenerator.Oracle;

public interface IOracleMetadataReader
{
    Task TestConnectionAsync(
        OracleConnectionOptions connectionOptions,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OracleSchemaSummary>> GetAccessibleSchemasAsync(
        OracleConnectionOptions connectionOptions,
        OracleMetadataReaderOptions options,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OracleTableSummary>> GetTablesAsync(
        OracleConnectionOptions connectionOptions,
        string schemaName,
        OracleMetadataReaderOptions options,
        CancellationToken cancellationToken);

    Task<OracleTableMetadata> GetTableMetadataAsync(
        OracleConnectionOptions connectionOptions,
        string schemaName,
        string tableName,
        CancellationToken cancellationToken);
}
