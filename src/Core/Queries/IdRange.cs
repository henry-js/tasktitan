using System.Runtime.InteropServices;

namespace TaskTitan.Core.Queries;

[StructLayout(LayoutKind.Auto)]
public record struct IdRange(int From, int To)
{
    public override readonly string ToString() => $"BETWEEN {From} AND {To}";
}
