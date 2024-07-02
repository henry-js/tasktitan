using Microsoft.Graph;

namespace TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;

public class TodoItemRepository : IMicrosoftTodoItemRepository
{
    public TodoItemRepository(GraphServiceClient client)
    {

    }
    public Task<int> AddAsync(TaskItem task)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(TaskItem task)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteByFilter(IEnumerable<Expression> filterExpressions)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TaskItem>> GetByFilterAsync(IEnumerable<Expression> expressions)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateByFilter(IEnumerable<Expression> expressions, IEnumerable<KeyValuePair<string, object?>> keyValues)
    {
        throw new NotImplementedException();
    }
}

public interface IMicrosoftTodoItemRepository
{
}
