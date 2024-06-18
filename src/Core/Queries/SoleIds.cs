namespace TaskTitan.Core.Queries;

public class SoleIds : List<int>
{
    public override string ToString()
    {
        return string.Join(',', this);
    }
}
