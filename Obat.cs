using System;
using System.Collections.Generic;

namespace TubesKPL
{
    public class Obat
    {
        // ID unik dari server (untuk identifikasi dan update/delete)
        public int Id { get; set; }

        // Nama obat
        public string Nama { get; set; } = string.Empty;

        // Kategori obat (string untuk API compatibility)
        public string Kategori { get; set; } = "Tablet";

        // Jumlah stok
        public int Stok { get; set; }

        // Harga per unit
        public decimal Harga { get; set; }

        // Tanggal kadaluarsa
        public DateTime ExpiredDate { get; set; }

        // Status obat (Available, LowStock, Expired) - string format dari API
        public string Status { get; set; } = "Available";

        // ============================================
        // BACKWARD COMPATIBILITY ALIASES (lowercase)
        // ============================================

        // Alias lowercase untuk 'Nama' - digunakan di banyak tempat dalam UI
        public string nama
        {
            get { return Nama; }
            set { Nama = value; }
        }

        // Alias lowercase untuk 'Stok' - digunakan di banyak tempat dalam UI
        public int stok
        {
            get { return Stok; }
            set { Stok = value; }
        }

        // Alias lowercase untuk 'Harga' - digunakan di banyak tempat dalam UI
        public decimal harga
        {
            get { return Harga; }
            set { Harga = value; }
        }

        // Alias lowercase untuk 'ExpiredDate' - digunakan di banyak tempat dalam UI
        public DateTime expiredDate
        {
            get { return ExpiredDate; }
            set { ExpiredDate = value; }
        }

        // Alias status sebagai enum untuk kompatibilitas dengan kode lama
        // Digunakan di ObatApiService.cs untuk convert status string ke enum
        public StatusObat status
        {
            get { return GetStatusEnum(); }
            set { Status = value.ToString(); }
        }

        // Alias kategori sebagai enum untuk kompatibilitas dengan kode lama
        // Digunakan di FormTambahObat.cs, JsonDataManager.cs, dan file UI lainnya
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

        // ============================================
        // KONSTRUKTOR
        // ============================================

        // Konstruktor default (untuk JSON parsing/deserialization dari API)
        public Obat()
        {
            Id = 0;
            Nama = string.Empty;
            Kategori = "Tablet";
            Stok = 0;
            Harga = 0;
            ExpiredDate = DateTime.Now.AddYears(1);
            Status = "Available";
        }

        // Konstruktor untuk membuat obat baru dari UI atau data lama
        // Parameter: nama, stok, harga, expiredDate, kategori (default "Tablet")
        public Obat(string nama, int stok, decimal harga, DateTime expiredDate, string kategori = "Tablet", int id = 0)
        {
            Id = id;
            Nama = nama ?? string.Empty;
            Stok = stok;
            Harga = harga;
            ExpiredDate = expiredDate;
            Kategori = kategori ?? "Tablet";
            Status = "Available";
            UpdateStatus();
        }

        // Konstruktor dengan enum kategori - masih digunakan di FormTambahObat.cs
        public Obat(string nama, int stok, decimal harga, DateTime expiredDate, KategoriObat kategoriEnum, int id = 0)
        {
            Id = id;
            Nama = nama ?? string.Empty;
            Stok = stok;
            Harga = harga;
            ExpiredDate = expiredDate;
            Kategori = kategoriEnum.ToString();
            Status = "Available";
            UpdateStatus();
        }

        // ============================================
        // HELPER METHODS
        // ============================================

        // Update status berdasarkan stok dan tanggal expired
        // Logika:
        // 1. Jika expired -> "Expired"
        // 2. Jika stok <= 5 -> "LowStock"
        // 3. Else -> "Available"
        public void UpdateStatus()
        {
            if (ExpiredDate < DateTime.Now)
            {
                Status = "Expired";
            }
            else if (Stok <= 5)
            {
                Status = "LowStock";
            }
            else
            {
                Status = "Available";
            }
        }

        // Convert string status ke StatusObat enum
        // Untuk kompatibilitas dengan kode lama yang menggunakan enum
        public StatusObat GetStatusEnum()
        {
            switch (Status?.ToLower())
            {
                case "lowstock":
                    return StatusObat.LowStock;
                case "expired":
                    return StatusObat.Expired;
                default:
                    return StatusObat.Available;
            }
        }

        // Override ToString untuk debugging dan logging
        public override string ToString()
        {
            return $"[{Id}] {Nama} | Stok: {Stok} | Status: {Status} | Exp: {ExpiredDate:yyyy-MM-dd}";
        }
    }

    // ============================================
    // ENUM UNTUK STATUS
    // ============================================

    // StatusObat enum untuk status obat (Available, LowStock, Expired)
    // Bisa di-convert dari Status string menggunakan GetStatusEnum()
    public enum StatusObat
    {
        Available,
        LowStock,
        Expired
    }

    // ============================================
    // ENUM UNTUK KATEGORI
    // ============================================

    // KategoriObat enum untuk tipe-tipe obat
    // Digunakan di UI form dan JsonDataManager untuk backward compatibility
    public enum KategoriObat
    {
        Tablet,
        Salep,
        Sirup,
        Vitamin,
        Antibiotik,
        AntiJamur
    }
}
