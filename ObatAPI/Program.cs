using System.Text.Json;
using System.Text.Json.Serialization;
using ObatAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Mendaftarkan fitur-fitur yang dibutuhkan program API.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Konfigurasi JSON serialization dengan property naming camelCase
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Menambahkan halaman otomatis agar API mudah dites melalui browser.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mendaftarkan services untuk dependency injection
builder.Services.AddScoped<IObatService, ObatService>();

// Mengizinkan aplikasi utama yang kita buat untuk mengambil data dari API ini.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Mengatur urutan langkah-langkah dalam memproses data masuk.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
