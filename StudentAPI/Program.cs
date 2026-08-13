using StudentAPIBusinessLayer;

var builder = WebApplication.CreateBuilder(args);


string _connectionString = builder.Configuration.GetConnectionString("StudentDB")
    ?? throw new InvalidOperationException("Connection string 'StudentDB' not found in appsettings.");

builder.Services.AddSingleton<IStudent>(sp => new Student(_connectionString));

// Add services to the container
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
