using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserPreference.Api.Services;
using UserPreference.Api.Data;
using UserPreference.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Preference API", Version = "v1" });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });
});

// Configure Azure services
ConfigureAzureServices(builder.Services, builder.Configuration);

// Configure authentication
ConfigureAuthentication(builder.Services, builder.Configuration);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:5002", "http://localhost:5003")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static void ConfigureAzureServices(IServiceCollection services, IConfiguration configuration)
{
    // Azure Cosmos DB
    services.AddSingleton<ICosmosDbService>(provider =>
    {
        var connectionString = configuration["CosmosDb:ConnectionString"];
        var databaseName = configuration["CosmosDb:DatabaseName"];
        var containerName = configuration["CosmosDb:ContainerName"];
        
        return new CosmosDbService(connectionString!, databaseName!, containerName!);
    });

    // Azure Service Bus
    services.AddSingleton<IServiceBusService>(provider =>
    {
        var connectionString = configuration["ServiceBus:ConnectionString"];
        var topicName = configuration["ServiceBus:TopicName"];
        var logger = provider.GetRequiredService<ILogger<ServiceBusService>>();
        
        return new ServiceBusService(connectionString!, topicName!, logger);
    });

    // Azure App Configuration
    services.AddSingleton<IAzureAppConfigurationService>(provider =>
    {
        var connectionString = configuration["AzureAppConfiguration:ConnectionString"];
        return new AzureAppConfigurationService(connectionString!);
    });

    // User Preference Service
    services.AddScoped<IUserPreferenceService, UserPreferenceService>();
}

static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = configuration["Authentication:Authority"];
            options.Audience = configuration["Authentication:Audience"];
            options.RequireHttpsMetadata = false; // Set to true in production
            
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

    services.AddAuthorization();
}
