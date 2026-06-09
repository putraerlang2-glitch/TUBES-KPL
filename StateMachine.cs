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
            new StatusConfig 
            { 
                Name = STATUS_EXPIRED, 
                Color = Color.FromArgb(220, 53, 69), 
                Condition = o => o.ExpiredDate < DateTime.Now 
            },
            new StatusConfig 
            { 
                Name = STATUS_LOW_STOCK, 
                Color = Color.FromArgb(255, 193, 7), 
                Condition = o => o.Stok <= LOW_STOCK_THRESHOLD 
            },
            new StatusConfig 
            { 
                Name = STATUS_AVAILABLE, 
                Color = Color.FromArgb(40, 167, 69), 
                Condition = o => true 
            }
        };


        public static void EvaluateStatus(Obat obat)
        {
            if (obat == null) return;
            obat.Status = StatusTable.First(cfg => cfg.Condition(obat)).Name;
        }

        public static Color GetStatusColor(string status)
        {
            return StatusTable.FirstOrDefault(cfg => cfg.Name == status)?.Color ?? Color.White;
        }

        public static void ApplyStatusColors(DataGridView grid, int colIndex = 4)
        {
            if (grid?.Rows.Count == 0) return;

            for (int i = 0; i < grid.Rows.Count; i++)
            {
                string status = grid.Rows[i].Cells[colIndex].Value?.ToString() ?? STATUS_AVAILABLE;
                grid.Rows[i].DefaultCellStyle.BackColor = GetStatusColor(status);
            }
        }

        public static void GetStatusCounts(List<Obat> list, out int available, out int lowStock, out int expired)
        {
            available = lowStock = expired = 0;
            if (list == null) return;

            foreach (var obat in list)
            {
                if (obat.Status == STATUS_EXPIRED) expired++;
                else if (obat.Status == STATUS_LOW_STOCK) lowStock++;
                else available++;
            }
        }

        public static void ShowNotifications(List<Obat> list)
        {
            GetStatusCounts(list, out _, out var low, out var exp);
            if (exp + low == 0) return;

            string msg = (exp > 0 ? $"⚠️ Ada {exp} obat EXPIRED\n" : "") +
                         (low > 0 ? $"⚠️ Ada {low} obat STOK MENIPIS" : "");

            if (!string.IsNullOrEmpty(msg))
                MessageBox.Show(msg.Trim(), "Notifikasi Status Obat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static string FormatTitleWithStats(string baseTitle, List<Obat> list)
        {
            if (list == null) return baseTitle;
            GetStatusCounts(list, out var avail, out var low, out var exp);
            return $"{baseTitle} - 🟢 {avail} | 🟡 {low} | 🔴 {exp}";
        }
    }
}
