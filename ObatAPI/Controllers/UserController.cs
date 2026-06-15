using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObatAPI.Data;
using ObatAPI.Models;
using ObatAPI.Helpers;

namespace ObatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ObatDbContext _dbContext;
        private readonly ILogger<UserController> _logger;

        public UserController(ObatDbContext dbContext, ILogger<UserController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _dbContext.User.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(int userId)
        {
            try
            {
                if (userId <= 0) return BadRequest(new { error = "Invalid ID" });

                var user = await _dbContext.User.FindAsync(userId);
                if (user == null) return NotFound(new { error = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetById");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(new { error = "Username and password are required" });

                var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Username == request.Username);
                if (user == null) return Unauthorized(new { error = "Invalid username or password" });

                bool isValid = PasswordHasher.VerifyPassword(request.Password, user.PasswordHash);
                if (!isValid) return Unauthorized(new { error = "Invalid username or password" });

                return Ok(new
                {
                    userId = user.UserId,
                    username = user.Username,
                    nama = user.Nama,
                    role = user.Role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Password) || 
                    string.IsNullOrWhiteSpace(request.Nama))
                    return BadRequest(new { error = "Username, password, and nama are required" });

                var existingUser = await _dbContext.User.FirstOrDefaultAsync(u => u.Username == request.Username);
                if (existingUser != null) return Conflict(new { error = "Username already exists" });

                var newUser = new User
                {
                    Username = request.Username,
                    Nama = request.Nama,
                    PasswordHash = PasswordHasher.HashPassword(request.Password),
                    Role = request.Role ?? "Kasir",
                    CreatedAt = DateTime.Now
                };

                _dbContext.User.Add(newUser);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { userId = newUser.UserId }, new
                {
                    userId = newUser.UserId,
                    username = newUser.Username,
                    nama = newUser.Nama,
                    role = newUser.Role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Register");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}
