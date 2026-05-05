using Microsoft.AspNetCore.Mvc;
using ObatAPI.Models;

namespace ObatAPI.Controllers
{
    /// <summary>
    /// ObatController menyediakan API endpoints untuk manajemen data obat
    /// Client: TubesKPL (WinForms) akan consume API ini via HttpClient
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ObatController : ControllerBase
    {
        // In-memory database (untuk demo/testing)
        // Production: gunakan database seperti SQL Server atau EF Core
        private static List<Obat> _obatDatabase = new List<Obat>
        {
            new Obat 
            { 
                Id = 1, 
                Nama = "Paracetamol", 
                Kategori = "Tablet",
                Stok = 100,
                Harga = 5000,
                ExpiredDate = DateTime.Now.AddYears(1),
                Status = "Available"
            },
            new Obat 
            { 
                Id = 2, 
                Nama = "Amoxicillin", 
                Kategori = "Tablet",
                Stok = 5,
                Harga = 10000,
                ExpiredDate = DateTime.Now.AddMonths(6),
                Status = "LowStock"
            },
            new Obat 
            { 
                Id = 3, 
                Nama = "Vitamin C", 
                Kategori = "Tablet",
                Stok = 0,
                Harga = 3000,
                ExpiredDate = DateTime.Now.AddDays(-1),
                Status = "Expired"
            }
        };

        private readonly ILogger<ObatController> _logger;

        public ObatController(ILogger<ObatController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// GET /api/obat - Ambil semua data obat
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                _logger.LogInformation("GET /api/obat - Fetch all obat");

                // Update status tiap obat sebelum return
                foreach (var obat in _obatDatabase)
                {
                    UpdateObatStatus(obat);
                }

                return Ok(_obatDatabase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/obat/{id} - Ambil obat berdasarkan ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                _logger.LogInformation($"GET /api/obat/{id}");

                var obat = _obatDatabase.FirstOrDefault(o => o.Id == id);
                if (obat == null)
                    return NotFound(new { error = "Obat not found" });

                UpdateObatStatus(obat);
                return Ok(obat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetById({id})");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// POST /api/obat - Tambah obat baru
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] Obat obat)
        {
            try
            {
                _logger.LogInformation($"POST /api/obat - Create new obat: {obat.Nama}");

                if (obat == null)
                    return BadRequest(new { error = "Obat data is required" });

                // Generate ID
                obat.Id = _obatDatabase.Any() ? _obatDatabase.Max(o => o.Id) + 1 : 1;

                UpdateObatStatus(obat);
                _obatDatabase.Add(obat);

                return CreatedAtAction(nameof(GetById), new { id = obat.Id }, obat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// PUT /api/obat/{id} - Update obat
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Obat obat)
        {
            try
            {
                _logger.LogInformation($"PUT /api/obat/{id} - Update obat");

                var existingObat = _obatDatabase.FirstOrDefault(o => o.Id == id);
                if (existingObat == null)
                    return NotFound(new { error = "Obat not found" });

                existingObat.Nama = obat.Nama;
                existingObat.Kategori = obat.Kategori;
                existingObat.Stok = obat.Stok;
                existingObat.Harga = obat.Harga;
                existingObat.ExpiredDate = obat.ExpiredDate;

                UpdateObatStatus(existingObat);

                return Ok(existingObat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Update({id})");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// DELETE /api/obat/{id} - Hapus obat
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation($"DELETE /api/obat/{id}");

                var obat = _obatDatabase.FirstOrDefault(o => o.Id == id);
                if (obat == null)
                    return NotFound(new { error = "Obat not found" });

                _obatDatabase.Remove(obat);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Delete({id})");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Helper: Update status obat berdasarkan stok dan tanggal expired
        /// </summary>
        private void UpdateObatStatus(Obat obat)
        {
            if (obat.ExpiredDate < DateTime.Now)
            {
                obat.Status = "Expired";
            }
            else if (obat.Stok <= 5)
            {
                obat.Status = "LowStock";
            }
            else
            {
                obat.Status = "Available";
            }
        }

        /// <summary>
        /// GET /api/obat/status/summary - Ambil ringkasan status
        /// </summary>
        [HttpGet("status/summary")]
        public IActionResult GetStatusSummary()
        {
            try
            {
                _logger.LogInformation("GET /api/obat/status/summary");

                foreach (var obat in _obatDatabase)
                {
                    UpdateObatStatus(obat);
                }

                var summary = new
                {
                    available = _obatDatabase.Count(o => o.Status == "Available"),
                    lowStock = _obatDatabase.Count(o => o.Status == "LowStock"),
                    expired = _obatDatabase.Count(o => o.Status == "Expired"),
                    total = _obatDatabase.Count
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStatusSummary");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}
