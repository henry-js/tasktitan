using System.CommandLine;
using System.CommandLine.Hosting;

using Microsoft.Extensions.Hosting;

using TaskTitan.Infrastructure.ExternalSync;

namespace TaskTitan.Cli.Commands.Backup;

internal static class BackupExtensions
{
    public static RootCommand AddBackupCommands(this RootCommand root)
    {
        var fromOption = new Option<SupportedService>(
    aliases: ["-f", "--from"], () => SupportedService.ToDo, "Service to use. Defaults to Microsoft To Do"
);
        root.AddCommand(new ImportCommand(fromOption));
        root.AddCommand(new ExportCommand(fromOption));

        return root;
    }

    public static IHostBuilder UseBackupCommandHandlers(this IHostBuilder builder)
    {
        builder.UseCommandHandler<ImportCommand, ImportCommand.Handler>()
            .UseCommandHandler<ExportCommand, ExportCommand.Handler>();

        return builder;
    }
}
