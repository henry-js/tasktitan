using System.Data;

using Dapper;

using static TaskTitan.Data.DbConstants;
using TaskTitan.Data.DapperSqliteTypeHandlers;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
using TaskTitan.Core.Enums;
using SqlKata.Compilers;
using SqlKata.Execution;
using TaskTitan.Core.Expressions;
using SqlKata.Extensions;
using SqlKata;

namespace TaskTitan.Data.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    // private readonly TaskTitanDbContext _dbContext;
    private readonly IDbConnection _connection;
    private readonly ILogger<TaskItemRepository> _log;

    public TaskItemRepository(IDbConnection dbConnection, ILogger<TaskItemRepository> log)
    {
        // _dbContext = dbContext;
        _connection = dbConnection;
        _log = log;
        SqlMapper.AddTypeHandler(new TaskItemIdHandler());
        SqlMapper.AddTypeHandler(typeof(TaskItemAttribute), new TaskItemAttributeHandler());
        SqlMapper.AddTypeHandler(typeof(DateTime), new DateTimeHandler());
        SqlMapper.AddTypeHandler(typeof(TaskItemState), new TaskItemStateHandler());
    }

    public async Task<int> AddAsync(TaskItem task)
    {
        var compiler = new SqliteCompiler();
        var db = new QueryFactory(_connection, compiler);
        return await db.Query("tasks").InsertAsync(new
        {
            task.Id,
            task.Description,
            task.Project,
            task.Status,
            task.Entry,
            task.Modified,
            task.Due,
            task.Until,
            task.Wait,
            task.Start,
            task.End,
            task.Scheduled
        });
    }

    public async Task<int> DeleteAsync(TaskItem task)
    {
        var compiler = new SqliteCompiler();
        var db = new QueryFactory(_connection, compiler);
        var query = db.Query("tasks").Where("id", task.Id);

        _log.LogDebug("Generated sql: {sql}", compiler.Compile(query).ToString());

        return await query.DeleteAsync();
    }

    public async Task<int> DeleteByFilter(IEnumerable<Expression> filterExpressions)
    {
        var compiler = new SqliteCompiler();
        var db = new QueryFactory(_connection, compiler);
        var query = db.Query(TasksTable.TasksWithRowId).Select();
        foreach (var exp in filterExpressions)
        {
            query = exp switch
            {
                IdFilterExpression idExp => query.AddIdFilter(idExp),
                AttributeFilterExpression attrExp => query.AddAttributeFilterExpression(attrExp),
            };
        }
        _log.LogDebug("Generated sql: {sql}", compiler.Compile(query).ToString());

        return await query.DeleteAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        var sql = $"""
SELECT * FROM {TasksTable.TasksWithRowId}
""";
        var tasks = await _connection.QueryAsync<TaskItem>(sql);
        return tasks;
    }

    public Task<IEnumerable<TaskItem>> GetByFilterAsync(IEnumerable<Expression> expressions)
    {
        var compiler = new SqliteCompiler();
        var db = new QueryFactory(_connection, compiler);
        var query = db.Query(TasksTable.TasksWithRowId).Select("*");
        foreach (var exp in expressions)
        {
            query = exp switch
            {
                IdFilterExpression idExp => query.AddIdFilter(idExp),
                AttributeFilterExpression attrExp => query.AddAttributeFilterExpression(attrExp),
            };
        }
        _log.LogDebug("Generated sql: {sql}", compiler.Compile(query).ToString());
        return query.GetAsync<TaskItem>();
    }

    public async Task<int> UpdateByFilter(IEnumerable<Expression> expressions, IEnumerable<KeyValuePair<string, object>> keyValues)
    {
        var compiler = new SqliteCompiler();
        var db = new QueryFactory(_connection, compiler);
        var query = db.Query("tasks");
        foreach (var exp in expressions)
        {
            query = exp switch
            {
                IdFilterExpression idExp => query.AddIdFilter(idExp),
                OperatorExpression opExp when opExp.Value == "Or" => query.Or(),
            };
        }

        _log.LogDebug("Generated sql: {sql}", compiler.Compile(query).ToString());

        return await query.UpdateAsync(keyValues);
    }
}

public static class SqlKataQueryExtensions
{
    internal static Query AddIdFilter(this Query query, IdFilterExpression expression)
    {
        foreach (var range in expression.Ranges)
        {
            query.OrWhereBetween("RowId", range.From, range.To);
        }
        if (expression.Ids.Count > 0)
            query.OrWhereIn("RowId", expression.Ids);
        return query;
    }
    internal static Query AddAttributeFilterExpression(this Query query, AttributeFilterExpression expression)
    {
        return query.Where(expression.attribute, expression.Value);
    }
}
