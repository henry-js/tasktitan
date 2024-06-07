using System.Data;

using Dapper;

using static TaskTitan.Data.DbConstants;
using TaskTitan.Data.DapperSqliteTypeHandlers;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;

namespace TaskTitan.Data.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly TaskTitanDbContext _dbContext;
    private readonly IDbConnection _connection;
    private readonly ILogger<TaskItemRepository> _log;

    public TaskItemRepository(TaskTitanDbContext dbContext, IDbConnection dbConnection, ILogger<TaskItemRepository> log)
    {
        _dbContext = dbContext;
        _connection = dbConnection;
        _log = log;
        SqlMapper.AddTypeHandler(new TaskItemIdHandler());
    }

    public async Task<int> AddAsync(TaskItem task)
    {
        throw new NotImplementedException();
        _dbContext.Tasks.Add(task);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(TaskItem task)
    {
        throw new NotImplementedException();
        return await _dbContext.Tasks.Where(t => t.Id == task.Id).ExecuteDeleteAsync();
    }

    public async Task<int> DeleteRangeAsync(IEnumerable<TaskItem> tasks)
    {
        throw new NotImplementedException();
        return await _dbContext.Tasks.Where(t => tasks.Select(task => task.Id).Contains(t.Id)).ExecuteDeleteAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        var sql = $"""
SELECT * FROM {TasksTable.TasksWithRowId}
""";
        var tasks = await _connection.QueryAsync<TaskItem>(sql);
        return tasks;
    }

    public async Task<TaskItem?> GetById(TaskItemId id)
    {
        throw new NotImplementedException();
        var task = await _dbContext.Tasks.FindAsync(id);
        return task;
    }

    public async Task<IEnumerable<TaskItem>> GetByQueryFilter(IEnumerable<string> queryFilters)
    {
        string whereFilter = queryFilters.Any() ? "WHERE" : "";
        var sql = $"""
SELECT * FROM {TasksTable.TasksWithRowId}
    {whereFilter}
        {string.Join('\n', queryFilters)}
""";

        _log.LogInformation("SQL: {sql}", sql);

        var tasks = await _connection.QueryAsync<TaskItem>(sql);
        _log.LogInformation("Fetched {0} tasks", tasks.Count());
        return tasks;
    }

    public async Task<int> UpdateAsync(TaskItem task)
    {
        throw new NotImplementedException();
        _dbContext.Tasks.Update(task);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateRangeAsync(IEnumerable<TaskItem> tasks)
    {
        throw new NotImplementedException();
        _dbContext.Tasks.UpdateRange(tasks);
        return await _dbContext.SaveChangesAsync();
    }
}
