using System;
using System.ComponentModel.DataAnnotations;

namespace ObatAPI.Models
{
    /// <summary>
    /// Represents a user account in the pharmacy system
    /// </summary>
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username harus diisi")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Username harus antara 1-50 karakter")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama harus diisi")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Nama harus antara 1-100 karakter")]
        public string Nama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password harus diisi")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role harus diisi")]
        [StringLength(50)]
        public string Role { get; set; } = "Kasir";

        public DateTime CreatedAt { get; set; }

        public List<Transaksi> TransaksiList { get; set; } = new List<Transaksi>();
    }
}
