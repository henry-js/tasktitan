using System.Data;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

using SqlKata.Compilers;
using SqlKata.Execution;

using TaskTitan.Data.Repositories;

namespace TaskTitan.Data;

public static class DataExtensions
{
    public static IServiceCollection RegisterDb(this IServiceCollection services, string connectionString, ILogger? logger = null)
    {
        logger?.Debug("Sqlite connection string: {0}", connectionString);
        // services.AddDbContext<TaskTitanDbContext>(options =>
        //     options.UseSqlite(connectionString));
        services.AddTransient<IDbConnection>(sp => new SqliteConnection(connectionString));
        services.AddTransient<ITaskItemRepository, TaskItemRepository>();
        services.AddTransient(_ =>
        {
            var connection = new SqliteConnection(connectionString);
            var compiler = new SqliteCompiler();
            return new QueryFactory(connection, compiler);
        }
        );

        return services;
    }
}
