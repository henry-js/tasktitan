using System.CommandLine;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.Reflection;

using Microsoft.Extensions.Hosting;

namespace TaskTitan.Cli.Extensions;

public static class CommandLineExtensions
{
    private const string UseCommandHandler = "UseCommandHandler";
    public static IHostBuilder UseProjectCommandHandlers(this IHostBuilder builder)
    {
        var commandHandlers = typeof(CommandLineExtensions).Assembly.GetTypes().Where(t =>
            t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(ICommandHandler)) && t?.DeclaringType?.IsAssignableTo(typeof(Command)) == true);

        Type[] types = [typeof(IHostBuilder), typeof(Type), typeof(Type)];
        MethodInfo mi = typeof(HostingExtensions).GetMethod(UseCommandHandler, types)
            ?? throw new MissingMethodException($"No method on type {nameof(HostingExtensions)} named {UseCommandHandler} expecting parameters [{string.Join(",", types.Select(t => t.Name))}]");

        foreach (var handler in commandHandlers)
        {
            mi.Invoke(null, [builder, handler.DeclaringType, handler]);
        }
        return builder;
    }
}
