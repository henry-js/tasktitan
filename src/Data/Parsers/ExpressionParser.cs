
using Pidgin;
using Pidgin.Expression;

using TaskTitan.Configuration;
using TaskTitan.Data.Expressions;
using TaskTitan.Data.Reports;

using static Pidgin.Parser<char>;
using static Pidgin.Parser<string>;

using static Pidgin.Parser;
using static TaskTitan.Data.Enums;

namespace TaskTitan.Data.Parsers;

public static class ExpressionParser
{
    private static DateParser _dateParser = new DateParser(TimeProvider.System);
    private static ConfigDictionary<UserDefinedAttributeConfig> _udas = [];
    private static readonly Parser<char, char> _colon
        = Token(':');
    private static readonly Parser<char, char> _dash
        = Token('-');
    private static readonly Parser<char, char> _lParen
        = Try(Char('('));
    private static readonly Parser<char, char> _rParen
        = Try(SkipWhitespaces.Then(Char(')')));
    private static readonly Parser<char, string> _string
        = Token(c => c != '\'')
            .ManyString()
            .Between(Char('\''));
    private static Parser<char, Func<Expr, Expr, Expr>> Binary(Parser<char, BinaryOperator> op)
        => op.Select<Func<Expr, Expr, Expr>>(type => (l, r) => new BinaryFilter(l, type, r));
    private static readonly Parser<char, Func<Expr, Expr, Expr>> _and
        = Binary(
            Try(OneOf(
                Try(String("and").Between(SkipWhitespaces)),
                WhitespaceString
            )).ThenReturn(BinaryOperator.And)
        );
    private static readonly Parser<char, Func<Expr, Expr, Expr>> _or
        = Binary(
            Try(String("or").Between(SkipWhitespaces)).ThenReturn(BinaryOperator.Or)
        );
    private static readonly Parser<char, ColModifier> _tagOperator
        = OneOf(
            Char('+').ThenReturn(ColModifier.Include),
            Char('-').ThenReturn(ColModifier.Exclude)
        );
    private static readonly Parser<char, string> _attributeValue
        = OneOf(
            LetterOrDigit,
            _dash,
            _colon
        ).ManyString();

    internal static readonly Parser<char, Expr> _attribute
        = Map(
            (field, _, value) => TaskAttributeFactory.Create(field, value, _dateParser, _udas),
            LetterOrDigit.Or(Token('.')).AtLeastOnceString(),
            Token(':'),
            OneOf(
            _string,
            LetterOrDigit.Or(Token('-')).Or(Token(':')).ManyString()
            )
        ).Cast<Expr>();
    internal static readonly Parser<char, Expr> _tagExpression
        = Map(
            (modifier, value) => new TaskTag(value, modifier),
            _tagOperator,
            LetterOrDigit.AtLeastOnceString()
        ).Cast<Expr>();

    private static readonly Parser<char, Expr> _filtExpr = Pidgin.Expression.ExpressionParser.Build(
        expr =>
            OneOf(
                expr.Between(_lParen, _rParen),
                _attribute,
                _tagExpression
            ),
            [
                Operator.InfixL(_or),
                Operator.InfixL(_and),
            ]
        );

    public static void SetUdas(ConfigDictionary<UserDefinedAttributeConfig> udas) => _udas = udas;

    public static void SetTimeProvider(TimeProvider timeProvider)
        => _dateParser = (timeProvider is null)
            ? new DateParser(TimeProvider.System)
            : new DateParser(timeProvider);

    public static FilterExpression ParseFilter(string input)
        => _filtExpr
            .Select(expr => new FilterExpression(expr))
            .ParseOrThrow(input);

    public static CommandExpression ParseCommand(string input)
        => OneOf(
            _attribute,
            _tagExpression
        ).SeparatedAtLeastOnce(Token(' '))
        .Select(exprs => new CommandExpression(exprs.Cast<TaskAttribute>(), input))
        .ParseOrThrow(input);
}
