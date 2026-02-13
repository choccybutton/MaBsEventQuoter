namespace CateringQuotes.Application.Exceptions;

/// <summary>
/// Exception for input validation failures.
/// Contains field-level error details.
/// </summary>
public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(string message, Dictionary<string, string[]>? errors = null)
        : base(message)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
