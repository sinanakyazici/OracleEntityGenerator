namespace OracleEntityGenerator.Oracle;

public sealed record OracleMetadataReaderOptions
{
    public bool ShowSystemSchemas { get; init; }

    public IReadOnlyCollection<string> ExcludedSystemSchemas { get; init; } = new HashSet<string>(
        [
            "SYS",
            "SYSTEM",
            "XDB",
            "MDSYS",
            "CTXSYS",
            "ORDSYS",
            "WMSYS"
        ],
        StringComparer.OrdinalIgnoreCase);
}
