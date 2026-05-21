using System;
using System.Collections.Generic;
using System.Drawing;

namespace TubesKPL
{
    /// <summary>
    /// State Machine untuk menentukan status obat berdasarkan stok dan tanggal kadaluarsa.
    /// 
    /// Logic priority: Expired > SoonToExpire > LowStock > Available
    /// </summary>
    public static class ObatStateMachine
    {
        // Constants
        public const int LOW_STOCK_THRESHOLD = 5;
        public const int SOON_TO_EXPIRE_DAYS = 30;

        /// <summary>
        /// Calculate obat status based on stok and expired date.
        /// Priority: Expired (critical) > SoonToExpire > LowStock > Available (default)
        /// Boundary: SoonToExpire is 0 < days < 30 (strict: 30 days is still Available)
        /// </summary>
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

                // Priority 1: Expired (CRITICAL - must not be sold)
                if (expiredDate.Date < DateTime.Now.Date)
                    return "Expired";

                // Priority 2: SoonToExpire (strict boundary: 0 < days < 30)
                int daysUntilExpire = (int)(expiredDate.Date - DateTime.Now.Date).TotalDays;
                if (daysUntilExpire > 0 && daysUntilExpire < SOON_TO_EXPIRE_DAYS)
                    return "SoonToExpire";

                // Priority 3: LowStock (needs reorder)
                if (stok <= LOW_STOCK_THRESHOLD)
                    return "LowStock";

                // Priority 4: Available (default - safe)
                return "Available";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] CalculateStatus failed: {ex.Message}");
                return "Available"; // Safe default
            }
        }

        /// <summary>
        /// Get UI color for status.
        /// Defensive: invalid status returns safe green color.
        /// </summary>
        public static Color GetStatusColor(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                    return Color.FromArgb(200, 255, 200); // Green (safe default)

                switch (status.ToLower())
                {
                    case "expired":
                        return Color.FromArgb(255, 200, 200); // Red
                    case "soontoexpire":
                        return Color.FromArgb(255, 165, 0); // Orange
                    case "lowstock":
                        return Color.FromArgb(255, 255, 200); // Yellow
                    default:
                        return Color.FromArgb(200, 255, 200); // Green
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetStatusColor failed: {ex.Message}");
                return Color.FromArgb(200, 255, 200); // Green (safe default)
            }
        }

        /// <summary>
        /// Convert string status to enum.
        /// Defensive: invalid input returns Available (safe default).
        /// </summary>
        public static StatusObat GetStatusEnum(string statusString)
        {
            try
            {
                if (string.IsNullOrEmpty(statusString))
                    return StatusObat.Available;

                switch (statusString.ToLower())
                {
                    case "available":
                        return StatusObat.Available;
                    case "soontoexpire":
                        return StatusObat.SoonToExpire;
                    case "lowstock":
                        return StatusObat.LowStock;
                    case "expired":
                        return StatusObat.Expired;
                    default:
                        return StatusObat.Available;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetStatusEnum failed: {ex.Message}");
                return StatusObat.Available;
            }
        }

        /// <summary>
        /// Update status for all obat in list.
        /// Error in one item does not stop processing others.
        /// </summary>
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
        /// Get count summary of obat by status.
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
                            default:
                                available++;
                                break;
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
    }
}
