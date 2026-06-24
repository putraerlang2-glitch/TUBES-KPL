using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TubesKPL
{
    public static class StrukGenerator
    {
        private const string SEPARATOR_THICK = "======================================";
        private const string SEPARATOR_THIN = "--------------------------------------";
        private const string HEADER_TITLE = "           APOTEK TUBES KPL           ";
        private const string FOOTER_MESSAGE = "     TERIMA KASIH, SEMOGA LEKAS SEMBUH  ";

        public static string BuildStruk(List<ItemTransaksi> keranjang, decimal subtotal, decimal pajak, decimal diskon, decimal totalAkhir, decimal bayar, decimal kembalian)
        {
            StringBuilder sb = new StringBuilder();

            AppendHeader(sb);
            AppendItems(sb, keranjang);
            AppendSummary(sb, subtotal, pajak, diskon, totalAkhir, bayar, kembalian);
            AppendFooter(sb);

            return sb.ToString();
        }

        public static void GenerateStruk(List<ItemTransaksi> keranjang, decimal subtotal, decimal pajak, decimal diskon, decimal totalAkhir, decimal bayar, decimal kembalian)
        {
            string struk = BuildStruk(keranjang, subtotal, pajak, diskon, totalAkhir, bayar, kembalian);
            SaveToFile(struk);
        }

        private static void AppendHeader(StringBuilder sb)
        {
            sb.AppendLine(SEPARATOR_THICK);
            sb.AppendLine(HEADER_TITLE);
            sb.AppendLine(SEPARATOR_THICK);
            sb.AppendLine($"Tanggal : {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            AppendSeparator(sb, false);
        }

        private static void AppendItems(StringBuilder sb, List<ItemTransaksi> keranjang)
        {
            foreach (var item in keranjang)
            {
                sb.AppendLine(item.Obat.Nama);
                sb.AppendLine($"{item.Jumlah} x {FormatCurrency(item.Obat.Harga)} = {FormatCurrency(item.Subtotal())}");
            }
        }

        private static void AppendSummary(StringBuilder sb, decimal subtotal, decimal pajak, decimal diskon, decimal totalAkhir, decimal bayar, decimal kembalian)
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

        private static void AppendFooter(StringBuilder sb)
        {
            sb.AppendLine(SEPARATOR_THICK);
            sb.AppendLine(FOOTER_MESSAGE);
            sb.AppendLine(SEPARATOR_THICK);
        }

        private static void AppendSeparator(StringBuilder sb, bool thick)
        {
            sb.AppendLine(thick ? SEPARATOR_THICK : SEPARATOR_THIN);
        }

        private static void AddRow(StringBuilder sb, string label, decimal value)
        {
            sb.AppendLine($"{label.PadRight(17)}: {FormatCurrency(value)}");
        }

        private static void AddRowWithPrefix(StringBuilder sb, string label, decimal value, string prefix)
        {
            sb.AppendLine($"{label.PadRight(17)}: {prefix}{FormatCurrency(value)}");
        }

        private static string FormatCurrency(decimal amount)
        {
            return $"Rp {amount:N0}";
        }

        private static void SaveToFile(string content)
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