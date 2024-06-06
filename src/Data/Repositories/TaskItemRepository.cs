using System.Data;

using Dapper;

using static TaskTitan.Data.DbConstants;
using TaskTitan.Data.DapperSqliteTypeHandlers;

namespace TaskTitan.Data.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly TaskTitanDbContext _dbContext;
    private readonly IDbConnection _connection;

    public TaskItemRepository(TaskTitanDbContext dbContext, IDbConnection dbConnection)
    {
        _dbContext = dbContext;
        _connection = dbConnection;
        SqlMapper.AddTypeHandler(new TaskItemIdHandler());
    }

    public async Task<int> AddAsync(TaskItem task)
    {
        _dbContext.Tasks.Add(task);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(TaskItem task)
    {
        return await _dbContext.Tasks.Where(t => t.Id == task.Id).ExecuteDeleteAsync();
    }

    public async Task<int> DeleteRangeAsync(IEnumerable<TaskItem> tasks)
    {
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
        var task = await _dbContext.Tasks.FindAsync(id);
        return task;
    }

    public async Task<IEnumerable<TaskItem>> GetByQueryFilter(IEnumerable<Expression> queryFilters)
    {
        string whereFilter = /* queryFilters.Any() ? "WHERE " + queryFilters.ToQueryString() : */ "";
        var sql = $"""
SELECT * FROM {TasksTable.TasksWithRowId}
{whereFilter}
""";

        var tasks = await _connection.QueryAsync<TaskItem>(sql);
        return tasks;
    }

    public async Task<int> UpdateAsync(TaskItem task)
    {
        _dbContext.Tasks.Update(task);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateRangeAsync(IEnumerable<TaskItem> tasks)
    {
        _dbContext.Tasks.UpdateRange(tasks);
        return await _dbContext.SaveChangesAsync();
    }

    // private IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
}
