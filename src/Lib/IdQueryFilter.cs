using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static TaskTitan.Data.DbConstants.KeyWords;
using static TaskTitan.Data.DbConstants.TasksTable;
using TaskTitan.Core.Queries;
using TaskTitan.Lib.RegularExpressions;


namespace TaskTitan.Lib.Queries;

public class IdQueryFilter : ITaskQueryFilter
{
    public IEnumerable<IdRange> IdRange { get; } = [];
    public SoleIds SoleIds { get; } = [];

    public IdQueryFilter(string text)
    {
        var invariantCulture = CultureInfo.InvariantCulture;
        var regex = RegexPatterns.IdFilterPattern;
        var names = regex.GetGroupNames();
        MatchCollection matches = regex.Matches(text);
        var idRange = new List<IdRange>();
        var soleIds = new List<int>();
        foreach (Match match in matches)
        {
            if (match.Value.Contains('-'))
            {
                var split = match.Value.Split('-');
                idRange.Add(new(Convert.ToInt32(split[0], invariantCulture), Convert.ToInt32(split[1], invariantCulture)));
            }
            else
            {
                var val = Convert.ToInt32(match.Value, invariantCulture);
                if (!soleIds.Contains(val))
                    soleIds.Add(val);
            }
        }
        IdRange = idRange.OrderBy(range => range.From);
        SoleIds.AddRange(soleIds.Order());
    }
    public TaskQueryFilterType Type => TaskQueryFilterType.IdFilter;

    public string ToQueryString()
    {
        const char separator = ' ';
        int requiredOrKeywords = IdRange.Count() + (SoleIds.Count > 0 ? 1 : 0) - 1;
        var builder = new StringBuilder();
        if (SoleIds.Count > 0)
        {
            builder.AppendJoin(separator, RowId, In, $"({SoleIds})");
            if (requiredOrKeywords > 0)
            {
                builder.Append(separator);
                builder.Append("OR ");
                requiredOrKeywords--;
            }
        }
        foreach (var range in IdRange)
        {
            builder.AppendJoin(separator, RowId, Between, range.From, And, range.To);
            if (requiredOrKeywords > 0)
            {
                builder.Append(separator);
                builder.Append("OR ");
                requiredOrKeywords--;
            }
        }
        return builder.ToString();
    }
}

public class SoleIds : List<int>
{
    public override string ToString()
    {
        return string.Join(',', this);
    }
}
