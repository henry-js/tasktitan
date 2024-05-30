using System.Data;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

using TaskTitan.Data.Repositories;

namespace TaskTitan.Data;

public static class DataExtensions
{
    public static IServiceCollection RegisterDb(this IServiceCollection services, string connectionString, ILogger? logger = null)
    {
        logger?.Debug("Sqlite connection string: {0}", connectionString);
        services.AddDbContext<TaskTitanDbContext>(options =>
            options.UseSqlite(connectionString));
        services.AddTransient<IDbConnection>(sp => new SqliteConnection(connectionString));
        services.AddTransient<ITaskItemRepository, TaskItemRepository>();

        return services;
    }

    public static IServiceCollection RegisterDb(this IServiceCollection services, IConfiguration configuration, ILogger? logger = null)
    {
        var connectionString = configuration.GetConnectionString("TaskDbConnectionString");
        logger?.Debug("Sqlite connection string: {0}", connectionString);
        services.AddDbContext<TaskTitanDbContext>(options =>
            options.UseSqlite(connectionString));
        services.AddTransient<ITaskItemRepository, TaskItemRepository>();

        return services;
    }

}
