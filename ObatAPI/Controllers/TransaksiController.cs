using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
                return BadRequest(new { error = "Data transaksi atau rincian kosong" });

            // Log nilai yang diterima untuk debugging
            _logger.LogInformation("[DEBUG API] Received Transaction:");
            _logger.LogInformation($"[DEBUG API] NoStruk: {transaksi.NoStruk}");
            _logger.LogInformation($"[DEBUG API] Subtotal: {transaksi.Subtotal}");
            _logger.LogInformation($"[DEBUG API] PersentaseDiskon: {transaksi.PersentaseDiskon}");
            _logger.LogInformation($"[DEBUG API] NominalDiskon: {transaksi.NominalDiskon}");
            _logger.LogInformation($"[DEBUG API] PersentasePajak: {transaksi.PersentasePajak}");
            _logger.LogInformation($"[DEBUG API] NominalPajak: {transaksi.NominalPajak}");
            _logger.LogInformation($"[DEBUG API] TotalAkhir: {transaksi.TotalAkhir}");
            _logger.LogInformation($"[DEBUG API] UangBayar: {transaksi.UangBayar}");
            _logger.LogInformation($"[DEBUG API] UangKembalian: {transaksi.UangKembalian}");

            var strategy = _dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var dbTransaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    transaksi.CreatedAt = DateTime.Now;

                    _dbContext.Transaksi.Add(transaksi);
                    await _dbContext.SaveChangesAsync();

                    foreach (var detail in transaksi.DetailList)
                    {
                        if (detail.Jumlah <= 0)
                        {
                            await dbTransaction.RollbackAsync();
                            return BadRequest(new { error = "Invalid quantity", details = $"Jumlah must be greater than 0 for ObatId {detail.ObatId}" });
                        }

                        if (detail.HargaSatuan < 0)
                        {
                            await dbTransaction.RollbackAsync();
                            return BadRequest(new { error = "Invalid price", details = $"HargaSatuan cannot be negative for ObatId {detail.ObatId}" });
                        }

                        detail.TransaksiId = transaksi.TransaksiId;
                        
                        var obat = await _dbContext.Obat.FindAsync(detail.ObatId);
                        if (obat != null)
                        {
                            if (obat.Stok < detail.Jumlah)
                            {
                                await dbTransaction.RollbackAsync();
                                return BadRequest(new { error = $"Stok obat {obat.Nama} tidak mencukupi", details = $"Available: {obat.Stok}, Requested: {detail.Jumlah}" });
                            }

                            obat.Stok -= detail.Jumlah;
                            obat.UpdatedAt = DateTime.Now;
                            _dbContext.Obat.Update(obat);
                        }
                        else
                        {
                            await dbTransaction.RollbackAsync();
                            return BadRequest(new { error = $"Obat dengan ID {detail.ObatId} tidak ditemukan", details = "Please verify the medicine ID" });
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();

                    return Ok(transaksi);
                }
                catch (Exception ex)
                {
                    await dbTransaction.RollbackAsync();
                    _logger.LogError(ex, "Error saat memproses transaksi");
                    return StatusCode(500, new { error = "Terjadi kesalahan di server" });
                }
            });
        }
    }
}
