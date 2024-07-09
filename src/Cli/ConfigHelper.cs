using System.Diagnostics;

using Serilog;

using Tomlyn.Extensions.Configuration;

namespace TaskTitan.Cli;

public static class ConfigHelper
{
    private static string DbName => "tasks.db";
    private static string UserProfileDirectory => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private static string UserProfileDirectoryDataFolder => Path.Combine(UserProfileDirectory, ".tasktitan");
    public static string UserProfileDbPath => Path.Combine(UserProfileDirectoryDataFolder, DbName);
    internal static string UserConfigPath => Path.Combine(UserProfileDirectory, ".config", "tasktitan", "tasktitan.toml");

    internal static void FirstRun()
    {
        Directory.CreateDirectory(UserProfileDirectoryDataFolder);
        if (File.Exists(UserProfileDbPath)) return;

        var path = new TextPath(UserProfileDbPath.Replace(UserProfileDirectory, @"~\"));
        AnsiConsole.Markup("A task database could [bold]not[/] be found in: ");
        AnsiConsole.Write(path);
        AnsiConsole.WriteLine();

        if (!AnsiConsole.Confirm("Would you like a new database created, so tasktitan can proceed?")) return;

        var db = new DatabaseInitializer($"Data Source={UserProfileDbPath}");
        db.InitializeAsync();
    }

    internal static void AddToPath()
    {
        var expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tasktitan", "current");

        string? currentPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

        if (!string.IsNullOrEmpty(currentPath) && currentPath.Split(';').Contains(expectedPath, StringComparer.Ordinal))
        {
            Log.Information("tasktitan path is already in the PATH environment variable.");
        }
        else
        {
            string updatedPath = string.IsNullOrEmpty(currentPath) ? expectedPath : currentPath + ';' + expectedPath;

            Environment.SetEnvironmentVariable("Path", updatedPath, EnvironmentVariableTarget.User);

            Log.Information("tasktitan path has been added to the PATH environment variable.");
        }
    }

}
