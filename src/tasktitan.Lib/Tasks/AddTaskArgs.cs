namespace TaskTitan.Lib.Tasks;

public record AddTaskArgs(
    string Description, // Required parameter
    string? Project = null,
    string? DueDate = null, // Still string - parsing happens in the service
    char? Priority = null,
    string[]? Tags = null
// Add other creation-specific fields here later, e.g.:
// string? ScheduledDate = null,
// Dictionary<string, string>? InitialUdas = null
)
{
}