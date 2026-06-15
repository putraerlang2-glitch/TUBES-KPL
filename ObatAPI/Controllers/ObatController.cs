using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ObatAPI.Data;
using ObatAPI.Models;
using ObatAPI.Services;

namespace ObatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObatController : ControllerBase
    {
        private readonly ObatDbContext _dbContext;
        private readonly IObatStatusService _statusService;
        private readonly ILogger<ObatController> _logger;

        public ObatController(ObatDbContext dbContext, IObatStatusService statusService, ILogger<ObatController> logger)
        {
            _dbContext = dbContext;
            _statusService = statusService;
            _logger = logger;
        }

        private IActionResult HandleError(Exception ex, string action)
        {
            _logger.LogError(ex, $"Error in {action}");
            string error = ex is DbUpdateException ? "Database connection failed" : "Internal server error";
            return StatusCode(500, new { error, details = ex.Message });
        }

        private IActionResult? ValidateObat(Obat obat, bool isCreate = false)
        {
            if (isCreate && (obat == null || string.IsNullOrWhiteSpace(obat.Nama)))
                return BadRequest(new { error = "Obat data and Nama are required" });

            if (obat.ExpiredDate.Date < DateTime.Now.Date)
                return BadRequest(new { error = "ExpiredDate cannot be in the past" });

            if (obat.Stok < 0)
                return BadRequest(new { error = "Invalid stock quantity" });

            if (obat.Harga < 0)
                return BadRequest(new { error = "Invalid price" });

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                int skip = (page - 1) * pageSize;
                var totalCount = await _dbContext.Obat.CountAsync();
                var obatList = await _dbContext.Obat.OrderBy(o => o.Nama)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                foreach (var obat in obatList)
                    _statusService.EvaluateStatus(obat);

                return Ok(new
                {
                    data = obatList,
                    pagination = new { page, pageSize, totalCount, totalPages = (totalCount + pageSize - 1) / pageSize }
                });
            }
            catch (Exception ex)
            {
                return HandleError(ex, nameof(GetAll));
            }
        }

        [HttpGet("{obatId}")]
        public async Task<IActionResult> GetById(int obatId)
        {
            try
            {
                if (obatId <= 0)
                    return BadRequest(new { error = "Invalid ID" });

                var obat = await _dbContext.Obat.FindAsync(obatId);
                if (obat == null)
                    return NotFound(new { error = "Obat not found" });

                _statusService.EvaluateStatus(obat);
                return Ok(obat);
            }
            catch (Exception ex)
            {
                return HandleError(ex, nameof(GetById));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Obat obat)
        {
            try
            {
                var validationError = ValidateObat(obat, isCreate: true);
                if (validationError != null) return validationError;

                _statusService.EvaluateStatus(obat);
                obat.CreatedAt = DateTime.Now;
                obat.UpdatedAt = DateTime.Now;

                _dbContext.Obat.Add(obat);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { obatId = obat.ObatId }, obat);
            }
            catch (Exception ex)
            {
                return HandleError(ex, nameof(Create));
            }
        }

        [HttpPut("{obatId}")]
        public async Task<IActionResult> Update(int obatId, [FromBody] Obat obat)
        {
            try
            {
                if (obatId <= 0)
                    return BadRequest(new { error = "Invalid ID" });

                if (obat == null)
                    return BadRequest(new { error = "Obat data is required" });

                var validationError = ValidateObat(obat);
                if (validationError != null) return validationError;

                var existingObat = await _dbContext.Obat.FindAsync(obatId);
                if (existingObat == null)
                    return NotFound(new { error = "Obat not found" });

                existingObat.Nama = obat.Nama;
                existingObat.Kategori = obat.Kategori;
                existingObat.Stok = obat.Stok;
                existingObat.Harga = obat.Harga;
                existingObat.ExpiredDate = obat.ExpiredDate;
                existingObat.UpdatedAt = DateTime.Now;

                _statusService.EvaluateStatus(existingObat);

                _dbContext.Obat.Update(existingObat);
                await _dbContext.SaveChangesAsync();

                return Ok(existingObat);
            }
            catch (Exception ex)
            {
                return HandleError(ex, nameof(Update));
            }
        }

        [HttpDelete("{obatId}")]
        public async Task<IActionResult> Delete(int obatId)
        {
            try
            {
                if (obatId <= 0)
                    return BadRequest(new { error = "Invalid ID" });

                var obat = await _dbContext.Obat.FindAsync(obatId);
                if (obat == null)
                    return NotFound(new { error = "Obat not found" });

                _dbContext.Obat.Remove(obat);
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleError(ex, nameof(Delete));
            }
        }

        [HttpGet("status/summary")]
        public async Task<IActionResult> GetStatusSummary()
        {
            try
            {
                var obatList = await _dbContext.Obat.ToListAsync();

                foreach (var obat in obatList)
                    _statusService.EvaluateStatus(obat);

                var counterTable = new Dictionary<string, int>
                {
                    { "Available", 0 },
                    { "LowStock", 0 },
                    { "Expired", 0 }
                };

                foreach (var obat in obatList)
                {
                    string status = obat.Status ?? "Available";
                    if (counterTable.ContainsKey(status))
                        counterTable[status]++;
                    else
                        counterTable["Available"]++;
                }

                return Ok(new
                {
                    available = counterTable["Available"],
                    lowStock = counterTable["LowStock"],
                    expired = counterTable["Expired"],
                    total = obatList.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStatusSummary");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("status/rules")]
        public IActionResult GetStatusRules()
        {
            try
            {
                var rules = new object[]
                {
                    new { id = 1, statusName = "Expired", priority = 1, conditionType = "ExpiredDate", op = "<", threshold = 0, colorCode = "#DC3545", description = "Obat sudah kadaluarsa" },
                    new { id = 2, statusName = "LowStock", priority = 2, conditionType = "Stok", op = "<=", threshold = 5, colorCode = "#FFC107", description = "Stok obat menipis" },
                    new { id = 3, statusName = "Available", priority = 3, conditionType = "None", op = "==", threshold = 0, colorCode = "#28A745", description = "Obat tersedia" }
                };

                return Ok(new { rules = rules, version = "1.0" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStatusRules");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}
