using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TubesKPL
{
    public partial class Form1 : Form
    {
        List<Obat> daftarObat = new List<Obat>();

        public Form1() => InitializeComponent();
        public void RefreshData() => TampilkanData(daftarObat);

        private async void Form1_Load(object sender, EventArgs e) => await LoadFromApi();

        private async Task LoadFromApi()
        {
            try
            {
                using (var client = new ObatApiClient())
                {
                    Console.WriteLine("[Main Form] Fetching data from API...");
                    daftarObat = await client.GetAllObatAsync() ?? new List<Obat>();
                    Console.WriteLine($"[Main Form] Received {daftarObat.Count} items from API");
                }

                ObatApiService.Initialize(daftarObat);
                TampilkanData(daftarObat);
                StateMachine.ShowNotifications(daftarObat);
                this.Text = StateMachine.FormatTitleWithStats("Apotek", daftarObat);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Main Form Error] {ex.Message}");
                MessageBox.Show($"Gagal memuat data dari API: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                daftarObat.Clear();
                TampilkanData(daftarObat);
            }
        }

        private const string DATE_FORMAT = "dd/MM/yyyy";
        private const string CURRENCY_FORMAT = "C";

        private void TampilkanData(List<Obat> data)
        {
            foreach (var obat in data) StateMachine.EvaluateStatus(obat);
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new[] { "Nama Obat", "Kategori", "Stok", "Harga", "Tanggal Expired", "Status" });

            foreach (var obat in data)
                dt.Rows.Add(obat.Nama, obat.Kategori, obat.Stok, obat.Harga.ToString(CURRENCY_FORMAT), obat.ExpiredDate.ToString(DATE_FORMAT), obat.Status);

            tblObat.DataSource = dt;
            StateMachine.ApplyStatusColors(tblObat, 5);
        }

        private const string OBAT_TIDAK_ADA = "Obat tidak ditemukan";

        private void btnCariObat_Click(object sender, EventArgs e)
        {
            string cariKeyword = txtCariInputan.Text.ToLower().Trim();
            if (string.IsNullOrWhiteSpace(cariKeyword))
            {
                TampilkanData(daftarObat);
                return;
            }

            var hasilPencarian = daftarObat.Where(o => o.Nama.ToLower().Contains(cariKeyword)).ToList();
            if (hasilPencarian.Count > 0) TampilkanData(hasilPencarian);
            else MessageBox.Show(OBAT_TIDAK_ADA);
        }

        private async void btnTambahObat_Click(object sender, EventArgs e)
        {
            FormTambahObat formTambah = new FormTambahObat();
            if (formTambah.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (var client = new ObatApiClient())
                {
                    var obatBaru = await client.AddObatAsync(formTambah.ObatBaru);
                    daftarObat.Add(obatBaru);
                    TampilkanData(daftarObat);
                    this.Text = StateMachine.FormatTitleWithStats("Apotek", daftarObat);
                    MessageBox.Show("Obat berhasil disimpan ke database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Gagal menyimpan ke database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (tblObat.CurrentRow == null)
            {
                MessageBox.Show("Klik dulu obat yang ingin diubah!");
                return;
            }

            int selectedIndex = tblObat.CurrentRow.Index;
            if (selectedIndex < 0 || selectedIndex >= daftarObat.Count)
            {
                MessageBox.Show("Baris tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Obat selectedObat = daftarObat[selectedIndex];
            Form2 f2 = new Form2(selectedObat);
            if (f2.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (var client = new ObatApiClient())
                {
                    var updatedObat = await client.UpdateObatAsync(selectedObat.Id, selectedObat);
                    daftarObat[selectedIndex] = updatedObat;
                    TampilkanData(daftarObat);
                    this.Text = StateMachine.FormatTitleWithStats("Apotek", daftarObat);
                    MessageBox.Show("Obat berhasil diubah di database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Gagal merubah data di database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void btnHapus_Click(object sender, EventArgs e)
        {
            if (tblObat.CurrentRow == null) return;
            int selectedIndex = tblObat.CurrentRow.Index;
            if (selectedIndex < 0 || selectedIndex >= daftarObat.Count) return;

            Obat selectedObat = daftarObat[selectedIndex];
            if (MessageBox.Show($"Hapus {selectedObat.Nama} dari database secara permanen?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                using (var client = new ObatApiClient())
                {
                    if (await client.DeleteObatAsync(selectedObat.Id))
                    {
                        daftarObat.RemoveAt(selectedIndex);
                        TampilkanData(daftarObat);
                        this.Text = StateMachine.FormatTitleWithStats("Apotek", daftarObat);
                        MessageBox.Show("Obat berhasil dihapus dari database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else MessageBox.Show("Gagal menghapus data dari server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Gagal menghapus dari database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnTransaksi_Click(object sender, EventArgs e) => new FormTransaksi(daftarObat, this).Show();
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void txtCariInputan_TextChanged(object sender, EventArgs e) { }
    }
}
