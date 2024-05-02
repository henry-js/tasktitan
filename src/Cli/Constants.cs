using System.Reflection;

namespace TaskTitan.Cli;

public static class Constants
{
    private static readonly string ConfigurationDirectory;

    static Constants()
    {
#if DEBUG
        ConfigurationDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, ".tasktitan");
        DbConnectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), ".tasktitan", "tasks.db")}";
#else
        ConfigurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tasktitan");
         DbConnectionString = $"Data Source={Path.Combine(ConfigurationDirectory, "tasks.db")}";
#endif
    }

    public static string AppSettingsPath => Path.Combine(ConfigurationDirectory, "appsettings.json");
    public static string DbConnectionString { get; }
}
