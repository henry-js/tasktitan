using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Windows.Markup;

namespace TaskTitan.Lib.Expressions;

public class ExpressionParser : IExpressionParser
{
    private int _current;
    private List<Token> _tokens;

    public bool IsAtEnd => _current >= _tokens.Count;

    public Expression ParseFilter(string expression)
    {
        _current = 0;
        var tokenizer = new Tokenizer(expression);
        _tokens = tokenizer.ScanTokens();
        return ParseExpression();
    }

    private Expression ParseExpression()
    {
        if (Match(TokenType.ADDITIVE_TAG, TokenType.NEGATIVE_TAG))
        {
            return ParseTagFilter();
        }
        if (Match(TokenType.ATTRIBUTE_KEY))
        {
            return ParseAttributeFilter();
        }
        if (Match(TokenType.LEFT_PAREN))
        {
            return ParseGroupedExpression();
        }
        return null;
    }

    private GroupedExpression ParseGroupedExpression()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' before expression");
        var left = ParseExpression();
        // Advance();
        var @operator = ConsumeOneOf("Expect 'and' or 'or' operator", TokenType.AND, TokenType.OR).Value;
        var right = ParseExpression();
        Consume(TokenType.RIGHT_PAREN, "Expecte ')' after expression");
        return new GroupedExpression(left, @operator, right);
    }

    private AttributeFilter ParseAttributeFilter()
    {
        var attributeName = Peek().Value;
        Advance();
        var colon = Consume(TokenType.COLON, "Attribute key should be followed by a colon");
        var attributeValue = Consume(TokenType.ATTRIBUTE_VALUE, "Separator should be followed by a value");
        return new AttributeFilter(attributeName, attributeValue.Value);
    }

    private TagFilter ParseTagFilter()
    {
        var sign = Peek().Type == TokenType.ADDITIVE_TAG ? '+' : '-';
        Advance();
        var name = Consume(TokenType.TAGNAME, "Expect tag name after sign.").Value;
        return new TagFilter(sign, name);
    }

    private Token Consume(TokenType tokenType, string message)
    {
        return Check(tokenType) ? Advance() : throw new Exception(message);
    }

    private Token ConsumeNext(TokenType tokenType, string message)
    {
        Advance();
        return Check(tokenType) ? Advance() : throw new Exception(message);
    }
    private Token ConsumeOneOf(string message, params TokenType[] tokenType)
    {
        foreach (var type in tokenType)
        {
            if (Check(type)) return Advance();
        }
        throw new Exception(message);
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var tokenType in types)
        {
            if (Check(tokenType))
            {
                // Advance();
                return true;
            }
        }
        return false;
    }

    private Token Advance()
    {
        if (!IsAtEnd) _current++;
        return Previous();
    }

    private Token Previous() => _tokens[_current - 1];

    private bool Check(TokenType tokenType) => !IsAtEnd && Peek().Type == tokenType;

    private Token Peek() => _tokens[_current];
}
