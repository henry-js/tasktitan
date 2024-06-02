namespace TaskTitan.Core.Queries;

public interface ITaskQueryFilter
{
    public TaskQueryFilterType Type { get; }

    string ToQueryString();
}
