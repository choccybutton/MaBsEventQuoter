namespace CateringQuotes.Application.Exceptions;

/// <summary>
/// Exception for domain-level business rule violations.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
