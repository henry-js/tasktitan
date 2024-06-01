namespace TaskTitan.Core.Queries;

public static class QueryFilterExtensions
{
    public static string ToQueryString(this IEnumerable<ITaskQueryFilter> queryFilters)
    {
        if (queryFilters.Count() > 1 || queryFilters.Count() == 0) throw new Exception("Can't accept more than 1 query filter");

        var filter = queryFilters.First();
        return filter.ToQueryString();
    }
}
