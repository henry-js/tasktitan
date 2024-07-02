using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

using TaskTitan.Infrastructure.Expressions;
using TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;
using TaskTitan.Infrastructure.Services;

namespace TaskTitan.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITaskItemService, TaskItemService>();
        services.AddScoped<IExpressionParser, ExpressionParser>();
        services.AddScoped<IStringFilterConverter<DateTime>, Dates.DateTimeConverter>();
        services.AddMicrosoftTodoInfrastructure();

        return services;
    }
}
