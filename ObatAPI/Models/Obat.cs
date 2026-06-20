using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ObatAPI.Helpers;

namespace ObatAPI.Models
{
    public class Obat
    {
        public int ObatId { get; set; }

        [Required(ErrorMessage = "Nama obat harus diisi")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Nama harus antara 1-255 karakter")]
        public string Nama { get; set; } = string.Empty;

        [StringLength(100)]
        public string Kategori { get; set; } = "Tablet";

        [Range(0, int.MaxValue, ErrorMessage = "Stok tidak boleh negatif")]
        public int Stok { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Harga tidak boleh negatif")]
        public decimal Harga { get; set; }

        [Required(ErrorMessage = "Tanggal kadaluarsa harus diisi")]
        public DateTime ExpiredDate { get; set; }

        [JsonConverter(typeof(ObatStatusJsonConverter))]
        public ObatStatus Status { get; set; } = ObatStatus.Available;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
