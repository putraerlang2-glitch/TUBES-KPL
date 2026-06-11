using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net.NetworkInformation;
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
            BoxObat.DisplayMember = "Nama";

            label8.Text = "";
        }

    private void TampikanPesanLabel(string pesan, Color warna)
        {
            label8.Text = pesan;
            label8.ForeColor = warna;
        }

        // BIARKAN KOSONG AGAR DESAIN TIDAK ERROR
        private void txtCariInputan_TextChanged(object sender, EventArgs e) { }
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
        private async void BtnHitung_Click(object sender, EventArgs e)
        {
            if (keranjang.Count == 0)
            {
                TampikanPesanLabel("Masukkan obat yang mau dibeli dulu", Color.Red);
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
                TampikanPesanLabel("Masukkan angka nominal uang bayar yang valid!", Color.Red);
                return;
            }

            if (uangBayar < grandTotal)
            {
                TampikanPesanLabel($"Uang tidak cukup! Total Pembayaran:" +
                    $" {grandTotal.ToString("C")}", Color.Red);
                return;
            }

            decimal kembalian = uangBayar - grandTotal;

            // 4. Kirim Data Transaksi ke MySQL via API
            try
            {
                var dto = new TransaksiDTO
                {
                    NoStruk = "TRX-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    TanggalTransaksi = DateTime.Now,
                    Subtotal = subtotal,
                    PersentaseDiskon = RuntimeConfig.DiskonAktif * 100m,
                    NominalDiskon = nominalDiskon,
                    PersentasePajak = RuntimeConfig.PajakPPN * 100m,
                    NominalPajak = nominalPajak,
                    TotalAkhir = grandTotal,
                    UangBayar = uangBayar,
                    UangKembalian = kembalian
                };

                foreach (var item in keranjang)
                {
                    dto.DetailList.Add(new TransaksiDetailDTO
                    {
                        ObatId = item.obat.Id,
                        Jumlah = item.jumlah,
                        HargaSatuan = item.obat.Harga,
                        Subtotal = item.Subtotal()
                    });
                }

                // Call API
                using (var client = new ObatApiClient("https://localhost:7245"))
                {
                    await client.CheckoutTransaksiAsync(dto);
                }

                // Update Stok Obat di memory lokal (karena sukses di server)
                foreach (var item in keranjang)
                {
                    item.obat.Stok -= item.jumlah;
                    item.obat.UpdateStatus();
                }

                // 5. Panggil CODE REUSE (Cetak Struk)
                StrukGenerator.GenerateStruk(keranjang, subtotal, RuntimeConfig.PajakPPN, RuntimeConfig.DiskonAktif, grandTotal, uangBayar, kembalian);

                MessageBox.Show("Transaksi Berhasil disimpan ke MySQL!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 6. Bersihkan Keranjang
                keranjang.Clear();
                RefreshKeranjang();
                mainform.RefreshData();
                TBUangBayar.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Transaksi Gagal:\n{ex.Message}", "Error API", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                dt.Rows.Add(item.obat.Nama, item.obat.Harga.ToString("C"), item.jumlah, item.Subtotal().ToString("C"));
            }
            TabelKeranjang.DataSource = dt;
        }

        // Notifikasi Validasi Transakasi
        private const string ADD_SUCCESS_MESSAGE = "Obat berhasil ditambahkan";
        private const string ADD_ERROR_MESSAGE = "Terjadi kesalahan saat menambahkan obat.";

        private void btnCariObat_Click(object sender, EventArgs e)
        {
            try
            {
                Obat obat = (Obat)BoxObat.SelectedItem;

                if (!TransaksiValidator.ValidasiJumlah(
                    TBjumlah.Text,
                    out int jumlah))
                {
                    TampikanPesanLabel(
                        "Jumlah tidak valid",
                        Color.Red);
                    return;
                }

                ItemTransaksi item =
                    ItemTransaksiFactory.CreateItem(
                        obat,
                        jumlah);

                keranjang.Add(item);

                RefreshKeranjang();

                TampikanPesanLabel(
                    ADD_SUCCESS_MESSAGE,
                    Color.Green);
            }
            catch (ArgumentException ex)
            {
                TampikanPesanLabel(
                    ex.Message,
                    Color.Red);
            }
            catch (Exception ex)
            {
                TampikanPesanLabel(
                    ADD_ERROR_MESSAGE + ex.Message,
                    Color.Red);
            }
        }

        private void BtnHapus_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Get selected row index
                int index = TabelKeranjang.CurrentRow?.Index ?? -1;

                // 2. Check if row selected
                if (TabelKeranjang.CurrentRow == null)
                {
                    MessageBox.Show("Pilih item yang mau dihapus dulu ya!");
                    return;
                }

                // 3. Validate using validator
                TransaksiValidator.HapusValidator(index, keranjang.Count);

                // 4. Remove item
                keranjang.RemoveAt(index);
                RefreshKeranjang();

                MessageBox.Show("Item berhasil dihapus");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                TampikanPesanLabel("Terjadi kesalahan saat menghapus item: " + ex.Message, Color.Red);
            }
        }

        private void TBUangBayar_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click_1(object sender, EventArgs e)
        {

        }

        private void label8_Click_2(object sender, EventArgs e)
        {

        }
    }
}