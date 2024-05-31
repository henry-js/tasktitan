
using TaskTitan.Core.Queries;
using TaskTitan.Core.RegularExpressions;

namespace TaskTitan.Lib.Text;

public partial class TextFilterParser : ITextFilterParser
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
