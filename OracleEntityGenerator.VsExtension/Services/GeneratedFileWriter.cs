using System.IO;
using OracleEntityGenerator.Core.Generation;

namespace OracleEntityGenerator.VsExtension.Services;

public sealed class GeneratedFileWriter
{
    public async Task WriteFilesAsync(
        IReadOnlyList<GeneratedCodeFile> files,
        bool overwriteExistingFiles,
        CancellationToken cancellationToken)
    {
        if (files is null)
        {
            throw new ArgumentNullException(nameof(files));
        }

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(file.RelativeDirectory))
            {
                throw new ArgumentException("Generated file target directory cannot be empty.", nameof(files));
            }

            var targetDirectory = Path.GetFullPath(file.RelativeDirectory);
            var targetPath = Path.GetFullPath(Path.Combine(targetDirectory, file.FileName));

            if (!IsPathInsideDirectory(targetPath, targetDirectory))
            {
                throw new InvalidOperationException($"Generated file path escapes target directory: {file.FileName}");
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
