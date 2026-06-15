using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TubesKPL
{
    public static class StateMachine
    {
        public const string STATUS_EXPIRED = "Expired";
        public const string STATUS_LOW_STOCK = "LowStock";
        public const string STATUS_AVAILABLE = "Available";
        public const int LOW_STOCK_THRESHOLD = 5;

        private class StatusConfig
        {
            public string Name { get; set; }
            public Color Color { get; set; }
            public Func<Obat, bool> Condition { get; set; }
        }

        private static readonly StatusConfig[] StatusTable = new[]
        {
            new StatusConfig { Name = STATUS_EXPIRED, Color = Color.FromArgb(220, 53, 69), Condition = o => o.ExpiredDate < DateTime.Now },
            new StatusConfig { Name = STATUS_LOW_STOCK, Color = Color.FromArgb(255, 193, 7), Condition = o => o.Stok <= LOW_STOCK_THRESHOLD },
            new StatusConfig { Name = STATUS_AVAILABLE, Color = Color.FromArgb(40, 167, 69), Condition = o => true }
        };

        public static void EvaluateStatus(Obat obat)
        {
            if (obat != null) obat.Status = StatusTable.First(cfg => cfg.Condition(obat)).Name;
        }

        public static Color GetStatusColor(string status) => StatusTable.FirstOrDefault(cfg => cfg.Name == status)?.Color ?? Color.White;

        public static void ApplyStatusColors(DataGridView grid, int colIndex = 4)
        {
            if (grid?.Rows.Count == 0) return;

            foreach (DataGridViewRow row in grid.Rows)
            {
                string status = row.Cells[colIndex].Value?.ToString() ?? STATUS_AVAILABLE;
                row.DefaultCellStyle.BackColor = GetStatusColor(status);
            }
        }

        public static void GetStatusCounts(List<Obat> list, out int available, out int lowStock, out int expired)
        {
            available = lowStock = expired = 0;
            if (list == null) return;

            available = list.Count(o => o.Status == STATUS_AVAILABLE);
            lowStock = list.Count(o => o.Status == STATUS_LOW_STOCK);
            expired = list.Count(o => o.Status == STATUS_EXPIRED);
        }

        public static void ShowNotifications(List<Obat> list)
        {
            GetStatusCounts(list, out _, out var low, out var exp);
            if (exp + low == 0) return;

            string msg = (exp > 0 ? $"⚠️ Ada {exp} obat EXPIRED\n" : "") + (low > 0 ? $"⚠️ Ada {low} obat STOK MENIPIS" : "");
            if (!string.IsNullOrEmpty(msg)) MessageBox.Show(msg.Trim(), "Notifikasi Status Obat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static string FormatTitleWithStats(string baseTitle, List<Obat> list)
        {
            if (list == null) return baseTitle;
            GetStatusCounts(list, out var avail, out var low, out var exp);
            return $"{baseTitle} - 🟢 {avail} | 🟡 {low} | 🔴 {exp}";
        }
    }
}
