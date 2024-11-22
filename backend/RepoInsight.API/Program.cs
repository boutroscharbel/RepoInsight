using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Set up Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Log to the console
    .WriteTo.File("logs/repoinsight.txt", rollingInterval: RollingInterval.Day)  // Log to a file with daily rolling
    .WriteTo.ApplicationInsights(
        builder.Configuration["ApplicationInsights:InstrumentationKey"],
        TelemetryConverter.Traces)
    .CreateLogger();

// Use Serilog as the logging provider
builder.Host.UseSerilog();

// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:InstrumentationKey"]);
builder.Services.Configure<TelemetryConfiguration>((config) =>
{
    var instrumentationKey = builder.Configuration["ApplicationInsights:InstrumentationKey"];
    if (!string.IsNullOrEmpty(instrumentationKey))
    {
        config.InstrumentationKey = instrumentationKey;
    }
});

// Read CORS settings from appsettings.json
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(corsOrigins)
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<RepoInsightDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILetterFrequencyRepository, LetterFrequencyRepository>();

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthenticatedUsersOnly", policy => policy.RequireAuthenticatedUser());
});


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

    // Add JWT Bearer authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddScoped<GitHubService>();

var app = builder.Build();

app.UseCors(); // Enable CORS

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Repo Insight API v1");
        c.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
        c.OAuthClientSecret(builder.Configuration["AzureAd:ClientSecret"]);
        c.OAuthRealm(builder.Configuration["AzureAd:TenantId"]);
        c.OAuthAppName("Repo Insight API");
        c.OAuthScopeSeparator(" ");
        c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
    });
}

// Middleware to log requests
app.UseSerilogRequestLogging();

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
