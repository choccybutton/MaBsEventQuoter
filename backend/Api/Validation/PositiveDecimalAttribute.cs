using System.ComponentModel.DataAnnotations;

namespace CateringQuotes.Api.Validation;

/// <summary>
/// Custom validation attribute for ensuring decimal values are positive (> 0).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class PositiveDecimalAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
            return true; // Let Required attribute handle null

        if (decimal.TryParse(value.ToString(), out var decimalValue))
        {
            return decimalValue > 0;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field must be a positive decimal value greater than 0.";
    }
}
