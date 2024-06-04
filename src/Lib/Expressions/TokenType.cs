public enum TokenType
{
    LEFT_PAREN,
    RIGHT_PAREN,
    COLON,
    ADDITIVE_TAG,
    NEGATIVE_TAG,

    // Attributes
    STATUS, PROJECT, DUE, RECUR, UNTIL, LIMIT, WAIT,
    ENTRY, END, START, SCHEDULED, MODIFIED, DEPENDS,

    // Keywords
    OR, AND,
    TAGNAME,
    UNDEFINED_KEY,
    ATTRIBUTE_VALUE,

}

public record struct Token(TokenType Type, string Value);
