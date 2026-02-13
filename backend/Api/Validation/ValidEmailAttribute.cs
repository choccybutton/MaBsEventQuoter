using System.ComponentModel.DataAnnotations;

namespace CateringQuotes.Api.Validation;

/// <summary>
/// Custom validation attribute for email format validation.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ValidEmailAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return true; // Let Required attribute handle null/empty

        try
        {
            var valueStr = value.ToString();
            if (string.IsNullOrWhiteSpace(valueStr))
                return false;

            var addr = new System.Net.Mail.MailAddress(valueStr);
            return addr.Address == valueStr;
        }
        catch
        {
            return false;
        }
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field must be a valid email address.";
    }
}
