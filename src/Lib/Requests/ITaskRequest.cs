namespace TaskTitan.Lib.Services;

public interface ITaskRequest
{
    public string[] Filters { get; init; }
    public Action Operation { get; }

}
