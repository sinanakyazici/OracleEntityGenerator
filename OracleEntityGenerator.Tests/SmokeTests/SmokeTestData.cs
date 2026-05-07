using OracleEntityGenerator.Core.Metadata;

namespace OracleEntityGenerator.Tests.SmokeTests;

internal static class SmokeTestData
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

    public static OracleTableMetadata CreateIdentifierEdgeCaseTable()
    {
        return new OracleTableMetadata
        {
            SchemaName = "CRM\"EDGE",
            Name = "123_ORDER DETAILS",
            Comment = "Identifier edge case table",
            Columns =
            [
                new OracleColumnMetadata
                {
                    Name = "1ST_VALUE",
                    DataType = "NUMBER",
                    Precision = 38,
                    Scale = 0,
                    IsNullable = false,
                    Ordinal = 1
                },
                new OracleColumnMetadata
                {
                    Name = "PUBLIC",
                    DataType = "VARCHAR2",
                    Length = 20,
                    IsNullable = false,
                    Ordinal = 2
                },
                new OracleColumnMetadata
                {
                    Name = "DESCRIPTION_TEXT",
                    DataType = "CLOB",
                    Length = 4000,
                    IsNullable = true,
                    Comment = "CLOB should not emit HasMaxLength",
                    Ordinal = 3
                },
                new OracleColumnMetadata
                {
                    Name = "EVENT_TIME",
                    DataType = "TIMESTAMP WITH TIME ZONE",
                    IsNullable = true,
                    Ordinal = 4
                }
            ],
            PrimaryKey = new OraclePrimaryKeyMetadata
            {
                Name = "PK_123_ORDER_DETAILS",
                ColumnNames = ["1ST_VALUE"]
            }
        };
    }

    public static OracleTableMetadata CreateDuplicatePropertyNameTable()
    {
        return new OracleTableMetadata
        {
            SchemaName = "CRM",
            Name = "DUPLICATE_COLUMNS",
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
                },
                new OracleColumnMetadata
                {
                    Name = "CUSTOMER-ID",
                    DataType = "NUMBER",
                    Precision = 10,
                    Scale = 0,
                    IsNullable = false,
                    Ordinal = 2
                }
            ],
            PrimaryKey = new OraclePrimaryKeyMetadata
            {
                Name = "PK_DUPLICATE_COLUMNS",
                ColumnNames = ["CUSTOMER_ID"]
            }
        };
    }

    public static OracleTableMetadata CreatePreservedKeywordTable()
    {
        return new OracleTableMetadata
        {
            SchemaName = "CRM",
            Name = "class",
            Columns =
            [
                new OracleColumnMetadata
                {
                    Name = "public",
                    DataType = "NUMBER",
                    Precision = 10,
                    Scale = 0,
                    IsNullable = false,
                    Ordinal = 1
                },
                new OracleColumnMetadata
                {
                    Name = "string",
                    DataType = "VARCHAR2",
                    Length = 50,
                    IsNullable = true,
                    Ordinal = 2
                }
            ],
            PrimaryKey = new OraclePrimaryKeyMetadata
            {
                Name = "PK_CLASS",
                ColumnNames = ["public"]
            }
        };
    }
}
