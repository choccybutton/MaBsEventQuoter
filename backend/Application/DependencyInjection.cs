using CateringQuotes.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CateringQuotes.Application;

/// <summary>
/// Dependency injection configuration for Application layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Pricing service
        services.AddScoped<IQuotePricingService, QuotePricingService>();

        // Quote services
        services.AddScoped<IQuoteService, QuoteService>();
        services.AddScoped<IQuoteValidationService, QuoteValidationService>();

        // Customer services
        services.AddScoped<ICustomerValidationService, CustomerValidationService>();

        // Food item services
        services.AddScoped<IFoodItemValidationService, FoodItemValidationService>();

        return services;
    }
}
