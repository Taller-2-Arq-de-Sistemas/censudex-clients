using DotNetEnv;
using censudex_clients_service.src.Data;
using Microsoft.EntityFrameworkCore;
using censudex_clients_service.src.Repositories;
using censudex_clients_service.src.Extensions;

// Load environment variables from .env file
Env.Load();

// Retrieve database connection string from environment variables
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? throw new InvalidOperationException("No DB_CONNECTION_STRING variable found.");

/// <summary>
/// Main entry point for the Censudex Clients Service application.
/// </summary>
/// <remarks>
/// This application provides RESTful API endpoints for managing client data
/// in the Censudex system. It includes features for client registration,
/// retrieval, updating, and soft deletion with proper validation and security.
/// </remarks>
var builder = WebApplication.CreateBuilder(args);

// Add services to the dependency injection container

/// <summary>
/// Configures the Entity Framework DbContext with PostgreSQL provider.
/// </summary>
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseNpgsql(connectionString)); 

/// <summary>
/// Registers the client repository for dependency injection.
/// </summary>
builder.Services.AddScoped<IClientRepository, ClientRepository>();

/// <summary>
/// Adds controllers for handling HTTP API requests.
/// </summary>
builder.Services.AddControllers();

/// <summary>
/// Adds API explorer services for generating OpenAPI/Swagger documentation.
/// </summary>
builder.Services.AddEndpointsApiExplorer();

/// <summary>
/// Configures Swagger generation for API documentation.
/// </summary>
builder.Services.AddSwaggerGen();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline

/// <summary>
/// Enables Swagger middleware in development environment for API documentation and testing.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/// <summary>
/// Redirects HTTP requests to HTTPS for secure communication.
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Maps controller endpoints to the request pipeline.
/// </summary>
app.MapControllers();

/// <summary>
/// Initializes the database by applying migrations and seeding initial data.
/// </summary>
app.InitializeDatabase();

/// <summary>
/// Runs the application and starts listening for HTTP requests.
/// </summary>
app.Run();
