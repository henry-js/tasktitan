namespace TaskTitan.Lib.Tasks;

public record struct AddTaskArgs(
    string Description, // Required parameter
    string? Status = "pending",
    string? Project = null,
    string? DueDate = null, // Still string - parsing happens in the service
    string[]? Tags = null
// Add other creation-specific fields here later, e.g.:
// string? ScheduledDate = null,
// Dictionary<string, string>? InitialUdas = null
);