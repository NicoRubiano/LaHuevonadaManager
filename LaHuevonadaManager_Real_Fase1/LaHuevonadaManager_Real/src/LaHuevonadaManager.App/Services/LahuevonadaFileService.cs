using System.IO;
using System.IO.Compression;
using System.Text.Json;
using LaHuevonadaManager.Models;

namespace LaHuevonadaManager.Services;

public sealed class LahuevonadaFileService
{
    private const string DataEntryName = "data.json";
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public void Save(string path, AppData data)
    {
        var temp = Path.GetTempFileName();
        try
        {
            using (var zip = ZipFile.Open(temp, ZipArchiveMode.Update))
            {
                var entry = zip.CreateEntry(DataEntryName);
                using var stream = entry.Open();
                JsonSerializer.Serialize(stream, data, _jsonOptions);
            }
            File.Copy(temp, path, true);
        }
        finally { if (File.Exists(temp)) File.Delete(temp); }
    }

    public AppData Load(string path)
    {
        using var zip = ZipFile.OpenRead(path);
        var entry = zip.GetEntry(DataEntryName) ?? throw new InvalidDataException("Archivo .lahuevonada inválido.");
        using var stream = entry.Open();
        return JsonSerializer.Deserialize<AppData>(stream) ?? new AppData();
    }
}
