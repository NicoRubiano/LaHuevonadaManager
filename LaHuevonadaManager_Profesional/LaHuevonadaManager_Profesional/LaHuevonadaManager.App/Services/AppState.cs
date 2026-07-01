using LaHuevonadaManager.Models;

namespace LaHuevonadaManager.Services;

public static class AppState
{
    public static FarmDocument Document { get; set; } = new();
    public static string? CurrentFilePath { get; set; }
}
