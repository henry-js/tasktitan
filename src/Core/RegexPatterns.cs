using System.Text.RegularExpressions;

namespace TaskTitan.Core.RegularExpressions;

public static partial class RegexPatterns
{
    [GeneratedRegex(@"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase)]
    private static partial Regex AttributePatternRegex();
    public static readonly Regex AttributePattern = AttributePatternRegex();

    [GeneratedRegex("""(?<IdRange>\b\d+-\d+\b)|(?<SoleId>\b\d+\b)""")]
    private static partial Regex IdFilterPatternRegex();
    public static readonly Regex IdFilterPattern = IdFilterPatternRegex();
}
