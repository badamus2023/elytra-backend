using Drones.src.Api.Auth.Services;
using Drones.src.Api.Auth.Validators;
using Drones.src.Api.Common;
using Drones.src.Api.Common.BackgroundServices;
using Drones.src.Api.Common.Hubs;
using Drones.src.Api.Common.Middleware;
using Drones.src.Api.Common.Services;
using Drones.src.Api.Data;
using Drones.src.Api.DeliveryPoints.Services;
using Drones.src.Api.Dispatches.Services;
using Drones.src.Api.Drones.Services;
using Drones.src.Api.Orders.Services;
using Drones.src.Api.Payments.Services;
using Drones.src.Api.Products.Services;
using Drones.src.Api.Restaurants.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;

var dotEnvPath = FindDotEnv(Directory.GetCurrentDirectory())
    ?? FindDotEnv(AppContext.BaseDirectory);
if (dotEnvPath is not null)
    LoadDotEnv(dotEnvPath);

var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "Invalid value."
                        : error.ErrorMessage)
                    .ToArray(),
                StringComparer.OrdinalIgnoreCase);

        var payload = ApiErrorMapper.FromValidationErrors(errors);
        return new BadRequestObjectResult(payload);
    };
});

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Drones API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

//Stripe
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var secretKey = configuration["Stripe:SecretKey"];
    if (string.IsNullOrWhiteSpace(secretKey))
        throw new InvalidOperationException("Stripe:SecretKey is not configured");

    return new Stripe.StripeClient(secretKey);
});

//FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

//Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DronesDb")
        ?? throw new InvalidOperationException("ConnectionStrings:DronesDb is not configured")));

//Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IDroneService, DroneService>();
builder.Services.AddScoped<IDispatchService, DispatchService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IMaintenanceLogService, MaintenanceLogService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IDeliveryPointService, DeliveryPointService>();
builder.Services.AddScoped<IRealtimeNotificationService, RealtimeNotificationService>();
builder.Services.AddScoped<IRestaurantApplicationService, RestaurantApplicationService>();
builder.Services.AddScoped<IRestaurantOwnershipService, RestaurantOwnershipService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]
        ?? throw new InvalidOperationException("Redis:ConnectionString is not configured")));
builder.Services.AddSignalR();
builder.Services.AddHostedService<DroneTelemetrySubscriber>();


//JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"];
if (string.IsNullOrWhiteSpace(secret))
    throw new InvalidOperationException("JwtSettings:Secret is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secret)),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            if (!string.IsNullOrWhiteSpace(accessToken) &&
                context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var configuredOrigins = builder.Configuration["AllowedOrigins"];
        if (string.IsNullOrWhiteSpace(configuredOrigins))
            throw new InvalidOperationException("AllowedOrigins is not configured");

        var origins = configuredOrigins.Split(
            ',',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        policy.WithOrigins(origins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseMiddleware<ApiExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization(); 
app.MapControllers();
app.MapHub<DroneHub>("/hubs/drone");
app.MapHub<NotificationHub>("/hubs/notifications");

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var retries = 10;
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch (Exception ex)
        {
            retries--;
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Database migration failed, retrying... ({retries} left). Error: {ex.Message}", retries, ex.Message);
            Thread.Sleep(3000);
        }
    }
}

app.Run();

static string? FindDotEnv(string startDirectory)
{
    var directory = new DirectoryInfo(startDirectory);
    while (directory is not null)
    {
        var candidate = Path.Combine(directory.FullName, ".env");
        if (File.Exists(candidate))
            return candidate;

        directory = directory.Parent;
    }

    return null;
}

static void LoadDotEnv(string path)
{
    if (!File.Exists(path))
        return;

    foreach (var rawLine in File.ReadLines(path))
    {
        var line = rawLine.Trim();
        if (line.Length == 0 || line.StartsWith('#'))
            continue;

        if (line.StartsWith("export ", StringComparison.Ordinal))
            line = line[7..].TrimStart();

        var separatorIndex = line.IndexOf('=');
        if (separatorIndex <= 0)
            continue;

        var key = line[..separatorIndex].Trim();
        var value = line[(separatorIndex + 1)..].Trim();

        if (value.Length >= 2 &&
            ((value.StartsWith('"') && value.EndsWith('"')) ||
             (value.StartsWith('\'') && value.EndsWith('\''))))
        {
            value = value[1..^1];
        }

        if (Environment.GetEnvironmentVariable(key) is null)
            Environment.SetEnvironmentVariable(key, value);
    }
}
