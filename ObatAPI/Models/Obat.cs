namespace ObatAPI.Models
{
    /// <summary>
    /// Model Obat untuk API
    /// Merepresentasikan data obat yang akan dikirim ke client WinForms
    /// Entity Framework Core model untuk MySQL database
    /// </summary>
    public class Obat
    {
        /// <summary>
        /// ID unik obat (Primary Key, auto-increment di database)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nama obat
        /// </summary>
        public string Nama { get; set; } = string.Empty;

        /// <summary>
        /// Kategori obat (Tablet, Sirup, AntiJamur, Vitamin, dll)
        /// </summary>
        public string Kategori { get; set; } = "Tablet";

        /// <summary>
        /// Jumlah stok tersedia
        /// </summary>
        public int Stok { get; set; }

        /// <summary>
        /// Harga per unit
        /// </summary>
        public decimal Harga { get; set; }

        /// <summary>
        /// Tanggal kadaluarsa obat
        /// </summary>
        public DateTime ExpiredDate { get; set; }

        /// <summary>
        /// Status obat (Available, LowStock, Expired)
        /// Di-trigger oleh StateMachine saat CRUD operations
        /// </summary>
        public string Status { get; set; } = "Available";

        /// <summary>
        /// Timestamp saat record dibuat
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp saat record terakhir diupdate
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
