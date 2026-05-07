using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OracleEntityGenerator.CodeGeneration;
using OracleEntityGenerator.Core.Generation;
using OracleEntityGenerator.Core.Naming;

namespace OracleEntityGenerator.Tests.SmokeTests;

public sealed class GeneratedOutputSmokeTests
{
    [Fact]
    public void GeneratedEntityAndConfiguration_CompileForRegularTable()
    {
        var files = GenerateFiles(
            SmokeTestData.CreateOrderLinesTable(),
            new GenerationOptions
            {
                EntityNamespace = "Smoke.Domain.Entities",
                ConfigurationNamespace = "Smoke.Infrastructure.Persistence.Configurations"
            });

        WriteGeneratedFiles("regular-table", files);
        AssertGeneratedOutputCompiles(files);
    }

    [Fact]
    public void GeneratedEntityAndConfiguration_CompileForKeylessTableWhenEnabled()
    {
        var files = GenerateFiles(
            SmokeTestData.CreateKeylessCustomersTable(),
            new GenerationOptions
            {
                EntityNamespace = "Smoke.Domain.Entities",
                ConfigurationNamespace = "Smoke.Infrastructure.Persistence.Configurations",
                GenerateKeylessEntities = true
            });

        WriteGeneratedFiles("keyless-table", files);
        AssertGeneratedOutputCompiles(files);
    }

    [Fact]
    public void GeneratedEntityAndConfiguration_CompileWhenNullableReferenceTypesAreDisabled()
    {
        var files = GenerateFiles(
            SmokeTestData.CreateOrderLinesTable(),
            new GenerationOptions
            {
                EntityNamespace = "Smoke.Domain.Entities",
                ConfigurationNamespace = "Smoke.Infrastructure.Persistence.Configurations",
                NullableReferenceTypes = false
            });

        WriteGeneratedFiles("nullable-disabled", files);
        AssertGeneratedOutputCompiles(files);
    }

    [Fact]
    public void GeneratedEntityAndConfiguration_CompileForIdentifierEdgeCases()
    {
        var files = GenerateFiles(
            SmokeTestData.CreateIdentifierEdgeCaseTable(),
            new GenerationOptions
            {
                EntityNamespace = "Smoke.Domain.Entities",
                ConfigurationNamespace = "Smoke.Infrastructure.Persistence.Configurations"
            });

        WriteGeneratedFiles("identifier-edge-cases", files);
        AssertGeneratedOutputCompiles(files);
    }

    [Fact]
    public void GeneratedEntityAndConfiguration_CompileForPreservedKeywordIdentifiers()
    {
        var files = GenerateFiles(
            SmokeTestData.CreatePreservedKeywordTable(),
            new GenerationOptions
            {
                EntityNamespace = "Smoke.Domain.Entities",
                ConfigurationNamespace = "Smoke.Infrastructure.Persistence.Configurations",
                Naming = new NamingOptions
                {
                    PreserveOriginalNames = true
                }
            });

        WriteGeneratedFiles("preserved-keywords", files);
        AssertGeneratedOutputCompiles(files);
    }

    private static IReadOnlyList<GeneratedCodeFile> GenerateFiles(
        Core.Metadata.OracleTableMetadata table,
        GenerationOptions options)
    {
        var service = new OracleCodeGenerationService();
        return service.GenerateFiles([table], options);
    }

    private static void WriteGeneratedFiles(
        string scenarioName,
        IReadOnlyList<GeneratedCodeFile> files)
    {
        var outputDirectory = Path.GetFullPath(Path.Combine(GetTestProjectDirectory(), "output", scenarioName));
        Directory.CreateDirectory(outputDirectory);

        foreach (var file in files)
        {
            var relativeDirectory = string.IsNullOrWhiteSpace(file.RelativeDirectory)
                ? string.Empty
                : file.RelativeDirectory;

            var targetDirectory = Path.GetFullPath(Path.Combine(outputDirectory, relativeDirectory));
            var targetPath = Path.GetFullPath(Path.Combine(targetDirectory, file.FileName));

            if (!IsPathInsideDirectory(targetPath, outputDirectory))
            {
                throw new InvalidOperationException($"Generated file path escapes output directory: {file.FileName}");
            }

            Directory.CreateDirectory(targetDirectory);
            File.WriteAllText(targetPath, file.SourceText);

            Assert.True(File.Exists(targetPath), $"Generated output file was not written: {targetPath}");
        }
    }

    private static void AssertGeneratedOutputCompiles(
        IReadOnlyList<GeneratedCodeFile> files)
    {
        var syntaxTrees = files
            .Select(file => CSharpSyntaxTree.ParseText(
                file.SourceText,
                new CSharpParseOptions(LanguageVersion.Preview),
                path: file.FileName))
            .ToArray();

        var compilation = CSharpCompilation.Create(
            assemblyName: $"SmokeGeneratedOutput_{Guid.NewGuid():N}",
            syntaxTrees: syntaxTrees,
            references: GetTrustedPlatformReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable));

        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);

        Assert.True(result.Success, FormatDiagnostics(result.Diagnostics, files));
    }

    private static IReadOnlyList<MetadataReference> GetTrustedPlatformReferences()
    {
        var trustedPlatformAssemblies =
            (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");

        Assert.False(string.IsNullOrWhiteSpace(trustedPlatformAssemblies));

        return trustedPlatformAssemblies
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path))
            .ToArray();
    }

    private static string GetTestProjectDirectory()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
    }

    private static bool IsPathInsideDirectory(string path, string directory)
    {
        var normalizedDirectory = directory.EndsWith(Path.DirectorySeparatorChar)
            ? directory
            : directory + Path.DirectorySeparatorChar;

        return path.StartsWith(normalizedDirectory, StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatDiagnostics(
        IEnumerable<Diagnostic> diagnostics,
        IReadOnlyList<GeneratedCodeFile> files)
    {
        var failures = diagnostics
            .Where(x => x.Severity == DiagnosticSeverity.Error)
            .Select(x => x.ToString());

        return string.Join(
            Environment.NewLine,
            failures.Concat(files.Select(file => $"--- {file.FileName} ---{Environment.NewLine}{file.SourceText}")));
    }
}
