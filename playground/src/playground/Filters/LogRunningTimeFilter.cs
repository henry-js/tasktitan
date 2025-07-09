using System.Diagnostics;

namespace playground.Filters;

internal class LogRunningTimeFilter(ConsoleAppFilter next) : ConsoleAppFilter(next)
{
    public override async Task InvokeAsync(ConsoleAppContext context, CancellationToken cancellationToken)
    {
        var startTime = Stopwatch.GetTimestamp();
        ConsoleApp.Log($"Execute command at {DateTime.UtcNow.ToLocalTime()}"); // LocalTime for human readable time
        try
        {
            await Next.InvokeAsync(context, cancellationToken);
            ConsoleApp.Log($"Command execute successfully at {DateTime.UtcNow.ToLocalTime()}, Elapsed: " + (Stopwatch.GetElapsedTime(startTime)));
        }
        catch
        {
            ConsoleApp.Log($"Command execute failed at {DateTime.UtcNow.ToLocalTime()}, Elapsed: " + (Stopwatch.GetElapsedTime(startTime)));
            throw;
        }
    }
}