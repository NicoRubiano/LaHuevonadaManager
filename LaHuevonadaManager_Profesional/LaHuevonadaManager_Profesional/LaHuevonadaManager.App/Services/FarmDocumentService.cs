using System.IO;
using System.IO.Compression;
using System.Text.Json;
using LaHuevonadaManager.Models;

namespace LaHuevonadaManager.Services;

public class FarmDocumentService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public async Task SaveAsync(string path, FarmDocument document)
    {
        document.UpdatedAt = DateTime.Now;
        var temp = Path.Combine(Path.GetTempPath(), $"lahuevonada_{Guid.NewGuid():N}");
        Directory.CreateDirectory(temp);
        try
        {
            await File.WriteAllTextAsync(Path.Combine(temp, "data.json"), JsonSerializer.Serialize(document, JsonOptions));
            await File.WriteAllTextAsync(Path.Combine(temp, "metadata.json"), JsonSerializer.Serialize(new
            {
                app = "LaHuevonada Manager",
                version = "1.0.0",
                savedAt = DateTime.Now,
                farm = document.FarmName
            }, JsonOptions));
            var assets = Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png");
            if (File.Exists(assets)) File.Copy(assets, Path.Combine(temp, "logo.png"), true);
            if (File.Exists(path)) File.Delete(path);
            ZipFile.CreateFromDirectory(temp, path, CompressionLevel.Optimal, false);
        }
        finally { if (Directory.Exists(temp)) Directory.Delete(temp, true); }
    }

    public async Task<FarmDocument> OpenAsync(string path)
    {
        var temp = Path.Combine(Path.GetTempPath(), $"lahuevonada_{Guid.NewGuid():N}");
        Directory.CreateDirectory(temp);
        try
        {
            ZipFile.ExtractToDirectory(path, temp);
            var data = await File.ReadAllTextAsync(Path.Combine(temp, "data.json"));
            return JsonSerializer.Deserialize<FarmDocument>(data) ?? new FarmDocument();
        }
        finally { if (Directory.Exists(temp)) Directory.Delete(temp, true); }
    }

    public string CreateBackupPath(string originalPath)
    {
        var dir = Path.Combine(Path.GetDirectoryName(originalPath) ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Backups_LaHuevonada");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, $"Backup_LaHuevonada_{DateTime.Now:yyyyMMdd_HHmmss}.lahuevonada");
    }
}
