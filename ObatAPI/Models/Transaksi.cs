using System;
using System.Collections.Generic;

namespace ObatAPI.Models
{
    /// <summary>
    /// Represents a sales transaction in the pharmacy system
    /// </summary>
    public class Transaksi
    {
        public int TransaksiId { get; set; }
        public string NoStruk { get; set; } = string.Empty;
        public DateTime TanggalTransaksi { get; set; }
        
        public decimal Subtotal { get; set; }
        public decimal PersentaseDiskon { get; set; }
        public decimal NominalDiskon { get; set; }
        public decimal PersentasePajak { get; set; }
        public decimal NominalPajak { get; set; }
        public decimal TotalAkhir { get; set; }
        
        public decimal UangBayar { get; set; }
        public decimal UangKembalian { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public List<TransaksiDetail> DetailList { get; set; } = new List<TransaksiDetail>();
    }
}
