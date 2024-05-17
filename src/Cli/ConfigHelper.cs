using System.Reflection;

namespace TaskTitan.Cli;

public static class ConfigHelper
{
    public static string SourceDirectory => Path.GetDirectoryName(AppContext.BaseDirectory)!;
    public static string SourceDirectoryDataFolder => Path.Combine(SourceDirectory, ".tasktitan");
    public static string SourceDbPath => Path.Combine(SourceDirectoryDataFolder, DbName);
    public static string UserProfileDirectory => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public static string UserProfileDirectoryDataFolder => Path.Combine(UserProfileDirectory, ".tasktitan");
    public static string UserProfileDbPath => Path.Combine(UserProfileDirectoryDataFolder, DbName);
    public static string DbName => "tasks.db";

    public static string FindTaskTitanDataFolder()
    {
        if (Directory.Exists(SourceDirectoryDataFolder))
        {
            return SourceDirectoryDataFolder;
        }
        else if (Directory.Exists(UserProfileDirectoryDataFolder))
        {
            return UserProfileDirectoryDataFolder;
        }
        else throw new Exception("Could not find .tasktitan data folder");
    }
}
