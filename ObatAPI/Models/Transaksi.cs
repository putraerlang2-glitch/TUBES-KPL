using System;
using System.Collections.Generic;

namespace ObatAPI.Models
{
    /// <summary>
    /// Represents a sales transaction in the pharmacy system
    /// </summary>
    public class Transaksi
    {
        /// <summary>Transaction unique identifier</summary>
        public int Id { get; set; }

        /// <summary>Receipt number (generated automatically)</summary>
        public string NoStruk { get; set; } = string.Empty;

        /// <summary>Transaction date and time</summary>
        public DateTime TanggalTransaksi { get; set; }
        
        /// <summary>Sum of all item prices before discount and tax</summary>
        public decimal Subtotal { get; set; }

        /// <summary>Discount percentage applied (e.g., 10.5 for 10.5%)</summary>
        public decimal PersentaseDiskon { get; set; }

        /// <summary>Actual discount amount calculated from PersentaseDiskon</summary>
        public decimal NominalDiskon { get; set; }

        /// <summary>Tax percentage applied (e.g., 10 for 10%)</summary>
        public decimal PersentasePajak { get; set; }

        /// <summary>Actual tax amount calculated from PersentasePajak</summary>
        public decimal NominalPajak { get; set; }

        /// <summary>Final total after discount and tax (Subtotal - NominalDiskon + NominalPajak)</summary>
        public decimal TotalAkhir { get; set; }
        
        /// <summary>Cash amount received from customer</summary>
        public decimal UangBayar { get; set; }

        /// <summary>Change returned to customer (UangBayar - TotalAkhir)</summary>
        public decimal UangKembalian { get; set; }
        
        /// <summary>Timestamp when transaction was created</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>List of line items for this transaction</summary>
        public List<TransaksiDetail> DetailList { get; set; } = new List<TransaksiDetail>();
    }
}
