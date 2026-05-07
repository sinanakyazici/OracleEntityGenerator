namespace OracleEntityGenerator.Core.Generation;

public sealed record GeneratedCodeFile(
    string FileName,
    string RelativeDirectory,
    string SourceText);
