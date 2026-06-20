using ObatAPI.Data;
using ObatAPI.Models;

namespace ObatAPI.Services
{
    public interface IObatSeederService
    {
        Task SeedDatabaseAsync(ObatDbContext dbContext);
    }

    public class ObatSeederService : IObatSeederService
    {
        private readonly ILogger<ObatSeederService> _logger;

        public ObatSeederService(ILogger<ObatSeederService> logger)
        {
            _logger = logger;
        }

        public async Task SeedDatabaseAsync(ObatDbContext dbContext)
        {
            try
            {
                if (!dbContext.Obat.Any())
                {
                    _logger.LogInformation("[DB] Seeding sample data...");

                    var sampleData = new List<Obat>
                    {
                        new Obat { Nama = "Paracetamol", Kategori = "Tablet", Stok = 100, Harga = 5000, ExpiredDate = DateTime.Now.AddYears(1), Status = ObatStatus.Available },
                        new Obat { Nama = "Ibuprofen", Kategori = "Tablet", Stok = 12, Harga = 7000, ExpiredDate = DateTime.Now.AddMonths(6), Status = ObatStatus.Available },
                        new Obat { Nama = "Sanmol", Kategori = "Sirup", Stok = 15, Harga = 3000, ExpiredDate = DateTime.Now.AddMonths(4), Status = ObatStatus.Available },
                        new Obat { Nama = "HRIG", Kategori = "Injeksi", Stok = 12, Harga = 20000, ExpiredDate = DateTime.Now.AddMonths(2), Status = ObatStatus.Available },
                        new Obat { Nama = "Amoxicillin", Kategori = "Tablet", Stok = 5, Harga = 10000, ExpiredDate = DateTime.Now.AddMonths(6), Status = ObatStatus.LowStock },
                        new Obat { Nama = "Vitamin C", Kategori = "Tablet", Stok = 0, Harga = 500, ExpiredDate = DateTime.Now.AddDays(-1), Status = ObatStatus.Expired }
                    };

                    dbContext.Obat.AddRange(sampleData);
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"[DB] {sampleData.Count} sample records seeded!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DB] Error seeding data");
                throw;
            }
        }
    }
}
