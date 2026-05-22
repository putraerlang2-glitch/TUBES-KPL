using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TubesKPL
{
    /// <summary>
    /// State Machine untuk menentukan status obat berdasarkan stok dan tanggal kadaluarsa.
    /// Menggunakan table-driven approach untuk rules dan transitions.
    /// 
    /// Logic priority: Expired > SoonToExpire > LowStock > Available
    /// </summary>
    public static class ObatStateMachine
    {
        // Constants
        public const int LOW_STOCK_THRESHOLD = 5;
        public const int SOON_TO_EXPIRE_DAYS = 30;

        /// <summary>
        /// State rule definition untuk table-driven approach
        /// </summary>
        private class StateRule
        {
            public string Status { get; set; }
            public StatusObat StatusEnum { get; set; }
            public int Priority { get; set; } // Semakin tinggi, semakin diutamakan
            public Color DisplayColor { get; set; }
            public Func<int, DateTime, bool> EvaluateCondition { get; set; } // (stok, expiredDate) => bool
            public string Description { get; set; }
        }

        
        /// Tabel rules untuk semua status obat - ini adalah "state machine table"
        /// Diurutkan berdasarkan priority (descending) untuk evaluasi yang benar
       
        private static readonly List<StateRule> StateRulesTable = new List<StateRule>
        {
            // Priority 1 (CRITICAL): Expired - harus tidak dijual
            new StateRule
            {
                Status = "Expired",
                StatusEnum = StatusObat.Expired,
                Priority = 4,
                DisplayColor = Color.FromArgb(255, 200, 200), // Red
                EvaluateCondition = (stok, expiredDate) => 
                    expiredDate.Date < DateTime.Now.Date,
                Description = "Obat sudah kadaluarsa dan tidak boleh dijual"
            },

            // Priority 2: SoonToExpire - akan segera kadaluarsa (0 < days < 30)
            new StateRule
            {
                Status = "SoonToExpire",
                StatusEnum = StatusObat.SoonToExpire,
                Priority = 3,
                DisplayColor = Color.FromArgb(255, 165, 0), // Orange
                EvaluateCondition = (stok, expiredDate) =>
                {
                    int daysUntilExpire = (int)(expiredDate.Date - DateTime.Now.Date).TotalDays;
                    return daysUntilExpire > 0 && daysUntilExpire < SOON_TO_EXPIRE_DAYS;
                },
                Description = "Obat akan kadaluarsa dalam waktu 30 hari"
            },

            // Priority 3: LowStock - stok rendah, perlu reorder
            new StateRule
            {
                Status = "LowStock",
                StatusEnum = StatusObat.LowStock,
                Priority = 2,
                DisplayColor = Color.FromArgb(255, 255, 200), // Yellow
                EvaluateCondition = (stok, expiredDate) =>
                    stok <= LOW_STOCK_THRESHOLD,
                Description = "Stok obat rendah dan perlu dilakukan reorder"
            },

            // Priority 4 (DEFAULT): Available - kondisi normal dan aman
            new StateRule
            {
                Status = "Available",
                StatusEnum = StatusObat.Available,
                Priority = 1,
                DisplayColor = Color.FromArgb(200, 255, 200), // Green
                EvaluateCondition = (stok, expiredDate) =>
                    true, // Catch-all - selalu match sebagai default
                Description = "Obat tersedia dan dalam kondisi normal"
            }
        };

        // Pre-compute sorted table by priority untuk faster lookup
        private static readonly List<StateRule> SortedRulesTable = 
            StateRulesTable.OrderByDescending(r => r.Priority).ToList();

        // Lookup tables untuk O(1) color dan enum access
        private static readonly Dictionary<string, Color> ColorLookup = 
            StateRulesTable.ToDictionary(r => r.Status.ToLower(), r => r.DisplayColor);

        private static readonly Dictionary<string, StatusObat> EnumLookup =
            StateRulesTable.ToDictionary(r => r.Status.ToLower(), r => r.StatusEnum);

       
        public static string CalculateStatus(int stok, DateTime expiredDate)
        {
            try
            {
                // Defensive: normalize invalid inputs
                if (stok < 0)
                {
                    Console.WriteLine($"[WARNING] CalculateStatus: stok negative ({stok}), normalized to 0.");
                    stok = 0;
                }

                if (expiredDate == DateTime.MinValue || expiredDate == default(DateTime))
                {
                    Console.WriteLine("[WARNING] CalculateStatus: expiredDate invalid, return Available.");
                    return "Available";
                }

                // Table-driven evaluation: iterate rules dalam priority order
                foreach (var rule in SortedRulesTable)
                {
                    if (rule.EvaluateCondition(stok, expiredDate))
                    {
                        return rule.Status; // Return status dari rule yang match pertama
                    }
                }

                // Should not reach here, tapi sebagai safety fallback
                return "Available";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] CalculateStatus failed: {ex.Message}");
                return "Available"; // Safe default
            }
        }

        
        /// Get UI color for status menggunakan table lookup.
        /// O(1) lookup dari ColorLookup dictionary.
       
        public static Color GetStatusColor(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                    return Color.FromArgb(200, 255, 200); // Green (safe default)

                string statusLower = status.ToLower();

                // Table lookup: O(1) access
                if (ColorLookup.TryGetValue(statusLower, out var color))
                {
                    return color;
                }

                return Color.FromArgb(200, 255, 200); // Green (safe default untuk unknown status)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetStatusColor failed: {ex.Message}");
                return Color.FromArgb(200, 255, 200); // Green (safe default)
            }
        }

        
        /// Convert string status to enum menggunakan table lookup.
       
     
        public static StatusObat GetStatusEnum(string statusString)
        {
            try
            {
                if (string.IsNullOrEmpty(statusString))
                    return StatusObat.Available;

                string statusLower = statusString.ToLower();

                // Table lookup: O(1) access
                if (EnumLookup.TryGetValue(statusLower, out var statusEnum))
                {
                    return statusEnum;
                }

                return StatusObat.Available;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetStatusEnum failed: {ex.Message}");
                return StatusObat.Available;
            }
        }

        
        /// Update status untuk semua obat dalam list.
        /// Error di satu item tidak menghentikan processing item lainnya.
        
        public static void UpdateAllStatus(List<Obat> obatList)
        {
            try
            {
                if (obatList == null)
                {
                    Console.WriteLine("[WARNING] UpdateAllStatus: list is null.");
                    return;
                }

                int successCount = 0;
                int failureCount = 0;

                foreach (var obat in obatList)
                {
                    if (obat == null)
                    {
                        failureCount++;
                        continue;
                    }

                    try
                    {
                        // Gunakan table-driven CalculateStatus
                        string statusStr = CalculateStatus(obat.Stok, obat.ExpiredDate);
                        obat.Status = GetStatusEnum(statusStr);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        failureCount++;
                        Console.WriteLine($"[ERROR] UpdateAllStatus: failed to update '{obat?.Nama}': {ex.Message}");
                    }
                }

                if (failureCount > 0 || successCount > 0)
                    Console.WriteLine($"[INFO] UpdateAllStatus: {successCount} succeeded, {failureCount} failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] UpdateAllStatus failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Get count summary obat berdasarkan status menggunakan table-driven evaluation.
        /// </summary>
        public static void GetStatusSummary(List<Obat> obatList, out int available,
            out int lowStock, out int expired, out int soonToExpire)
        {
            available = lowStock = expired = soonToExpire = 0;

            try
            {
                if (obatList == null)
                    return;

                foreach (var obat in obatList)
                {
                    if (obat == null)
                        continue;

                    try
                    {
                        // Gunakan table-driven CalculateStatus
                        string status = CalculateStatus(obat.Stok, obat.ExpiredDate);

                        // Map status ke counter - menggunakan table untuk konsistensi
                        var matchingRule = StateRulesTable.FirstOrDefault(r => r.Status == status);
                        if (matchingRule != null)
                        {
                            switch (matchingRule.Status)
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
                                case "Available":
                                    available++;
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] GetStatusSummary: failed to process '{obat?.Nama}': {ex.Message}");
                        available++; // Safe default
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetStatusSummary failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Debug helper: print all state rules untuk verification
        /// </summary>
        public static void PrintStateRulesTable()
        {
            Console.WriteLine("");
            foreach (var rule in StateRulesTable.OrderByDescending(r => r.Priority))
            {
                Console.WriteLine($"[Priority {rule.Priority}] {rule.Status}: {rule.Description}");
            }
            Console.WriteLine("\n");
        }
    }
}

