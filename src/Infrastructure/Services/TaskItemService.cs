using FluentResults;

using System.Data;

using TaskTitan.Infrastructure.Expressions;

namespace TaskTitan.Infrastructure.Services;

public class TaskItemService(ITaskItemRepository repository, IExpressionParser parser, ILogger<TaskItemService> logger) : ITaskItemService
{
    private readonly ITaskItemRepository _repository = repository;
    private readonly IExpressionParser _parser = parser;
    private readonly ILogger<TaskItemService> _logger = logger;

    public async Task<Result<int>> Add(ITaskRequest request)
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

    public async Task<Result> Delete(TaskItem taskToDelete)
    {
        try
        {
            await _repository.DeleteAsync(taskToDelete);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete task {TaskID} failed", taskToDelete.Id);
            return Result.Fail(new Error(ex.Message));
        }

        return Result.Ok();
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
            return Result.Fail<int>(new Error(ex.Message));
        }

        return Result.Ok(rowsUpdated);
    }

    public async Task<Result<IEnumerable<TaskItem>>> GetTasks(IEnumerable<string> filters)
    {
        var expressions = filters.Any() ? filters.Distinct().Select(_parser.ParseFilter) : [];
        var tasks = await _repository.GetByFilterAsync(expressions);

        return Result.Ok(tasks);
    }

    public async Task<Result<int>> Update(ITaskRequest request)
    {
        if (request is not TaskItemModifyRequest updateRequest) throw new Exception();
        var filterExpressions = updateRequest.Filters.Select(_parser.ParseFilter);

        return await _repository.UpdateByFilter(filterExpressions, updateRequest.Attributes);
    }

    public async Task<Result<IEnumerable<TaskItem>>> GetTasks(ITaskRequest request)
    {
        if (request is not TaskItemGetRequest getRequest)
        {
            throw new Exception();
        }
        IEnumerable<TaskItem> tasks = Enumerable.Empty<TaskItem>();
        var expressions = getRequest.Filters.Select(_parser.ParseFilter);
        try
        {
            tasks = await _repository.GetTasks(expressions, getRequest.Fields.Select(f => f.FieldName).ToList());
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(ex.Message));
        }

        return Result.Ok(tasks);
    }
}
