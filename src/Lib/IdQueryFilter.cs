using System.Text.RegularExpressions;

namespace TaskTitan.Lib.Text;

public class IdQueryFilter : ITaskQueryFilter
{
    public List<IdRange> IdRange { get; } = [];
    public List<int> SoleIds { get; } = [];

    public IdQueryFilter(string text)
    {
        var invariantCulture = CultureInfo.InvariantCulture;
        var regex = RegexPatterns.IdFilterPattern;
        var names = regex.GetGroupNames();
        MatchCollection matches = regex.Matches(text);
        foreach (Match match in matches)
        {
            if (match.Value.Contains('-'))
            {
                var split = match.Value.Split('-');
                IdRange.Add(new(Convert.ToInt32(split[0], invariantCulture), Convert.ToInt32(split[1], invariantCulture)));
            }
            else
            {
                SoleIds.Add(Convert.ToInt32(match.Value, invariantCulture));
            }
        }
    }
    public TaskFilterType Type => TaskFilterType.IdFilter;
}
