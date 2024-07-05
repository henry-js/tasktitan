using System.Data;

using TaskTitan.Core.Enums;
using TaskTitan.Core.OperationResults;
using TaskTitan.Infrastructure.Expressions;

namespace TaskTitan.Infrastructure.Services;

public class TaskItemService(ITaskItemRepository repository, IExpressionParser parser, ILogger<TaskItemService> logger) : ITaskItemService
{
    private readonly ITaskItemRepository _repository = repository;
    private readonly IExpressionParser _parser = parser;
    private readonly ILogger<TaskItemService> _logger = logger;

    public async Task<int> Add(ITaskRequest request)
    {
        if (request is not TaskItemCreateRequest createRequest) throw new Exception();

        try
        {
            var result = await _repository.AddAsync(createRequest.Task);
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

    public async Task<Result> Delete(TaskItem taskToDelete)
    {
        try
        {
            await _repository.DeleteAsync(taskToDelete);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete task {TaskID} failed", taskToDelete.Id);
            return Result.Failure(new Error(1, ex.Message));
        }

        return Result.Success();
    }

    public async Task<Result<int>> Delete(ITaskRequest request)
    {
        if (request is not TaskItemDeleteRequest deleteRequest) throw new Exception();

        var filterExpressions = deleteRequest.Filters.Count() > 0 ? deleteRequest.Filters.Select(_parser.ParseFilter) : [];

        int rowsUpdated = 0;
        try
        {
            rowsUpdated = await _repository.DeleteByFilter(filterExpressions);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(new Error(500, ex.Message));
        }

        return Result<int>.Success(rowsUpdated);
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
