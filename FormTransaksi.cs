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

        private void TampilkanStruk(string isiStruk)
        {
            Form formStruk = new Form
            {
                Text = "Struk Transaksi",
                Width = 500,
                Height = 600,
                StartPosition = FormStartPosition.CenterParent,
                Icon = this.Icon,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = true
            };

            TextBox textBoxStruk = new TextBox
            {
                Text = isiStruk,
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Font = new Font("Courier New", 9),
                ScrollBars = ScrollBars.Both
            };

            Panel panelButton = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom,
                BackColor = SystemColors.Control
            };

            Button btnTutup = new Button
            {
                Text = "Tutup",
                Width = 100,
                Height = 35,
                Left = 190,
                Top = 7,
                DialogResult = DialogResult.OK
            };

            panelButton.Controls.Add(btnTutup);
            formStruk.Controls.Add(panelButton);
            formStruk.Controls.Add(textBoxStruk);
            formStruk.ShowDialog(this);
        }

        private void txtCariInputan_TextChanged(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private const string CONFIG_EMPTY_WARNING = "Kotak Pajak dan Diskon harus diisi angka dulu ya!";
        private const string CONFIG_INVALID_FORMAT = "Tolong masukkan format angka yang benar ya!";
        private const string CONFIG_SUCCESS_TITLE = "Config Berhasil";
        private const string DIALOG_WARNING_TITLE = "Peringatan";
        private const string DIALOG_ERROR_TITLE = "Error";

        private void BtnUbahConfig_Click(object sender, EventArgs e)
        {
            if (!ValidateConfigInput(TBPajak.Text, TBDiskon.Text))
            {
                return;
            }

            if (TryParseAndUpdateConfig(TBPajak.Text, TBDiskon.Text, out decimal pajakInput, out decimal diskonInput))
            {
                ShowConfigSuccessMessage(pajakInput, diskonInput);
            }
            else
            {
                ShowErrorMessage(CONFIG_INVALID_FORMAT, DIALOG_ERROR_TITLE);
            }
        }

        private bool ValidateConfigInput(string pajakText, string diskonText)
        {
            if (string.IsNullOrWhiteSpace(pajakText) || string.IsNullOrWhiteSpace(diskonText))
            {
                ShowWarningMessage(CONFIG_EMPTY_WARNING, DIALOG_WARNING_TITLE);
                return false;
            }

            return true;
        }

        private bool TryParseAndUpdateConfig(string pajakText, string diskonText, out decimal pajakInput, out decimal diskonInput)
        {
            pajakInput = 0m;
            diskonInput = 0m;

            if (!decimal.TryParse(pajakText, out pajakInput) || !decimal.TryParse(diskonText, out diskonInput))
            {
                return false;
            }

            decimal pajakDesimal = ConvertPercentageToDecimal(pajakInput);
            decimal diskonDesimal = ConvertPercentageToDecimal(diskonInput);

            RuntimeConfig.UpdateConfig(pajakDesimal, diskonDesimal);
            return true;
        }

        private decimal ConvertPercentageToDecimal(decimal percentage)
        {
            return percentage / 100m;
        }

        private void ShowConfigSuccessMessage(decimal pajak, decimal diskon)
        {
            string message = $"Mantap! Konfigurasi berhasil diubah secara Runtime.\n" +
                           $"Pajak Baru: {pajak}%\n" +
                           $"Diskon Baru: {diskon}%\n\n" +
                           $"Silakan klik 'Bayar' untuk melihat perubahannya pada total belanja.";

            MessageBox.Show(message, CONFIG_SUCCESS_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowWarningMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowErrorMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

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

            decimal nominalDiskon = subtotal * RuntimeConfig.DiskonAktif;
            decimal subtotalSetelahDiskon = subtotal - nominalDiskon;
            decimal nominalPajak = subtotalSetelahDiskon * RuntimeConfig.PajakPPN;
            decimal grandTotal = subtotalSetelahDiskon + nominalPajak;

            if (!decimal.TryParse(TBUangBayar.Text, out decimal uangBayar))
            {
                TampikanPesanLabel("Masukkan angka nominal uang bayar yang valid!", Color.Red);
                return;
            }

            if (uangBayar < grandTotal)
            {
                TampikanPesanLabel($"Uang tidak cukup! Total Pembayaran: {grandTotal:C}", Color.Red);
                return;
            }

            decimal kembalian = uangBayar - grandTotal;

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
                    UangKembalian = kembalian,
                    UserId = 1 // Sementara hardcode untuk testing (id user admin di database)
                };

                // Log nilai untuk debugging
                Console.WriteLine($"[DEBUG] Subtotal: {dto.Subtotal}");
                Console.WriteLine($"[DEBUG] NominalDiskon: {dto.NominalDiskon}");
                Console.WriteLine($"[DEBUG] NominalPajak: {dto.NominalPajak}");
                Console.WriteLine($"[DEBUG] TotalAkhir: {dto.TotalAkhir}");
                Console.WriteLine($"[DEBUG] UangBayar: {dto.UangBayar}");
                Console.WriteLine($"[DEBUG] UangKembalian: {dto.UangKembalian}");

                foreach (var item in keranjang)
                {
                    dto.DetailList.Add(new TransaksiDetailDTO
                {
                    ObatId = item.Obat.ObatId,
                    Jumlah = item.Jumlah,
                    HargaSatuan = item.Obat.Harga,
                    Subtotal = item.Subtotal()
                });
                }

                using (var client = new ObatApiClient("https://localhost:7245"))
                {
                    await client.CheckoutTransaksiAsync(dto);
                }

                // Simpan histori transaksi lokal
                try
                {
                    TransactionHistoryService.AppendTransaction(dto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[History Save Error] {ex.Message}");
                }

                foreach (var item in keranjang)
                {
                    item.Obat.Stok -= item.Jumlah;
                    item.Obat.UpdateStatus();
                }

                string struk = StrukGenerator.BuildStruk(keranjang, subtotal, RuntimeConfig.PajakPPN, RuntimeConfig.DiskonAktif, grandTotal, uangBayar, kembalian);
                StrukGenerator.GenerateStruk(keranjang, subtotal, RuntimeConfig.PajakPPN, RuntimeConfig.DiskonAktif, grandTotal, uangBayar, kembalian);

                TampilkanStruk(struk);
                
                // Log activity
                ActivityHistoryService.LogActivity("TRANSAKSI", $"Transaksi berhasil dengan total: {dto.TotalAkhir:C}", userId: dto.UserId);

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
            dt.Columns.Add("Harga Akhir");

            foreach (var item in keranjang)
            {
                decimal subtotal = item.Subtotal();
                decimal hargaAkhir = CalculateFinalPrice(subtotal);

                dt.Rows.Add(
                    item.Obat.Nama, 
                    item.Obat.Harga.ToString("C"), 
                    item.Jumlah, 
                    subtotal.ToString("C"),
                    hargaAkhir.ToString("C"));
            }
            TabelKeranjang.DataSource = dt;
        }

        private decimal CalculateFinalPrice(decimal subtotal)
        {
            decimal nominalDiskon = subtotal * RuntimeConfig.DiskonAktif;
            decimal subtotalSetelahDiskon = subtotal - nominalDiskon;
            decimal nominalPajak = subtotalSetelahDiskon * RuntimeConfig.PajakPPN;
            decimal hargaAkhir = subtotalSetelahDiskon + nominalPajak;

            return hargaAkhir;
        }

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
                int index = TabelKeranjang.CurrentRow?.Index ?? -1;

                if (TabelKeranjang.CurrentRow == null)
                {
                    MessageBox.Show("Pilih item yang mau dihapus dulu ya!");
                    return;
                }

                TransaksiValidator.HapusValidator(index, keranjang.Count);

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

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}