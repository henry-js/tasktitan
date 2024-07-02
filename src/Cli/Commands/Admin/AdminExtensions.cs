using System.CommandLine;
using System.CommandLine.Hosting;

using Microsoft.Extensions.Hosting;

namespace TaskTitan.Cli.Commands.Admin;

internal static class AdminExtensions
{
    public static RootCommand AddAdminCommands(this RootCommand root)
    {
        root.AddCommand(new BogusCommand());

        return root;
    }

    public static IHostBuilder UseAdminCommandHandlers(this IHostBuilder builder)
    {
        builder.UseCommandHandler<BogusCommand, BogusCommand.Handler>();

        return builder;
    }
}
