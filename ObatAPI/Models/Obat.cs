using ObatAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace ObatAPI.Models
{
    /// <summary>
    /// Model Obat untuk API
    /// Merepresentasikan data obat yang akan dikirim ke client WinForms
    /// </summary>
    public class Obat
    {
        /// <summary>
        /// ID unik obat (opsional, bisa digunakan untuk identifikasi)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nama obat
        /// </summary>
        [Required(ErrorMessage = "Nama obat harus diisi")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Nama obat harus antara 1-200 karakter")]
        public string Nama { get; set; } = string.Empty;

        /// <summary>
        /// Kategori obat (Tablet, Sirup, AntiJamur, Vitamin, dll)
        /// </summary>
        [Required(ErrorMessage = "Kategori harus diisi")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Kategori harus antara 1-100 karakter")]
        public string Kategori { get; set; } = string.Empty;

        /// <summary>
        /// Jumlah stok tersedia
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Stok tidak boleh negatif")]
        public int Stok { get; set; }

        /// <summary>
        /// Harga per unit
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Harga tidak boleh negatif")]
        public decimal Harga { get; set; }

        /// <summary>
        /// Tanggal kadaluarsa obat
        /// </summary>
        public DateTime ExpiredDate { get; set; }

        /// <summary>
        /// Status obat (Available, LowStock, Expired, SoonToExpire)
        /// </summary>
        public ObatStatus Status { get; set; } = ObatStatus.Available;

        /// <summary>
        /// Waktu pembaruan terakhir
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
