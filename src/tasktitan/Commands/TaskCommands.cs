namespace tasktitan.Commands;

public class TaskCommands
{
    private readonly ILogger<TaskCommands> _logger;
    private readonly ITaskService _taskService;

    public TaskCommands(ILogger<TaskCommands> logger, ITaskService taskService)
    {
        _logger = logger;
        _taskService = taskService;
    }

    /// <summary>
    /// Adds a new task. Uses standard options.
    /// </summary>
    /// <param name="description">The description of the task (required).</param>
    /// <param name="project">-p|--project, Assign task to a project.</param>
    /// <param name="due">-d|--due, Set a due date (e.g., 'tomorrow', 'eom', '2024-12-31').</param>
    /// <param name="priority">--priority, Set priority (H, M, L).</param>
    /// <param name="tag">-t|--tag, Add one or more tags.</param>
    public async Task Add(
        [Argument] string description,
        string? project = null,
        string? due = null,
        char? priority = null,
        string[]? tag = null)
    {
        _logger.LogDebug("Executing Add command");
        // Simple Add command remains unchanged in structure
        await _taskService.AddTaskAsync(description, project, due, priority, tag);
    }

    /// <summary>
    /// Lists tasks. Can use standard options for simple filtering OR a complex filter string.
    /// If no filters are provided, may use the active context (handled by service).
    /// Standard options and --filter are typically ANDed together if both are provided.
    /// </summary>
    /// <param name="project">-p|--project, Filter by project.</param>
    /// <param name="status">--status, Filter by status (default: pending).</param>
    /// <param name="priority">--priority, Filter by priority (H, M, L).</param>
    /// <param name="tag">-t|--tag, Filter by tag(s).</param>
    /// <param name="filter">-f|--filter, Apply a complex filter string (e.g., "project:Work and (status:pending or +urgent)"). Overrides/combines with standard options based on service logic.</param>
    /// <param name="sort">-s|--sort, Sort key(s) (e.g., 'priority-', 'due+').</param>
    public async Task List(
        string? project = null,
        string? status = "pending",
        char? priority = null,
        string[]? tag = null,
        string? filter = null, // <-- New filter option
        string[]? sort = null)
    {
        _logger.LogDebug("Executing List command with standard filters and/or --filter='{FilterString}'", filter ?? "null");
        // Pass both standard filters and the raw filter string to the service
        await _taskService.ListTasksAsync(project, status, priority, tag, sort, filter);
    }

    /// <summary>
    /// Marks one or more tasks as completed. Specify a single Task ID/UUID OR use --filter for bulk operation.
    /// </summary>
    /// <param name="taskId">The ID or UUID of the single task to mark as done. Omit if using --filter.</param>
    /// <param name="filter">-f|--filter, A filter string identifying multiple tasks to mark as done. Use instead of taskId.</param>
    public async Task Done(
        [Argument] string? taskId = null, // Now optional
        string? filter = null)
    {
        ValidateIdOrFilter(taskId, filter, nameof(Done)); // Ensure one and only one is provided

        if (taskId != null)
        {
            _logger.LogDebug("Executing Done command for Task ID: {TaskId}", taskId);
            await _taskService.MarkTaskDoneAsync(taskId);
        }
        else // filter must be non-null here due to validation
        {
            _logger.LogDebug("Executing bulk Done command for filter: {FilterString}", filter);
            await _taskService.MarkTasksDoneAsync(filter!); // Use bulk service method
        }
    }

    /// <summary>
    /// Modifies attributes of an existing task or multiple tasks based on a filter.
    /// Provide a single Task ID/UUID OR use --filter. Modification options apply accordingly.
    /// </summary>
    /// <param name="taskId">The ID or UUID of the single task to modify. Omit if using --filter.</param>
    /// <param name="filter">-f|--filter, A filter string identifying multiple tasks to modify. Use instead of taskId.</param>
    /// <param name="project">-p|--project, Change the project assignment.</param>
    /// <param name="due">-d|--due, Change the due date.</param>
    /// <param name="priority">--priority, Change the priority (H, M, L).</param>
    /// <param name="addTag">--add-tag, Tag(s) to add to the task(s).</param>
    /// <param name="removeTag">--remove-tag, Tag(s) to remove from the task(s).</param>
    public async Task Modify(
        [Argument] string? taskId = null, // Now optional
        string? filter = null,            // Filter option
        string? project = null,
        string? due = null,
        char? priority = null,
        string[]? addTag = null,
        string[]? removeTag = null)
    {
        ValidateIdOrFilter(taskId, filter, nameof(Modify));

        bool hasModifications = project != null || due != null || priority != null || addTag?.Length > 0 || removeTag?.Length > 0;
        if (!hasModifications)
        {
            // Maybe use ConsoleAppException for framework integration
            throw new ArgumentException("No modifications specified. Use options like --project, --due, --priority, --add-tag, --remove-tag.");
        }

        if (taskId != null)
        {
            _logger.LogDebug("Executing Modify command for Task ID: {TaskId}", taskId);
            await _taskService.ModifyTaskAsync(taskId, project, due, priority, addTag, removeTag);
        }
        else // filter must be non-null
        {
            _logger.LogDebug("Executing bulk Modify command for filter: {FilterString}", filter);
            await _taskService.ModifyTasksAsync(filter!, project, due, priority, addTag, removeTag); // Use bulk service method
        }
    }

