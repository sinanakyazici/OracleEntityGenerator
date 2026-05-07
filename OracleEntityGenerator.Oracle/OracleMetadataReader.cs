using System.Data;
using Oracle.ManagedDataAccess.Client;
using OracleEntityGenerator.Core.Metadata;

namespace OracleEntityGenerator.Oracle;

public sealed class OracleMetadataReader : IOracleMetadataReader
{
    public async Task TestConnectionAsync(
        OracleConnectionOptions connectionOptions,
        CancellationToken cancellationToken)
    {
        ThrowIfNull(connectionOptions, nameof(connectionOptions));

        using var connection = CreateConnection(connectionOptions);
        await connection.OpenAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OracleSchemaSummary>> GetAccessibleSchemasAsync(
        OracleConnectionOptions connectionOptions,
        OracleMetadataReaderOptions options,
        CancellationToken cancellationToken)
    {
        ThrowIfNull(connectionOptions, nameof(connectionOptions));
        ThrowIfNull(options, nameof(options));

        const string sql = """
            SELECT
                t.OWNER,
                COUNT(*) AS TABLE_COUNT
            FROM ALL_TABLES t
            WHERE t.NESTED = 'NO'
              AND t.TABLE_NAME NOT LIKE 'BIN$%'
            GROUP BY t.OWNER
            ORDER BY t.OWNER
            """;

        using var connection = CreateConnection(connectionOptions);
        await connection.OpenAsync(cancellationToken);

        using var command = CreateCommand(connection, sql);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var schemas = new List<OracleSchemaSummary>();

        while (await reader.ReadAsync(cancellationToken))
        {
            var schemaName = reader.GetString(0);

            if (IsExcludedSchema(schemaName, options))
            {
                continue;
            }

            schemas.Add(new OracleSchemaSummary(
                schemaName,
                Convert.ToInt32(reader.GetDecimal(1))));
        }

        return schemas;
    }

    public async Task<IReadOnlyList<OracleTableSummary>> GetTablesAsync(
        OracleConnectionOptions connectionOptions,
        string schemaName,
        OracleMetadataReaderOptions options,
        CancellationToken cancellationToken)
    {
        ThrowIfNull(connectionOptions, nameof(connectionOptions));
        ThrowIfNullOrWhiteSpace(schemaName, nameof(schemaName));
        ThrowIfNull(options, nameof(options));

        if (IsExcludedSchema(schemaName, options))
        {
            return [];
        }

        const string sql = """
            SELECT
                t.OWNER,
                t.TABLE_NAME,
                tc.COMMENTS,
                COUNT(c.COLUMN_NAME) AS COLUMN_COUNT,
                CASE WHEN pk.CONSTRAINT_NAME IS NULL THEN 0 ELSE 1 END AS HAS_PRIMARY_KEY
            FROM ALL_TABLES t
            LEFT JOIN ALL_TAB_COMMENTS tc
                ON tc.OWNER = t.OWNER
               AND tc.TABLE_NAME = t.TABLE_NAME
            LEFT JOIN ALL_TAB_COLUMNS c
                ON c.OWNER = t.OWNER
               AND c.TABLE_NAME = t.TABLE_NAME
            LEFT JOIN ALL_CONSTRAINTS pk
                ON pk.OWNER = t.OWNER
               AND pk.TABLE_NAME = t.TABLE_NAME
               AND pk.CONSTRAINT_TYPE = 'P'
            WHERE t.OWNER = :schemaName
              AND t.NESTED = 'NO'
              AND t.TABLE_NAME NOT LIKE 'BIN$%'
            GROUP BY
                t.OWNER,
                t.TABLE_NAME,
                tc.COMMENTS,
                pk.CONSTRAINT_NAME
            ORDER BY t.TABLE_NAME
            """;

        using var connection = CreateConnection(connectionOptions);
        await connection.OpenAsync(cancellationToken);

        using var command = CreateCommand(connection, sql);
        command.Parameters.Add(CreateVarcharParameter("schemaName", schemaName.ToUpperInvariant()));

        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var tables = new List<OracleTableSummary>();

        while (await reader.ReadAsync(cancellationToken))
        {
            tables.Add(new OracleTableSummary(
                reader.GetString(0),
                reader.GetString(1),
                await GetNullableStringAsync(reader, 2, cancellationToken),
                Convert.ToInt32(reader.GetDecimal(3)),
                reader.GetDecimal(4) == 1));
        }

        return tables;
    }

    public async Task<OracleTableMetadata> GetTableMetadataAsync(
        OracleConnectionOptions connectionOptions,
        string schemaName,
        string tableName,
        CancellationToken cancellationToken)
    {
        ThrowIfNull(connectionOptions, nameof(connectionOptions));
        ThrowIfNullOrWhiteSpace(schemaName, nameof(schemaName));
        ThrowIfNullOrWhiteSpace(tableName, nameof(tableName));

        using var connection = CreateConnection(connectionOptions);
        await connection.OpenAsync(cancellationToken);

        var normalizedSchemaName = schemaName.ToUpperInvariant();
        var normalizedTableName = tableName.ToUpperInvariant();

        var comment = await GetTableCommentAsync(
            connection,
            normalizedSchemaName,
            normalizedTableName,
            cancellationToken);

        var columns = await GetColumnsAsync(
            connection,
            normalizedSchemaName,
            normalizedTableName,
            cancellationToken);

        if (columns.Count == 0)
        {
            throw new InvalidOperationException(
                $"Oracle table '{normalizedSchemaName}.{normalizedTableName}' was not found or has no readable columns.");
        }

        var primaryKey = await GetPrimaryKeyAsync(
            connection,
            normalizedSchemaName,
            normalizedTableName,
            cancellationToken);

        var foreignKeys = await GetForeignKeysAsync(
            connection,
            normalizedSchemaName,
            normalizedTableName,
            cancellationToken);

        var uniqueConstraints = await GetUniqueConstraintsAsync(
            connection,
            normalizedSchemaName,
            normalizedTableName,
            cancellationToken);

        var indexes = await GetIndexesAsync(
            connection,
            normalizedSchemaName,
            normalizedTableName,
            cancellationToken);

        return new OracleTableMetadata
        {
            SchemaName = normalizedSchemaName,
            Name = normalizedTableName,
            Comment = comment,
            Columns = columns,
            PrimaryKey = primaryKey,
            ForeignKeys = foreignKeys,
            UniqueConstraints = uniqueConstraints,
            Indexes = indexes
        };
    }

    private static async Task<string?> GetTableCommentAsync(
        OracleConnection connection,
        string schemaName,
        string tableName,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT COMMENTS
            FROM ALL_TAB_COMMENTS
            WHERE OWNER = :schemaName
              AND TABLE_NAME = :tableName
            """;

        using var command = CreateCommand(connection, sql);
        command.Parameters.Add(CreateVarcharParameter("schemaName", schemaName));
        command.Parameters.Add(CreateVarcharParameter("tableName", tableName));

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is null or DBNull ? null : Convert.ToString(result);
    }

    private static async Task<IReadOnlyList<OracleColumnMetadata>> GetColumnsAsync(
        OracleConnection connection,
        string schemaName,
        string tableName,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                c.COLUMN_NAME,
                c.DATA_TYPE,
                c.DATA_LENGTH,
                c.DATA_PRECISION,
                c.DATA_SCALE,
                c.NULLABLE,
                c.DATA_DEFAULT,
                cc.COMMENTS,
                c.COLUMN_ID
            FROM ALL_TAB_COLUMNS c
            LEFT JOIN ALL_COL_COMMENTS cc
                ON cc.OWNER = c.OWNER
               AND cc.TABLE_NAME = c.TABLE_NAME
               AND cc.COLUMN_NAME = c.COLUMN_NAME
            WHERE c.OWNER = :schemaName
              AND c.TABLE_NAME = :tableName
            ORDER BY c.COLUMN_ID
            """;

        using var command = CreateCommand(connection, sql);
        command.Parameters.Add(CreateVarcharParameter("schemaName", schemaName));
        command.Parameters.Add(CreateVarcharParameter("tableName", tableName));

        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var columns = new List<OracleColumnMetadata>();

        while (await reader.ReadAsync(cancellationToken))
        {
            columns.Add(new OracleColumnMetadata
            {
                Name = reader.GetString(0),
                DataType = reader.GetString(1),
                Length = await GetNullableInt32Async(reader, 2, cancellationToken),
                Precision = await GetNullableInt32Async(reader, 3, cancellationToken),
                Scale = await GetNullableInt32Async(reader, 4, cancellationToken),
                IsNullable = reader.GetString(5).Equals("Y", StringComparison.OrdinalIgnoreCase),
                DefaultValue = await GetNullableStringAsync(reader, 6, cancellationToken),
                Comment = await GetNullableStringAsync(reader, 7, cancellationToken),
                Ordinal = Convert.ToInt32(reader.GetDecimal(8))
            });
        }

        return columns;
    }

    private static async Task<OraclePrimaryKeyMetadata?> GetPrimaryKeyAsync(
        OracleConnection connection,
        string schemaName,
        string tableName,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                ac.CONSTRAINT_NAME,
                acc.COLUMN_NAME
            FROM ALL_CONSTRAINTS ac
            INNER JOIN ALL_CONS_COLUMNS acc
                ON acc.OWNER = ac.OWNER
               AND acc.CONSTRAINT_NAME = ac.CONSTRAINT_NAME
               AND acc.TABLE_NAME = ac.TABLE_NAME
            WHERE ac.OWNER = :schemaName
              AND ac.TABLE_NAME = :tableName
              AND ac.CONSTRAINT_TYPE = 'P'
            ORDER BY acc.POSITION
            """;

        using var command = CreateCommand(connection, sql);
        command.Parameters.Add(CreateVarcharParameter("schemaName", schemaName));
        command.Parameters.Add(CreateVarcharParameter("tableName", tableName));

        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        string? constraintName = null;
        var columnNames = new List<string>();

        while (await reader.ReadAsync(cancellationToken))
        {
            constraintName ??= reader.GetString(0);
            columnNames.Add(reader.GetString(1));
        }

        return constraintName is null
            ? null
            : new OraclePrimaryKeyMetadata
            {
                Name = constraintName,
                ColumnNames = columnNames
            };
    }

    private static async Task<IReadOnlyList<OracleForeignKeyMetadata>> GetForeignKeysAsync(
        OracleConnection connection,
        string schemaName,
        string tableName,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                fk.CONSTRAINT_NAME,
                fk_col.COLUMN_NAME,
                pk.OWNER AS REFERENCED_OWNER,
                pk.TABLE_NAME AS REFERENCED_TABLE_NAME,
                pk_col.COLUMN_NAME AS REFERENCED_COLUMN_NAME
            FROM ALL_CONSTRAINTS fk
            INNER JOIN ALL_CONS_COLUMNS fk_col
                ON fk_col.OWNER = fk.OWNER
               AND fk_col.CONSTRAINT_NAME = fk.CONSTRAINT_NAME
               AND fk_col.TABLE_NAME = fk.TABLE_NAME
            INNER JOIN ALL_CONSTRAINTS pk
                ON pk.OWNER = fk.R_OWNER
               AND pk.CONSTRAINT_NAME = fk.R_CONSTRAINT_NAME
            INNER JOIN ALL_CONS_COLUMNS pk_col
                ON pk_col.OWNER = pk.OWNER
               AND pk_col.CONSTRAINT_NAME = pk.CONSTRAINT_NAME
               AND pk_col.POSITION = fk_col.POSITION
            WHERE fk.OWNER = :schemaName
              AND fk.TABLE_NAME = :tableName
              AND fk.CONSTRAINT_TYPE = 'R'
            ORDER BY fk.CONSTRAINT_NAME, fk_col.POSITION
            """;

        using var command = CreateCommand(connection, sql);
        command.Parameters.Add(CreateVarcharParameter("schemaName", schemaName));
        command.Parameters.Add(CreateVarcharParameter("tableName", tableName));

        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var foreignKeys = new Dictionary<string, MutableForeignKey>(StringComparer.OrdinalIgnoreCase);

        while (await reader.ReadAsync(cancellationToken))
        {
            var constraintName = reader.GetString(0);

            if (!foreignKeys.TryGetValue(constraintName, out var foreignKey))
            {
                foreignKey = new MutableForeignKey(
                    constraintName,
                    reader.GetString(2),
                    reader.GetString(3));
                foreignKeys.Add(constraintName, foreignKey);
            }

            foreignKey.ColumnNames.Add(reader.GetString(1));
            foreignKey.ReferencedColumnNames.Add(reader.GetString(4));
        }

        return foreignKeys.Values
            .Select(x => new OracleForeignKeyMetadata
            {
                Name = x.Name,
                ColumnNames = x.ColumnNames,
                ReferencedSchemaName = x.ReferencedSchemaName,
                ReferencedTableName = x.ReferencedTableName,
                ReferencedColumnNames = x.ReferencedColumnNames
            })
            .ToArray();
    }

    private static async Task<IReadOnlyList<OracleUniqueConstraintMetadata>> GetUniqueConstraintsAsync(
        OracleConnection connection,
        string schemaName,
        string tableName,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                ac.CONSTRAINT_NAME,
                acc.COLUMN_NAME
            FROM ALL_CONSTRAINTS ac
            INNER JOIN ALL_CONS_COLUMNS acc
                ON acc.OWNER = ac.OWNER
               AND acc.CONSTRAINT_NAME = ac.CONSTRAINT_NAME
               AND acc.TABLE_NAME = ac.TABLE_NAME
            WHERE ac.OWNER = :schemaName
              AND ac.TABLE_NAME = :tableName
              AND ac.CONSTRAINT_TYPE = 'U'
            ORDER BY ac.CONSTRAINT_NAME, acc.POSITION
            """;

        using var command = CreateCommand(connection, sql);
        command.Parameters.Add(CreateVarcharParameter("schemaName", schemaName));
        command.Parameters.Add(CreateVarcharParameter("tableName", tableName));

        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var constraints = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        while (await reader.ReadAsync(cancellationToken))
        {
            var constraintName = reader.GetString(0);
            if (!constraints.TryGetValue(constraintName, out var columnNames))
            {
                columnNames = [];
                constraints.Add(constraintName, columnNames);
            }

            columnNames.Add(reader.GetString(1));
        }

        return constraints
            .Select(x => new OracleUniqueConstraintMetadata
            {
                Name = x.Key,
                ColumnNames = x.Value
            })
            .ToArray();
    }

    private static async Task<IReadOnlyList<OracleIndexMetadata>> GetIndexesAsync(
        OracleConnection connection,
        string schemaName,
        string tableName,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                ai.INDEX_NAME,
                ai.UNIQUENESS,
                aic.COLUMN_NAME
            FROM ALL_INDEXES ai
            INNER JOIN ALL_IND_COLUMNS aic
                ON aic.INDEX_OWNER = ai.OWNER
               AND aic.INDEX_NAME = ai.INDEX_NAME
               AND aic.TABLE_OWNER = ai.TABLE_OWNER
               AND aic.TABLE_NAME = ai.TABLE_NAME
            WHERE ai.TABLE_OWNER = :schemaName
              AND ai.TABLE_NAME = :tableName
            ORDER BY ai.INDEX_NAME, aic.COLUMN_POSITION
            """;

        using var command = CreateCommand(connection, sql);
        command.Parameters.Add(CreateVarcharParameter("schemaName", schemaName));
        command.Parameters.Add(CreateVarcharParameter("tableName", tableName));

        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var indexes = new Dictionary<string, MutableIndex>(StringComparer.OrdinalIgnoreCase);

        while (await reader.ReadAsync(cancellationToken))
        {
            var indexName = reader.GetString(0);
            if (!indexes.TryGetValue(indexName, out var index))
            {
                index = new MutableIndex(
                    indexName,
                    reader.GetString(1).Equals("UNIQUE", StringComparison.OrdinalIgnoreCase));
                indexes.Add(indexName, index);
            }

            index.ColumnNames.Add(reader.GetString(2));
        }

        return indexes.Values
            .Select(x => new OracleIndexMetadata
            {
                Name = x.Name,
                ColumnNames = x.ColumnNames,
                IsUnique = x.IsUnique
            })
            .ToArray();
    }

    private static OracleConnection CreateConnection(OracleConnectionOptions connectionOptions)
    {
        return new OracleConnection(connectionOptions.ToConnectionString());
    }

    private static OracleCommand CreateCommand(OracleConnection connection, string commandText)
    {
        return new OracleCommand(commandText, connection)
        {
            BindByName = true,
            CommandType = CommandType.Text
        };
    }

    private static OracleParameter CreateVarcharParameter(string name, string value)
    {
        return new OracleParameter(name, OracleDbType.Varchar2, value, ParameterDirection.Input);
    }

    private static bool IsExcludedSchema(
        string schemaName,
        OracleMetadataReaderOptions options)
    {
        return !options.ShowSystemSchemas
            && options.ExcludedSystemSchemas.Contains(schemaName);
    }

    private static void ThrowIfNull<T>(T? value, string parameterName)
        where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    private static void ThrowIfNullOrWhiteSpace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", parameterName);
        }
    }

    private static async Task<int?> GetNullableInt32Async(
        OracleDataReader reader,
        int ordinal,
        CancellationToken cancellationToken)
    {
        return await reader.IsDBNullAsync(ordinal, cancellationToken)
            ? null
            : Convert.ToInt32(reader.GetDecimal(ordinal));
    }

    private static async Task<string?> GetNullableStringAsync(
        OracleDataReader reader,
        int ordinal,
        CancellationToken cancellationToken)
    {
        return await reader.IsDBNullAsync(ordinal, cancellationToken)
            ? null
            : reader.GetString(ordinal);
    }

    private sealed record MutableForeignKey(
        string Name,
        string ReferencedSchemaName,
        string ReferencedTableName)
    {
        public List<string> ColumnNames { get; } = [];

        public List<string> ReferencedColumnNames { get; } = [];
    }

    private sealed record MutableIndex(
        string Name,
        bool IsUnique)
    {
        public List<string> ColumnNames { get; } = [];
    }
}
