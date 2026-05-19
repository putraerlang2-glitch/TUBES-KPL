using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ObatAPI.Enums;

namespace ObatAPI.Models
{
    /// <summary>
    /// Model untuk data obat dengan validasi dan JSON serialization
    /// </summary>
    public class Obat
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama obat wajib diisi")]
        [StringLength(100, MinimumLength = 3, 
            ErrorMessage = "Nama obat harus antara 3-100 karakter")]
        public string Nama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori obat wajib diisi")]
        [StringLength(50, MinimumLength = 2, 
            ErrorMessage = "Kategori harus antara 2-50 karakter")]
        public string Kategori { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Stok tidak boleh negatif")]
        public int Stok { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", 
            ErrorMessage = "Harga harus lebih besar dari 0")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", 
            ErrorMessage = "Harga hanya boleh memiliki maksimal 2 desimal")]
        public decimal Harga { get; set; }

        [Required(ErrorMessage = "Tanggal kadaluarsa wajib diisi")]
        [DataType(DataType.DateTime)]
        public DateTime ExpiredDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ObatStatus Status { get; set; } = ObatStatus.Available;

        /// <summary>
        /// Timestamp kapan data obat terakhir diperbarui
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Validasi model sebelum digunakan
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Validasi: ExpiredDate harus di masa depan (lebih besar dari hari ini)
            if (ExpiredDate.Date <= DateTime.Now.Date)
            {
                results.Add(new ValidationResult(
                    "Tanggal kadaluarsa tidak boleh di masa lalu atau hari ini",
                    new[] { nameof(ExpiredDate) }));
            }

            // Validasi: Nama tidak boleh hanya whitespace
            if (string.IsNullOrWhiteSpace(Nama))
            {
                results.Add(new ValidationResult(
                    "Nama obat tidak boleh kosong atau hanya spasi",
                    new[] { nameof(Nama) }));
            }

            // Validasi: Kategori tidak boleh hanya whitespace
            if (string.IsNullOrWhiteSpace(Kategori))
            {
                results.Add(new ValidationResult(
                    "Kategori tidak boleh kosong atau hanya spasi",
                    new[] { nameof(Kategori) }));
            }

            return results;
        }
    }
}
