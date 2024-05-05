using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

namespace TaskTitan.Data;

public static class DataExtensions
{
    public static IServiceCollection RegisterDb(this IServiceCollection services, string connectionString, ILogger? logger = null)
    {
        logger?.Debug("Sqlite connection string: {0}", connectionString);
        return services.AddDbContext<TaskTitanDbContext>(options =>
            options.UseSqlite(connectionString));
    }
}
