using System;
using System.Text.Json.Serialization;

namespace ObatAPI.Models
{
    /// <summary>
    /// Represents a line item in a sales transaction
    /// </summary>
    public class TransaksiDetail
    {
        public int TransaksiDetailId { get; set; }
        public int TransaksiId { get; set; }
        public int ObatId { get; set; }
        
        public int Jumlah { get; set; }
        public decimal HargaSatuan { get; set; }
        public decimal Subtotal { get; set; }

        [JsonIgnore]
        public Transaksi? Transaksi { get; set; }

        public Obat? Obat { get; set; }
    }
}