    /// <summary>
    /// Shows detailed information for a specific task. Only operates on a single ID/UUID.
    /// </summary>
    /// <param name="taskId">The ID or UUID of the task to show.</param>
    public async Task Show([Argument] string taskId) // Remains ID-based only
    {
        _logger.LogDebug("Executing Show command for Task ID: {TaskId}", taskId);
        await _taskService.ShowTaskAsync(taskId);
    }

    /// <summary>
    /// Deletes one or more tasks (marks as deleted). Specify a single Task ID/UUID OR use --filter for bulk operation.
    /// </summary>
    /// <param name="taskId">The ID or UUID of the single task to delete. Omit if using --filter.</param>
    /// <param name="filter">-f|--filter, A filter string identifying multiple tasks to delete. Use instead of taskId.</param>
    public async Task Delete(
        [Argument] string? taskId = null, // Optional
        string? filter = null)
    {
        ValidateIdOrFilter(taskId, filter, nameof(Delete));

        if (taskId != null)
        {
            _logger.LogDebug("Executing Delete command for Task ID: {TaskId}", taskId);
            await _taskService.DeleteTaskAsync(taskId);
        }
        else // filter must be non-null
        {
            _logger.LogDebug("Executing bulk Delete command for filter: {FilterString}", filter);
            await _taskService.DeleteTasksAsync(filter!); // Use bulk service method
        }
    }

    /// <summary>
    /// Adds an annotation to one or more tasks. Specify a single Task ID/UUID OR use --filter for bulk operation.
    /// </summary>
    /// <param name="annotation">The annotation text to add.</param>
    /// <param name="taskId">The ID or UUID of the single task to annotate. Omit if using --filter.</param>
    /// <param name="filter">-f|--filter, A filter string identifying multiple tasks to annotate. Use instead of taskId.</param>
    public async Task Annotate(
        [Argument] string annotation,      // Annotation text is now the first argument
        string? taskId = null,             // Task ID is now optional *parameter*
        string? filter = null)             // Filter option
    {
        // We need taskId OR filter, so let's make taskId an option for clarity when filter is used
        // Alternative: Keep taskId as [Argument(1)] string? taskId = null, but might be confusing.
        // Let's rethink the signature slightly for clarity.

        // *** Revised Annotate Signature for Better Clarity ***
        // Putting taskId/filter as options might be clearer:
        // public async Task Annotate(
        //     [Argument] string annotation,
        //     [Option("id")] string? taskId = null, // Explicit ID option
        //     [Option("filter")] string? filter = null) // Explicit filter option

        // Let's stick to the original pattern for consistency for now, but acknowledge potential confusion.
        // User would type: tasktitan annotate "My Note" 123 OR tasktitan annotate "My Note" --filter "project:X"
        ValidateIdOrFilter(taskId, filter, nameof(Annotate));

        if (taskId != null)
        {
            _logger.LogDebug("Executing Annotate command for Task ID: {TaskId}", taskId);
            await _taskService.AnnotateTaskAsync(taskId, annotation);
        }
        else // filter must be non-null
        {
            _logger.LogDebug("Executing bulk Annotate command for filter: {FilterString}", filter);
            await _taskService.AnnotateTasksAsync(filter!, annotation); // Use bulk service method
        }
    }


    // --- Helper Methods ---

    /// <summary>
    /// Validates that either taskId or filter is provided, but not both.
    /// </summary>
    private void ValidateIdOrFilter(string? taskId, string? filter, string commandName)
    {
        if (string.IsNullOrWhiteSpace(taskId) && string.IsNullOrWhiteSpace(filter))
        {
            // Consider ConsoleAppException for better framework integration
            throw new ArgumentException($"Command '{commandName}' requires either a Task ID/UUID argument or the --filter option.");
        }

        if (!string.IsNullOrWhiteSpace(taskId) && !string.IsNullOrWhiteSpace(filter))
        {
            throw new ArgumentException($"Command '{commandName}' cannot accept both a Task ID/UUID argument and the --filter option simultaneously.");
        }
    }
}