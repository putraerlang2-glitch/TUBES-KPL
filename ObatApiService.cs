using System;
using System.Collections.Generic;
using System.Linq;
using static TubesKPL.Obat;

namespace TubesKPL
{
    // [AYONDI] API Simulasi untuk Obat - Tanpa Database, Tanpa HTTP Server
    // File ini menyediakan layer API yang dapat digunakan untuk mensimulasikan
    // API response tanpa menggunakan HTTP server atau database
    // Tujuan: Memudahkan integrasi API ke dalam aplikasi WinForms

    /// <summary>
    /// [AYONDI] Class untuk response API GetStatusSummary()
    /// Menyimpan ringkasan status obat
    /// </summary>
    public class StatusSummaryResponse
    {
        // [AYONDI] Properti untuk menyimpan jumlah obat per status
        public int TotalAvailable { get; set; }
        public int TotalLowStock { get; set; }
        public int TotalExpired { get; set; }
        public int TotalObat { get; set; }
        public DateTime ResponseTime { get; set; }

        // [AYONDI] Constructor
        public StatusSummaryResponse()
        {
            ResponseTime = DateTime.Now;
        }
    }

    /// <summary>
    /// [AYONDI] Class untuk response API GetAll()
    /// Menyimpan list obat dengan metadata
    /// </summary>
    public class ObatListResponse
    {
        // [AYONDI] Properti untuk menyimpan data obat dan metadata
        public List<Obat> Data { get; set; }
        public int Count { get; set; }
        public string Message { get; set; }
        public DateTime ResponseTime { get; set; }
        public bool Success { get; set; }

        // [AYONDI] Constructor
        public ObatListResponse()
        {
            Data = new List<Obat>();
            ResponseTime = DateTime.Now;
            Success = true;
            Message = "Success";
        }
    }

    /// <summary>
    /// [AYONDI] Class ObatApiService
    /// Menyediakan method-method untuk mengakses data obat seperti API
    /// Tidak menggunakan HTTP, hanya simulasi API response
    /// Kompatibel dengan .NET Framework 4.7.2 dan C# 7.3
    /// </summary>
    public static class ObatApiService
    {
        // [AYONDI] Data internal: List<Obat> sebagai simulated database
        // Dalam implementasi nyata, ini bisa diganti dengan HTTP call atau database
        private static List<Obat> _internalData = new List<Obat>();

        // [AYONDI] Flag untuk menandakan apakah data sudah diinisialisasi
        private static bool _initialized = false;

        // [AYONDI] Method untuk inisialisasi data dari List<Obat> yang diberikan
        // Harus dipanggil sekali saat awal aplikasi
        /// <summary>
        /// [AYONDI] Inisialisasi data API dengan List<Obat>
        /// Parameter: List<Obat> sourceData - data sumber dari Form1
        /// </summary>
        public static void Initialize(List<Obat> sourceData)
        {
            // [AYONDI] Copy data dari source ke internal storage
            // Menggunakan deep copy untuk menghindari reference issues
            _internalData.Clear();
            foreach (var obat in sourceData)
            {
                _internalData.Add(obat);
            }
            _initialized = true;
        }

        // [AYONDI] Method API: GetAll() - Mengambil semua data obat
        // Simulasi: GET /obat
        /// <summary>
        /// [AYONDI] Mengambil semua data obat dalam format API response
        /// Return: ObatListResponse berisi list<Obat> dan metadata
        /// </summary>
        public static ObatListResponse GetAll()
        {
            // [AYONDI] Buat response object
            ObatListResponse response = new ObatListResponse();

            // [AYONDI] Update status setiap obat sebelum mengembalikan
            foreach (var obat in _internalData)
            {
                obat.UpdateStatus();
            }

            // [AYONDI] Isi response dengan data
            response.Data = _internalData;
            response.Count = _internalData.Count;
            response.Message = "Retrieved " + _internalData.Count + " obat successfully";

            return response;
        }

        // [AYONDI] Method API: GetStatusSummary() - Mengambil ringkasan status
        // Simulasi: GET /obat/status
        /// <summary>
        /// [AYONDI] Mengambil ringkasan status obat (Available, LowStock, Expired)
        /// Return: StatusSummaryResponse berisi jumlah obat per status
        /// </summary>
        public static StatusSummaryResponse GetStatusSummary()
        {
            // [AYONDI] Buat response object
            StatusSummaryResponse response = new StatusSummaryResponse();

            // [AYONDI] Update status setiap obat terlebih dahulu
            foreach (var obat in _internalData)
            {
                obat.UpdateStatus();
            }

            // [AYONDI] Hitung jumlah obat per status menggunakan LINQ Where dan Count
            response.TotalAvailable = _internalData.Count(o => o.status == StatusObat.Available);
            response.TotalLowStock = _internalData.Count(o => o.status == StatusObat.LowStock);
            response.TotalExpired = _internalData.Count(o => o.status == StatusObat.Expired);
            response.TotalObat = _internalData.Count;

            return response;
        }

