using System.Globalization;
using System.Text;

namespace OracleEntityGenerator.Core.Naming;

public sealed class CSharpNamingService : INamingService
{
    private static readonly HashSet<string> ReservedKeywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char",
        "checked", "class", "const", "continue", "decimal", "default", "delegate",
        "do", "double", "else", "enum", "event", "explicit", "extern", "false",
        "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit",
        "in", "int", "interface", "internal", "is", "lock", "long", "namespace",
        "new", "null", "object", "operator", "out", "override", "params", "private",
        "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
        "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
        "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
        "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
    };

    public string GetClassName(string oracleTableName, NamingOptions options)
    {
        if (string.IsNullOrWhiteSpace(oracleTableName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(oracleTableName));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return ToCSharpIdentifier(
            RemoveConfiguredPrefix(oracleTableName, options.TablePrefixesToRemove),
            options);
    }

    public string GetPropertyName(string oracleColumnName, NamingOptions options)
    {
        if (string.IsNullOrWhiteSpace(oracleColumnName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(oracleColumnName));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return ToCSharpIdentifier(
            RemoveConfiguredPrefix(oracleColumnName, options.ColumnPrefixesToRemove),
            options);
    }

    private static string RemoveConfiguredPrefix(string name, IReadOnlyCollection<string> prefixes)
    {
        foreach (var prefix in prefixes)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                continue;
            }

            if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return name.Substring(prefix.Length);
            }
        }

        return name;
    }

    private static string ToCSharpIdentifier(string oracleName, NamingOptions options)
    {
        var transformedName = options.PreserveOriginalNames
            ? oracleName
            : ToPascalCase(oracleName);

        var identifier = SanitizeIdentifier(transformedName);
        return ReservedKeywords.Contains(identifier) ? $"@{identifier}" : identifier;
    }

    private static string ToPascalCase(string value)
    {
        var builder = new StringBuilder(value.Length);
        var capitalizeNext = true;

        foreach (var character in value)
        {
            if (!char.IsLetterOrDigit(character))
            {
                capitalizeNext = true;
                continue;
            }

            if (capitalizeNext)
            {
                builder.Append(char.ToUpper(character, CultureInfo.InvariantCulture));
                capitalizeNext = false;
                continue;
            }

            builder.Append(char.ToLower(character, CultureInfo.InvariantCulture));
        }

        return builder.ToString();
    }

    private static string SanitizeIdentifier(string value)
    {
        var builder = new StringBuilder(value.Length);

        foreach (var character in value)
        {
            if (builder.Length == 0)
            {
                if (char.IsLetter(character) || character == '_')
                {
                    builder.Append(character);
                }
                else if (char.IsDigit(character))
                {
                    builder.Append('_');
                    builder.Append(character);
                }

                continue;
            }

            builder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
        }

        return builder.Length == 0 ? "_" : builder.ToString();
    }
}
