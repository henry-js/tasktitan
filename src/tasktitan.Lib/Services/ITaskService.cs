// Placeholder Interface Updates (Illustrative)
public interface ITaskService
{
    // Add: No change needed in signature
    Task AddTaskAsync(string description, string? project, string? dueDate, char? priority, string[]? tags);

    // List: Now accepts standard filters AND a filter string. Needs context awareness.
    Task ListTasksAsync(string? projectFilter, string? statusFilter, char? priorityFilter, string[]? tagFilters, string[]? sortKeys, string? rawFilterString); // Added rawFilterString

    // Done: Overload or modify to handle filter
    Task MarkTaskDoneAsync(string taskId);
    Task MarkTasksDoneAsync(string filterString); // New method for bulk

    // Modify: Overload or modify for filter-based bulk modification
    Task ModifyTaskAsync(string taskId, string? newProject, string? newDueDate, char? newPriority, string[]? addTags, string[]? removeTags);
    Task ModifyTasksAsync(string filterString, string? newProject, string? newDueDate, char? newPriority, string[]? addTags, string[]? removeTags); // New method for bulk

    // Show: No change needed
    Task ShowTaskAsync(string taskId);

    // Delete: Overload or modify for filter
    Task DeleteTaskAsync(string taskId);
    Task DeleteTasksAsync(string filterString); // New method for bulk

    // Annotate: Overload or modify for filter
    Task AnnotateTaskAsync(string taskId, string annotation);
    Task AnnotateTasksAsync(string filterString, string annotation); // New method for bulk

    // Config methods remain the same
    Task<string?> GetConfigValueAsync(string key);
    Task SetConfigValueAsync(string key, string value);
    Task ListConfigValuesAsync();

    // Context related (could be separate service, simplified here)
    // Task<string?> GetActiveContextFilterAsync(); // Service needs to know the context
}

// --- Corresponding Updates Needed in InMemoryTaskService or other implementations ---
// You would need to implement the new ...TasksAsync methods and update ListTasksAsync logic.
// The bulk methods would find all tasks matching the filterString and apply the operation.
// ListTasksAsync would combine standard filters, the rawFilterString, and potentially context.

public class TaskService : ITaskService
{
    public Task AddTaskAsync(string description, string? project, string? dueDate, char? priority, string[]? tags)
    {
        throw new NotImplementedException();
    }

    public Task AnnotateTaskAsync(string taskId, string annotation)
    {
        throw new NotImplementedException();
    }

    public Task AnnotateTasksAsync(string filterString, string annotation)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTaskAsync(string taskId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTasksAsync(string filterString)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetConfigValueAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task ListConfigValuesAsync()
    {
        throw new NotImplementedException();
    }

    public Task ListTasksAsync(string? projectFilter, string? statusFilter, char? priorityFilter, string[]? tagFilters, string[]? sortKeys, string? rawFilterString)
    {
        throw new NotImplementedException();
    }

    public Task MarkTaskDoneAsync(string taskId)
    {
        throw new NotImplementedException();
    }

    public Task MarkTasksDoneAsync(string filterString)
    {
        throw new NotImplementedException();
    }

    public Task ModifyTaskAsync(string taskId, string? newProject, string? newDueDate, char? newPriority, string[]? addTags, string[]? removeTags)
    {
        throw new NotImplementedException();
    }

    public Task ModifyTasksAsync(string filterString, string? newProject, string? newDueDate, char? newPriority, string[]? addTags, string[]? removeTags)
    {
        throw new NotImplementedException();
    }

    public Task SetConfigValueAsync(string key, string value)
    {
        throw new NotImplementedException();
    }

    public Task ShowTaskAsync(string taskId)
    {
        throw new NotImplementedException();
    }
}