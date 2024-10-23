using Microsoft.Extensions.Configuration;
using TaskTitan.Data.Reports;

namespace TaskTitan.Configuration;

public static class Global
{
    private static ReportDictionary _reports = [];
    public static ReportDictionary Reports => _reports;
    public const string APP_NAME = "TaskTitan";
    // public static readonly string DIRECTORY_NAME = $".{APP_NAME}".ToLower();

    public static string DataDirectoryPath => Xdg.DATA_HOME is not null
        ? Path.Combine(Xdg.DATA_HOME, APP_NAME.ToLower())
        : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APP_NAME.ToLower());
    public static string ConfigDirectoryPath => Xdg.CONFIG_HOME is not null
        ? Path.Combine(Xdg.CONFIG_HOME, APP_NAME.ToLower())
        : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APP_NAME.ToLower());

    public static string StateDirectoryPath => Xdg.STATE_HOME is not null
        ? Path.Combine(Xdg.STATE_HOME, APP_NAME.ToLower())
        : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APP_NAME.ToLower());

    public static void CreateConfigurationDirectories()
    {
        if (!Directory.Exists(DataDirectoryPath)) Directory.CreateDirectory(DataDirectoryPath);
        if (!Directory.Exists(ConfigDirectoryPath)) Directory.CreateDirectory(ConfigDirectoryPath);
        if (!Directory.Exists(StateDirectoryPath)) Directory.CreateDirectory(StateDirectoryPath);
    }

    public static void LoadReports(ReportDictionary reports)
    {
        _reports = reports;
    }
}
