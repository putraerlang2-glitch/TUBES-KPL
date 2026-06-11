using System;
using System.Text.Json.Serialization;

namespace ObatAPI.Models
{
    /// <summary>
    /// Represents a line item in a sales transaction
    /// </summary>
    public class TransaksiDetail
    {
        /// <summary>Line item unique identifier</summary>
        public int Id { get; set; }

        /// <summary>Reference to the parent transaction</summary>
        public int TransaksiId { get; set; }

        /// <summary>Reference to the medicine being purchased</summary>
        public int ObatId { get; set; }
        
        /// <summary>Quantity of medicine purchased in this line</summary>
        public int Jumlah { get; set; }

        /// <summary>Unit price of the medicine at time of transaction</summary>
        public decimal HargaSatuan { get; set; }

        /// <summary>Line total (Jumlah * HargaSatuan)</summary>
        public decimal Subtotal { get; set; }

        /// <summary>Timestamp when line item was created</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Navigation property to parent transaction (not serialized)</summary>

        [JsonIgnore]
        public Transaksi? Transaksi { get; set; }

        /// <summary>Navigation property to the medicine details</summary>
        public Obat? Obat { get; set; }
    }
}
