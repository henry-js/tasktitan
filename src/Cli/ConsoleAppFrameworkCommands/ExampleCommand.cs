using ConsoleAppFramework;

using Microsoft.Extensions.Options;

using TaskTitan.Data.Reports;

namespace TaskTitan.Cli.ConsoleAppFrameworkCommands;

internal class ExampleCommand(IOptions<ReportDictionary> reportOptions)
{
    private readonly ReportDictionary _reportOptions = reportOptions.Value;

    public void Do([Argument] string[] input)
    {
        Console.WriteLine($"Received input: {string.Join(' ', input)}");
    }
}
