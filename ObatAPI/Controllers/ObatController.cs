using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObatAPI.Data;
using ObatAPI.Models;

namespace ObatAPI.Controllers
{
    /// <summary>
    /// ObatController - Refactored untuk EF Core + MySQL Database
    /// 
    /// Architecture:
    /// - PRIMARY: MySQL Database via EF Core (persistent storage)
    /// - DUAL EVALUATION: StateMachine.EvaluateStatus() at API-side (server)
    /// - CLIENT-SIDE: TubesKPL akan juga evaluate untuk UI display
    /// 
    /// CRUD Operations:
    /// 1. Receive request dari TubesKPL
    /// 2. Save/Update to MySQL via EF Core
    /// 3. Evaluate status dengan StateMachine (server-side)
    /// 4. Return data dengan status terbaru
    /// 5. TubesKPL akan re-evaluate untuk UI consistency
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ObatController : ControllerBase
    {
        private readonly ObatDbContext _dbContext;
        private readonly ILogger<ObatController> _logger;

        public ObatController(ObatDbContext dbContext, ILogger<ObatController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/obat - Ambil semua data obat dari database
        /// 
        /// Process:
        /// 1. Query semua obat dari MySQL
        /// 2. Evaluate status untuk setiap obat (StateMachine)
        /// 3. Return dengan status terbaru
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("GET /api/obat - Fetch all obat from MySQL");

                // Query dari database
                var obatList = await _dbContext.Obat
                    .OrderBy(o => o.Nama)
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {obatList.Count} obat from database");

                // Evaluate status untuk setiap obat (server-side)
                // Ini akan di-duplikasi evaluation di client-side juga
                foreach (var obat in obatList)
                {
                    EvaluateObatStatus(obat);
                }

                return Ok(obatList);
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
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation($"GET /api/obat/{id}");

                var obat = await _dbContext.Obat.FindAsync(id);
                if (obat == null)
                    return NotFound(new { error = "Obat not found" });

                EvaluateObatStatus(obat);
                return Ok(obat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetById({id})");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// POST /api/obat - Tambah obat baru ke database
        /// 
        /// Process:
        /// 1. Validate input
        /// 2. Evaluate status dengan StateMachine
        /// 3. Save to MySQL
        /// 4. Return obat dengan status & timestamps
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Obat obat)
        {
            try
            {
                _logger.LogInformation($"POST /api/obat - Create new obat: {obat?.Nama}");

                if (obat == null || string.IsNullOrWhiteSpace(obat.Nama))
                    return BadRequest(new { error = "Obat data and Nama are required" });

                // Evaluate status sebelum save
                EvaluateObatStatus(obat);

                // Set timestamps
                obat.CreatedAt = DateTime.Now;
                obat.UpdatedAt = DateTime.Now;

                // Add to database
                _dbContext.Obat.Add(obat);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Obat '{obat.Nama}' created with ID {obat.Id} and status '{obat.Status}'");

                return CreatedAtAction(nameof(GetById), new { id = obat.Id }, obat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/obat/{id} - Update obat di database
        /// 
        /// Process:
        /// 1. Find obat by ID
        /// 2. Update fields
        /// 3. Re-evaluate status dengan StateMachine (very important!)
        /// 4. Save to MySQL
        /// 5. Return updated obat
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Obat obat)
        {
            try
            {
                _logger.LogInformation($"PUT /api/obat/{id} - Update obat");

                if (obat == null)
                    return BadRequest(new { error = "Obat data is required" });

                var existingObat = await _dbContext.Obat.FindAsync(id);
                if (existingObat == null)
                    return NotFound(new { error = "Obat not found" });

                // Update fields
                existingObat.Nama = obat.Nama;
                existingObat.Kategori = obat.Kategori;
                existingObat.Stok = obat.Stok;
                existingObat.Harga = obat.Harga;
                existingObat.ExpiredDate = obat.ExpiredDate;
                existingObat.UpdatedAt = DateTime.Now;

                // Re-evaluate status (CRITICAL - status mungkin berubah saat update)
                EvaluateObatStatus(existingObat);

                _logger.LogInformation($"Obat '{existingObat.Nama}' updated, status: {existingObat.Status}");

                // Update database
                _dbContext.Obat.Update(existingObat);
                await _dbContext.SaveChangesAsync();

                return Ok(existingObat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Update({id})");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/obat/{id} - Hapus obat dari database
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation($"DELETE /api/obat/{id}");

                var obat = await _dbContext.Obat.FindAsync(id);
                if (obat == null)
                    return NotFound(new { error = "Obat not found" });

                _dbContext.Obat.Remove(obat);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Obat '{obat.Nama}' deleted");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Delete({id})");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// GET /api/obat/status/summary - Ambil ringkasan status menggunakan Table-Driven Dictionary
        /// 
        /// Pure Table-Driven Aggregation:
        /// - Query semua obat dari database
        /// - Evaluate status untuk setiap obat
        /// - Use Dictionary Counter Table untuk aggregation (bukan LINQ Count)
        /// - Return summary dengan counts per status
        /// </summary>
        [HttpGet("status/summary")]
        public async Task<IActionResult> GetStatusSummary()
        {
            try
            {
                _logger.LogInformation("GET /api/obat/status/summary");

                // Query semua obat dari database
                var obatList = await _dbContext.Obat.ToListAsync();

                // Evaluate status untuk setiap obat sebelum aggregation
                foreach (var obat in obatList)
                {
                    EvaluateObatStatus(obat);
                }

                // Table-Driven: Dictionary Counter Table untuk aggregation
                var counterTable = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Available", 0 },
                    { "LowStock", 0 },
                    { "Expired", 0 }
                };

                // Iterasi dan increment counter tanpa conditional chain
                foreach (var obat in obatList)
                {
                    string status = obat.Status ?? "Available";

                    if (counterTable.ContainsKey(status))
                    {
                        counterTable[status]++;
                    }
                    else
                    {
                        counterTable["Available"]++;
                    }
                }

                // One-way data extraction
                var summary = new
                {
                    available = counterTable["Available"],
                    lowStock = counterTable["LowStock"],
                    expired = counterTable["Expired"],
                    total = obatList.Count
                };

                _logger.LogInformation($"Status Summary: Available={summary.available}, LowStock={summary.lowStock}, Expired={summary.expired}");

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStatusSummary");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// GET /api/obat/status/rules - Ambil tabel rule status (reference untuk client)
        /// </summary>
        [HttpGet("status/rules")]
        public IActionResult GetStatusRules()
        {
            try
            {
                _logger.LogInformation("GET /api/obat/status/rules");

                // Return rules sesuai StateMachine di client
                var rules = new object[]
                {
                    new
                    {
                        id = 1,
                        statusName = "Expired",
                        priority = 1,
                        conditionType = "ExpiredDate",
                        op = "<",
                        threshold = 0,
                        colorCode = "#FFC8C8",
                        description = "Obat sudah kadaluarsa"
                    },
                    new
                    {
                        id = 2,
                        statusName = "LowStock",
                        priority = 2,
                        conditionType = "Stok",
                        op = "<=",
                        threshold = 5,
                        colorCode = "#FFFFC8",
                        description = "Stok obat menipis"
                    },
                    new
                    {
                        id = 3,
                        statusName = "Available",
                        priority = 3,
                        conditionType = "None",
                        op = "==",
                        threshold = 0,
                        colorCode = "#C8FFC8",
                        description = "Obat tersedia dalam jumlah cukup"
                    }
                };

                var response = new
                {
                    rules = rules,
                    lastUpdated = DateTime.Now,
                    version = "1.0"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStatusRules");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Helper: Evaluate status obat berdasarkan StateRulesTable logic
        /// 
        /// Implementasi StateMachine di server-side untuk consistency
        /// - Check ExpiredDate < Now → Expired (Priority 1)
        /// - Check Stok <= 5 → LowStock (Priority 2)
        /// - Else → Available (Priority 3)
        /// </summary>
        private void EvaluateObatStatus(Obat obat)
        {
            if (obat == null)
                return;

            // Priority 1: Expired
            if (obat.ExpiredDate < DateTime.Now)
            {
                obat.Status = "Expired";
            }
            // Priority 2: LowStock
            else if (obat.Stok <= 5)
            {
                obat.Status = "LowStock";
            }
            // Priority 3: Available (default)
            else
            {
                obat.Status = "Available";
            }
        }
    }
}
