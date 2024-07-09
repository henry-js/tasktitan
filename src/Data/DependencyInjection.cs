using System.Data;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

using SqlKata.Compilers;
using SqlKata.Execution;

using TaskTitan.Data.Repositories;

namespace TaskTitan.Data;

public static class DependencyInjection
{
    public static IServiceCollection RegisterDb(this IServiceCollection services, string connectionString, ILogger? logger = null)
    {
        logger?.Debug("Sqlite connection string: {0}", connectionString);
        services.AddDbContext<TaskTitanDbContext>(options =>
            options.UseSqlite(connectionString));
        services.AddTransient<IDbConnection>(sp => new SqliteConnection(connectionString));
        services.AddTransient<ITaskItemRepository, TaskItemRepository>();
        services.AddTransient(_ =>
        {
            var connection = new SqliteConnection(connectionString);
            var compiler = new SqliteCompiler();
            return new QueryFactory(connection, compiler);
        });

        return services;
    }
}

public class TaskTitanDbContextFactory : IDesignTimeDbContextFactory<TaskTitanDbContext>
{
    private static string DbName => "tasks.db";
    private static string UserProfileDirectory => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private static string UserProfileDirectoryDataFolder => Path.Combine(UserProfileDirectory, ".tasktitan");
    public static string UserProfileDbPath => Path.Combine(UserProfileDirectoryDataFolder, DbName);
    public TaskTitanDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TaskTitanDbContext>();
        optionsBuilder.UseSqlite($"Data Source={UserProfileDbPath}");
        return new TaskTitanDbContext(optionsBuilder.Options);
    }
}
