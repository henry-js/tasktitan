using System.Runtime.InteropServices;

namespace TaskTitan.Lib.Text;

[StructLayout(LayoutKind.Auto)]
public record struct IdRange(int From, int To)
{
    public override string ToString() => $"BETWEEN {From} AND {To}";
}
