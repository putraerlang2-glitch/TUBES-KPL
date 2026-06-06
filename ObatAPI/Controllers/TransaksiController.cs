using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObatAPI.Data;
using ObatAPI.Models;

namespace ObatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaksiController : ControllerBase
    {
        private readonly ObatDbContext _dbContext;
        private readonly ILogger<TransaksiController> _logger;

        public TransaksiController(ObatDbContext dbContext, ILogger<TransaksiController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaksi([FromBody] Transaksi transaksi)
        {
            if (transaksi == null || transaksi.DetailList == null || transaksi.DetailList.Count == 0)
            {
                return BadRequest(new { error = "Data transaksi atau rincian kosong" });
            }

            // Menggunakan Execution Strategy agar kompatibel dengan Auto-Retry EF Core
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var dbTransaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    _logger.LogInformation($"POST /api/transaksi - NoStruk: {transaksi.NoStruk}");

                    // 1. Set timestamp transaksi
                    transaksi.CreatedAt = DateTime.Now;

                    // 2. Simpan transaksi ke tabel transaksi
                    _dbContext.Transaksi.Add(transaksi);
                    await _dbContext.SaveChangesAsync(); // Mendapatkan transaksi.Id dari auto-increment

                    // 3. Proses DetailList dan update stok
                    foreach (var detail in transaksi.DetailList)
                    {
                        detail.TransaksiId = transaksi.Id; // Sambungkan FK
                        
                        // Ambil obat terkait dari DB untuk update stok
                        var obat = await _dbContext.Obat.FindAsync(detail.ObatId);
                        if (obat != null)
                        {
                            if (obat.Stok < detail.Jumlah)
                            {
                                await dbTransaction.RollbackAsync();
                                return BadRequest(new { error = $"Stok obat {obat.Nama} tidak mencukupi" });
                            }

                            // Kurangi stok
                            obat.Stok -= detail.Jumlah;
                            obat.UpdatedAt = DateTime.Now;
                            _dbContext.Obat.Update(obat);
                        }
                        else
                        {
                            await dbTransaction.RollbackAsync();
                            return BadRequest(new { error = $"Obat dengan ID {detail.ObatId} tidak ditemukan" });
                        }
                    }

                    // 4. Simpan rincian ke tabel transaksi_detail
                    await _dbContext.SaveChangesAsync();
                    
                    // Commit semua perubahan
                    await dbTransaction.CommitAsync();

                    _logger.LogInformation($"Transaksi berhasil disimpan dengan ID: {transaksi.Id}");
                    return Ok(transaksi);
                }
                catch (Exception ex)
                {
                    await dbTransaction.RollbackAsync();
                    _logger.LogError(ex, "Error saat memproses transaksi");
                    return StatusCode(500, new { error = "Terjadi kesalahan di server", detail = ex.Message });
                }
            });
        }
    }
}
