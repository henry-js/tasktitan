using Microsoft.Extensions.Options;

using Pidgin;

using TaskTitan.Lib.Configuration;

namespace TaskTitan.Lib.Parsing; // Or a suitable namespace

public interface IFilterParser
{
    /// <summary>
    /// Parses a filter string into a FilterExpression tree.
    /// </summary>
    /// <param name="input">The raw filter string (e.g., "project:Work and +urgent").</param>
    /// <returns>A FilterExpression representing the parsed structure.</returns>
    /// <exception cref="Pidgin.ParseException{char}">Thrown if parsing fails.</exception>
    FilterExpression ParseFilter(string input);

    // Optional: Include command parsing if needed as part of this service's responsibility
    // CommandExpression ParseCommand(string input);
}

public class PidginFilterParser : IFilterParser
{
    // Dependencies can be injected
    private readonly TimeProvider _timeProvider;
    private readonly ConfigDictionary<AttributeDefinition> _udas;

    // Inject necessary configuration and services
    public PidginFilterParser(IOptions<TaskTitanConfig> taskTitanOptions, TimeProvider timeProvider)
    {
        _timeProvider = timeProvider ?? TimeProvider.System;

        // Assuming TaskTitanConfig holds the UDA definitions loaded from configuration
        _udas = taskTitanOptions.Value?.Uda ?? new ConfigDictionary<AttributeDefinition>();

        // IMPORTANT: Configure the static ExpressionParser instance here.
        // This assumes the application lifetime allows for this static configuration.
        // If UDAs or TimeProvider could change per-request (unlikely for CLI),
        // this static approach needs rethinking (e.g., making ExpressionParser instance-based).
        ExpressionParser.SetUdas(_udas);
        ExpressionParser.SetTimeProvider(_timeProvider);
    }

    public FilterExpression ParseFilter(string input)
    {
        try
        {
            // Delegate to your existing static parser method
            return ExpressionParser.ParseFilter(input);
        }
        // Catch Pidgin's ParseException and potentially wrap it or re-throw
        // depending on desired error handling upstream.
        catch (ParseException<char> ex)
        {
            // Log the detailed error?
            // Consider throwing a custom application exception for easier handling
            // throw new FilterParseException($"Failed to parse filter: {ex.Message}. Input: '{input}'", ex);
            throw; // Re-throw for now
        }
        catch (Exception ex) // Catch unexpected errors during parsing/factory creation
        {
            // Log error
            // throw new FilterParseException($"An unexpected error occurred during filter parsing. Input: '{input}'", ex);
            throw; // Re-throw
        }
    }

    // Implement ParseCommand if included in the interface
    // public CommandExpression ParseCommand(string input)
    // {
    //     return ExpressionParser.ParseCommand(input);
    // }
}

// Optional: Custom Exception
// public class FilterParseException : Exception
// {
//     public FilterParseException(string message, Exception innerException)
//         : base(message, innerException) { }
// }