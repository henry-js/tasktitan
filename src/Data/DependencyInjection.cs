using System.Data;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

using SqlKata.Compilers;
using SqlKata.Execution;

using TaskTitan.Data.Repositories;

namespace TaskTitan.Data;

public static class DependencyInjection
{
    public static IServiceCollection RegisterDb(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        services.AddSingleton<DatabaseInitializer>();
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
