using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TubesKPL;

namespace TubesKPL
{
    public partial class FormTransaksi : Form
    {
        List<Obat> daftarObat;
        Form1 mainform;
        List<ItemTransaksi> keranjang = new List<ItemTransaksi>();

        public FormTransaksi(List<Obat> data, Form1 f1)
        {
            InitializeComponent();
            daftarObat = data;
            mainform = f1;

            BoxObat.DataSource = daftarObat;
            BoxObat.DisplayMember = "nama";
        }

        // BIARKAN KOSONG AGAR DESAIN TIDAK ERROR
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        // TOMBOL UBAH CONFIGURATOR
        private void BtnUbahConfig_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBPajak.Text) || string.IsNullOrWhiteSpace(TBDiskon.Text))
            {
                MessageBox.Show("Kotak Pajak dan Diskon harus diisi angka dulu ya!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (decimal.TryParse(TBPajak.Text, out decimal pajakInput) &&
                decimal.TryParse(TBDiskon.Text, out decimal diskonInput))
            {
                decimal pajakDesimal = pajakInput / 100m;
                decimal diskonDesimal = diskonInput / 100m;

                RuntimeConfig.UpdateConfig(pajakDesimal, diskonDesimal);

                MessageBox.Show($"Mantap! Konfigurasi berhasil diubah secara Runtime.\nPajak Baru: {pajakInput}%\nDiskon Baru: {diskonInput}%\n\nSilakan klik 'Bayar' untuk melihat perubahannya pada total belanja.", "Config Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Tolong masukkan format angka yang benar ya!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // TOMBOL BAYAR / HITUNG
        private void BtnHitung_Click(object sender, EventArgs e)
        {
            if (keranjang.Count == 0)
            {
                MessageBox.Show("Masukkan obat yang mau dibeli dulu");
                return;
            }

            decimal subtotal = 0;
            foreach (var item in keranjang)
            {
                subtotal += item.Subtotal();
            }

            // 2. Terapkan RUNTIME CONFIGURATOR
            decimal nominalDiskon = subtotal * RuntimeConfig.DiskonAktif;
            decimal subtotalSetelahDiskon = subtotal - nominalDiskon;
            decimal nominalPajak = subtotalSetelahDiskon * RuntimeConfig.PajakPPN;
            decimal grandTotal = subtotalSetelahDiskon + nominalPajak;

            // 3. Validasi Uang Bayar dari TBUangBayar
            if (!decimal.TryParse(TBUangBayar.Text, out decimal uangBayar))
            {
                MessageBox.Show("Masukkan angka nominal uang bayar yang valid!");
                return;
            }

            if (uangBayar < grandTotal)
            {
                MessageBox.Show($"Uang tidak cukup! Total Pembayaran: {grandTotal.ToString("C")}");
                return;
            }

            decimal kembalian = uangBayar - grandTotal;

            // 4. Update Stok Obat
            foreach (var item in keranjang)
            {
                item.obat.stok -= item.jumlah;
                item.obat.UpdateStatus();
            }

            // 5. Panggil CODE REUSE (Cetak Struk)
            StrukGenerator.GenerateStruk(keranjang, subtotal, RuntimeConfig.PajakPPN, RuntimeConfig.DiskonAktif, grandTotal, uangBayar, kembalian);

            // 6. Bersihkan Keranjang
            keranjang.Clear();
            RefreshKeranjang();
            mainform.RefreshData();
            TBUangBayar.Clear();
        }

        private void RefreshKeranjang()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Nama Obat");
            dt.Columns.Add("Harga");
            dt.Columns.Add("Jumlah");
            dt.Columns.Add("Sub total");

            foreach (var item in keranjang)
            {
                dt.Rows.Add(item.obat.nama, item.obat.harga.ToString("C"), item.jumlah, item.Subtotal().ToString("C"));
            }
            TabelKeranjang.DataSource = dt;
        }

        // TOMBOL TAMBAH KE KERANJANG
        private void button1_Click(object sender, EventArgs e)
        {
            int jumlah;
            Obat o = (Obat)BoxObat.SelectedItem;

            if (string.IsNullOrWhiteSpace(TBjumlah.Text))
            {
                MessageBox.Show("Masukkan obat yang mau di beli ya");
                return;
            }

            if (!int.TryParse(TBjumlah.Text, out jumlah))
            {
                MessageBox.Show("Tolong Masukkan Angka ya");
                return;
            }

            if (jumlah > o.stok)
            {
                MessageBox.Show("Stok tidak cukup");
                return;
            }

            ItemTransaksi item = new ItemTransaksi()
            {
                obat = o,
                jumlah = jumlah
            };

            keranjang.Add(item);
            RefreshKeranjang();
        }
    }

    public class ItemTransaksi
    {
        public Obat obat { get; set; }
        public int jumlah { get; set; }

        public decimal Subtotal()
        {
            return obat.harga * jumlah;
        }
    }
}