using System;
using System.Collections.Generic;

namespace ObatAPI.Models
{
    public class Transaksi
    {
        public int Id { get; set; }
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

        /// <summary>Timestamp when transaction was created</summary>
        public DateTime CreatedAt { get; set; }

        public List<TransaksiDetail> DetailList { get; set; } = new List<TransaksiDetail>();
    }
}
