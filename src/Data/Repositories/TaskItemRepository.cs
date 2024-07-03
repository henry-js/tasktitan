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
    private readonly QueryFactory _db;
    private readonly ILogger<TaskItemRepository> _logger;

    public TaskItemRepository(QueryFactory db, ILogger<TaskItemRepository> log)
    {
        SqlMapper.AddTypeHandler(new TaskItemIdHandler());
        SqlMapper.AddTypeHandler(typeof(TaskItemAttribute), new TaskItemAttributeHandler());
        SqlMapper.AddTypeHandler(typeof(DateTime), new DateTimeHandler());
        SqlMapper.AddTypeHandler(typeof(TaskItemState), new TaskItemStateHandler());

        _db = db;
        _logger = log;
        _db.Logger = compiled => _logger.LogInformation("Compiled: {Sql}", compiled.ToString());
    }

    public async Task<int> AddAsync(TaskItem task)
    {
        string[] excludeCols = [nameof(task.Entry), nameof(task.RowId), nameof(task.Modified)];
        Dictionary<string, object> dict = [];
        foreach (var prop in task.GetType().GetProperties())
        {
            if (excludeCols.Contains(prop.Name)) continue;
            var val = prop.GetValue(task);
            if (val is null) continue;
            dict.Add(prop.Name, val);
        }

        _ = await _db.Query("tasks").InsertAsync(dict);
        return await CountPending();
    }

    private async Task<int> CountPending()
    {
        return await _db.Connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tasks WHERE tasks.status = @Status", new { Status = TaskItemState.Pending });
    }

    public async Task<int> DeleteAsync(TaskItem task)
    {
        var query = _db.Query("tasks").Where(TaskItemAttribute.Id, task.Id);

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
                _ => throw new NotImplementedException(),
            };
        }
        return await query.DeleteAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        var tasks = await _db.Query(TasksTable.TasksWithRowId).GetAsync<TaskItem>();
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
                _ => throw new NotImplementedException(),
            };
        }
        return query.GetAsync<TaskItem>();
    }

    public async Task<int> UpdateByFilter(IEnumerable<Expression> expressions, IEnumerable<KeyValuePair<string, object?>> keyValues)
    {
        var query = _db.Query("tasks_with_rowId");
        foreach (var exp in expressions)
        {
            query = exp switch
            {
                IdFilterExpression idExp => query.AddIdFilter(idExp),
                OperatorExpression opExp when opExp.Value == "Or" => query.Or(),
                _ => throw new NotImplementedException(),
            };
        }
        var taskIds = query.Select(TaskItemAttribute.Id).Get<string>();

        return await _db.Query("tasks").WhereIn("Id", taskIds).UpdateAsync(keyValues);
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
