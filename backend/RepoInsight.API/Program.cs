using System.Reflection;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Set up Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Log to the console
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)  // Log to a file with daily rolling
    .CreateLogger();

// Use Serilog as the logging provider
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        // Add XML comment for better documentation (optional, but recommended)
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

        // Optional: Set the title and description for the API documentation
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Repo Insight API",
            Version = "v1",
            Description = "API for analyzing GitHub repository data."
        });
    });

builder.Services.AddSingleton<GitHubService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // Enable the Swagger UI
    app.UseSwaggerUI(c =>
    {
        // Set Swagger UI to point to the Swagger JSON endpoint
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GitHub Stats API V1");
        c.RoutePrefix = "swagger"; // This will make Swagger UI available at /swagger
    });
}

// Middleware to log requests
app.UseSerilogRequestLogging(); 

// app.UseHttpsRedirection();

app.MapControllers();

app.Run();
