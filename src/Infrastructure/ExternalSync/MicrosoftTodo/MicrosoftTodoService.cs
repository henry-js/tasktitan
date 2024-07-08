using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

using TaskTitan.Core.OperationResults;
using TaskTitan.Infrastructure.Dtos;

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
            return Result<string>.Failure(new Error(404, "Microsoft Graph failed to create tasklist"));
        }
        else
            return Result<string>.Success(result.Id);
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
            return Result.Failure(new Error(501, ex.Message));
            throw;
        }

        if (result is null)
        {
            _logger.LogWarning("Export task failed");
            return Result.Failure(new Error(404, "Failed to export tasktitan task to Microsoft To Do"));
        }

        return Result.Success();
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
        if (todos is not null)
            return Result<IEnumerable<TodoTask>>.Success(todos?.Value!);
        else
        {
            return Result<IEnumerable<TodoTask>>.Failure(new Error(500, $"Could not find todo list with id {id}"));
        }
    }

    public async Task<List<TaskItem>> GetTasksToExportAsync(bool all = false)
    {
        var tasks = (await _repository.GetAllAsync());
        return all ? tasks.ToList() : tasks.Where(t => t.Status != TaskItemState.Pending || t.Status != TaskItemState.Deleted).ToList();
    }
}
