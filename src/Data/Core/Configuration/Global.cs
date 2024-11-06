using Microsoft.Extensions.Configuration;
using static System.Environment;
using TaskTitan.Data.Reports;

namespace TaskTitan.Core.Configuration;

public static class Global
{
    static Global()
    {
        if (!Directory.Exists(DataDirectoryPath)) Directory.CreateDirectory(DataDirectoryPath);
        if (!Directory.Exists(ConfigDirectoryPath)) Directory.CreateDirectory(ConfigDirectoryPath);
        if (!Directory.Exists(StateDirectoryPath)) Directory.CreateDirectory(StateDirectoryPath);
    }
    public const string APP_NAME = "TaskTitan";
    public static string DataDirectoryPath => Path.Combine(XDG_DATA_HOME, APP_NAME.ToLower());
    public static string ConfigDirectoryPath => Path.Combine(XDG_CONFIG_HOME, APP_NAME.ToLower());
    public static string StateDirectoryPath => Path.Combine(XDG_STATE_HOME, APP_NAME.ToLower());


    private static string XDG_DATA_HOME => GetXDGPath(nameof(XDG_DATA_HOME), GetFolderPath(SpecialFolder.LocalApplicationData));
    private static string XDG_CONFIG_HOME => GetXDGPath(nameof(XDG_CONFIG_HOME), GetFolderPath(SpecialFolder.LocalApplicationData));
    private static string XDG_STATE_HOME => GetXDGPath(nameof(XDG_STATE_HOME), GetFolderPath(SpecialFolder.LocalApplicationData));

    private static string GetXDGPath(string envVar, string fallbackPath)
    {
        string? path = GetEnvironmentVariable(envVar, EnvironmentVariableTarget.User);
        return !string.IsNullOrEmpty(path) ? path : Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), fallbackPath);
    }
}
