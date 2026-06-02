using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TubesKPL
{
    /// <summary>
    /// StateMachine: Menampung seluruh table-driven logic untuk status obat
    /// - StateRuleTable definition
    /// - Status evaluation based on rules
    /// - Color mapping
    /// - Notification logic
    /// </summary>
    public static class StateMachine
    {
        /// <summary>
        /// State Rule definition untuk table-driven pattern
        /// </summary>
        public class StateRule
        {
            public string StatusName { get; set; }
            public int Priority { get; set; }
            public string ConditionType { get; set; }  // "ExpiredDate", "Stok", "None"
            public string Operator { get; set; }        // "<", "<=", ">", ">=", "=="
            public int Threshold { get; set; }
            public Color RowColor { get; set; }
        }

        /// <summary>
        /// STATE RULES TABLE - Table-driven pattern untuk status obat
        /// </summary>
        private static readonly List<StateRule> StateRulesTable = new List<StateRule>
        {
            // Rule 1: Expired (Priority 1)
            new StateRule
            {
                StatusName = "Expired",
                Priority = 1,
                ConditionType = "ExpiredDate",
                Operator = "<",
                Threshold = 0,
                RowColor = Color.FromArgb(255, 200, 200)  // Light red #FFC8C8
            },

            // Rule 2: LowStock (Priority 2)
            new StateRule
            {
                StatusName = "LowStock",
                Priority = 2,
                ConditionType = "Stok",
                Operator = "<=",
                Threshold = 5,
                RowColor = Color.FromArgb(255, 255, 200)  // Light yellow #FFFFC8
            },

            // Rule 3: Available (Priority 3 - fallback)
            new StateRule
            {
                StatusName = "Available",
                Priority = 3,
                ConditionType = "None",
                Operator = "==",
                Threshold = 0,
                RowColor = Color.FromArgb(200, 255, 200)  // Light green #C8FFC8
            }
        };

        /// <summary>
        /// Update status obat berdasarkan StateRulesTable
        /// Evaluasi rules berdasarkan priority order
        /// </summary>
        public static void EvaluateStatus(Obat obat)
        {
            if (obat == null)
                return;

            // Evaluasi rules berdasarkan priority
            var activeRules = StateRulesTable
                .OrderBy(r => r.Priority)
                .ToList();

            foreach (var rule in activeRules)
            {
                if (EvaluateRule(obat, rule))
                {
                    obat.Status = rule.StatusName;
                    return;
                }
            }

            // Fallback ke Available
            obat.Status = "Available";
        }

        /// <summary>
        /// Evaluasi apakah obat memenuhi kondisi rule
        /// </summary>
        private static bool EvaluateRule(Obat obat, StateRule rule)
        {
            switch (rule.ConditionType)
            {
                case "ExpiredDate":
                    return EvaluateDateCondition(obat.ExpiredDate, rule.Operator);

                case "Stok":
                    return EvaluateStokCondition(obat.Stok, rule.Operator, rule.Threshold);

                case "None":
                    return true;  // Always match (fallback)

                default:
                    return false;
            }
        }

        /// <summary>
        /// Helper: Evaluasi kondisi tanggal expired
        /// </summary>
        private static bool EvaluateDateCondition(DateTime expiredDate, string op)
        {
            switch (op)
            {
                case "<":
                    return expiredDate < DateTime.Now;
                case "<=":
                    return expiredDate <= DateTime.Now;
                case ">":
                    return expiredDate > DateTime.Now;
                case ">=":
                    return expiredDate >= DateTime.Now;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Helper: Evaluasi kondisi stok
        /// </summary>
        private static bool EvaluateStokCondition(int stok, string op, int threshold)
        {
            switch (op)
            {
                case "<":
                    return stok < threshold;
                case "<=":
                    return stok <= threshold;
                case ">":
                    return stok > threshold;
                case ">=":
                    return stok >= threshold;
                case "==":
                    return stok == threshold;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Ambil color untuk status tertentu dari StateRulesTable
        /// </summary>
        public static Color GetColorForStatus(string status)
        {
            var rule = StateRulesTable.FirstOrDefault(r => r.StatusName == status);
            return rule?.RowColor ?? Color.White;
        }

        /// <summary>
        /// Apply warna ke DataGrid berdasarkan status
        /// </summary>
        public static void ApplyStatusColors(DataGridView dataGridView, int statusColumnIndex = 5)
        {
            if (dataGridView == null || dataGridView.Rows.Count == 0)
                return;

            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                var cell = dataGridView.Rows[i].Cells[statusColumnIndex];
                string status = cell.Value?.ToString() ?? "Available";

                Color color = GetColorForStatus(status);
                dataGridView.Rows[i].DefaultCellStyle.BackColor = color;
            }
        }

        /// <summary>
        /// Hitung statistik status dari list obat
        /// </summary>
        public static Dictionary<string, int> GetStatusStatistics(List<Obat> daftarObat)
        {
            var stats = new Dictionary<string, int>();

            foreach (var rule in StateRulesTable)
            {
                int count = daftarObat.Count(o => o.Status == rule.StatusName);
                stats[rule.StatusName] = count;
            }

            return stats;
        }

        /// <summary>
        /// Tampilkan notifikasi untuk status yang perlu perhatian
        /// </summary>
        public static void ShowNotifications(List<Obat> daftarObat)
        {
            var expiredObats = daftarObat.Where(o => o.Status == "Expired").ToList();
            var lowStockObats = daftarObat.Where(o => o.Status == "LowStock").ToList();

            string message = "";

            // Notifikasi Expired
            if (expiredObats.Count > 0)
            {
                message += $"⚠️ {expiredObats.Count} obat EXPIRED:\n";
                message += string.Join("\n", expiredObats.Select(o => $"  • {o.Nama}"));
                message += "\n\n";
            }

            // Notifikasi Low Stock
            if (lowStockObats.Count > 0)
            {
                message += $"⚠️ {lowStockObats.Count} obat STOK MENIPIS:\n";
                message += string.Join("\n", lowStockObats.Select(o => $"  • {o.Nama} (Stok: {o.Stok})"));
            }

            // Tampilkan notifikasi jika ada
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(
                    message,
                    "Notifikasi Status Obat",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        /// <summary>
        /// Format title bar dengan statistik status
        /// </summary>
        public static string FormatTitleWithStats(string baseTitle, List<Obat> daftarObat)
        {
            var stats = GetStatusStatistics(daftarObat);

            string available = stats.ContainsKey("Available") ? stats["Available"].ToString() : "0";
            string lowStock = stats.ContainsKey("LowStock") ? stats["LowStock"].ToString() : "0";
            string expired = stats.ContainsKey("Expired") ? stats["Expired"].ToString() : "0";

            return $"{baseTitle} - 🟢 {available} | 🟡 {lowStock} | 🔴 {expired}";
        }

        /// <summary>
        /// Get StateRulesTable (untuk reference/dokumentasi)
        /// </summary>
        public static IReadOnlyList<StateRule> GetStateRulesTable()
        {
            return StateRulesTable.AsReadOnly();
        }

        /// <summary>
        /// AggregateStatusCount - Table-Driven aggregation menggunakan Dictionary Counter Table
        /// Refactor dari Control-Driven (switch-case) menjadi pure Table-Driven pattern
        /// 
        /// Menerapkan prinsip:
        /// - Defensive Programming: null checks dan out parameter initialization
        /// - Dictionary-based Counter Table: Direct Access tanpa conditional logic
        /// - StringComparer.OrdinalIgnoreCase: Case-insensitive key matching
        /// 
        /// Parameter:
        ///   obatList: List obat yang akan diagregasi
        ///   available: Output parameter - jumlah obat Available
        ///   lowStock: Output parameter - jumlah obat LowStock
        ///   expired: Output parameter - jumlah obat Expired
        /// </summary>
        public static void AggregateStatusCount(
            List<Obat> obatList,
            out int available,
            out int lowStock,
            out int expired)
        {
            // Defensive Programming: Initialize out parameters dengan nilai default
            available = 0;
            lowStock = 0;
            expired = 0;

            // Defensive Programming: Check null obatList untuk menghindari NullReferenceException
            if (obatList == null)
            {
                return;
            }

            // Table-Driven: Counter Table berbasis Dictionary dengan OrdinalIgnoreCase
            // Menggantikan sepenuhnya struktur switch-case yang Control-Driven
            var counterTable = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "Available", 0 },
                { "LowStock", 0 },
                { "Expired", 0 }
            };

            // Iterasi forEach: Setiap obat dievaluasi status-nya
            foreach (var obat in obatList)
            {
                // Pemanggilan method EvaluateStatus untuk mendapatkan status string
                string status = obat.Status ?? "Available";

                // Direct Access: Menggunakan status string sebagai key untuk increment counter
                // Tidak perlu if-else chain, langsung akses dictionary dan increment
                if (counterTable.ContainsKey(status))
                {
                    counterTable[status]++;
                }
                else
                {
                    // Fallback untuk status yang tidak dikenali
                    counterTable["Available"]++;
                }
            }

            // One-way data extraction: Ambil nilai dari Dictionary, assign ke out parameters
            available = counterTable["Available"];
            lowStock = counterTable["LowStock"];
            expired = counterTable["Expired"];
        }
    }
}
