using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ObatAPI.Models
{
    /// <summary>
    /// Represents a line item in a sales transaction
    /// </summary>
    public class TransaksiDetail
    {
        public int TransaksiDetailId { get; set; }

        [Required(ErrorMessage = "Transaksi ID harus diisi")]
        public int TransaksiId { get; set; }

        [Required(ErrorMessage = "Obat ID harus diisi")]
        public int ObatId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Jumlah pembelian harus lebih dari 0")]
        public int Jumlah { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Harga satuan tidak boleh negatif")]
        public decimal HargaSatuan { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Subtotal tidak boleh negatif")]
        public decimal Subtotal { get; set; }

        [JsonIgnore]
        public Transaksi? Transaksi { get; set; }

        public Obat? Obat { get; set; }
    }
}
