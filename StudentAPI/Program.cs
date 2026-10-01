using System.Text;
using StudentAPIBusinessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// Create the application builder.
// This object is responsible for configuring services and middleware.
var builder = WebApplication.CreateBuilder(args);

string _connectionString = builder.Configuration["ConnectionStrings:StudentDB"]
    ?? throw new InvalidOperationException("Connection string 'StudentDB' not found.");

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

builder.Services.AddSingleton<IStudent>(sp => 
            new Student(_connectionString, sp.GetRequiredService<IPasswordHasher>()));

builder.Services.AddSingleton<IUser>(sp => new User(_connectionString));

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// ===============================
// Authorization Configuration
// ===============================
// Register authorization services.
// This enables attributes like [Authorize] and role-based authorization.
builder.Services.AddAuthorization();

// Add services to the container
builder.Services.AddControllers();

// ===============================
// Swagger Configuration
// ===============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
