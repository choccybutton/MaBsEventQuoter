using Serilog;
using CateringQuotes.Application;
using CateringQuotes.Infrastructure;
using CateringQuotes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(
            "logs/app-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "CateringQuotes.Api")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);
});

// Add services
builder.Services.AddControllers();

// Application services
builder.Services.AddApplication();

// Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Catering Quotes API",
        Version = "1.0.0",
        Description = "API for managing catering quotes"
    });

    var xmlFile = Path.Combine(AppContext.BaseDirectory, "CateringQuotes.Api.xml");
    if (File.Exists(xmlFile))
    {
        options.IncludeXmlComments(xmlFile);
    }
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:3000" };

        policy
            .WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("X-Total-Count");
    });
});

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Apply migrations and seed database (in Development only)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<CateringQuotesDbContext>();
        await dbContext.Database.MigrateAsync();

        var seeder = new DatabaseSeeder(dbContext);
        await seeder.SeedAsync();
    }
}

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Catering Quotes API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapHealthChecks("/health");
app.MapControllers();

// Map minimal endpoints for health
app.MapGet("/", () => Results.Ok(new { message = "Catering Quotes API is running" }))
    .WithName("GetRoot")
    .Produces(200)
    .WithOpenApi();

// Development-only endpoints
if (app.Environment.IsDevelopment())
{
    app.MapPost("/api/system/reset-db", async (IServiceProvider services, bool seedDemo = false) =>
    {
        var dbContext = services.GetRequiredService<CateringQuotesDbContext>();
        var seeder = new DatabaseSeeder(dbContext);
        await seeder.ResetAndSeedAsync(seedDemo);
        return Results.Ok(new { message = "Database reset complete" });
    })
    .WithName("ResetDatabase")
    .WithOpenApi()
    .Produces(200)
    .Produces(403);
}

app.Run();
