using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TubesKPL
{
    public sealed class StrukGenerator
    {
        // Implementasi Singleton yang thread-safe menggunakan Lazy<T>
        private static readonly Lazy<StrukGenerator> _instance = 
            new Lazy<StrukGenerator>(() => new StrukGenerator());
        
        public static StrukGenerator Instance => _instance.Value;

        private const string SEPARATOR_THICK = "======================================";
        private const string SEPARATOR_THIN = "--------------------------------------";
        private const string HEADER_TITLE = "           APOTEK TUBES KPL           ";
        private const string FOOTER_MESSAGE = "     TERIMA KASIH, SEMOGA LEKAS SEMBUH  ";

        private StrukGenerator()
        {
        }

        // --- WRAPPER STATIC ---
        // Dipertahankan agar form atau class lain yang masih pakai pemanggilan lama
        // (StrukGenerator.GenerateStruk) tidak langsung error saat transisi.
        public static string BuildStruk(List<ItemTransaksi> keranjang, decimal subtotal, decimal pajak, decimal diskon, decimal totalAkhir, decimal bayar, decimal kembalian)
        {
            return Instance.BuildStrukInstance(keranjang, subtotal, pajak, diskon, totalAkhir, bayar, kembalian);
        }

        public static void GenerateStruk(List<ItemTransaksi> keranjang, decimal subtotal, decimal pajak, decimal diskon, decimal totalAkhir, decimal bayar, decimal kembalian)
        {
            Instance.GenerateStrukInstance(keranjang, subtotal, pajak, diskon, totalAkhir, bayar, kembalian);
        }

        // --- INSTANCE METHODS ---
        public string BuildStrukInstance(List<ItemTransaksi> keranjang, decimal subtotal, decimal pajak, decimal diskon, decimal totalAkhir, decimal bayar, decimal kembalian)
        {
            StringBuilder sb = new StringBuilder();

            AppendHeader(sb);
            AppendItems(sb, keranjang);
            AppendSummary(sb, subtotal, pajak, diskon, totalAkhir, bayar, kembalian);
            AppendFooter(sb);

            return sb.ToString();
        }

        public void GenerateStrukInstance(List<ItemTransaksi> keranjang, decimal subtotal, decimal pajak, decimal diskon, decimal totalAkhir, decimal bayar, decimal kembalian)
        {
            string struk = BuildStrukInstance(keranjang, subtotal, pajak, diskon, totalAkhir, bayar, kembalian);
            SaveToFile(struk);
        }
    
        private void AppendHeader(StringBuilder sb)
        {
            sb.AppendLine(SEPARATOR_THICK);
            sb.AppendLine(HEADER_TITLE);
            sb.AppendLine(SEPARATOR_THICK);
            sb.AppendLine($"Tanggal : {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            AppendSeparator(sb, false);
        }

        private void AppendItems(StringBuilder sb, List<ItemTransaksi> keranjang)
        {
            foreach (var item in keranjang)
            {
                sb.AppendLine(item.obat.nama);
                sb.AppendLine($"{item.jumlah} x {FormatCurrency(item.obat.harga)} = {FormatCurrency(item.Subtotal())}");
            }
        }

        private void AppendSummary(StringBuilder sb, decimal subtotal, decimal pajak, decimal diskon, decimal totalAkhir, decimal bayar, decimal kembalian)
        {
            AppendSeparator(sb, false);
            AddRow(sb, "Subtotal", subtotal);

            decimal nominalDiskon = subtotal * diskon;
            decimal nominalPajak = (subtotal - nominalDiskon) * pajak;

            AddRowWithPrefix(sb, $"Diskon ({(diskon * 100):0}%)", nominalDiskon, "- ");
            AddRowWithPrefix(sb, $"Pajak PPN ({(pajak * 100):0}%)", nominalPajak, "+ ");
            AppendSeparator(sb, false);

            AddRow(sb, "TOTAL BAYAR", totalAkhir);
            AddRow(sb, "UANG DITERIMA", bayar);
            AddRow(sb, "KEMBALIAN", kembalian);
        }

        private void AppendFooter(StringBuilder sb)
        {
            sb.AppendLine(SEPARATOR_THICK);
            sb.AppendLine(FOOTER_MESSAGE);
            sb.AppendLine(SEPARATOR_THICK);
        }

        private void AppendSeparator(StringBuilder sb, bool thick)
        {
            sb.AppendLine(thick ? SEPARATOR_THICK : SEPARATOR_THIN);
        }

        private void AddRow(StringBuilder sb, string label, decimal value)
        {
            sb.AppendLine($"{label.PadRight(17)}: {FormatCurrency(value)}");
        }

        private void AddRowWithPrefix(StringBuilder sb, string label, decimal value, string prefix)
        {
            sb.AppendLine($"{label.PadRight(17)}: {prefix}{FormatCurrency(value)}");
        }

        private string FormatCurrency(decimal amount)
        {
            return $"Rp {amount:N0}";
        }

        private void SaveToFile(string content)
        {
            try
            {
                string namaFile = $"Struk_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                File.WriteAllText(namaFile, content);
                MessageBox.Show($"Transaksi Berhasil!\nStruk disimpan di folder project dengan nama:\n{namaFile}", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menyimpan struk: {ex.Message}");
            }
        }
    }
}