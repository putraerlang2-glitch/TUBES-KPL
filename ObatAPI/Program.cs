using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using ObatAPI.Data;
using ObatAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// Add services to the container
// =====================================================

// Add Controllers
builder.Services.AddControllers();

// Add Swagger untuk API documentation & testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS untuk allow WinForms client (localhost) 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// =====================================================
// Setup Entity Framework Core dengan MySQL
// =====================================================
// Connection string untuk MySQL Laragon (default: localhost, user: root, no password)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Port=3306;Database=tubes_kpl;Uid=root;Pwd=;";

// Configure DbContext untuk MySQL menggunakan Pomelo provider
builder.Services.AddDbContext<ObatDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 4, 8)), // MySQL 8.4.8 di Laragon
        mysqlOptions =>
        {
            mysqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            mysqlOptions.CommandTimeout(30);
        }
    );
});

// =====================================================
// Logging
// =====================================================
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// =====================================================
// Database Initialization & Migrations
// =====================================================
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ObatDbContext>();

        // Apply migrations automatically
        Console.WriteLine("[DB] Applying EF Core migrations...");
        dbContext.Database.Migrate();
        Console.WriteLine("[DB] Migrations applied successfully!");

        // Seed sample data if database is empty
        if (!dbContext.Obat.Any())
        {
            Console.WriteLine("[DB] Seeding sample data...");
            var sampleData = new List<Obat>
            {
                new Obat { Nama = "Paracetamol", Kategori = "Tablet", Stok = 100, Harga = 5000, ExpiredDate = DateTime.Now.AddYears(1), Status = "Available" },
                new Obat { Nama = "Ibuprofen", Kategori = "Tablet", Stok = 12, Harga = 7000, ExpiredDate = DateTime.Now.AddMonths(6), Status = "Available" },
                new Obat { Nama = "Sanmol", Kategori = "Sirup", Stok = 15, Harga = 3000, ExpiredDate = DateTime.Now.AddMonths(4), Status = "Available" },
                new Obat { Nama = "HRIG", Kategori = "Injeksi", Stok = 12, Harga = 20000, ExpiredDate = DateTime.Now.AddMonths(2), Status = "Available" },
                new Obat { Nama = "Amoxicillin", Kategori = "Tablet", Stok = 5, Harga = 10000, ExpiredDate = DateTime.Now.AddMonths(6), Status = "LowStock" },
                new Obat { Nama = "Vitamin C", Kategori = "Tablet", Stok = 0, Harga = 500, ExpiredDate = DateTime.Now.AddDays(-1), Status = "Expired" }
            };

            dbContext.Obat.AddRange(sampleData);
            dbContext.SaveChanges();
            Console.WriteLine($"[DB] {sampleData.Count} sample records seeded!");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[ERROR] Database initialization failed: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

// =====================================================
// Configure the HTTP request pipeline
// =====================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("[APP] ObatAPI starting on https://localhost:7103");
app.Run();

