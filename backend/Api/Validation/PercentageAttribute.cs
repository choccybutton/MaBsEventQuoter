using System.ComponentModel.DataAnnotations;

namespace CateringQuotes.Api.Validation;

/// <summary>
/// Custom validation attribute for percentage values (0 to 1 inclusive).
/// Used for VAT rates, markup percentages, etc.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class PercentageAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
            return true; // Let Required attribute handle null

        if (decimal.TryParse(value.ToString(), out var decimalValue))
        {
            return decimalValue >= 0 && decimalValue <= 1;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field must be a percentage value between 0 and 1 (inclusive).";
    }
}
