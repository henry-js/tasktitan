using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TaskTitan.Data;

public static class DataExtensions
{
    public static IServiceCollection RegisterDb(this IServiceCollection services, string connectionString)
    {
        return services.AddDbContext<TaskTitanDbContext>(options =>
            options.UseSqlite(connectionString));
    }
}
