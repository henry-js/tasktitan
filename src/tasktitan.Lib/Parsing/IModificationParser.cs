using Parlot;
using Parlot.Fluent;

using static Parlot.Fluent.Parsers;


namespace TaskTitan.Lib.Parsing; // Or appropriate namespace

public interface IModificationParser
{
    /// <summary>
    /// Parses a modification string into a sequence of modification commands.
    /// </summary>
    /// <param name="input">The raw modification string (e.g., "project:Work +urgent").</param>
    /// <returns>A list of IModificationCommand objects.</returns>
    /// <exception cref="ParseException">Or a custom exception if parsing fails.</exception>
    IReadOnlyList<IModificationCommand> Parse(string input);
}

public class ParlotModificationParser : IModificationParser
{

    private static readonly Parser<char> plus = Terms.Char('+');
    private static readonly Parser<char> minus = Terms.Char('-');
    private static readonly Parser<char> colon = Terms.Char(':');

    private static readonly Parser<char> lParen = Terms.Char('(');
    private static readonly Parser<char> rParen = Terms.Char(')');
    private static readonly Parser<TextSpan> quotedString = Terms.String(StringLiteralQuotes.SingleOrDouble);
    private static readonly Sequence<char, TextSpan> addTag = plus.And(Literals.Identifier());

    public IReadOnlyList<IModificationCommand> Parse(string input)
    {
        var addTag = plus.And(Literals.Identifier());
        var removeTag = minus.And(Literals.Identifier());

        throw new NotImplementedException();
    }
}