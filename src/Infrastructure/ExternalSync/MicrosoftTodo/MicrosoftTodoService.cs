using FluentResults;

using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;

public class MicrosoftTodoService : IExternalTaskService
{
    private readonly ILogger<MicrosoftTodoService> _logger;
    private readonly GraphServiceClient _client;
    private readonly ITaskItemRepository _repository;
    public const string DisplayName = "TaskTitan tasks";

    public MicrosoftTodoService(ILogger<MicrosoftTodoService> logger, GraphServiceClient client, ITaskItemRepository repository)
    {
        _logger = logger;
        _client = client;
        _repository = repository;
    }

    public async Task<Result<string>> CreateNewListAsync()
    {
        _logger.LogInformation("Creating new TodoTaskList in Microsoft Todo");
        var requestBody = new TodoTaskList
        {
            DisplayName = "TaskTitan tasks",
            IsOwner = true,
        };

        var result = await _client.Me.Todo.Lists.PostAsync(requestBody);

        if (result is null || result.Id is null)
        {
            _logger.LogError("Failed to create list");
            return Result.Fail(new Error("Microsoft Graph failed to create tasklist"));
        }
        else
            return Result.Ok(result.Id);
    }

    public async Task<Result> ExportTaskAsync(string listId, TaskItem task)
    {
        _logger.LogInformation("Exporting task {TaskItemId} to list {TodoListId}", task.Id, listId);
        var requestBody = new TodoTask
        {
            Title = task.Description,
            DueDateTime = task.Due?.Value.ToDateTimeTimeZone(),
            CompletedDateTime = task.End?.Value.ToDateTimeTimeZone(),
            // TODO: Add TaskItem.Notes property
            // Body = task?.Notes,
            LinkedResources = [
                new LinkedResource
                {
                    ApplicationName = "TaskTitan",
                    DisplayName = $"TaskItemId : {task.Id}",
                    ExternalId = task.Id.ToString(),
                }
            ]
        };
        TodoTask? result;
        try
        {
            result = await _client.Me.Todo.Lists[listId].Tasks.PostAsync(requestBody);
        }
        catch (ODataError ex)
        {
            _logger.LogError(ex, "Failed to export tasktitan task to Microsoft To Do");
            return Result.Fail(new Error(ex.Message));
            throw;
        }

        if (result is null)
        {
            _logger.LogWarning("Export task failed");
            return Result.Fail(new Error("Failed to export tasktitan task to Microsoft To Do"));
        }

        return Result.Ok();
    }

    public async Task<IEnumerable<TodoTaskList>> GetAsync()
    {
        var response = await _client.Me.Todo.Lists.GetAsync();

        IEnumerable<TodoTaskList> lists = Enumerable.Empty<TodoTaskList>();
        lists = response?.Value?.Where(l => l.IsOwner == true && l.IsShared == false) ?? [];
        foreach (var list in lists)
        {
            list.Tasks = (await _client.Me.Todo.Lists[list.Id].Tasks.GetAsync())?.Value ?? [];
        }
        return lists ?? [];
    }

    public async Task<string?> GetListIdAsync()
    {
        _logger.LogInformation("Searching Microsoft To Do for valid TaskTitan list");
        var lists = await _client.Me.Todo.Lists.GetAsync();
        var taskTitanList = lists?.Value?.SingleOrDefault(l => l.DisplayName == DisplayName || l.DisplayName?.Contains("tasktitan") == true);

        if (taskTitanList is not null) return taskTitanList.Id;

        _logger.LogWarning("Could not find valid list");
        return null;
    }

    public async Task<IEnumerable<TodoTaskList>> GetListsAsync()
    {
        var response = await _client.Me.Todo.Lists.GetAsync();

        return response?.Value ?? [];
    }

    public async Task<Result<IEnumerable<TodoTask>>> FetchExistingExportedAsync(string? id)
    {
        // TODO: Convert to ExternalTaskDtos before returning
        var todos = await _client.Me.Todo.Lists[id].Tasks.GetAsync();
        return todos is not null
            ? Result.Ok(todos?.Value!.AsEnumerable() ?? [])
            : Result.Fail(new Error($"Could not find todo list with id {id}"));
    }

    public async Task<List<TaskItem>> GetTasksToExportAsync(bool all = false)
    {
        var tasks = await _repository.GetAllAsync();
        return all ? tasks.ToList() : tasks.Where(t => t.Status != TaskItemState.Pending || t.Status != TaskItemState.Deleted).ToList();
    }
}
