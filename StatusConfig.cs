using System;
using System.Collections.Generic;
using System.Drawing;
using static TubesKPL.Obat;

namespace TubesKPL
{
    // [AYONDI] Table-Driven Construction untuk Status Obat
    // File ini menggunakan Dictionary untuk mapping Status Obat ke Warna, Text, dan Deskripsi
    // Tujuan: Menghilangkan if-else panjang dan membuat kode lebih maintainable
    // Semua status mapping dikontrol melalui Dictionary, bukan hardcode

    /// <summary>
    /// [AYONDI] Class StatusConfig
    /// Menyimpan konfigurasi warna, text, dan deskripsi untuk setiap status obat
    /// </summary>
    public class StatusConfig
    {
        // [AYONDI] Properti untuk warna, text, dan deskripsi status
        public Color Color { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }

        // [AYONDI] Constructor untuk inisialisasi StatusConfig
        public StatusConfig(Color color, string text, string description)
        {
            Color = color;
            Text = text;
            Description = description;
        }
    }

    /// <summary>
    /// [AYONDI] Class StatusConfigManager
    /// Mengelola mapping antara StatusObat dengan konfigurasi (warna, text, deskripsi)
    /// Menggunakan Dictionary sebagai Table-Driven Construction
    /// </summary>
    public static class StatusConfigManager
    {
        // [AYONDI] Dictionary utama: mapping StatusObat ke StatusConfig
        // Ini adalah "tabel" yang mengontrol semua konfigurasi status
        // Format: StatusObat -> StatusConfig(Color, Text, Description)
        private static Dictionary<StatusObat, StatusConfig> statusConfigTable = new Dictionary<StatusObat, StatusConfig>()
        {
            // [AYONDI] Status Available: Obat tersedia, stok cukup, belum expired
            // Warna: Hijau (RGB: 200, 255, 200)
            // Text: "Available" 
            // Deskripsi: Penjelasan untuk user
            {
                StatusObat.Available,
                new StatusConfig(
                    Color.FromArgb(200, 255, 200),  // [AYONDI] RGB hijau untuk status baik
                    "Available",                     // [AYONDI] Text yang ditampilkan
                    "Obat tersedia dengan stok cukup dan belum kadaluarsa"  // [AYONDI] Deskripsi
                )
            },

            // [AYONDI] Status LowStock: Stok obat kurang dari batas minimum kategori
            // Warna: Kuning (RGB: 255, 255, 200)
            // Text: "LowStock"
            // Deskripsi: Peringatan stok rendah
            {
                StatusObat.LowStock,
                new StatusConfig(
                    Color.FromArgb(255, 255, 200),  // [AYONDI] RGB kuning untuk peringatan
                    "LowStock",                      // [AYONDI] Text yang ditampilkan
                    "Stok obat rendah, segera lakukan pemesanan"  // [AYONDI] Deskripsi
                )
            },

            // [AYONDI] Status Expired: Tanggal expired sudah lewat
            // Warna: Merah (RGB: 255, 200, 200)
            // Text: "Expired"
            // Deskripsi: Peringatan kadaluarsa
            {
                StatusObat.Expired,
                new StatusConfig(
                    Color.FromArgb(255, 200, 200),  // [AYONDI] RGB merah untuk bahaya
                    "Expired",                       // [AYONDI] Text yang ditampilkan
                    "Obat sudah kadaluarsa, harus dimusnahkan"  // [AYONDI] Deskripsi
                )
            }
        };

        // [AYONDI] Method untuk mengambil StatusConfig berdasarkan StatusObat
        // Digunakan untuk mendapatkan warna, text, dan deskripsi dari sebuah status
        /// <summary>
        /// [AYONDI] Mengambil konfigurasi status berdasarkan enum StatusObat
        /// </summary>
        public static StatusConfig GetConfig(StatusObat status)
        {
            // [AYONDI] Cek apakah status ada di Dictionary
            // Jika tidak ada, return konfigurasi default (Available)
            if (statusConfigTable.ContainsKey(status))
            {
                return statusConfigTable[status];
            }
            else
            {
                // [AYONDI] Fallback ke Available jika status tidak ditemukan
                return statusConfigTable[StatusObat.Available];
            }
        }

        // [AYONDI] Method untuk mengambil warna dari status
        /// <summary>
        /// [AYONDI] Mengambil warna yang sesuai dengan status obat
        /// </summary>
        public static Color GetColor(StatusObat status)
        {
            return GetConfig(status).Color;
        }

        // [AYONDI] Method untuk mengambil text dari status
        /// <summary>
        /// [AYONDI] Mengambil text display yang sesuai dengan status obat
        /// </summary>
        public static string GetText(StatusObat status)
        {
            return GetConfig(status).Text;
        }

        // [AYONDI] Method untuk mengambil deskripsi dari status
        /// <summary>
        /// [AYONDI] Mengambil deskripsi yang sesuai dengan status obat
        /// </summary>
        public static string GetDescription(StatusObat status)
        {
            return GetConfig(status).Description;
        }

        // [AYONDI] Method untuk mengambil semua status yang tersedia
        /// <summary>
        /// [AYONDI] Mengembalikan Dictionary semua status dan konfigurasinya
        /// Berguna untuk iterasi atau debugging
        /// </summary>
        public static Dictionary<StatusObat, StatusConfig> GetAllConfigs()
        {
            return new Dictionary<StatusObat, StatusConfig>(statusConfigTable);
        }
    }
}
