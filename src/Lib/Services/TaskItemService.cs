using System.Data;

using TaskTitan.Core.Enums;
using TaskTitan.Lib.Expressions;

namespace TaskTitan.Lib.Services;

public class TaskItemService(ITaskItemRepository repository, IExpressionParser parser, IStringFilterConverter<DateTime> stringConverter, ILogger<TaskItemService> logger) : ITaskItemService
{
    private readonly ITaskItemRepository _repository = repository;
    private readonly IExpressionParser _parser = parser;
    private readonly ILogger<TaskItemService> _logger = logger;

    public async Task<int> Add(ITaskRequest request)
    {
        if (request is not TaskItemCreateRequest createRequest) throw new Exception();
        var task = TaskItem.CreateNew(createRequest.NewTask.Description);
        task.Due = stringConverter.ConvertFrom(createRequest.NewTask.Due);
        task.Scheduled = stringConverter.ConvertFrom(createRequest.NewTask.Scheduled);
        task.Wait = stringConverter.ConvertFrom(createRequest.NewTask.Wait);
        task.Until = stringConverter.ConvertFrom(createRequest.NewTask.Until);
        try
        {
            var result = await _repository.AddAsync(task);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("Save failed: {exception}", ex.Message);
            return -1;
        }
    }

    private void MapAttributes(TaskItem task, Dictionary<TaskItemAttribute, string> attributes)
    {
        foreach (var attr in attributes)
        {
            if (attr.Key == TaskItemAttribute.Description)
            {
                continue;
            }

        }
    }

    public async Task Delete(int rowId)
    {
        _logger.LogInformation("deleting Task {rowid}", rowId);
        var task = (await GetTasks([])).SingleOrDefault(t => t.RowId == rowId);

        if (task is null)
        {
            _logger.LogWarning("Task not found");
            return;
        }

        // dbContext.Tasks.Remove(task);
        _logger.LogInformation("Task deleted");
    }

    public async Task Delete(TaskItem taskToDelete)
    {
        await _repository.DeleteAsync(taskToDelete);
    }

    public async Task<int> Delete(ITaskRequest request)
    {
        if (request is not TaskItemDeleteRequest deleteRequest) throw new Exception();

        var filterExpressions = deleteRequest.Filters.Count() > 0 ? deleteRequest.Filters.Select(_parser.ParseFilter) : [];
        return await _repository.DeleteByFilter(filterExpressions);
    }

    public async Task<IEnumerable<TaskItem>> GetTasks(IEnumerable<string> filters)
    {
        var expressions = filters.Any() ? filters.Select(_parser.ParseFilter) : [];
        return await _repository.GetByFilterAsync(expressions);
    }

    public async Task<int> Update(ITaskRequest request)
    {
        if (request is not TaskItemModifyRequest updateRequest) throw new Exception();
        var filterExpressions = updateRequest.Filters.Select(_parser.ParseFilter);
        return await _repository.UpdateByFilter(filterExpressions, updateRequest.Attributes);
    }
}
