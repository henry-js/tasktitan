using System.Text.RegularExpressions;

namespace TaskTitan.Lib.Text;

public class IdQueryFilter : ITaskQueryFilter
{
    public List<RangePair> IdRange { get; } = [];
    public List<int> SoleIds { get; } = [];

    public IdQueryFilter(string text)
    {
        var regex = RegexPatterns.IdFilterPattern;
        var names = regex.GetGroupNames();
        MatchCollection matches = regex.Matches(text);
        foreach (Match match in matches)
        {
            if (match.Value.Contains('-'))
            {
                var split = match.Value.Split('-');
                IdRange.Add(new(Convert.ToInt32(split[0]), Convert.ToInt32(split[1])));
            }
            else
            {
                SoleIds.Add(Convert.ToInt32(match.Value));
            }
        }
    }
    public TaskFilterType Type => TaskFilterType.IdFilter;


}

public record struct RangePair(int From, int To);
