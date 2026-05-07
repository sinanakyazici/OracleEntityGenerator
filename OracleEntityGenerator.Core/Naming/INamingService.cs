namespace OracleEntityGenerator.Core.Naming;

public interface INamingService
{
    string GetClassName(string oracleTableName, NamingOptions options);

    string GetPropertyName(string oracleColumnName, NamingOptions options);
}
