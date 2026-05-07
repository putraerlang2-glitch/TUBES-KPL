using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TubesKPL
{
    // [LANGGA - CODE REUSE]
    public static class StrukGenerator
    {
        public static void GenerateStruk(List<ItemTransaksi> keranjang, decimal subtotal, decimal pajak, decimal diskon, decimal totalAkhir, decimal bayar, decimal kembalian)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("======================================");
            sb.AppendLine("           APOTEK TUBES KPL           ");
            sb.AppendLine("======================================");
            sb.AppendLine($"Tanggal : {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine("--------------------------------------");

            foreach (var item in keranjang)
            {
                sb.AppendLine(item.obat.nama);
                sb.AppendLine($"{item.jumlah} x Rp {item.obat.harga:N0} = Rp {item.Subtotal():N0}");
            }

            sb.AppendLine("--------------------------------------");
            sb.AppendLine($"Subtotal         : Rp {subtotal:N0}");

            decimal nominalDiskon = subtotal * diskon;
            decimal nominalPajak = (subtotal - nominalDiskon) * pajak;

            sb.AppendLine($"Diskon ({(diskon * 100):0}%)    : - Rp {nominalDiskon:N0}");
            sb.AppendLine($"Pajak PPN ({(pajak * 100):0}%) : + Rp {nominalPajak:N0}");
            sb.AppendLine("--------------------------------------");
            sb.AppendLine($"TOTAL BAYAR      : Rp {totalAkhir:N0}");
            sb.AppendLine($"UANG DITERIMA    : Rp {bayar:N0}");
            sb.AppendLine($"KEMBALIAN        : Rp {kembalian:N0}");
            sb.AppendLine("======================================");
            sb.AppendLine("     TERIMA KASIH, SEMOGA LEKAS SEMBUH  ");
            sb.AppendLine("======================================");

            try
            {
                string namaFile = $"Struk_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                File.WriteAllText(namaFile, sb.ToString());
                MessageBox.Show($"Transaksi Berhasil!\nStruk disimpan di folder project dengan nama:\n{namaFile}", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menyimpan struk: {ex.Message}");
            }
        }
    }
}