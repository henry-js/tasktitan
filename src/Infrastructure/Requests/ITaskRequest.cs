namespace TaskTitan.Infrastructure.Services;

public interface ITaskRequest
{
    public string[] Filters { get; init; }
    public Action Operation { get; }

}
