using System.Text;
using StudentAPIBusinessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

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
