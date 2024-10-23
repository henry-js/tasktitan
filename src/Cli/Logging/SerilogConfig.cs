using Serilog;
using TaskTitan.Configuration;

namespace TaskTitan.Cli.Logging;

public static class SerilogConfig
{
    public static readonly LoggerConfiguration LoggerConfiguration = new LoggerConfiguration()
        .MinimumLevel.Debug()
            .WriteTo.File($@"{Global.StateDirectoryPath}\logs\app.log",
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
            rollingInterval: RollingInterval.Day
            )
            .WriteTo.Console()
            .Enrich.WithProperty("Application Name", "<APP NAME>");
}
