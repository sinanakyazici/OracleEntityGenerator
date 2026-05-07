using System.IO;
using OracleEntityGenerator.Core.Generation;

namespace OracleEntityGenerator.VsExtension.Services;

public sealed class GeneratedFileWriter
{
    public async Task WriteFilesAsync(
        string baseDirectory,
        IReadOnlyList<GeneratedCodeFile> files,
        bool overwriteExistingFiles,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseDirectory));
        }

        if (files is null)
        {
            throw new ArgumentNullException(nameof(files));
        }

        var normalizedBaseDirectory = Path.GetFullPath(baseDirectory);
        Directory.CreateDirectory(normalizedBaseDirectory);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var targetDirectory = Path.GetFullPath(Path.Combine(
                normalizedBaseDirectory,
                file.RelativeDirectory ?? string.Empty));
            var targetPath = Path.GetFullPath(Path.Combine(targetDirectory, file.FileName));

            if (!IsPathInsideDirectory(targetPath, normalizedBaseDirectory))
            {
                throw new InvalidOperationException($"Generated file path escapes output directory: {file.FileName}");
            }

            if (!overwriteExistingFiles && File.Exists(targetPath))
            {
                throw new IOException($"File already exists: {targetPath}");
            }

            Directory.CreateDirectory(targetDirectory);
            using var writer = new StreamWriter(targetPath, append: false);
            await writer.WriteAsync(file.SourceText);
        }
    }

    private static bool IsPathInsideDirectory(string path, string directory)
    {
        var separator = Path.DirectorySeparatorChar.ToString();
        var normalizedDirectory = directory.EndsWith(separator, StringComparison.Ordinal)
            ? directory
            : directory + separator;

        return path.StartsWith(normalizedDirectory, StringComparison.OrdinalIgnoreCase);
    }
}
