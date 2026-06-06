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

        [JsonIgnore]
        public Transaksi? Transaksi { get; set; }
        public Obat? Obat { get; set; }
    }
}
