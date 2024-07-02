// using System.Text.RegularExpressions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

namespace TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;

internal static class DependencyInjection
{
    public static IServiceCollection AddMicrosoftTodoInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton(sp => new GraphServiceClient(TodoAuthenticationProviderFactory.GetAuthenticationProvider().Result));
        services.AddScoped<IExternalTaskService, MicrosoftTodoService>();
        return services;
    }
}
