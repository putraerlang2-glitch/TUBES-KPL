using System;
using System.Collections.Generic;

namespace TubesKPL
{
    /// <summary>
    /// Model untuk data obat yang bekerja dengan ObatAPI.
    /// Status menggunakan Enum untuk type-safety.
    /// </summary>
    public class Obat
    {
        /// <summary>Nomor identitas obat</summary>
        public int Id { get; set; }

        /// <summary>Nama obat</summary>
        public string Nama { get; set; } = string.Empty;

        /// <summary>Kategori/tipe obat</summary>
        public string Kategori { get; set; } = "Tablet";

        /// <summary>Jumlah stok yang tersedia</summary>
        public int Stok { get; set; }

        /// <summary>Harga obat</summary>
        public decimal Harga { get; set; }

        /// <summary>Tanggal kadaluarsa</summary>
        public DateTime ExpiredDate { get; set; }

        /// <summary>Status obat (Available, LowStock, Expired, SoonToExpire)</summary>
        public StatusObat Status { get; set; } = StatusObat.Available;

        /// <summary>Kategori obat dalam bentuk enum</summary>
        public KategoriObat kategori
        {
            get
            {
                try
                {
                    return (KategoriObat)Enum.Parse(typeof(KategoriObat), Kategori);
                }
                catch
                {
                    return KategoriObat.Tablet;
                }
            }
            set { Kategori = value.ToString(); }
        }

        /// <summary>Konstruktor kosong</summary>
        public Obat()
        {
            Id = 0;
            Nama = string.Empty;
            Kategori = "Tablet";
            Stok = 0;
            Harga = 0;
            ExpiredDate = DateTime.Now.AddYears(1);
            Status = StatusObat.Available;
        }

        /// <summary>Konstruktor dengan kategori teks</summary>
        public Obat(string nama, int stok, decimal harga, DateTime expiredDate, string kategori = "Tablet", int id = 0)
        {
            Id = id;
            Nama = nama ?? string.Empty;
            Stok = stok;
            Harga = harga;
            ExpiredDate = expiredDate;
            Kategori = kategori ?? "Tablet";
            Status = StatusObat.Available;
            UpdateStatus();
        }

        /// <summary>Konstruktor dengan kategori enum</summary>
        public Obat(string nama, int stok, decimal harga, DateTime expiredDate, KategoriObat kategoriEnum, int id = 0)
        {
            Id = id;
            Nama = nama ?? string.Empty;
            Stok = stok;
            Harga = harga;
            ExpiredDate = expiredDate;
            Kategori = kategoriEnum.ToString();
            Status = StatusObat.Available;
            UpdateStatus();
        }

        /// <summary>Perbarui status obat berdasarkan stok dan tanggal kadaluarsa</summary>
        public void UpdateStatus()
        {
            string statusString = ObatStateMachine.CalculateStatus(Stok, ExpiredDate);
            Status = ObatStateMachine.GetStatusEnum(statusString);
        }

        /// <summary>Tampilkan data obat dalam satu baris</summary>
        public override string ToString()
        {
            return $"[{Id}] {Nama} | Stok: {Stok} | Status: {Status} | Exp: {ExpiredDate:yyyy-MM-dd}";
        }
    }

    /// <summary>
    /// Enum untuk status obat
    /// </summary>
    public enum StatusObat
    {
        /// <summary>Obat tersedia dengan stok normal</summary>
        Available = 0,

        /// <summary>Obat memiliki stok rendah (di bawah 5)</summary>
        LowStock = 1,

        /// <summary>Obat telah melewati tanggal kadaluarsa</summary>
        Expired = 2,

        /// <summary>Obat akan kadaluarsa dalam 30 hari ke depan</summary>
        SoonToExpire = 3
    }

    /// <summary>
    /// Enum untuk kategori obat
    /// </summary>
    public enum KategoriObat
    {
        /// <summary>Obat tablet</summary>
        Tablet = 0,

        /// <summary>Obat salep</summary>
        Salep = 1,

        /// <summary>Obat sirup</summary>
        Sirup = 2,

        /// <summary>Suplemen vitamin</summary>
        Vitamin = 3,

        /// <summary>Obat antibiotik</summary>
        Antibiotik = 4,

        /// <summary>Obat anti jamur</summary>
        AntiJamur = 5
    }
}
