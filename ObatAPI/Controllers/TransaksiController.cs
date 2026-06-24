using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObatAPI.Data;
using ObatAPI.Models;
using ObatAPI.Services;

namespace ObatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaksiController : ControllerBase
    {
        private readonly ObatDbContext _dbContext;
        private readonly IObatStatusService _obatStatusService;
        private readonly ILogger<TransaksiController> _logger;

        public TransaksiController(ObatDbContext dbContext, IObatStatusService obatStatusService, ILogger<TransaksiController> logger)
        {
            _dbContext = dbContext;
            _obatStatusService = obatStatusService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaksi([FromBody] Transaksi transaksi)
        {
            if (transaksi == null || transaksi.DetailList == null || transaksi.DetailList.Count == 0)
                return BadRequest(new { error = "Data transaksi atau rincian kosong" });

            var strategy = _dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var dbTransaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    // 1. Validasi User
                    var user = await _dbContext.User.FindAsync(transaksi.UserId);
                    if (user == null)
                    {
                        await dbTransaction.RollbackAsync();
                        return BadRequest(new { error = "User tidak ditemukan" });
                    }

                    // 2. Hitung dan Validasi Setiap Detail Transaksi
                    var newDetails = new List<TransaksiDetail>();
                    decimal totalSubtotal = 0;
                    foreach (var detail in transaksi.DetailList)
                    {
                        // Validasi Obat
                        var obat = await _dbContext.Obat.FindAsync(detail.ObatId);
                        if (obat == null)
                        {
                            await dbTransaction.RollbackAsync();
                            return BadRequest(new { error = $"Obat dengan ID {detail.ObatId} tidak ditemukan" });
                        }

                        // Validasi Stok
                        if (obat.Stok < detail.Jumlah)
                        {
                            await dbTransaction.RollbackAsync();
                            return BadRequest(new { error = $"Stok obat {obat.Nama} tidak mencukupi. Tersedia: {obat.Stok}, Diminta: {detail.Jumlah}" });
                        }

                        // Buat Detail Transaksi BARU tanpa ID
                        var newDetail = new TransaksiDetail
                        {
                            ObatId = detail.ObatId,
                            Jumlah = detail.Jumlah,
                            HargaSatuan = obat.Harga,
                            Subtotal = detail.Jumlah * obat.Harga
                        };
                        newDetails.Add(newDetail);
                        totalSubtotal += newDetail.Subtotal;
                    }

                    // 3. Hitung Semua Nilai Transaksi
                    var nominalDiskon = totalSubtotal * transaksi.PersentaseDiskon / 100;
                    var nilaiSetelahDiskon = totalSubtotal - nominalDiskon;
                    var nominalPajak = nilaiSetelahDiskon * transaksi.PersentasePajak / 100;
                    var totalAkhir = nilaiSetelahDiskon + nominalPajak;

                    // 4. Validasi Uang Bayar
                    if (transaksi.UangBayar < totalAkhir)
                    {
                        await dbTransaction.RollbackAsync();
                        return BadRequest(new { error = "Uang bayar tidak boleh kurang dari total akhir" });
                    }

                    // Buat Transaksi BARU tanpa ID
                    var newTransaksi = new Transaksi
                    {
                        NoStruk = transaksi.NoStruk,
                        TanggalTransaksi = transaksi.TanggalTransaksi == default ? DateTime.Now : transaksi.TanggalTransaksi,
                        Subtotal = totalSubtotal,
                        PersentaseDiskon = transaksi.PersentaseDiskon,
                        NominalDiskon = nominalDiskon,
                        PersentasePajak = transaksi.PersentasePajak,
                        NominalPajak = nominalPajak,
                        TotalAkhir = totalAkhir,
                        UangBayar = transaksi.UangBayar,
                        UangKembalian = transaksi.UangBayar - totalAkhir,
                        UserId = transaksi.UserId,
                        CreatedAt = DateTime.Now,
                        DetailList = newDetails
                    };

                    // 5. Simpan Transaksi
                    _dbContext.Transaksi.Add(newTransaksi);
                    await _dbContext.SaveChangesAsync();

                    // 6. Update Stok dan Status Obat
                    foreach (var detail in newTransaksi.DetailList)
                    {
                        var obat = await _dbContext.Obat.FindAsync(detail.ObatId);
                        if (obat != null)
                        {
                            obat.Stok -= detail.Jumlah;
                            obat.UpdatedAt = DateTime.Now;
                            _obatStatusService.EvaluateStatus(obat);
                            _dbContext.Obat.Update(obat);
                        }
                    }

                    // 7. Simpan Semua Perubahan
                    await _dbContext.SaveChangesAsync();

                    // 8. Commit Transaction
                    await dbTransaction.CommitAsync();

                    return Ok(new
                    {
                        message = "Transaksi berhasil disimpan",
                        transaksi = newTransaksi
                    });
                }
                catch (Exception ex)
                {
                    await dbTransaction.RollbackAsync();
                    _logger.LogError(ex, "Error saat memproses transaksi");
                    return StatusCode(500, new { error = "Terjadi kesalahan di server" });
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransaksi()
        {
            try
            {
                var transaksiList = await _dbContext.Transaksi
                    .Include(t => t.User)
                    .Include(t => t.DetailList)
                    .ThenInclude(d => d.Obat)
                    .OrderByDescending(t => t.TanggalTransaksi)
                    .ToListAsync();

                return Ok(transaksiList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saat mengambil data transaksi");
                return StatusCode(500, new { error = "Terjadi kesalahan di server" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaksiById(int id)
        {
            try
            {
                var transaksi = await _dbContext.Transaksi
                    .Include(t => t.User)
                    .Include(t => t.DetailList)
                    .ThenInclude(d => d.Obat)
                    .FirstOrDefaultAsync(t => t.TransaksiId == id);

                if (transaksi == null)
                    return NotFound(new { error = "Transaksi tidak ditemukan" });

                return Ok(transaksi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saat mengambil data transaksi");
                return StatusCode(500, new { error = "Terjadi kesalahan di server" });
            }
        }
    }
}
