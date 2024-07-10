using Microsoft.Extensions.Logging.Abstractions;

using Constants = TaskTitan.Data.Constants;

namespace TaskTitan.Cli;

public static class ConfigHelper
{
    internal static string UserProfileDirectory => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    internal static string TaskTitanDirectory => Path.Combine(UserProfileDirectory, ".config", "tasktitan");
    internal static string UserConfigFile => Path.Combine(TaskTitanDirectory, "tasktitan.toml");
    internal static string ConnectionString => "Data Source=" + Path.Combine(TaskTitanDirectory, "tasks.db");

    internal static void EnsureDirectoryExists()
    {
        Directory.CreateDirectory(TaskTitanDirectory);
    }

    internal static void EnsureTaskEnvVarExists()
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

    internal static void UpdateDatabase()
    {
        var initializer = new DatabaseInitializer(ConnectionString);
        initializer.Initialize();
    }
}
