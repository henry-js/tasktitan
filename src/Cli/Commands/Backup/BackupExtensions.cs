using TaskTitan.Infrastructure.ExternalSync;

namespace TaskTitan.Cli.Commands.Backup;

internal static class BackupExtensions
{
    public static RootCommand AddBackupCommands(this RootCommand root)
    {
        var serviceOption = new Option<SupportedService>(
    aliases: ["-s", "--service"], () => SupportedService.ToDo, "Service to use. Defaults to Microsoft To Do"
);
        root.AddCommand(new ImportCommand(serviceOption));

        root.AddCommand(new ExportCommand());

        return root;
    }

    public static IHostBuilder UseBackupCommandHandlers(this IHostBuilder builder)
    {
        builder.UseCommandHandler<ImportCommand, ImportCommand.Handler>()
            .UseCommandHandler<ExportCommand, ExportCommand.Handler>();

        return builder;
    }
}