        // [AYONDI] Method API: GetByStatus() - Mengambil obat berdasarkan status tertentu
        /// <summary>
        /// [AYONDI] Mengambil data obat dengan status tertentu
        /// Parameter: StatusObat status
        /// Return: ObatListResponse berisi list<Obat> dengan status tertentu
        /// </summary>
        public static ObatListResponse GetByStatus(StatusObat status)
        {
            // [AYONDI] Buat response object
            ObatListResponse response = new ObatListResponse();

            // [AYONDI] Filter data berdasarkan status menggunakan LINQ Where
            var filteredData = _internalData.Where(o => o.status == status).ToList();

            // [AYONDI] Isi response
            response.Data = filteredData;
            response.Count = filteredData.Count;
            response.Message = "Retrieved " + filteredData.Count + " obat with status " + status.ToString();

            return response;
        }

        // [AYONDI] Method API: GetByKategori() - Mengambil obat berdasarkan kategori
        /// <summary>
        /// [AYONDI] Mengambil data obat dengan kategori tertentu
        /// Parameter: KategoriObat kategori
        /// Return: ObatListResponse berisi list<Obat> dengan kategori tertentu
        /// </summary>
        public static ObatListResponse GetByKategori(KategoriObat kategori)
        {
            // [AYONDI] Buat response object
            ObatListResponse response = new ObatListResponse();

            // [AYONDI] Filter data berdasarkan kategori menggunakan LINQ Where
            var filteredData = _internalData.Where(o => o.kategori == kategori).ToList();

            // [AYONDI] Isi response
            response.Data = filteredData;
            response.Count = filteredData.Count;
            response.Message = "Retrieved " + filteredData.Count + " obat with kategori " + kategori.ToString();

            return response;
        }

        // [AYONDI] Method API: Search() - Mencari obat berdasarkan nama
        /// <summary>
        /// [AYONDI] Mencari obat berdasarkan nama (case-insensitive)
        /// Parameter: string searchTerm - term pencarian
        /// Return: ObatListResponse berisi hasil pencarian
        /// </summary>
        public static ObatListResponse Search(string searchTerm)
        {
            // [AYONDI] Buat response object
            ObatListResponse response = new ObatListResponse();

            // [AYONDI] Convert search term ke lowercase untuk case-insensitive search
            string term = searchTerm.ToLower();

            // [AYONDI] Filter data berdasarkan nama menggunakan LINQ Where dan Contains
            var filteredData = _internalData.Where(o => o.nama.ToLower().Contains(term)).ToList();

            // [AYONDI] Isi response
            response.Data = filteredData;
            response.Count = filteredData.Count;
            response.Message = "Found " + filteredData.Count + " obat matching '" + searchTerm + "'";

            return response;
        }

        // [AYONDI] Method API: GetStats() - Mengambil statistik lengkap
        /// <summary>
        /// [AYONDI] Mengambil statistik lengkap: total obat, total stok, total nilai
        /// Return: Dictionary<string, object> berisi statistik
        /// </summary>
        public static Dictionary<string, object> GetStats()
        {
            // [AYONDI] Update status semua obat
            foreach (var obat in _internalData)
            {
                obat.UpdateStatus();
            }

            // [AYONDI] Buat Dictionary untuk statistik
            Dictionary<string, object> stats = new Dictionary<string, object>();

            // [AYONDI] Hitung berbagai statistik
            stats["TotalObat"] = _internalData.Count;
            stats["TotalStok"] = _internalData.Sum(o => o.stok);
            stats["TotalNilai"] = _internalData.Sum(o => (long)o.harga * o.stok);  // [AYONDI] Cast to long untuk menghindari overflow
            stats["TotalAvailable"] = _internalData.Count(o => o.status == StatusObat.Available);
            stats["TotalLowStock"] = _internalData.Count(o => o.status == StatusObat.LowStock);
            stats["TotalExpired"] = _internalData.Count(o => o.status == StatusObat.Expired);
            stats["AverageHarga"] = _internalData.Count > 0 ? _internalData.Average(o => o.harga) : 0;
            stats["ResponseTime"] = DateTime.Now;

            return stats;
        }

        // [AYONDI] Method helper: Cek apakah API sudah diinisialisasi
        /// <summary>
        /// [AYONDI] Helper method untuk cek status inisialisasi
        /// Return: bool - true jika sudah diinisialisasi
        /// </summary>
        public static bool IsInitialized()
        {
            return _initialized;
        }

        // [AYONDI] Method helper: Reset data API
        /// <summary>
        /// [AYONDI] Reset semua data dan inisialisasi flag
        /// Berguna untuk testing atau reset aplikasi
        /// </summary>
        public static void Reset()
        {
            _internalData.Clear();
            _initialized = false;
        }
    }
}
