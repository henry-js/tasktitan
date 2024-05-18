using System.Reflection;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli.Help;

namespace TaskTitan.Cli;

public static class ConfigHelper
{
    private static string SourceDirectory => Path.GetDirectoryName(AppContext.BaseDirectory)!;
    private static string SourceDirectoryDataFolder => Path.Combine(SourceDirectory, ".tasktitan");
    private static string SourceDbPath => Path.Combine(SourceDirectoryDataFolder, DbName);
    private static string UserProfileDirectory => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private static string UserProfileDirectoryDataFolder => Path.Combine(UserProfileDirectory, ".tasktitan");
    private static string UserProfileDbPath => Path.Combine(UserProfileDirectoryDataFolder, DbName);
    private static string DbName => "tasks.db";

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

    internal static async Task FirstRun(Microsoft.Extensions.Hosting.IHost app)
    {
        if (File.Exists(UserProfileDbPath)) return;

        var path = new TextPath(UserProfileDbPath.Replace(UserProfileDirectory, @"~\"));
        AnsiConsole.MarkupLine("A task database could [bold]not[/] be found in:");
        AnsiConsole.Write(path);
        AnsiConsole.WriteLine();

        if (!AnsiConsole.Confirm("Would you like a new database created, so tasktitan can proceed?")) return;

        // Ensure db exists
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskTitanDbContext>();
        await db.Database.EnsureCreatedAsync();
        await db.Database.MigrateAsync();
        //     throw new TaskTitanDatabaseNotFoundException($"Expected to find default {DbName} in source directory");
    }
}
