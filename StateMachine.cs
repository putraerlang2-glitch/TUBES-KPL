using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TubesKPL
{
    // ============================================================
    // DESIGN PATTERN: Table-Driven Design + Strategy Pattern
    // ============================================================
    // Table-Driven Design: Menciptakan data structure (StatusTable)
    // untuk menghindari nested if/switch statements.
    // 
    // Strategy Pattern: Func<Obat,bool> adalah strategy yang berbeda
    // untuk mengevaluasi setiap status (Expired, LowStock, Available).
    // Setiap strategy bisa diubah tanpa mengubah logic evaluasi.
    //
    // BENEFIT: 
    // - Mudah tambah status baru (cukup tambah entry di StatusTable)
    // - Tidak perlu modifikasi method EvaluateStatus()
    // - Logic terpusat & testable
    // ============================================================
    public static class StateMachine
    {
        // [CLEAN CODE] Named constants: Tidak hardcode magic strings
        public const string STATUS_EXPIRED = "Expired";
        public const string STATUS_LOW_STOCK = "LowStock";
        public const string STATUS_AVAILABLE = "Available";
        // [STANDARD CODE] Magic number extracted to named constant
        public const int LOW_STOCK_THRESHOLD = 5;

        // [DESIGN PATTERN] Private class untuk encapsulation data
        private class StatusConfig
        {
            public string Name { get; set; }
            // [SECURE CODE] Color value menggunakan RGB standard (tidak hardcode)
            public Color Color { get; set; }
            // [DESIGN PATTERN] Func<T,bool> = Strategy: evaluasi kondisi untuk setiap status
            public Func<Obat, bool> Condition { get; set; }
        }

        // [TABLE-DRIVEN DESIGN] Single source of truth untuk semua status
        // Menggantikan: switch(obat.status) { case Expired: ... case LowStock: ... }
        // Benefit: Mudah di-maintain, testable, extensible
        private static readonly StatusConfig[] StatusTable = new[]
        {
            // [STRATEGY PATTERN] Strategy 1: Cek expired date
            new StatusConfig 
            { 
                Name = STATUS_EXPIRED, 
                Color = Color.FromArgb(220, 53, 69),  // [STANDARD CODE] RGB untuk warna merah
                // [CLEAN CODE] Lambda expression yang jelas: apakah obat sudah kadaluarsa?
                Condition = o => o.ExpiredDate < DateTime.Now 
            },
            // [STRATEGY PATTERN] Strategy 2: Cek stok rendah
            new StatusConfig 
            { 
                Name = STATUS_LOW_STOCK, 
                Color = Color.FromArgb(255, 193, 7),  // [STANDARD CODE] RGB untuk warna kuning
                // [CLEAN CODE] Lambda expression: apakah stok <= threshold?
                Condition = o => o.Stok <= LOW_STOCK_THRESHOLD 
            },
            // [STRATEGY PATTERN] Strategy 3: Default (tersedia)
            new StatusConfig 
            { 
                Name = STATUS_AVAILABLE, 
                Color = Color.FromArgb(40, 167, 69),  // [STANDARD CODE] RGB untuk warna hijau
                // [CLEAN CODE] Fallback strategy: selalu true (default)
                Condition = o => true 
            }
        };


        // [METHOD] Evaluasi status obat menggunakan table-driven logic
        // [CLEAN CODE] Guard clause di awal untuk null check
        // [DESIGN PATTERN] Menggunakan LINQ First() untuk mencari strategy yang cocok
        public static void EvaluateStatus(Obat obat)
        {
            if (obat == null) return;  // [CLEAN CODE] Guard clause: null check di awal
            // [DESIGN PATTERN] LINQ First(): Cari strategy yang Condition-nya return true
            // Contoh: untuk obat expired, Condition (o => o.ExpiredDate < DateTime.Now) return true
            obat.Status = StatusTable.First(cfg => cfg.Condition(obat)).Name;
        }

        // [METHOD] Ambil warna berdasarkan status
        // [CLEAN CODE] SingleResponsibility: Hanya mapping status -> warna
        // [SECURE CODE] FirstOrDefault + null coalescing: Aman jika status tidak ditemukan
        public static Color GetStatusColor(string status)
        {
            return StatusTable.FirstOrDefault(cfg => cfg.Name == status)?.Color ?? Color.White;
        }

        // [METHOD] Terapkan warna ke setiap row di DataGridView
        // [CLEAN CODE] Guard clause untuk safety
        // [STANDARD CODE] Loop dengan index untuk akses cell
        public static void ApplyStatusColors(DataGridView grid, int colIndex = 4)
        {
            if (grid?.Rows.Count == 0) return;  // [CLEAN CODE] Guard clause + null propagation operator

            for (int i = 0; i < grid.Rows.Count; i++)
            {
                // [CLEAN CODE] Null coalescing: default ke STATUS_AVAILABLE jika null
                string status = grid.Rows[i].Cells[colIndex].Value?.ToString() ?? STATUS_AVAILABLE;
                grid.Rows[i].DefaultCellStyle.BackColor = GetStatusColor(status);
            }
        }

        // [METHOD] Hitung berapa banyak obat per status
        // [CLEAN CODE] Output parameters (out) untuk return multiple values
        // [DESIGN PATTERN] Dictionary-like counting logic
        public static void GetStatusCounts(List<Obat> list, out int available, out int lowStock, out int expired)
        {
            available = lowStock = expired = 0;
            if (list == null) return;  // [SECURE CODE] Guard clause

            foreach (var obat in list)
            {
                // [CLEAN CODE] If-else chain: cek status dan increment counter
                if (obat.Status == STATUS_EXPIRED) expired++;
                else if (obat.Status == STATUS_LOW_STOCK) lowStock++;
                else available++;
            }
        }

        // [METHOD] Tampilkan notifikasi untuk obat expired & low stock
        // [CLEAN CODE] Guard clause: jika tidak ada masalah, return early
        // [CLEAN CODE] Ternary operator untuk conditional string building
        public static void ShowNotifications(List<Obat> list)
        {
            GetStatusCounts(list, out _, out var low, out var exp);  // [CLEAN CODE] Discard (_) untuk unused parameter
            if (exp + low == 0) return;  // Guard clause: tidak perlu notifikasi

            // [CLEAN CODE] String interpolation + ternary operator untuk conditional message
            string msg = (exp > 0 ? $"⚠️ Ada {exp} obat EXPIRED\n" : "") +
                         (low > 0 ? $"⚠️ Ada {low} obat STOK MENIPIS" : "");

            // [SECURE CODE] Check if message not empty sebelum show
            if (!string.IsNullOrEmpty(msg))
                MessageBox.Show(msg.Trim(), "Notifikasi Status Obat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // [METHOD] Format title dengan statistik status (emoji + count)
        // [CLEAN CODE] Guard clause untuk null check
        // [CLEAN CODE] String interpolation dengan emoji untuk user-friendly display
        public static string FormatTitleWithStats(string baseTitle, List<Obat> list)
        {
            if (list == null) return baseTitle;
            GetStatusCounts(list, out var avail, out var low, out var exp);
            // [STANDARD CODE] Emoji sebagai visual indicator: 🟢=Available, 🟡=LowStock, 🔴=Expired
            return $"{baseTitle} - 🟢 {avail} | 🟡 {low} | 🔴 {exp}";
        }
    }
}
