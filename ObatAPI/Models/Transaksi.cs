using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObatAPI.Models
{
    /// <summary>
    /// Represents a sales transaction in the pharmacy system
    /// </summary>
    public class Transaksi
    {
        public int TransaksiId { get; set; }

        [Required(ErrorMessage = "Nomor struk harus diisi")]
        [StringLength(50, ErrorMessage = "Nomor struk tidak boleh lebih dari 50 karakter")]
        public string NoStruk { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tanggal transaksi harus diisi")]
        public DateTime TanggalTransaksi { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Subtotal tidak boleh negatif")]
        public decimal Subtotal { get; set; }

        [Range(0, 100, ErrorMessage = "Persentase diskon harus antara 0 sampai 100")]
        public decimal PersentaseDiskon { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Nominal diskon tidak boleh negatif")]
        public decimal NominalDiskon { get; set; }

        [Range(0, 100, ErrorMessage = "Persentase pajak harus antara 0 sampai 100")]
        public decimal PersentasePajak { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Nominal pajak tidak boleh negatif")]
        public decimal NominalPajak { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Total akhir tidak boleh negatif")]
        public decimal TotalAkhir { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Uang bayar tidak boleh negatif")]
        public decimal UangBayar { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Uang kembalian tidak boleh negatif")]
        public decimal UangKembalian { get; set; }

        [Required(ErrorMessage = "User ID harus diisi")]
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
        public List<TransaksiDetail> DetailList { get; set; } = new List<TransaksiDetail>();
    }
}
