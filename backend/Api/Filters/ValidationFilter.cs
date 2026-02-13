using CateringQuotes.Application.DTOs;
using CateringQuotes.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CateringQuotes.Api.Filters;

/// <summary>
/// Action filter for validating input models before they reach controller actions.
/// Validates DTOs and returns standardized validation error responses.
/// </summary>
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Only process POST and PUT requests
        if (context.HttpContext.Request.Method != "POST" &&
            context.HttpContext.Request.Method != "PUT")
            return;

        // Check if model state is valid
        if (!context.ModelState.IsValid)
        {
            // Build error dictionary from model state
            var errors = new Dictionary<string, string[]>();

            foreach (var modelState in context.ModelState.Values)
            {
                var fieldErrors = modelState.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                if (fieldErrors.Length > 0)
                {
                    // Get the property name from model state key
                    foreach (var entry in context.ModelState)
                    {
                        if (entry.Value == modelState)
                        {
                            errors[entry.Key] = fieldErrors;
                            break;
                        }
                    }
                }
            }

            // Return validation error response
            var response = new ApiErrorResponse
            {
                Message = "Validation failed",
                Errors = errors,
                StatusCode = 400,
                Timestamp = DateTime.UtcNow
            };

            context.Result = new BadRequestObjectResult(response);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}

/// <summary>
/// Extension method to register the validation filter globally.
/// </summary>
public static class ValidationFilterExtensions
{
    public static IServiceCollection AddValidationFilters(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
        });

        return services;
    }
}
