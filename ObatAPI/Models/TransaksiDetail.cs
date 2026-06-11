using System;
using System.Text.Json.Serialization;

namespace ObatAPI.Models
{
    public class TransaksiDetail
    {
        public int Id { get; set; }
        public int TransaksiId { get; set; }
        public int ObatId { get; set; }
        
        public int Jumlah { get; set; }
        public decimal HargaSatuan { get; set; }
        public decimal Subtotal { get; set; }

        /// <summary>Timestamp when line item was created</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Navigation property to parent transaction (not serialized)</summary>

        [JsonIgnore]
        public Transaksi? Transaksi { get; set; }
        public Obat? Obat { get; set; }
    }
}
