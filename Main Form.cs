using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static TubesKPL.Obat;

namespace TubesKPL
{
    // [AYONDI - ANALISIS] Form1 adalah main form aplikasi Apotek
    // Form ini menampilkan daftar obat, mengelola status, dan menampilkan notifikasi
    public partial class Form1 : Form
    {
        // [AYONDI - REFACTOR] daftarObat adalah in-memory list untuk data obat
        // Data akan diisi dari API saat Form Load
        List<Obat> daftarObat = new List<Obat>();

        // [AYONDI - REFACTOR] RefreshData digunakan untuk refresh tampilan dan statistik
        public void RefreshData()
        {
            TampilkanData(daftarObat);
            TampilkanStatistik();
        }

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Helper method untuk get sample data (fallback saat API tidak tersedia)
        /// </summary>
        private List<Obat> GetSampleData()
        {
            return new List<Obat>()
            {
                new Obat("Paracetamol", 21, 5000, new DateTime(2025, 1, 15), "Tablet"),
                new Obat("Ibuprofen", 12, 7000, new DateTime(2024, 7, 20), "Tablet"),
                new Obat("Sanmol", 15, 3000, new DateTime(2025, 4, 5), "Sirup"),
                new Obat("HRIG", 12, 20000, new DateTime(2024, 3, 10), "AntiJamur"),
                new Obat("Influenza", 10, 2000, new DateTime(2025, 3, 15), "Tablet"),
                new Obat("Vitamin C", 11, 500000, new DateTime(2026, 8, 1), "Vitamin")
            };
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // ⚠️ PENTING: Pastikan ObatAPI sudah running sebelum load data!
            // URL: https://localhost:7103/swagger (untuk test Swagger)
            // Jalankan: dotnet run di folder ObatAPI

            ObatApiClient client = null;
            try
            {
                // [AYONDI - INTEGRATION] Load data dari ObatAPI menggunakan HttpClient
                client = new ObatApiClient("https://localhost:7103");

                System.Console.WriteLine("[MAIN FORM] Fetching data from API...");

                // Async call untuk ambil semua data obat dari API
                daftarObat = await client.GetAllObatAsync();

                System.Console.WriteLine($"[MAIN FORM] Received {(daftarObat?.Count ?? 0)} items from API");

                if (daftarObat == null || daftarObat.Count == 0)
                {
                    MessageBox.Show(
                        "Tidak ada data dari API. Menggunakan sample data.\n\n" +
                        "Pastikan ObatAPI sudah running di https://localhost:7103",
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Fallback: gunakan sample data jika API kosong
                    daftarObat = GetSampleData();
                }

                // [AYONDI - REFACTOR] Initialize API service dengan data dari server
                ObatApiService.Initialize(daftarObat);

                // ⚠️ HttpApiServer TIDAK DIGUNAKAN - API ada di ObatAPI project
                // HttpApiServer.SetObatData(daftarObat);  // DEPRECATED

                // [AYONDI - REFACTOR] Tampilkan data di grid dan notifikasi
                TampilkanData(daftarObat);
                TampilkanNotifikasi();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[ERROR] Form1_Load: {ex.Message}");
                System.Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");

                MessageBox.Show(
                    $"Error loading data from API:\n\n{ex.Message}\n\n" +
                    "Pastikan ObatAPI sudah running di https://localhost:7103\n\n" +
                    "Menggunakan sample data sebagai fallback.",
                    "API Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                // Fallback: gunakan sample data jika API tidak bisa diakses
                daftarObat = GetSampleData();

                ObatApiService.Initialize(daftarObat);
                TampilkanData(daftarObat);
                TampilkanNotifikasi();
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        // [AYONDI - REFACTOR] TampilkanData menampilkan list obat ke DataGridView
        // Update status semua obat, populate DataTable, terapkan warna
        private void TampilkanData(List<Obat> data)
        {
            // Update status setiap obat
            foreach (var obat in data)
            {
                obat.UpdateStatus();
            }

            // Buat DataTable dengan struktur kolom
            DataTable dt = new DataTable();
            dt.Columns.Add("Nama Obat");
            dt.Columns.Add("Kategori");
            dt.Columns.Add("Stok");
            dt.Columns.Add("Harga");
            dt.Columns.Add("Tanggal Expired");
            dt.Columns.Add("Status");

            // Populate DataTable dengan property baru (PascalCase)
            foreach (var obat in data)
            {
                dt.Rows.Add(
                    obat.Nama, 
                    obat.Kategori, 
                    obat.Stok, 
                    obat.Harga.ToString("C"),
                    obat.ExpiredDate.ToString("dd/MM/yyyy"), 
                    obat.Status  // Status sekarang string dari API
                );
            }

            // Set DataSource dan terapkan warna status
            tblObat.DataSource = dt;
            TerapkanWarnaStatus();
        }

       
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (tblObat.CurrentRow == null)
            {
                MessageBox.Show("Klik dulu baris obat yang mau diubah di tabel!");
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

            if (f2.ShowDialog() == DialogResult.OK)
            {
                
                TampilkanData(daftarObat);
                TampilkanStatistik();
            }
        }

        // [AYONDI - REFACTOR] TerapkanWarnaStatus mengaplikasikan warna ke baris DataGridView
        // Sederhana: Expired=merah, LowStock=kuning, Available=hijau
        private void TerapkanWarnaStatus()
        {
            for (int i = 0; i < tblObat.Rows.Count; i++)
            {
                string status = tblObat.Rows[i].Cells[5].Value?.ToString();
                DataGridViewRow row = tblObat.Rows[i];

                if (status == "Expired")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                else if (status == "LowStock")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200);
                else
                    row.DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 200);
            }
        }

        // [AYONDI - REFACTOR] TampilkanNotifikasi menampilkan peringatan obat expired
        // Dijalankan saat Form Load dan setelah perubahan data
        private void TampilkanNotifikasi()
        {
            var obatExpired = daftarObat.Where(o => o.Status == "Expired").ToList();

            if (obatExpired.Count > 0)
            {
                string pesan = "⚠️ PERINGATAN: Obat expired:\n" + string.Join("\n", obatExpired.Select(o => "- " + o.Nama));
                MessageBox.Show(pesan, "Obat Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            TampilkanStatistik();
        }

        // [AYONDI - REFACTOR] TampilkanStatistik menampilkan ringkasan status di title bar
        private void TampilkanStatistik()
        {
            int available = daftarObat.Count(o => o.Status == "Available");
            int low = daftarObat.Count(o => o.Status == "LowStock");
            int expired = daftarObat.Count(o => o.Status == "Expired");

            this.Text = $"Apotek - Avail: {available} | Low: {low} | Expired: {expired}";
        }

        // [AYONDI - ANALISIS] button1_Click adalah handler untuk tombol search
        // Search obat berdasarkan nama (case-insensitive)
        private void button1_Click(object sender, EventArgs e) 
        {
            // [AYONDI - ANALISIS] Get search term dari textBox1 dan convert ke lowercase
            string inputan = textBox1.Text.ToLower();
            // [AYONDI - ANALISIS] Filter obat menggunakan LINQ Where dan Contains (case-insensitive)
            // Hasil adalah List<Obat> yang nama-nya mengandung search term
            var hasil = daftarObat.Where(o => o.Nama.ToLower().Contains(inputan)).ToList();

            if (hasil.Count > 0) TampilkanData(hasil);
            else MessageBox.Show("Obat tidak ditemukan");
        }

        // [AYONDI - ANALISIS] button2_Click adalah handler untuk tombol tambah obat
        // Membuka FormTambahObat dialog, jika OK maka tambah obat ke daftarObat
        private void button2_Click(object sender, EventArgs e) 
        {
            // [AYONDI - ANALISIS] Buka FormTambahObat sebagai modal dialog
            FormTambahObat formTambah = new FormTambahObat();
            // [AYONDI - ANALISIS] Jika user klik OK (bukan Cancel)
            if (formTambah.ShowDialog() == DialogResult.OK)
            {
                // [AYONDI - ANALISIS] Tambah obat baru ke List
                daftarObat.Add(formTambah.obatBaru);
                // [AYONDI - ANALISIS] Refresh tampilan dan statistik
                TampilkanData(daftarObat);
                TampilkanStatistik();
            }
        }

        // [AYONDI - ANALISIS] btnHapus_Click adalah handler untuk tombol hapus obat
        // Delete obat yang selected di DataGridView
        private void btnHapus_Click(object sender, EventArgs e)
        {
            // [AYONDI - ANALISIS] Cek apakah ada baris yang selected
            if (tblObat.CurrentRow != null)
            {
                // [AYONDI - ANALISIS] Get nama obat dari Cell[0] (Nama Obat column)
                string nama = tblObat.CurrentRow.Cells[0].Value.ToString();
                // [AYONDI - ANALISIS] Tampilkan konfirmasi dialog
                if (MessageBox.Show($"Hapus {nama}?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // [AYONDI - ANALISIS] Remove semua obat dengan nama yang match menggunakan RemoveAll
                    daftarObat.RemoveAll(o => o.Nama == nama);
                    // [AYONDI - ANALISIS] Refresh tampilan dan statistik
                    TampilkanData(daftarObat);
                    TampilkanStatistik();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void button3_Click(object sender, EventArgs e)
        {

            FormTransaksi ft = new FormTransaksi(daftarObat, this);
            ft.Show();
        }
    }

    // ⚠️ CATATAN: Model Obat sudah dipindahkan ke Obat.cs (file terpisah)
    // File lama di Main Form.cs sudah dihapus untuk menghindari duplikasi
    // Gunakan model dari Obat.cs yang support API integration
}
