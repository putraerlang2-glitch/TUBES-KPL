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
        public string Nama { get; set; } = string.Empty;

        /// <summary>
        /// Kategori obat (Tablet, Sirup, AntiJamur, Vitamin, dll)
        /// </summary>
        public string Kategori { get; set; } = string.Empty;

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
        /// </summary>
        public string Status { get; set; } = "Available";
    }
}
