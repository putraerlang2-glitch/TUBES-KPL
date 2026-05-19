using System;
using System.Collections.Generic;
using System.Drawing;

namespace TubesKPL

    // REFISI LENGKAP: Menambahkan SoonToExpire untuk state machine yang lebih detail
{
    // Menyimpan aturan status obat di dalam tabel agar lebih rapi tanpa banyak kondisi if-else.
    public static class ObatStateMachine
    {
        // Batas jumlah stok minimal sebelum statusnya menjadi LowStock.
        public const int LOW_STOCK_THRESHOLD = 5;

        // Batas hari sebelum kadaluarsa untuk status SoonToExpire
        // Obat yang akan kadaluarsa dalam 30 hari ke depan akan diprioritaskan untuk dijual
        public const int SOON_TO_EXPIRE_DAYS = 30;

        // Menentukan warna baris di tampilan tabel UI untuk setiap status obat (4 status lengkap)
        private static readonly Dictionary<string, Color> StatusColorTable = new Dictionary<string, Color>()
        {
            { "Available",     Color.FromArgb(200, 255, 200) },  // 🟢 Hijau - Stok Normal
            { "SoonToExpire",  Color.FromArgb(255, 165, 0) },    // 🟠 Oranye - Akan Kadaluarsa
            { "LowStock",      Color.FromArgb(255, 255, 200) },  // 🟡 Kuning - Stok Rendah
            { "Expired",       Color.FromArgb(255, 200, 200) }   // 🔴 Merah - Sudah Kadaluarsa
        };

        // Mengubah teks status dari API menjadi bentuk enum agar mudah digunakan di dalam kode (4 mapping lengkap)
        private static readonly Dictionary<string, StatusObat> StatusEnumTable = new Dictionary<string, StatusObat>()
        {
            { "available",     StatusObat.Available },
            { "soontoexpire",  StatusObat.SoonToExpire },
            { "lowstock",      StatusObat.LowStock },
            { "expired",       StatusObat.Expired }
        };

        // Tabel aturan penentuan status obat berdasarkan urutan prioritas (4 rules lengkap)
        // Angka 1 = prioritas TERTINGGI (dicek pertama), angka 4 = prioritas TERENDAH (dicek terakhir)
        // PRIORITAS: Expired > SoonToExpire > LowStock > Available
        private static readonly List<(string StatusName, Func<int, DateTime, bool> Rule, int Priority)> StatusRuleTable = new List<(string, Func<int, DateTime, bool>, int)>()
        {
            // RULE 1 (Priority 1 - TERTINGGI): Expired
            // Mengecek apakah obat sudah melewati tanggal kadaluarsa.
            // Jika sudah expired, langsung return "Expired" tanpa cek rule berikutnya.
            ("Expired",      (stok, expDate) => expDate < DateTime.Now, 1),

            // RULE 2 (Priority 2): SoonToExpire
            // Mengecek apakah obat akan kadaluarsa dalam 30 hari ke depan (ketat: kurang dari 30 hari, bukan termasuk hari ke-30).
            // Ini penting agar bisa diprioritaskan untuk dijual sebelum benar-benar expired.
            ("SoonToExpire", (stok, expDate) => 
            {
                int daysUntilExpire = (int)(expDate.Date - DateTime.Now.Date).TotalDays;
                return daysUntilExpire > 0 && daysUntilExpire < SOON_TO_EXPIRE_DAYS;
            }, 2),

            // RULE 3 (Priority 3): LowStock
            // Mengecek apakah jumlah stok obat hampir habis (di bawah threshold).
            ("LowStock",     (stok, expDate) => stok <= LOW_STOCK_THRESHOLD, 3),

            // RULE 4 (Priority 4 - TERENDAH): Available
            // Default fallback - jika tidak masuk rule 1, 2, dan 3, status obat adalah tersedia.
            ("Available",    (stok, expDate) => true, 4)
        };

        // Menentukan status obat berdasarkan stok dan tanggal kadaluarsa dengan mencocokkannya ke tabel aturan.
        public static string CalculateStatus(int stok, DateTime expiredDate)
        {
            // Mengurutkan aturan pengecekan dari prioritas paling tinggi.
            var sortedRules = new List<(string StatusName, Func<int, DateTime, bool> Rule, int Priority)>(StatusRuleTable);
            sortedRules.Sort((a, b) => a.Priority.CompareTo(b.Priority));

            // Memeriksa setiap aturan secara berurutan. Jika cocok, langsung kembalikan statusnya.
            foreach (var (statusName, rule, priority) in sortedRules)
            {
                if (rule(stok, expiredDate))
                    return statusName;
            }

            return "Available";
        }

        // Mengambil warna dari tabel konfigurasi untuk menampilkan status obat di UI.
        public static Color GetStatusColor(string status)
        {
            if (string.IsNullOrEmpty(status))
                status = "Available"; 

            return StatusColorTable.ContainsKey(status)
                ? StatusColorTable[status]
                : Color.FromArgb(200, 255, 200); 
        }

        // Mengubah status obat dari teks menjadi nilai enum yang sesuai.
        public static StatusObat GetStatusEnum(string statusString)
        {
            if (string.IsNullOrEmpty(statusString))
                return StatusObat.Available;

            string key = statusString.ToLower();
            return StatusEnumTable.ContainsKey(key)
                ? StatusEnumTable[key]
                : StatusObat.Available; 
        }

        // Memperbarui status semua data obat di dalam list sekaligus.
        public static void UpdateAllStatus(List<Obat> obatList)
        {
            if (obatList == null) return;
            foreach (var obat in obatList)
            {
                string statusStr = CalculateStatus(obat.Stok, obat.ExpiredDate);
                obat.Status = GetStatusEnum(statusStr);
            }
        }

        // Menghitung total jumlah obat untuk masing-masing kategori status (4 status lengkap).
        public static void GetStatusSummary(List<Obat> obatList, out int available, out int lowStock, out int expired, out int soonToExpire)
        {
            available = 0;
            lowStock = 0;
            expired = 0;
            soonToExpire = 0;

            if (obatList == null) return;

            foreach (var obat in obatList)
            {
                string status = CalculateStatus(obat.Stok, obat.ExpiredDate);

                switch (status)
                {
                    case "Expired":
                        expired++;
                        break;
                    case "SoonToExpire":
                        soonToExpire++;
                        break;
                    case "LowStock":
                        lowStock++;
                        break;
                    default: // Available
                        available++;
                        break;
                }
            }
        }
    }
}
