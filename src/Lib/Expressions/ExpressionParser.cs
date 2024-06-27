using TaskTitan.Core.Expressions;
using TaskTitan.Core.Queries;

namespace TaskTitan.Lib.Expressions;

public class ExpressionParser : IExpressionParser
{
    private int _current;
    private List<Token> _tokens = [];

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
        if (Match(TokenType.NUMBER, TokenType.RANGE))
        {
            return ParseIdFilterExpression();
        }
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
        throw new Exception($"Parsing expression failed at: {_tokens[_current]}");
    }

    private IdFilterExpression ParseIdFilterExpression()
    {
        List<int> ids = [];
        SoleIds soleIds = [];
        List<IdRange> ranges = [];
        while (Check(TokenType.NUMBER) || Check(TokenType.RANGE))
        {
            var value = Advance();
            if (value.Type == TokenType.NUMBER)
            {
                var val = Convert.ToInt32(value.Value);
                if (!ids.Contains(val))
                    ids.Add(val);
            }
            else
            {
                var split = value.Value.Split("-");
                var from = Convert.ToInt32(split[0]);
                var to = Convert.ToInt32(split[^1]);
                ranges.Add(new IdRange(from, to));
            }
        }
        soleIds.AddRange(ids.Order());
        return new IdFilterExpression(ranges.ToArray(), soleIds);
    }

    private GroupedFilterExpression ParseGroupedExpression()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' before expression");
        var left = ParseExpression();
        // Advance();
        var @operator = ConsumeOneOf("Expect 'and' or 'or' operator", TokenType.AND, TokenType.OR).Value;
        var right = ParseExpression();
        Consume(TokenType.RIGHT_PAREN, "Expecte ')' after expression");
        return new GroupedFilterExpression(left, @operator, right);
    }

    private AttributeFilterExpression ParseAttributeFilter()
    {
        var attributeName = Peek().Value;
        Advance();
        var colon = Consume(TokenType.COLON, "Attribute key should be followed by a colon");
        var attrValue = Consume(TokenType.ATTRIBUTE_VALUE, "Separator should be followed by a value").Value;
        if (TaskItemState.Values.Contains(attrValue))
        {
            return new AttributeFilterExpression(attributeName, attrValue);
        }
        // throw new Exception($"Invalid attribute {attrValue}");
        return new AttributeFilterExpression(attributeName, attrValue);
    }

    private TagFilterExpression ParseTagFilter()
    {
        var sign = Peek().Type == TokenType.ADDITIVE_TAG ? '+' : '-';
        Advance();
        var name = Consume(TokenType.TAGNAME, "Expect tag name after sign.").Value;
        return new TagFilterExpression(sign, name);
    }

    private Token Consume(TokenType tokenType, string message)
    {
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
