using System.IO;
using System.Text.Json;

namespace OracleEntityGenerator.VsExtension.Settings;

public sealed class GeneratorSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public string SettingsFilePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OracleEntityGenerator",
        "settings.json");

    public async Task SaveAsync(
        GeneratorSettingsProfile profile,
        CancellationToken cancellationToken)
    {
        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        var directory = Path.GetDirectoryName(SettingsFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(profile, JsonOptions);
        using var writer = new StreamWriter(SettingsFilePath, append: false);
        await writer.WriteAsync(json);
    }

    public async Task<GeneratorSettingsProfile?> LoadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(SettingsFilePath))
        {
            return null;
        }

        using var stream = File.OpenRead(SettingsFilePath);
        return await JsonSerializer.DeserializeAsync<GeneratorSettingsProfile>(stream, JsonOptions, cancellationToken);
    }
}
