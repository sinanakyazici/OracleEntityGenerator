using OracleEntityGenerator.Core.Metadata;
using OracleEntityGenerator.Core.TypeMapping;

namespace OracleEntityGenerator.Tests.TypeMapping;

public sealed class OracleTypeMapperTests
{
    [Fact]
    public void MapColumn_RejectsNullColumn()
    {
        var mapper = new OracleTypeMapper();

        Assert.Throws<ArgumentNullException>(
            () => mapper.MapColumn(null!, new OracleTypeMappingOptions()));
    }

    [Fact]
    public void MapColumn_RejectsNullOptions()
    {
        var mapper = new OracleTypeMapper();
        var column = CreateColumn("NUMBER", 10, 0);

        Assert.Throws<ArgumentNullException>(
            () => mapper.MapColumn(column, null!));
    }

    [Theory]
    [InlineData("VARCHAR2", null, null, "string")]
    [InlineData("NUMBER", 1, 0, "bool")]
    [InlineData("NUMBER", 3, 0, "byte")]
    [InlineData("NUMBER", 5, 0, "short")]
    [InlineData("NUMBER", 10, 0, "int")]
    [InlineData("NUMBER", 19, 0, "long")]
    [InlineData("NUMBER", 38, 0, "decimal")]
    [InlineData("NUMBER", null, null, "long")]
    [InlineData("NUMBER", 10, null, "long")]
    [InlineData("NUMBER", 10, -2, "int")]
    [InlineData("NUMBER", 12, 2, "decimal")]
    [InlineData("NVARCHAR2", null, null, "string")]
    [InlineData("CHAR", null, null, "string")]
    [InlineData("NCHAR", null, null, "string")]
    [InlineData("CLOB", null, null, "string")]
    [InlineData("NCLOB", null, null, "string")]
    [InlineData("RAW", null, null, "byte[]")]
    [InlineData("LONG RAW", null, null, "byte[]")]
    [InlineData("FLOAT", null, null, "float")]
    [InlineData("BINARY_FLOAT", null, null, "float")]
    [InlineData("BINARY_DOUBLE", null, null, "double")]
    [InlineData("DATE", null, null, "DateTime")]
    [InlineData("TIMESTAMP", null, null, "DateTime")]
    [InlineData("TIMESTAMP(6)", null, null, "DateTime")]
    [InlineData("TIMESTAMP WITH LOCAL TIME ZONE", null, null, "DateTime")]
    [InlineData("TIMESTAMP WITH TIME ZONE", null, null, "DateTimeOffset")]
    [InlineData("BLOB", null, null, "byte[]")]
    [InlineData("XMLTYPE", null, null, "string")]
    public void MapColumn_MapsOracleTypes(
        string dataType,
        int? precision,
        int? scale,
        string expectedType)
    {
        var mapper = new OracleTypeMapper();
        var column = CreateColumn(dataType, precision, scale);

        var mapping = mapper.MapColumn(column, new OracleTypeMappingOptions());

        Assert.Equal(expectedType, mapping.TypeName);
        Assert.False(mapping.IsUnsupported);
    }

    [Fact]
    public void MapColumn_CanMapNumberOneAsByte()
    {
        var mapper = new OracleTypeMapper();
        var column = CreateColumn("NUMBER", 1, 0);
        var options = new OracleTypeMappingOptions
        {
            NumberOneMapping = OracleNumberOneMapping.Byte
        };

        var mapping = mapper.MapColumn(column, options);

        Assert.Equal("byte", mapping.TypeName);
    }

    [Fact]
    public void MapColumn_ReturnsUnsupportedWarningForUnknownType()
    {
        var mapper = new OracleTypeMapper();
        var column = CreateColumn("SDO_GEOMETRY", null, null);

        var mapping = mapper.MapColumn(column, new OracleTypeMappingOptions());

        Assert.True(mapping.IsUnsupported);
        Assert.Equal("object", mapping.TypeName);
        Assert.Contains("Unsupported Oracle data type", mapping.Warning);
    }

    private static OracleColumnMetadata CreateColumn(
        string dataType,
        int? precision,
        int? scale)
    {
        return new OracleColumnMetadata
        {
            Name = "VALUE",
            DataType = dataType,
            Precision = precision,
            Scale = scale,
            IsNullable = false,
            Ordinal = 1
        };
    }
}
