using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ObatAPI.Enums;
using ObatAPI.Models;
using ObatAPI.Services;

namespace ObatAPI.Controllers
{
    /// <summary>
    /// Controller untuk mengelola data obat dengan error handling dan validasi yang ketat
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ObatController : ControllerBase
    {
        /// <summary>
        /// Database sementara untuk menyimpan data obat saat program berjalan.
        /// Dalam production, gunakan database sesungguhnya seperti SQL Server atau PostgreSQL.
        /// </summary>
        private static readonly List<Obat> _obatDatabase = new List<Obat>
        {
            new Obat 
            { 
                Id = 1, 
                Nama = "Paracetamol", 
                Kategori = "Tablet",
                Stok = 100,
                Harga = 5000,
                ExpiredDate = DateTime.Now.AddYears(1),
                Status = ObatStatus.Available
            },
            new Obat 
            { 
                Id = 2, 
                Nama = "Amoxicillin", 
                Kategori = "Tablet",
                Stok = 5,
                Harga = 10000,
                ExpiredDate = DateTime.Now.AddMonths(6),
                Status = ObatStatus.LowStock
            },
            new Obat 
            { 
                Id = 3, 
                Nama = "Vitamin C", 
                Kategori = "Tablet",
                Stok = 0,
                Harga = 3000,
                ExpiredDate = DateTime.Now.AddDays(45),
                Status = ObatStatus.SoonToExpire
            }
        };

        private readonly ILogger<ObatController> _logger;
        private readonly IObatService _obatService;

        /// <summary>
        /// Constructor dengan dependency injection
        /// </summary>
        public ObatController(ILogger<ObatController> logger, IObatService obatService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _obatService = obatService ?? throw new ArgumentNullException(nameof(obatService));
        }

        /// <summary>
        /// GET: /api/obat
        /// Mengambil semua data obat dengan status terbaru
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<Obat>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                _logger.LogInformation("GET /api/obat - Fetching all obat");

                // Validasi database tidak null
                if (_obatDatabase == null || _obatDatabase.Count == 0)
                {
                    _logger.LogWarning("Obat database is empty");
                    return Ok(ApiResponse<List<Obat>>.SuccessResponse(
                        new List<Obat>(), 
                        "No obat found"));
                }

                // Update status untuk setiap obat sebelum dikirim
                foreach (var obat in _obatDatabase)
                {
                    _obatService.UpdateObatStatus(obat);
                }

                _logger.LogInformation($"Successfully retrieved {_obatDatabase.Count} obat records");
                return Ok(ApiResponse<List<Obat>>.SuccessResponse(_obatDatabase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll endpoint");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse("Terjadi kesalahan pada server"));
            }
        }

        /// <summary>
        /// GET: /api/obat/{id}
        /// Mengambil data satu obat berdasarkan ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Obat>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public IActionResult GetById([Range(1, int.MaxValue, ErrorMessage = "ID harus lebih besar dari 0")] int id)
        {
            try
            {
                _logger.LogInformation($"GET /api/obat/{id} - Fetching obat by ID");

                // Validasi parameter ID
                if (id <= 0)
                {
                    _logger.LogWarning($"Invalid ID parameter: {id}");
                    return BadRequest(ApiResponse<object>.ErrorResponse("ID harus lebih besar dari 0"));
                }

                // Cari obat berdasarkan ID
                var obat = _obatDatabase.FirstOrDefault(o => o.Id == id);
                if (obat == null)
                {
                    _logger.LogWarning($"Obat with ID {id} not found");
                    return NotFound(ApiResponse<object>.NotFoundResponse($"Obat dengan ID {id} tidak ditemukan"));
                }

                // Update status sebelum dikembalikan
                _obatService.UpdateObatStatus(obat);

                _logger.LogInformation($"Successfully retrieved obat with ID {id}");
                return Ok(ApiResponse<Obat>.SuccessResponse(obat));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetById({id}) endpoint");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse("Terjadi kesalahan pada server"));
            }
        }

        /// <summary>
        /// POST: /api/obat
        /// Menambahkan data obat baru
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Obat>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] Obat? obat)
        {
            try
            {
                _logger.LogInformation("POST /api/obat - Creating new obat");

                // Validasi: obat tidak boleh null
                if (obat == null)
                {
                    _logger.LogWarning("Attempted to create obat with null data");
                    return BadRequest(ApiResponse<object>.ErrorResponse("Data obat tidak boleh kosong"));
                }

                // Validasi data obat
                if (!_obatService.ValidateObatData(obat, out var errors))
                {
                    _logger.LogWarning($"Validation failed for new obat: {string.Join(", ", errors)}");
                    return BadRequest(ApiResponse<object>.ValidationErrorResponse(errors));
                }

                // Generate ID baru
                obat.Id = _obatDatabase.Any() ? _obatDatabase.Max(o => o.Id) + 1 : 1;

                // Update status
                _obatService.UpdateObatStatus(obat);

                // Simpan ke database
                _obatDatabase.Add(obat);

                _logger.LogInformation($"Successfully created obat with ID {obat.Id}: {obat.Nama}");
                return CreatedAtAction(nameof(GetById), new { id = obat.Id }, 
                    ApiResponse<Obat>.SuccessResponse(obat, "Obat berhasil ditambahkan"));
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNull error in Create");
                return BadRequest(ApiResponse<object>.ErrorResponse($"Error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create endpoint");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse("Terjadi kesalahan pada server saat membuat obat baru"));
            }
        }

        /// <summary>
        /// PUT: /api/obat/{id}
        /// Memperbarui data obat yang sudah ada
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Obat>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public IActionResult Update([Range(1, int.MaxValue, ErrorMessage = "ID harus lebih besar dari 0")] int id, 
                                   [FromBody] Obat? obat)
        {
            try
            {
                _logger.LogInformation($"PUT /api/obat/{id} - Updating obat");

                // Validasi parameter ID
                if (id <= 0)
                {
                    _logger.LogWarning($"Invalid ID parameter: {id}");
                    return BadRequest(ApiResponse<object>.ErrorResponse("ID harus lebih besar dari 0"));
                }

                // Validasi: data obat tidak boleh null
                if (obat == null)
                {
                    _logger.LogWarning($"Attempted to update obat {id} with null data");
                    return BadRequest(ApiResponse<object>.ErrorResponse("Data obat tidak boleh kosong"));
                }

                // Validasi data obat
                if (!_obatService.ValidateObatData(obat, out var errors))
                {
                    _logger.LogWarning($"Validation failed for update obat {id}");
                    return BadRequest(ApiResponse<object>.ValidationErrorResponse(errors));
                }

                // Cari obat yang akan diupdate
                var existingObat = _obatDatabase.FirstOrDefault(o => o.Id == id);
                if (existingObat == null)
                {
                    _logger.LogWarning($"Obat with ID {id} not found for update");
                    return NotFound(ApiResponse<object>.NotFoundResponse($"Obat dengan ID {id} tidak ditemukan"));
                }

                // Update property
                existingObat.Nama = obat.Nama;
                existingObat.Kategori = obat.Kategori;
                existingObat.Stok = obat.Stok;
                existingObat.Harga = obat.Harga;
                existingObat.ExpiredDate = obat.ExpiredDate;

                // Update status berdasarkan data terbaru
                _obatService.UpdateObatStatus(existingObat);

                _logger.LogInformation($"Successfully updated obat with ID {id}");
                return Ok(ApiResponse<Obat>.SuccessResponse(existingObat, "Obat berhasil diperbarui"));
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNull error in Update");
                return BadRequest(ApiResponse<object>.ErrorResponse($"Error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Update({id}) endpoint");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse("Terjadi kesalahan pada server saat memperbarui obat"));
            }
        }

        /// <summary>
        /// DELETE: /api/obat/{id}
        /// Menghapus data obat berdasarkan ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public IActionResult Delete([Range(1, int.MaxValue, ErrorMessage = "ID harus lebih besar dari 0")] int id)
        {
            try
            {
                _logger.LogInformation($"DELETE /api/obat/{id} - Deleting obat");

                // Validasi parameter ID
                if (id <= 0)
                {
                    _logger.LogWarning($"Invalid ID parameter: {id}");
                    return BadRequest(ApiResponse<object>.ErrorResponse("ID harus lebih besar dari 0"));
                }

                // Cari obat yang akan dihapus
                var obat = _obatDatabase.FirstOrDefault(o => o.Id == id);
                if (obat == null)
                {
                    _logger.LogWarning($"Obat with ID {id} not found for deletion");
                    return NotFound(ApiResponse<object>.NotFoundResponse($"Obat dengan ID {id} tidak ditemukan"));
                }

                // Hapus dari database
                _obatDatabase.Remove(obat);

                _logger.LogInformation($"Successfully deleted obat with ID {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Delete({id}) endpoint");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse("Terjadi kesalahan pada server saat menghapus obat"));
            }
        }

        /// <summary>
        /// GET: /api/obat/status/summary
        /// Mengambil ringkasan jumlah obat berdasarkan statusnya
        /// </summary>
        [HttpGet("status/summary")]
        [ProducesResponseType(typeof(ApiResponse<ObatStatusSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public IActionResult GetStatusSummary()
        {
            try
            {
                _logger.LogInformation("GET /api/obat/status/summary - Fetching status summary");

                // Validasi database
                if (_obatDatabase == null || _obatDatabase.Count == 0)
                {
                    var emptyResponse = new ObatStatusSummary
                    {
                        Available = 0,
                        LowStock = 0,
                        Expired = 0,
                        SoonToExpire = 0,
                        Total = 0
                    };

                    return Ok(ApiResponse<ObatStatusSummary>.SuccessResponse(emptyResponse, "Database kosong"));
                }

                // Update status untuk semua obat
                foreach (var obat in _obatDatabase)
                {
                    _obatService.UpdateObatStatus(obat);
                }

                // Hitung ringkasan
                var summary = new ObatStatusSummary
                {
                    Available = _obatDatabase.Count(o => o.Status == ObatStatus.Available),
                    LowStock = _obatDatabase.Count(o => o.Status == ObatStatus.LowStock),
                    Expired = _obatDatabase.Count(o => o.Status == ObatStatus.Expired),
                    SoonToExpire = _obatDatabase.Count(o => o.Status == ObatStatus.SoonToExpire),
                    Total = _obatDatabase.Count
                };

                _logger.LogInformation($"Successfully retrieved status summary. Total: {summary.Total}, Available: {summary.Available}, LowStock: {summary.LowStock}, Expired: {summary.Expired}, SoonToExpire: {summary.SoonToExpire}");
                return Ok(ApiResponse<ObatStatusSummary>.SuccessResponse(summary));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStatusSummary endpoint");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse("Terjadi kesalahan pada server saat mengambil ringkasan status"));
            }
        }

        /// <summary>
        /// GET: /api/obat/expired/list
        /// Mengambil semua obat yang telah kadaluarsa
        /// </summary>
        [HttpGet("expired/list")]
        [ProducesResponseType(typeof(ApiResponse<List<Obat>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public IActionResult GetExpiredObat()
        {
            try
            {
                _logger.LogInformation("GET /api/obat/expired/list - Fetching expired obat");

                // Update status semua obat terlebih dahulu
                foreach (var obat in _obatDatabase)
                {
                    _obatService.UpdateObatStatus(obat);
                }

                // Filter obat yang sudah expired
                var expiredObat = _obatDatabase
                    .Where(o => o.Status == ObatStatus.Expired)
                    .ToList();

                _logger.LogInformation($"Found {expiredObat.Count} expired obat");
                return Ok(ApiResponse<List<Obat>>.SuccessResponse(expiredObat, 
                    $"Ditemukan {expiredObat.Count} obat yang telah kadaluarsa"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetExpiredObat endpoint");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse("Terjadi kesalahan pada server saat mengambil obat kadaluarsa"));
            }
        }

        /// <summary>
        /// GET: /api/obat/soon-to-expire/list?days=30
        /// Mengambil obat yang akan kadaluarsa dalam periode tertentu
        /// </summary>
        [HttpGet("soon-to-expire/list")]
        [ProducesResponseType(typeof(ApiResponse<List<Obat>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public IActionResult GetSoonToExpireObat([FromQuery][Range(1, 365, ErrorMessage = "Days harus antara 1-365")] int days = 30)
        {
            try
            {
                _logger.LogInformation($"GET /api/obat/soon-to-expire/list?days={days} - Fetching soon-to-expire obat");

                // Validasi parameter days
                if (days <= 0 || days > 365)
                {
                    _logger.LogWarning($"Invalid days parameter: {days}");
                    return BadRequest(ApiResponse<object>.ErrorResponse("Days harus antara 1 hingga 365"));
                }

                // Update status semua obat terlebih dahulu
                foreach (var obat in _obatDatabase)
                {
                    _obatService.UpdateObatStatus(obat);
                }

                // Filter obat yang akan segera expired
                var soonToExpireObat = _obatDatabase
                    .Where(o => o.Status == ObatStatus.SoonToExpire || o.Status == ObatStatus.Expired)
                    .ToList();

                _logger.LogInformation($"Found {soonToExpireObat.Count} soon-to-expire obat");
                return Ok(ApiResponse<List<Obat>>.SuccessResponse(soonToExpireObat, 
                    $"Ditemukan {soonToExpireObat.Count} obat yang akan kadaluarsa dalam {days} hari"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "ArgumentError in GetSoonToExpireObat");
                return BadRequest(ApiResponse<object>.ErrorResponse($"Error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSoonToExpireObat endpoint");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse("Terjadi kesalahan pada server saat mengambil obat yang akan kadaluarsa"));
            }
        }

        /// <summary>
        /// GET: /api/obat/low-stock/list
        /// Mengambil obat dengan stok rendah
        /// </summary>
        [HttpGet("low-stock/list")]
        [ProducesResponseType(typeof(ApiResponse<List<Obat>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public IActionResult GetLowStockObat()
        {
            try
            {
                _logger.LogInformation("GET /api/obat/low-stock/list - Fetching low stock obat");

                // Update status semua obat terlebih dahulu
                foreach (var obat in _obatDatabase)
                {
                    _obatService.UpdateObatStatus(obat);
                }

                // Filter obat dengan stok rendah
                var lowStockObat = _obatDatabase
                    .Where(o => o.Status == ObatStatus.LowStock)
                    .OrderBy(o => o.Stok)
                    .ToList();

                _logger.LogInformation($"Found {lowStockObat.Count} low stock obat");
                return Ok(ApiResponse<List<Obat>>.SuccessResponse(lowStockObat, 
                    $"Ditemukan {lowStockObat.Count} obat dengan stok rendah"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetLowStockObat endpoint");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse("Terjadi kesalahan pada server saat mengambil obat stok rendah"));
            }
        }
    }
}
