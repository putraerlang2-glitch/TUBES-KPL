// FILE: CONTOH_PENGGUNAAN.cs
// Referensi untuk memahami implementasi Sistem Status Obat dan Notifikasi

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TubesKPL
{
    /*
    ====================================================================
    CONTOH PENGGUNAAN: SISTEM STATUS OBAT DAN NOTIFIKASI
    ====================================================================
    */

    // ====================================================================
    // 1. ENUM STATUS OBAT
    // ====================================================================
    /*
    public enum StatusObat
    {
        Available,   // Obat tersedia, stok >= 10, belum expired
        LowStock,    // Stok < 10
        Expired      // Tanggal expired sudah lewat
    }
    
    Cara penggunaan:
    
    StatusObat status = StatusObat.Available;
    
    if (status == StatusObat.Expired)
    {
        Console.WriteLine("Obat sudah kadaluarsa!");
    }
    */

    // ====================================================================
    // 2. CLASS OBAT DENGAN STATE-BASED LOGIC
    // ====================================================================
    /*
    public class Obat
    {
        public string nama { get; set; }
        public int stok { get; set; }
        public decimal harga { get; set; }
        public DateTime expiredDate { get; set; }
        public StatusObat status { get; set; }

        public Obat(string nama, int stok, decimal harga, DateTime expiredDate)
        {
            this.nama = nama;
            this.stok = stok;
            this.harga = harga;
            this.expiredDate = expiredDate;
            this.status = HitungStatus();  // Status dihitung saat pembuatan object
        }

        // METHOD: Menghitung status secara otomatis
        public StatusObat HitungStatus()
        {
            // Rule 1: Cek expired terlebih dahulu
            if (expiredDate < DateTime.Now)
            {
                return StatusObat.Expired;
            }
            // Rule 2: Cek stok rendah
            else if (stok < 10)
            {
                return StatusObat.LowStock;
            }
            // Rule 3: Default status
            else
            {
                return StatusObat.Available;
            }
        }

        // METHOD: Update status (dipanggil sebelum display)
        public void UpdateStatus()
        {
            status = HitungStatus();
        }
    }
    
    Contoh penggunaan:
    
    var obat1 = new Obat("Paracetamol", 21, 5000, new DateTime(2025, 6, 15));
    Console.WriteLine($"{obat1.nama} - Status: {obat1.status}");
    // Output: Paracetamol - Status: Available
    
    var obat2 = new Obat("Sanmol", 5, 3000, new DateTime(2024, 12, 31));
    Console.WriteLine($"{obat2.nama} - Status: {obat2.status}");
    // Output: Sanmol - Status: Expired (jika tanggal hari ini > 31/12/2024)
    */

    // ====================================================================
    // 3. MEMBUAT DATA SAMPLE
    // ====================================================================
    /*
    List<Obat> daftarObat = new List<Obat>()
    {
        new Obat("Paracetamol", 21, 5000, new DateTime(2025, 6, 15)),  // Available
        new Obat("Ibuprofen", 17, 7000, new DateTime(2025, 8, 20)),    // Available
        new Obat("Sanmol", 5, 3000, new DateTime(2024, 12, 31)),       // Expired + LowStock
        new Obat("HRIG", 3, 20000, new DateTime(2024, 11, 10)),        // Expired + LowStock
        new Obat("Influenza", 15, 2000, new DateTime(2025, 3, 15)),    // Available
        new Obat("Jane Doe", 50, 500000, new DateTime(2026, 1, 1))     // Available
    };
    
    Penjelasan:
    - Paracetamol: Stok 21 >= 10, expired 2025 (masa depan) → AVAILABLE (Hijau)
    - Ibuprofen: Stok 17 >= 10, expired 2025 (masa depan) → AVAILABLE (Hijau)
    - Sanmol: Stok 5 < 10, expired 2024 (masa lalu) → EXPIRED (Merah)
    - HRIG: Stok 3 < 10, expired 2024 (masa lalu) → EXPIRED (Merah)
    - Influenza: Stok 15 >= 10, expired 2025 (masa depan) → AVAILABLE (Hijau)
    - Jane Doe: Stok 50 >= 10, expired 2026 (masa depan) → AVAILABLE (Hijau)
    */

    // ====================================================================
    // 4. MENAMPILKAN DATA DI DATAGRIDVIEW
    // ====================================================================
    /*
    private void TampilkanData(List<Obat> data)
    {
        // Step 1: Update status semua obat
        foreach (var obat in data)
        {
            obat.UpdateStatus();
        }

        // Step 2: Buat DataTable
        DataTable dt = new DataTable();
        dt.Columns.Add("Nama Obat");
        dt.Columns.Add("Stok");
        dt.Columns.Add("Harga");
        dt.Columns.Add("Tanggal Expired");
        dt.Columns.Add("Status");

        // Step 3: Masukkan data ke DataTable
        foreach (var obat in data)
        {
            dt.Rows.Add(
                obat.nama,                                    // Nama Obat
                obat.stok,                                    // Stok
                obat.harga.ToString("C"),                     // Harga (format currency)
                obat.expiredDate.ToString("dd/MM/yyyy"),      // Tanggal Expired
                obat.status.ToString()                        // Status
            );
        }

        // Step 4: Set DataSource ke DataGridView
        tblObat.DataSource = dt;

        // Step 5: Terapkan warna berdasarkan status
        TerapkanWarnaStatus();
    }
    
    Hasil:
    DataGridView akan menampilkan:
    
    | Nama Obat | Stok | Harga    | Tanggal Expired | Status    |
    |-----------|------|----------|-----------------|-----------|
    | Paracetamol| 21  | Rp 5.000 | 15/06/2025     | Available | ← Hijau
    | Sanmol    | 5    | Rp 3.000 | 31/12/2024     | Expired   | ← Merah
    | ...       | ...  | ...      | ...            | ...       |
    */

    // ====================================================================
    // 5. MENERAPKAN VISUAL INDIKATOR (WARNA)
    // ====================================================================
    /*
    private void TerapkanWarnaStatus()
    {
        // Iterasi setiap baris di DataGridView
        for (int i = 0; i < tblObat.Rows.Count; i++)
        {
            // Ambil status dari kolom ke-4 (Index 4)
            string status = tblObat.Rows[i].Cells[4].Value.ToString();
            
            // Ambil row yang sedang diproses
            DataGridViewRow row = tblObat.Rows[i];

            // Set warna background berdasarkan status
            switch (status)
            {
                case "Expired":
                    // Warna Merah Muda
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                    break;
                
                case "LowStock":
                    // Warna Kuning Muda
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200);
                    break;
                
                case "Available":
                    // Warna Hijau Muda
                    row.DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 200);
                    break;
            }
        }
    }
    
    Hasil:
    - Baris dengan status "Expired" akan berwarna MERAH
    - Baris dengan status "LowStock" akan berwarna KUNING
    - Baris dengan status "Available" akan berwarna HIJAU
    */

    // ====================================================================
    // 6. MENAMPILKAN NOTIFIKASI
    // ====================================================================
    /*
    private void TampilkanNotifikasi()
    {
        // Step 1: Filter obat yang expired
        List<Obat> obatExpired = daftarObat
            .Where(o => o.status == StatusObat.Expired)
            .ToList();

        // Step 2: Filter obat dengan stok rendah
        List<Obat> obatLowStock = daftarObat
            .Where(o => o.status == StatusObat.LowStock)
            .ToList();

        // Step 3: Tampilkan notifikasi obat expired
        if (obatExpired.Count > 0)
        {
            string pesan = "⚠️ PERINGATAN: Ada obat yang sudah expired:\n\n";
            foreach (var obat in obatExpired)
            {
                pesan += $"- {obat.nama} (Expired: {obat.expiredDate:dd/MM/yyyy})\n";
            }
            MessageBox.Show(pesan, "Obat Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Step 4: Tampilkan notifikasi obat low stock
        if (obatLowStock.Count > 0)
        {
            string pesan = "⚠️ PERHATIAN: Ada obat dengan stok rendah:\n\n";
            foreach (var obat in obatLowStock)
            {
                pesan += $"- {obat.nama} (Stok: {obat.stok})\n";
            }
            MessageBox.Show(pesan, "Stok Rendah", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Step 5: Tampilkan statistik
        TampilkanStatistik();
    }
    
    Hasil:
    Dialog 1: Menampilkan daftar obat yang sudah expired
    Dialog 2: Menampilkan daftar obat dengan stok rendah
    */

    // ====================================================================
    // 7. MENAMPILKAN STATISTIK
    // ====================================================================
    /*
    private void TampilkanStatistik()
    {
        // Hitung jumlah obat per status menggunakan LINQ
        int jumlahAvailable = daftarObat
            .Count(o => o.status == StatusObat.Available);
        
        int jumlahLowStock = daftarObat
            .Count(o => o.status == StatusObat.LowStock);
        
        int jumlahExpired = daftarObat
            .Count(o => o.status == StatusObat.Expired);

        // Tampilkan di title bar
        this.Text = $"Form1 - Available: {jumlahAvailable} | Low Stock: {jumlahLowStock} | Expired: {jumlahExpired}";
    }
    
    Contoh Output:
    Title Bar: "Form1 - Available: 4 | Low Stock: 2 | Expired: 0"
    */

    // ====================================================================
    // 8. FUNGSI PENCARIAN
    // ====================================================================
    /*
    private void button1_Click(object sender, EventArgs e)
    {
        // Step 1: Ambil input dari TextBox dan ubah ke huruf kecil
        string inputan = textBox1.Text.ToLower();
        
        // Step 2: Validasi input
        if (inputan == "")
        {
            MessageBox.Show("Masukan Nama Obat Dulu");
            return;
        }

        // Step 3: Cari obat yang cocok
        List<Obat> hasil = new List<Obat>();
        for (int i = 0; i < daftarObat.Count; i++)
        {
            if (daftarObat[i].nama.ToLower().Contains(inputan))
            {
                hasil.Add(daftarObat[i]);
            }
        }

        // Step 4: Tampilkan hasil
        if (hasil.Count > 0)
        {
            TampilkanData(hasil);
        }
        else
        {
            MessageBox.Show("Obat tidak ada");
            TampilkanData(daftarObat);  // Tampilkan semua obat kembali
        }
    }
    
    Contoh:
    - Input: "Paracetamol" → Tampilkan 1 baris (Status: Available, Warna: Hijau)
    - Input: "Sanmol" → Tampilkan 1 baris (Status: Expired, Warna: Merah)
    - Input: "XYZ" → Tampilkan "Obat tidak ada"
    */

    // ====================================================================
    // 9. ALUR APLIKASI
    // ====================================================================
    /*
    APLIKASI DIMULAI
         ↓
    [Form1_Load Event]
         ↓
    TampilkanData(daftarObat)
         ├─ UpdateStatus() untuk setiap obat
         ├─ Buat DataTable dengan 5 kolom
         ├─ Set DataGridView.DataSource = DataTable
         └─ TerapkanWarnaStatus()
         ↓
    TampilkanNotifikasi()
         ├─ Cek & tampilkan alert obat expired
         ├─ Cek & tampilkan alert obat low stock
         └─ TampilkanStatistik() → Update title bar
         ↓
    [APLIKASI SIAP DIGUNAKAN]
         ↓
    User Klik Tombol "Cari"
         ↓
    button1_Click()
         ├─ Validasi input
         ├─ Filter daftarObat berdasarkan nama
         └─ TampilkanData(hasil)
    */

    // ====================================================================
    // 10. CARA MEMODIFIKASI DATA
    // ====================================================================
    /*
    Untuk menambah obat baru:
    
    daftarObat.Add(new Obat("Nama Obat", stok, harga, new DateTime(tahun, bulan, hari)));
    
    Contoh:
    
    daftarObat.Add(new Obat("Aspirin", 20, 2000, new DateTime(2025, 12, 31)));
    TampilkanData(daftarObat);  // Refresh display
    
    ---
    
    Untuk menghapus obat:
    
    daftarObat.RemoveAt(index);
    TampilkanData(daftarObat);
    
    ---
    
    Untuk update stok:
    
    daftarObat[0].stok = 15;
    daftarObat[0].UpdateStatus();  // Re-calculate status
    TampilkanData(daftarObat);
    */

    // ====================================================================
    // KESIMPULAN
    // ====================================================================
    /*
    
    KONSEP STATE-BASED PROGRAMMING:
    ─────────────────────────────────
    
    Status obat didasarkan pada kondisi (state) saat ini:
    
    ┌─────────────────────────────────────────────────┐
    │         KONDISI              │      STATUS       │
    ├──────────────────────────────┼───────────────────┤
    │ ExpiredDate < DateTime.Now   │ Expired           │
    │ Stok < 10                    │ LowStock          │
    │ Selain itu                   │ Available         │
    └──────────────────────────────┴───────────────────┘
    
    State TIDAK diset manual, tapi dihitung otomatis melalui method HitungStatus().
    Ini memastikan status selalu akurat dan konsisten.
    
    ---
    
    FITUR-FITUR UTAMA:
    ──────────────────
    
    1. Logika State-Based: Status dihitung secara otomatis
    2. Visual Indikator: Warna untuk setiap status
    3. Notifikasi: Alert untuk obat expired dan low stock
    4. Statistik: Summary di title bar
    5. Pencarian: Filter berdasarkan nama obat
    
    ---
    
    PENGEMBANGAN LEBIH LANJUT:
    ──────────────────────────
    
    - Tambah fitur Edit/Delete
    - Simpan data ke database
    - Export ke Excel
    - Advanced search dengan filter
    - History log untuk tracking
    - Auto-reorder notification
    
    */
}
