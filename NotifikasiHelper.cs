using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static TubesKPL.Obat;

namespace TubesKPL
{
    // [AYONDI] NotifikasiHelper adalah utility class untuk menampilkan notifikasi
    // Dapat menampilkan notifikasi berdasarkan status obat
    // Kompatibel dengan mode API maupun mode lokal

    /// <summary>
    /// [AYONDI] Class NotifikasiHelper
    /// Menyediakan method-method untuk menampilkan berbagai tipe notifikasi
    /// Fokus pada notifikasi status obat (expired, lowstock)
    /// </summary>
    public static class NotifikasiHelper
    {
        // [AYONDI] Method untuk show notifikasi obat expired
        /// <summary>
        /// [AYONDI] Tampilkan notifikasi untuk obat yang expired
        /// Parameter: List<Obat> daftarObat - list obat untuk di-check
        /// </summary>
        public static void TampilkanNotifikasiExpired(List<Obat> daftarObat)
        {
            // [AYONDI] Filter obat dengan status Expired
            var obatExpired = daftarObat.FindAll(o => o.status == StatusObat.Expired);

            if (obatExpired.Count > 0)
            {
                // [AYONDI] Build pesan: header + list obat
                string pesan = "\u26a0\ufe0f PERINGATAN: Obat Expired (" + obatExpired.Count + ")\n\n";
                for (int i = 0; i < obatExpired.Count; i++)
                {
                    pesan += "- " + obatExpired[i].nama + " (expired: " + obatExpired[i].expiredDate.ToString("dd/MM/yyyy") + ")";
                    if (i < obatExpired.Count - 1)
                    {
                        pesan += "\n";
                    }
                }

                // [AYONDI] Show MessageBox dengan warning icon
                MessageBox.Show(pesan, "Obat Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // [AYONDI] Method untuk show notifikasi obat low stock
        /// <summary>
        /// [AYONDI] Tampilkan notifikasi untuk obat dengan stok rendah
        /// Parameter: List<Obat> daftarObat - list obat untuk di-check
        /// </summary>
        public static void TampilkanNotifikasiLowStock(List<Obat> daftarObat)
        {
            // [AYONDI] Filter obat dengan status LowStock
            var obatLowStock = daftarObat.FindAll(o => o.status == StatusObat.LowStock);

            if (obatLowStock.Count > 0)
            {
                // [AYONDI] Build pesan: header + list obat
                string pesan = "\u26a1 PERINGATAN: Stok Rendah (" + obatLowStock.Count + ")\n\n";
                for (int i = 0; i < obatLowStock.Count; i++)
                {
                    pesan += "- " + obatLowStock[i].nama + " (stok: " + obatLowStock[i].stok + ")";
                    if (i < obatLowStock.Count - 1)
                    {
                        pesan += "\n";
                    }
                }

                // [AYONDI] Show MessageBox dengan warning icon
                MessageBox.Show(pesan, "Stok Rendah", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // [AYONDI] Method untuk show notifikasi ringkas (summary)
        /// <summary>
        /// [AYONDI] Tampilkan notifikasi ringkas berisi summary semua status
        /// Parameter: List<Obat> daftarObat - list obat untuk di-check
        /// </summary>
        public static void TampilkanNotifikasiSummary(List<Obat> daftarObat)
        {
            // [AYONDI] Count jumlah obat per status
            int available = daftarObat.FindAll(o => o.status == StatusObat.Available).Count;
            int lowStock = daftarObat.FindAll(o => o.status == StatusObat.LowStock).Count;
            int expired = daftarObat.FindAll(o => o.status == StatusObat.Expired).Count;

            // [AYONDI] Build pesan summary
            string pesan = "SUMMARY STATUS OBAT\n\n";
            pesan += "\u2713 Available: " + available + "\n";
            pesan += "\u26a1 Low Stock: " + lowStock + "\n";
            pesan += "\u26a0\ufe0f Expired: " + expired + "\n\n";
            pesan += "Total: " + daftarObat.Count;

            // [AYONDI] Show MessageBox dengan information icon
            MessageBox.Show(pesan, "Status Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // [AYONDI] Method untuk show notifikasi dari API response
        /// <summary>
        /// [AYONDI] Tampilkan notifikasi berdasarkan StatusSummaryResponse dari API
        /// Parameter: StatusSummaryResponse response - response dari API
        /// </summary>
        public static void TampilkanNotifikasiDariApi(StatusSummaryResponse response)
        {
            // [AYONDI] Build pesan dari API response
            string pesan = "API SUMMARY STATUS OBAT\n\n";
            pesan += "\u2713 Available: " + response.TotalAvailable + "\n";
            pesan += "\u26a1 Low Stock: " + response.TotalLowStock + "\n";
            pesan += "\u26a0\ufe0f Expired: " + response.TotalExpired + "\n\n";
            pesan += "Total: " + response.TotalObat + "\n";
            pesan += "Response Time: " + response.ResponseTime.ToString("HH:mm:ss");

            // [AYONDI] Show MessageBox
            MessageBox.Show(pesan, "API Status Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // [AYONDI] Method untuk show success notification
        /// <summary>
        /// [AYONDI] Tampilkan notifikasi sukses dengan pesan custom
        /// Parameter: string message - pesan yang ingin ditampilkan
        /// </summary>
        public static void TampilkanNotifikasiSukses(string message)
        {
            MessageBox.Show(message, "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // [AYONDI] Method untuk show error notification
        /// <summary>
        /// [AYONDI] Tampilkan notifikasi error dengan pesan custom
        /// Parameter: string message - pesan error
        /// </summary>
        public static void TampilkanNotifikasiError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // [AYONDI] Method untuk show warning notification
        /// <summary>
        /// [AYONDI] Tampilkan notifikasi warning dengan pesan custom
        /// Parameter: string message - pesan warning
        /// </summary>
        public static void TampilkanNotifikasiWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
