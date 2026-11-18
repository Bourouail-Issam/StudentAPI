using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StudentAPI;
using StudentAPI.Auth;
using StudentAPI.Authorization;
using StudentAPIBusinessLayer;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

// Create the application builder.
// This object is responsible for configuring services and middleware.
var builder = WebApplication.CreateBuilder(args);

string _connectionString = builder.Configuration["ConnectionStrings:StudentDB"]
    ?? throw new InvalidOperationException("Connection string 'StudentDB' not found.");

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<IStudent>(sp =>  new Student(_connectionString,
    sp.GetRequiredService<IPasswordHasher>()));

builder.Services.AddScoped<IUser>(sp => new User(_connectionString));

// Register the Handler in program.cs
builder.Services.AddSingleton<IAuthorizationHandler, StudentOwnerOrAdminHandler>();

builder.Services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

// =============
// RateLimiting
// =============
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthLimiter", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            });
    });
});



// ===============================
// JWT Authentication Configuration
// ===============================

var jwtKey = builder.Configuration["Jwt:Key"]
     ?? throw new InvalidOperationException("JWT not Exist."); 

// Register authentication services in the dependency injection container.
// JwtBearerDefaults.AuthenticationScheme tells ASP.NET Core that
// JWT Bearer authentication will be the default authentication method.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
        options.MapInboundClaims = false;
        options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
    });

// ===============================
// Authorization Configuration
// ===============================
// Register authorization services.
// This enables attributes like [Authorize] and role-based authorization.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOwnerOrAdmin", policy =>
        policy.Requirements.Add(new StudentOwnerOrAdminRequirement()));
});

// Add services to the container
builder.Services.AddControllers();

// ===============================
// Swagger Configuration
// ===============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    // ===============================
    // 1) Define the JWT Bearer security scheme
    // ===============================
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // The name of the HTTP header where the token will be sent.
        Name = "Authorization",

        // Indicates this is an HTTP authentication scheme.
        Type = SecuritySchemeType.Http,

        // Specifies the authentication scheme name.
        // Must be exactly "Bearer" for JWT Bearer tokens.
        Scheme = "Bearer",

        // Optional metadata to describe the token format.
        BearerFormat = "JWT",

        // Specifies that the token is sent in the request header.
        In = ParameterLocation.Header,

        // Text shown in Swagger UI to guide the user.
        Description = "Enter: Bearer {your   JWT token}"
    });

    // ===============================
    // 2) Require the Bearer scheme for secured endpoints
    // ===============================

    // This tells Swagger that endpoints protected by [Authorize]
    // require the Bearer token defined above.
    option.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("StudentApiCorsPolicy", policy =>
    {
        policy
              .WithOrigins(
                    "https://localhost:7221",
                    "http://localhost:5233"
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



var app = builder.Build();


// ===============================
// Configure HTTP Request Pipeline
// ===============================

// Enable Swagger only in development environment.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();
app.UseCors("StudentApiCorsPolicy");
app.UseRateLimiter();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
    {
        await context.Response.WriteAsync("Too many login attempts. Please try again later.");
    }
});
// IMPORTANT:
// Authentication middleware must run BEFORE authorization middleware.
// Authentication identifies the user.
// Authorization decides what the user is allowed to do.
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes (e.g., /api/Students, /api/Auth).
app.MapControllers();

// Start the application.
app.Run();
