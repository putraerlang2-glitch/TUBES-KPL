using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using ObatAPI.Data;
using ObatAPI.Models;
using ObatAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost", "https://localhost", "http://127.0.0.1", "https://127.0.0.1")
                     .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                     .WithHeaders("Content-Type", "Authorization");
    });
});

builder.Services.AddScoped<IObatStatusService, ObatStatusService>();
builder.Services.AddScoped<IObatSeederService, ObatSeederService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured. Set it in appsettings.json or environment variable.");
}

builder.Services.AddDbContext<ObatDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 4, 8)),
        mysqlOptions =>
        {
            mysqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            mysqlOptions.CommandTimeout(30);
        }
    );
});

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ObatDbContext>();
        var seederService = scope.ServiceProvider.GetRequiredService<IObatSeederService>();

        Console.WriteLine("[DB] Applying EF Core migrations...");
        dbContext.Database.Migrate();
        Console.WriteLine("[DB] Migrations applied successfully!");

        await seederService.SeedDatabaseAsync(dbContext);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[ERROR] Database initialization failed: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("[APP] ObatAPI starting on https://localhost:7103");
app.Run();

