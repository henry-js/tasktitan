using TaskTitan.Core.Queries;

namespace TaskTitan.Lib.Text;

public interface ITextFilterParser
{
    ITaskQueryFilter Parse(string text);
}
