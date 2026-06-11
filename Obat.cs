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

        // Konstruktor

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

        // Konstruktor dengan validasi input
        public Obat(string nama, int stok, decimal harga, DateTime expiredDate, string kategori = "Tablet", int id = 0)
        {
            if (string.IsNullOrWhiteSpace(nama))
                throw new ArgumentException("Nama obat tidak boleh kosong");
            if (stok < 0)
                throw new ArgumentException("Stok tidak boleh negatif");
            if (harga <= 0)
                throw new ArgumentException("Harga harus lebih dari 0");

            Id = id;
            Nama = nama.Trim();
            Stok = stok;
            Harga = harga;
            ExpiredDate = expiredDate;
            Kategori = kategori ?? "Tablet";
            Status = "Available";
            UpdateStatus();
        }

        // Konstruktor dengan enum kategori
        public Obat(string nama, int stok, decimal harga, DateTime expiredDate, KategoriObat kategoriEnum, int id = 0)
        {
            if (string.IsNullOrWhiteSpace(nama))
                throw new ArgumentException("Nama obat tidak boleh kosong");
            if (stok < 0)
                throw new ArgumentException("Stok tidak boleh negatif");
            if (harga <= 0)
                throw new ArgumentException("Harga harus lebih dari 0");

            Id = id;
            Nama = nama.Trim();
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
