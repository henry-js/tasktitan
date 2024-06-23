using System.Data;

using Dapper;

using static TaskTitan.Data.DbConstants;
using TaskTitan.Data.DapperSqliteTypeHandlers;
using Microsoft.Extensions.Logging;
using TaskTitan.Core.Enums;
using SqlKata.Execution;
using TaskTitan.Core.Expressions;
using SqlKata;

namespace TaskTitan.Data.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly IDbConnection _connection;
    private readonly QueryFactory _db;
    private readonly ILogger<TaskItemRepository> _logger;

    public TaskItemRepository(QueryFactory db, ILogger<TaskItemRepository> log)
    {
        // _dbContext = dbContext;
        _db = db;
        _logger = log;
        _db.Logger = compiled => _logger.LogInformation("Compiled: {Sql}", compiled.ToString());

        SqlMapper.AddTypeHandler(new TaskItemIdHandler());
        SqlMapper.AddTypeHandler(typeof(TaskItemAttribute), new TaskItemAttributeHandler());
        SqlMapper.AddTypeHandler(typeof(DateTime), new DateTimeHandler());
        SqlMapper.AddTypeHandler(typeof(TaskItemState), new TaskItemStateHandler());
    }

    public async Task<int> AddAsync(TaskItem task)
    {
        int updated = await _db.Query("tasks").InsertAsync(new
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

        return updated <= 1 ? 0 : (int)await _db.Query("tasks").Where(TaskItemAttribute.Status, TaskItemState.Pending).AsCount().FirstAsync();
    }

    public async Task<int> DeleteAsync(TaskItem task)
    {
        var query = _db.Query("tasks").Where("id", task.Id);

        return await query.DeleteAsync();
    }

    public async Task<int> DeleteByFilter(IEnumerable<Expression> filterExpressions)
    {
        var query = _db.Query(TasksTable.TasksWithRowId).Select();
        foreach (var exp in filterExpressions)
        {
            query = exp switch
            {
                IdFilterExpression idExp => query.AddIdFilter(idExp),
                AttributeFilterExpression attrExp => query.AddAttributeFilterExpression(attrExp),
            };
        }

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
        var query = _db.Query(TasksTable.TasksWithRowId).Select("*");
        foreach (var exp in expressions)
        {
            query = exp switch
            {
                IdFilterExpression idExp => query.AddIdFilter(idExp),
                AttributeFilterExpression attrExp => query.AddAttributeFilterExpression(attrExp),
            };
        }
        return query.GetAsync<TaskItem>();
    }

    public async Task<int> UpdateByFilter(IEnumerable<Expression> expressions, IEnumerable<KeyValuePair<string, object>> keyValues)
    {
        var query = _db.Query("tasks");
        foreach (var exp in expressions)
        {
            query = exp switch
            {
                IdFilterExpression idExp => query.AddIdFilter(idExp),
                OperatorExpression opExp when opExp.Value == "Or" => query.Or(),
            };
        }

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
