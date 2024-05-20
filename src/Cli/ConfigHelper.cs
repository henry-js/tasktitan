using System.Diagnostics;

namespace TaskTitan.Cli;

public static class ConfigHelper
{
    private static string DbName => "tasks.db";
    private static string DbName => "tasks.db";
    private static string UserProfileDirectory => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private static string UserProfileDirectoryDataFolder => Path.Combine(UserProfileDirectory, ".tasktitan");
    public static string UserProfileDbPath => Path.Combine(UserProfileDirectoryDataFolder, DbName);
    public static string UserProfileDbPath => Path.Combine(UserProfileDirectoryDataFolder, DbName);

    // public static string FindTaskTitanDataFolder()
    // {
    //     if (Directory.Exists(SourceDirectoryDataFolder))
    //     {
    //         return SourceDirectoryDataFolder;
    //     }
    //     else if (Directory.Exists(UserProfileDirectoryDataFolder))
    //     {
    //         return UserProfileDirectoryDataFolder;
    //     }
    //     else throw new Exception("Could not find .tasktitan data folder");
    // }

    internal static void FirstRun()
    {
        Directory.CreateDirectory(UserProfileDirectoryDataFolder);
        Directory.CreateDirectory(UserProfileDirectoryDataFolder);
        if (File.Exists(UserProfileDbPath)) return;

        var path = new TextPath(UserProfileDbPath.Replace(UserProfileDirectory, @"~\"));
        AnsiConsole.MarkupLine("A task database could [bold]not[/] be found in:");
        AnsiConsole.Write(path);
        AnsiConsole.WriteLine();

        if (!AnsiConsole.Confirm("Would you like a new database created, so tasktitan can proceed?")) return;

        var dbbundle = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasksdb_migrations.exe");
        Process.Start(dbbundle);
        var dbbundle = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasksdb_migrations.exe");
        Process.Start(dbbundle);
    }
}
