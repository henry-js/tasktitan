using TaskTitan.Core.Queries;
using TaskTitan.Lib.Queries;

using static TaskTitan.Lib.RegularExpressions.RegexPatterns;

namespace TaskTitan.Lib.Text;

public class TextFilterParser : ITextFilterParser
{
    public ITaskQueryFilter Parse(string text)
    {
        ITaskQueryFilter filter = null!;
        switch (text)
        {
            case string input when IdFilterPattern.IsMatch(input):
                filter = new IdQueryFilter(input);
                break;
        }
        return filter;
    }
}
