
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;

public class MicrosoftTodoService : IExternalTaskService
{
    private readonly GraphServiceClient _client;

    public MicrosoftTodoService(GraphServiceClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<TodoTaskList>> GetAsync()
    {
        var response = await _client.Me.Todo.Lists.GetAsync();

        IEnumerable<TodoTaskList> lists = Enumerable.Empty<TodoTaskList>();
        lists = response?.Value?.Where(l => l.IsOwner == true && l.IsShared == false) ?? [];
        foreach (var list in lists)
        {
            list.Tasks = (await _client.Me.Todo.Lists[list.Id].Tasks.GetAsync())?.Value ?? [];
        }
        return lists ?? [];
    }

    public async Task<IEnumerable<TodoTaskList>> GetListsAsync()
    {
        var response = await _client.Me.Todo.Lists.GetAsync();

        return response?.Value ?? [];
    }

    public async Task<IEnumerable<TodoTask>> GetTasks(string? id)
    {
        return (await _client.Me.Todo.Lists[id].Tasks.GetAsync())?.Value ?? [];
    }
}

public interface IExternalTaskService
{
    Task<IEnumerable<TodoTaskList>> GetAsync();
    Task<IEnumerable<TodoTaskList>> GetListsAsync();
    Task<IEnumerable<TodoTask>> GetTasks(string? id);
}
