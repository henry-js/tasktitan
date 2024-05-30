using System.Data;

using Dapper;
using static TaskTitan.Data.DbConstants;
using Microsoft.Data.Sqlite;

using TaskTitan.Core.OperationResults;
using TaskTitan.Data.DapperSqliteTypeHandlers;

namespace TaskTitan.Data.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly TaskTitanDbContext _dbContext;
    private IDbConnection _connection;

    public TaskItemRepository(TaskTitanDbContext dbContext, IDbConnection dbConnection)
    {
        _dbContext = dbContext;
        _connection = dbConnection;
        SqlMapper.AddTypeHandler(new TaskItemIdHandler());
    }

    public async Task<Result> AddAsync(TaskItem task)
    {
        _dbContext.Tasks.Add(task);
        var result = await _dbContext.SaveChangesAsync();
        return result > 0 ? Result.Success() : Result.Failure(new Error(-99, "I have not been defined yet"));
    }

    public async Task<Result> DeleteAsync(TaskItem task)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> DeleteRangeAsync(IEnumerable<TaskItem> tasks)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        var sql = $"""
SELECT * FROM {TasksWithRowId}
""";
        var tasks = await _connection.QueryAsync<TaskItem>(sql);
        return tasks;
    }

    public async Task<TaskItem> GetById(TaskItemId id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TaskItem>> GetByQueryFilter(IEnumerable<ITaskQueryFilter> queryFilters)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UpdateAsync(TaskItem task)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UpdateRangeAsync(IEnumerable<TaskItem> tasks)
    {
        throw new NotImplementedException();
    }

    // private IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
}

public class RepositoryExtensions
{
}
