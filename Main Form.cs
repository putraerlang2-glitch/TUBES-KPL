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
        // [AYONDI - ANALISIS] daftarObat adalah List yang menyimpan semua data obat
        // Ini adalah "database" in-memory (tanpa database external)
        // Data diinisialisasi dengan 6 sample obat saat startup
        List<Obat> daftarObat = new List<Obat>()
        {
            new Obat("Paracetamol", 21, 5000, new DateTime(2025, 1, 15), KategoriObat.Tablet),
            new Obat("Ibuprofen", 12, 7000, new DateTime(2024, 7, 20), KategoriObat.Tablet),
            new Obat("Sanmol", 15, 3000, new DateTime(2025, 4, 5), KategoriObat.Sirup),
            new Obat("HRIG", 12, 20000, new DateTime(2024, 3, 10), KategoriObat.AntiJamur),
            new Obat("Influenza", 10, 2000, new DateTime(2025, 3, 15), KategoriObat.Tablet),
            new Obat("Jane Doe", 11, 500000, new DateTime(2026, 8, 1), KategoriObat.Vitamin)
        };

        // [AYONDI] Flag untuk toggle antara API mode dan Local List mode
        // useApi = false (default): Gunakan daftarObat List secara langsung (mode lokal)
        // useApi = true: Gunakan ObatApiService untuk mendapatkan data (mode API simulasi)
        private bool useApi = false;

        // [AYONDI - ANALISIS] RefreshData() adalah method untuk refresh tampilan
        // Dipanggil setelah ada perubahan data (tambah, hapus, update)
        // Melakukan 2 hal: tampilkan data dan tampilkan statistik
        public void RefreshData()
        {
            TampilkanData(daftarObat);
            TampilkanStatistik();
        }

        // [AYONDI] ToggleApi() adalah method untuk switch antara mode API dan mode lokal
        // useApi = true: Gunakan ObatApiService (simulasi API)
        // useApi = false: Gunakan daftarObat List biasa
        // Berguna untuk testing/demo API tanpa perlu actual HTTP server
        public void ToggleApi()
        {
            // [AYONDI] Switch flag useApi
            useApi = !useApi;

            // [AYONDI] Tampilkan mode saat ini di message box atau console
            string mode = useApi ? "API Mode (ObatApiService)" : "Local List Mode";
            MessageBox.Show("Switched to: " + mode, "Mode Toggle", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // [AYONDI] Refresh data untuk update ke mode baru
            TampilkanData(daftarObat);
            TampilkanStatistik();
        }

        // [AYONDI] GetObatData() adalah helper method untuk get data obat
        // Tergantung flag useApi, return data dari API atau dari List
        // Return: List<Obat>
        private List<Obat> GetObatData()
        {
            // [AYONDI] Jika useApi = true, ambil dari ObatApiService
            if (useApi)
            {
                // [AYONDI] Call API GetAll() untuk mendapatkan semua obat
                var response = ObatApiService.GetAll();
                // [AYONDI] Return data dari response
                return response.Data;
            }
            else
            {
                // [AYONDI] Jika useApi = false, gunakan List lokal
                return daftarObat;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // [AYONDI] Initialize ObatApiService dengan data dari daftarObat
            // Diperlukan satu kali saat form load
            ObatApiService.Initialize(daftarObat);

            // [AYONDI] Tampilkan data obat
            TampilkanData(daftarObat);
            // [AYONDI] Tampilkan notifikasi obat expired (jika ada)
            TampilkanNotifikasi();
        }

        // [AYONDI - ANALISIS] TampilkanData() menampilkan List<Obat> ke DataGridView tblObat
        // Proses: 
        //   1. Update status semua obat (check expired, cek stok rendah)
        //   2. Buat DataTable dengan kolom-kolom
        //   3. Loop setiap obat dan add row ke DataTable
        //   4. Set DataSource ke tblObat
        //   5. Aplikasikan warna sesuai status
        // Parameter data: List<Obat> yang ingin ditampilkan (bisa hasil filter/search)
        private void TampilkanData(List<Obat> data)
        {
            // [AYONDI - ANALISIS] Update status setiap obat untuk consistency
            // Menggunakan foreach loop untuk iterate semua obat
            foreach (var obat in data)
            {
                obat.UpdateStatus();
            }

            // [AYONDI - ANALISIS] Membuat DataTable structure dengan 6 kolom
            // DataTable adalah data source untuk DataGridView
            DataTable dt = new DataTable();
            dt.Columns.Add("Nama Obat");
            dt.Columns.Add("Kategori");
            dt.Columns.Add("Stok");
            dt.Columns.Add("Harga");
            dt.Columns.Add("Tanggal Expired");
            dt.Columns.Add("Status");

            // [AYONDI - ANALISIS] Populate DataTable dengan data dari List<Obat>
            // Format: nama, kategori, stok, harga (format currency), tanggal (format dd/MM/yyyy), status
            foreach (var obat in data)
            {
                dt.Rows.Add(obat.nama,obat.kategori.ToString(), obat.stok, obat.harga.ToString("C"),
                    obat.expiredDate.ToString("dd/MM/yyyy"), obat.status.ToString());
            }

            // [AYONDI - ANALISIS] Set DataTable sebagai DataSource untuk DataGridView
            // Ini membuat data tampil di tabel
            tblObat.DataSource = dt;
            // [AYONDI - ANALISIS] Terapkan warna baris sesuai status (expired=merah, lowstock=kuning, available=hijau)
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

        // [AYONDI - ANALISIS] TerapkanWarnaStatus() menerapkan warna ke baris DataGridView berdasarkan status
        // Ini menggunakan IF-ELSE hardcode untuk mapping Status -> Warna
        // CATATAN: Ini adalah candidate untuk refactoring dengan StatusConfigManager (Table-Driven)
        // Saat ini: manual hardcode, bisa ditingkatkan ke StatusConfig.GetColor(status)
        private void TerapkanWarnaStatus()
        {
            // [AYONDI - ANALISIS] Loop semua baris di DataGridView
            for (int i = 0; i < tblObat.Rows.Count; i++)
            {
                // [AYONDI - ANALISIS] Get status dari cell index 5 (Status column)
                // Cell[5] karena urutan kolom: 0=Nama, 1=Kategori, 2=Stok, 3=Harga, 4=Expired, 5=Status
                string status = tblObat.Rows[i].Cells[5].Value?.ToString();
                DataGridViewRow row = tblObat.Rows[i];

                // [AYONDI - ANALISIS] Mapping status ke warna RGB:
                // Expired (merah): RGB(255, 200, 200) = light red
                // LowStock (kuning): RGB(255, 255, 200) = light yellow
                // Available (hijau): RGB(200, 255, 200) = light green
                if (status == "Expired") row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                else if (status == "LowStock") row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200);
                else if (status == "Available") row.DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 200);
            }
        }

        // [AYONDI - ANALISIS] TampilkanNotifikasi() menampilkan peringatan obat expired saat form load
        // Menggunakan LINQ Where dan Select untuk filter dan format data
        // Jika ada obat expired, tampilkan MessageBox dengan list obat yang expired
        private void TampilkanNotifikasi()
        {
            // [AYONDI - ANALISIS] Filter obat dengan status Expired menggunakan LINQ Where
            // ToList() mengkonversi hasil ke List<Obat>
            var obatExpired = daftarObat.Where(o => o.status == StatusObat.Expired).ToList();
            
            // [AYONDI - ANALISIS] Jika ada obat expired, tampilkan notifikasi
            if (obatExpired.Count > 0)
            {
                // [AYONDI - ANALISIS] Format pesan: 
                // - Header "⚠️ PERINGATAN: Obat expired:"
                // - Diikuti list nama obat dengan string.Join
                // - Select(o => $"- {o.nama}") membuat format "- Nama Obat"
                string pesan = "⚠️ PERINGATAN: Obat expired:\n" + string.Join("\n", obatExpired.Select(o => $"- {o.nama}"));
                MessageBox.Show(pesan, "Obat Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // [AYONDI - ANALISIS] Update statistik di title bar
            TampilkanStatistik();
        }

        // [AYONDI - ANALISIS] TampilkanStatistik() menampilkan ringkasan statistik di title bar
        // Menggunakan LINQ Count dengan condition untuk hitung jumlah obat per status
        // Format: "Apotek - Avail: X | Low: Y | Expired: Z"
        private void TampilkanStatistik()
        {
            // [AYONDI] Get data berdasarkan mode (API atau lokal)
            List<Obat> dataToCount = GetObatData();

            // [AYONDI - ANALISIS] Hitung jumlah obat per status menggunakan LINQ Count()
            // Ini efficient dan readable dibanding loop manual
            int available = dataToCount.Count(o => o.status == StatusObat.Available);
            int low = dataToCount.Count(o => o.status == StatusObat.LowStock);
            int expired = dataToCount.Count(o => o.status == StatusObat.Expired);
            
            // [AYONDI] Add mode indicator di title bar untuk visibility
            string modeIndicator = useApi ? " [API MODE]" : " [LOCAL MODE]";
            
            // [AYONDI - ANALISIS] Update form title dengan statistik
            // String interpolation untuk format message
            this.Text = $"Apotek - Avail: {available} | Low: {low} | Expired: {expired}{modeIndicator}";
        }

        // [AYONDI - ANALISIS] button1_Click adalah handler untuk tombol search
        // Search obat berdasarkan nama (case-insensitive)
        private void button1_Click(object sender, EventArgs e) 
        {
            // [AYONDI - ANALISIS] Get search term dari textBox1 dan convert ke lowercase
            string inputan = textBox1.Text.ToLower();
            // [AYONDI - ANALISIS] Filter obat menggunakan LINQ Where dan Contains (case-insensitive)
            // Hasil adalah List<Obat> yang nama-nya mengandung search term
            var hasil = daftarObat.Where(o => o.nama.ToLower().Contains(inputan)).ToList();

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
                    daftarObat.RemoveAll(o => o.nama == nama);
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

    // [AYONDI - ANALISIS] Enum StatusObat mendefinisikan 3 status obat
    // Available: Obat siap jual (stok cukup, belum expired)
    // LowStock: Stok kurang dari batas minimum kategori
    // Expired: Tanggal expired sudah lewat
    public enum StatusObat { Available, LowStock, Expired }

    // [AYONDI - ANALISIS] Class Obat adalah model/entity untuk data obat
    // Properties: nama, stok, harga, expiredDate, status, kategori
    // Method: UpdateStatus() untuk update status berdasarkan expired date dan stok
    public class Obat
    {
        public string nama { get; set; }
        public int stok { get; set; }
        public decimal harga { get; set; }
        public DateTime expiredDate { get; set; }
        public StatusObat status { get; set; }
        public KategoriObat kategori { get; set; }

        // [AYONDI - ANALISIS] batasMinimumStok adalah static Dictionary untuk Table-Driven status logic
        // Mapping KategoriObat -> Minimum Stok
        // Digunakan di UpdateStatus() untuk determine apakah status LowStock
        // Ini adalah contoh Table-Driven Construction (bukan hardcode if-else)
        private static Dictionary<KategoriObat, int> batasMinimumStok = new Dictionary<KategoriObat, int>()
{
        { KategoriObat.Tablet, 10 },
        { KategoriObat.Salep, 8 },
        { KategoriObat.Sirup, 11 },
        { KategoriObat.Vitamin, 8 },
        { KategoriObat.Antibiotik, 15 },
        { KategoriObat.AntiJamur, 7 }
};

        public Obat(string nama, int stok, decimal harga, DateTime expiredDate, KategoriObat kategori)
        {
            this.nama = nama;
            this.stok = stok;
            this.harga = harga;
            this.expiredDate = expiredDate;
            this.kategori = kategori;
            UpdateStatus();
        }

        // [AYONDI - ANALISIS] UpdateStatus() menghitung dan update status obat
        // Logic:
        //   1. Jika expired date < hari ini -> Expired
        //   2. Else jika stok < batas minimum kategori -> LowStock
        //   3. Else -> Available
        // Ini adalah State-Based Logic yang perlu dipanggil sebelum display
        public void UpdateStatus()
        {
            // [AYONDI - ANALISIS] Check expired terlebih dahulu (priority tertinggi)
            if (expiredDate < DateTime.Now)
                status = StatusObat.Expired;
            // [AYONDI - ANALISIS] Check stok rendah (priority kedua)
            else if (stok < batasMinimumStok[kategori])
                status = StatusObat.LowStock; 
            // [AYONDI - ANALISIS] Default: Available
            else status = StatusObat.Available;
        }
        // [AYONDI - ANALISIS] Enum KategoriObat mendefinisikan tipe-tipe obat
        // Setiap kategori memiliki batas minimum stok yang berbeda
        // Digunakan untuk menentukan status LowStock
        public enum KategoriObat
        {
            Tablet,
            Salep,
            Sirup,
            Vitamin,
            Antibiotik,
            AntiJamur
        }
    }
}
