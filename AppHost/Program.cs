using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL database
var postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("catering_quotes");

// Backend API (using relative path)
var api = builder
    .AddProject("api", "../backend/Api/CateringQuotes.Api.csproj")
    .WithReference(postgres)
    .WithHttpEndpoint(port: 5000)
    .WithHttpsEndpoint(port: 5001)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

// Frontend (React app)
var frontend = builder
    .AddNpmApp("frontend", "../frontend")
    .WithHttpEndpoint(port: 3000)
    .WithReference(api)
    .WithEnvironment("VITE_API_URL", "http://localhost:5000")
    .WithEnvironment("VITE_APP_ENV", "development");

builder.Build().Run();
