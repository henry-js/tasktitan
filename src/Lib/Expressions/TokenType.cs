namespace TaskTitan.Lib.Expressions;

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
    ATTRIBUTE_KEY,
    ATTRIBUTE_VALUE,

    // Keywords
    OR, AND,
    TAGNAME,
    UNDEFINED_KEY,
    Text,
    NUMBER,
    DASH,
    RANGE,
}
