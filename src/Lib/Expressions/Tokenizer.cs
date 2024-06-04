using System.Text.RegularExpressions;

using static TaskTitan.Lib.Expressions.TokenType;

namespace TaskTitan.Lib.Expressions;

public class Tokenizer
{
    private readonly string _expression;
    private int _currentPos;
    private readonly int _maxLength;
    private readonly List<Token> _tokens = [];

    private static readonly Dictionary<string, TokenType> keywords = new()
    {
        ["status"] = STATUS,
        ["project"] = PROJECT,
        ["due"] = DUE,
        ["recur"] = RECUR,
        ["until"] = UNTIL,
        ["limit"] = LIMIT,
        ["wait"] = WAIT,
        ["entry"] = ENTRY,
        ["wait"] = WAIT,
        ["entry"] = ENTRY,
        ["end"] = END,
        ["start"] = START,
        ["scheduled"] = SCHEDULED,
        ["modified"] = MODIFIED,
        ["depends"] = DEPENDS,
        ["or"] = OR,
        ["and"] = AND,
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
        switch (_expression[_currentPos])
        {
            case ' ':
                _currentPos++;
                break;
            case '+':
                _currentPos++;
                _tokens.Add(new Token(ADDITIVE_TAG, "+"));
                ReadTag();
                break;
            case '-':
                _currentPos++;
                _tokens.Add(new Token(NEGATIVE_TAG, "-"));
                ReadTag();
                break;
            case '(':
                _currentPos++;
                _tokens.Add(new Token(LEFT_PAREN, "("));
                break;
            case ')':
                _currentPos++;
                _tokens.Add(new Token(RIGHT_PAREN, ")"));
                break;
            case var c when char.IsLetter(c):
                ReadString();
                break;
            // case var _ when Regex.IsMatch(_expression[_currentPos..], @"^\w+:\w+"):
            //     ReadKeyValuePair();
            //     break;
            default:
                return;
        }
    }

    private void ReadString()
    {
        int start = _currentPos;
        while (_currentPos < _maxLength && char.IsLetterOrDigit(_expression[_currentPos]))
        {
            _currentPos++;
        }
        string text = _expression[start.._currentPos];
        if (!HasMoreTokens)
        {
            _tokens.Add(new Token(TokenType.Text, text));
            return;
        }
        if (Peek() == ':')
        {
            ReadKeyValuePair(text);
        }
        if (keywords.TryGetValue(text, out TokenType type))
        {
            _tokens.Add(new Token(type, text));
        }
    }

    private char Peek() => _expression[_currentPos];

    private void ReadKeyValuePair(string? key = null)
    {
        int from;
        if (key is not null)
        {
            _tokens.Add(new Token(ATTRIBUTE_KEY, key));
        }
        else
        {
            from = _currentPos;
            while (_currentPos < _maxLength && char.IsLetterOrDigit(_expression[_currentPos]))
            {
                _currentPos++;
            }
            key = _expression[from.._currentPos];
            _tokens.Add(new Token(ATTRIBUTE_KEY, key));
        }


        _tokens.Add(new Token(COLON, ":"));
        _currentPos++;
        from = _currentPos;
        while (_currentPos < _maxLength && char.IsLetterOrDigit(_expression[_currentPos]))
        {
            _currentPos++;
        }
        var value = _expression[from.._currentPos];
        _tokens.Add(new Token(ATTRIBUTE_VALUE, value));
    }

    private void ReadTag()
    {
        int from = _currentPos;

        while (_currentPos < _maxLength && char.IsLetterOrDigit(_expression[_currentPos]))
        {
            _currentPos++;
        }
        var tag = _expression[from.._currentPos];
        _tokens.Add(new Token(TAGNAME, tag));
    }
}
