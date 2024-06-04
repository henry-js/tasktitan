using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace TaskTitan.Lib.Expressions;

public class ExpressionParser : IExpressionParser
{


    public object ParseFilter(string expression)
    {
        var tokenizer = new Tokenizer(expression);
        var tokens = tokenizer.ScanTokens();
        return tokens;
    }

}

public class Tokenizer
{
    private readonly string _expression;
    private int _currentPos;
    private readonly int _maxLength;
    private readonly List<Token> _tokens = [];

    private static readonly Dictionary<string, TokenType> keywords = new()
    {
        ["status"] = TokenType.STATUS,
        ["project"] = TokenType.PROJECT,
        ["due"] = TokenType.DUE,
        ["recur"] = TokenType.RECUR,
        ["until"] = TokenType.UNTIL,
        ["limit"] = TokenType.LIMIT,
        ["wait"] = TokenType.WAIT,
        ["entry"] = TokenType.ENTRY,
        ["wait"] = TokenType.WAIT,
        ["entry"] = TokenType.ENTRY,
        ["end"] = TokenType.END,
        ["start"] = TokenType.START,
        ["scheduled"] = TokenType.SCHEDULED,
        ["modified"] = TokenType.MODIFIED,
        ["depends"] = TokenType.DEPENDS,
    };
    public Tokenizer(string expression)
    {
        _expression = expression;
        _maxLength = expression.Length;
        _currentPos = 0;
    }

    public bool HasMoreTokens => _currentPos < _maxLength;

    public List<Token> ScanTokens()
    {

        while (HasMoreTokens)
        {
            NextToken();
        }

        return _tokens;
    }
    public void NextToken()
    {
        if (!HasMoreTokens)
            return;
        switch (_expression)
        {
            case var _ when _expression.StartsWith('+'):
                _currentPos++;
                _tokens.Add(new Token(TokenType.ADDITIVE_TAG, "+"));
                ReadTag();
                break;
            case var _ when _expression.StartsWith('-'):
                _currentPos++;
                _tokens.Add(new Token(TokenType.NEGATIVE_TAG, "-"));
                ReadTag();
                break;
            case var _ when Regex.IsMatch(_expression, @"\w+:\w+"):
                ReadKeyValuePair();
                break;
            default:
                return;
        }
    }

    private void ReadKeyValuePair()
    {
        int from = _currentPos;

        while (_currentPos < _maxLength & char.IsLetterOrDigit(_expression[_currentPos]))
        {
            _currentPos++;
        }
        var key = _expression[from.._currentPos];

        if (keywords.TryGetValue(key, out var keyType))
        {
            _tokens.Add(new Token(keyType, key));
        }
        else
        {
            _tokens.Add(new Token(TokenType.UNDEFINED_KEY, key));
        }

        _tokens.Add(new Token(TokenType.COLON, ":"));
        _currentPos++;
        _tokens.Add(new Token(TokenType.ATTRIBUTE_VALUE, _expression[_currentPos..]));
        _currentPos = _maxLength;
    }

    private void ReadTag()
    {
        int from = _currentPos;

        while (_currentPos < _maxLength && char.IsLetterOrDigit(_expression[_currentPos]))
        {
            _currentPos++;
        }
        var tag = _expression[from.._currentPos];
        _tokens.Add(new Token(TokenType.TAGNAME, tag));
    }
}
