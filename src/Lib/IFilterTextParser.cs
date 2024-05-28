using System.Text.RegularExpressions;

namespace TaskTitan.Lib.Text;

public interface IFilterTextParser
{
    void Parse(string text);

    FilterType GetFilterType(string text);
}

public partial class FilterTextParser : IFilterTextParser
{
    public void Parse(string text)
    {
        IdRangePatternRegex().IsMatch(text);
    }


    [GeneratedRegex(@"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase)]
    private static partial Regex AttributePatternRegex();

    [GeneratedRegex(@"[\d-,]")]
    private static partial Regex IdRangePatternRegex();

    public FilterType GetFilterType(string text)
    {
        throw new NotImplementedException();
    }
}

public enum FilterType
{
    IdRangeFilter,
    AttributeFilter,

}
