using OracleEntityGenerator.Core.Metadata;

namespace OracleEntityGenerator.Tests;

internal static class TestTableFactory
{
    public static OracleTableMetadata CreateOrderLinesTable()
    {
        return new OracleTableMetadata
        {
            SchemaName = "CRM",
            Name = "ORDER_LINES",
            Comment = "Order lines table",
            Columns =
            [
                new OracleColumnMetadata
                {
                    Name = "ORDER_ID",
                    DataType = "NUMBER",
                    Precision = 10,
                    Scale = 0,
                    IsNullable = false,
                    Ordinal = 1
                },
                new OracleColumnMetadata
                {
                    Name = "LINE_NO",
                    DataType = "NUMBER",
                    Precision = 5,
                    Scale = 0,
                    IsNullable = false,
                    Ordinal = 2
                },
                new OracleColumnMetadata
                {
                    Name = "ORDER_NO",
                    DataType = "VARCHAR2",
                    Length = 30,
                    IsNullable = false,
                    Ordinal = 3
                },
                new OracleColumnMetadata
                {
                    Name = "AMOUNT",
                    DataType = "NUMBER",
                    Precision = 12,
                    Scale = 2,
                    IsNullable = false,
                    Ordinal = 4
                },
                new OracleColumnMetadata
                {
                    Name = "NOTE",
                    DataType = "VARCHAR2",
                    Length = 200,
                    IsNullable = true,
                    Comment = "Line note",
                    Ordinal = 5
                },
                new OracleColumnMetadata
                {
                    Name = "CREATED_AT",
                    DataType = "DATE",
                    IsNullable = false,
                    Ordinal = 6
                }
            ],
            PrimaryKey = new OraclePrimaryKeyMetadata
            {
                Name = "PK_ORDER_LINES",
                ColumnNames = ["ORDER_ID", "LINE_NO"]
            }
        };
    }

    public static OracleTableMetadata CreateKeylessCustomersTable()
    {
        return new OracleTableMetadata
        {
            SchemaName = "CRM",
            Name = "CUSTOMERS",
            Columns =
            [
                new OracleColumnMetadata
                {
                    Name = "CUSTOMER_ID",
                    DataType = "NUMBER",
                    Precision = 10,
                    Scale = 0,
                    IsNullable = false,
                    Ordinal = 1
                }
            ]
        };
    }

    public static OracleTableMetadata CreateNumberMetadataEdgeCaseTable()
    {
        return new OracleTableMetadata
        {
            SchemaName = "CRM",
            Name = "NUMBER_EDGE_CASES",
            Columns =
            [
                new OracleColumnMetadata
                {
                    Name = "ID",
                    DataType = "NUMBER",
                    Precision = 10,
                    Scale = 0,
                    IsNullable = false,
                    Ordinal = 1
                },
                new OracleColumnMetadata
                {
                    Name = "UNKNOWN_SCALE_VALUE",
                    DataType = "NUMBER",
                    Precision = 10,
                    Scale = null,
                    IsNullable = false,
                    Ordinal = 2
                },
                new OracleColumnMetadata
                {
                    Name = "LOWER_CASE_AMOUNT",
                    DataType = " number ",
                    Precision = 12,
                    Scale = 2,
                    IsNullable = false,
                    Ordinal = 3
                },
                new OracleColumnMetadata
                {
                    Name = "NO_PRECISION_VALUE",
                    DataType = "NUMBER",
                    Precision = null,
                    Scale = 2,
                    IsNullable = false,
                    Ordinal = 4
                }
            ],
            PrimaryKey = new OraclePrimaryKeyMetadata
            {
                Name = "PK_NUMBER_EDGE_CASES",
                ColumnNames = ["ID"]
            }
        };
    }
}
