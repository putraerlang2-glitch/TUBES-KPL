# PANDUAN TESTING: SISTEM STATUS OBAT DAN NOTIFIKASI

## 📋 Daftar Persiapan

Sebelum melakukan testing, pastikan:

- ✅ Visual Studio sudah membuka project TubesKPL
- ✅ File Form1.cs sudah diupdate dengan semua perubahan
- ✅ Tidak ada error di Error List
- ✅ Project sudah di-rebuild

---

## 🚀 Langkah-Langkah Testing

### **TEST 1: Aplikasi Startup & Tampilan Data**

**Tujuan**: Memverifikasi aplikasi berjalan dan menampilkan data dengan benar

**Langkah**:
1. Tekan `Ctrl + F5` untuk menjalankan aplikasi tanpa debugging
2. Tunggu form Form1 muncul

**Expected Result**:
- ✅ Form memiliki title: `Form1 - Available: 4 | Low Stock: 2 | Expired: 0`
- ✅ DataGridView menampilkan 6 baris data
- ✅ DataGridView memiliki 5 kolom:
  - Nama Obat
  - Stok
  - Harga
  - Tanggal Expired
  - Status
- ✅ Data terlihat lengkap dan terformat dengan baik

**Screenshot Expected**:
```
┌─────────────────────────────────────────────────────────┐
│ Form1 - Available: 4 | Low Stock: 2 | Expired: 0       │
├─────────────────────────────────────────────────────────┤
│ Nama Obat    │ Stok │ Harga    │ Expired    │ Status   │
├──────────────┼──────┼──────────┼────────────┼──────────┤
│ Paracetamol  │ 21   │ Rp 5.000 │ 15/06/2025 │Available │ (Hijau)
│ Ibuprofen    │ 17   │ Rp 7.000 │ 20/08/2025 │Available │ (Hijau)
│ Sanmol       │ 5    │ Rp 3.000 │ 31/12/2024 │Expired   │ (Merah)
│ HRIG         │ 3    │Rp 20.000 │ 10/11/2024 │Expired   │ (Merah)
│ Influenza    │ 15   │ Rp 2.000 │ 15/03/2025 │Available │ (Hijau)
│ Jane Doe     │ 50   │Rp500.000 │ 01/01/2026 │Available │ (Hijau)
└──────────────┴──────┴──────────┴────────────┴──────────┘
```

**Jika Gagal**:
- [ ] Cek apakah Form1.Designer.cs sudah ter-update dengan DataGridView
- [ ] Pastikan `TampilkanData()` dipanggil di `Form1_Load()`
- [ ] Lihat error di Output window (Debug > Windows > Output)

---

### **TEST 2: Notifikasi Obat Expired**

**Tujuan**: Memverifikasi notifikasi untuk obat yang sudah kadaluarsa

**Langkah**:
1. Aplikasi startup (dari TEST 1)
2. Perhatikan MessageBox yang muncul pertama kali

**Expected Result**:
- ✅ MessageBox muncul dengan judul: `Obat Expired`
- ✅ Pesan berisi daftar obat yang expired:
  ```
  ⚠️ PERINGATAN: Ada obat yang sudah expired:

  - Sanmol (Expired: 31/12/2024)
  - HRIG (Expired: 10/11/2024)
  ```
- ✅ Icon adalah Warning (⚠️)

**Jika Gagal**:
- [ ] Pastikan data sample memiliki tanggal expired di masa lalu
- [ ] Cek apakah method `TampilkanNotifikasi()` dipanggil
- [ ] Verifikasi kondisi `if (obatExpired.Count > 0)`

---

### **TEST 3: Notifikasi Obat Low Stock**

**Tujuan**: Memverifikasi notifikasi untuk obat dengan stok rendah

**Langkah**:
1. Klik "OK" pada MessageBox obat expired
2. Perhatikan MessageBox berikutnya

**Expected Result**:
- ✅ MessageBox muncul dengan judul: `Stok Rendah`
- ✅ Pesan berisi daftar obat low stock:
  ```
  ⚠️ PERHATIAN: Ada obat dengan stok rendah:

  - Sanmol (Stok: 5)
  - HRIG (Stok: 3)
  ```
- ✅ Icon adalah Information (ℹ️)

**Jika Gagal**:
- [ ] Pastikan data sample memiliki stok < 10
- [ ] Cek apakah method `TampilkanNotifikasi()` dipanggil
- [ ] Verifikasi kondisi `if (obatLowStock.Count > 0)`

---

### **TEST 4: Visual Indikator Warna**

**Tujuan**: Memverifikasi warna background sesuai status

**Langkah**:
1. Klik "OK" pada MessageBox stok rendah
2. Lihat warna background pada setiap baris di DataGridView

**Expected Result**:
- ✅ Baris Paracetamol: Hijau muda (Status: Available)
- ✅ Baris Ibuprofen: Hijau muda (Status: Available)
- ✅ Baris Sanmol: Merah muda (Status: Expired)
- ✅ Baris HRIG: Merah muda (Status: Expired)
- ✅ Baris Influenza: Hijau muda (Status: Available)
- ✅ Baris Jane Doe: Hijau muda (Status: Available)

**Pemetaan Warna**:
| Status | Warna | RGB |
|--------|-------|-----|
| Available | Hijau Muda | 200, 255, 200 |
| LowStock | Kuning Muda | 255, 255, 200 |
| Expired | Merah Muda | 255, 200, 200 |

**Jika Gagal**:
- [ ] Pastikan method `TerapkanWarnaStatus()` dipanggil setelah DataSource di-set
- [ ] Cek apakah nilai RGB Color sudah benar
- [ ] Verifikasi index kolom Status adalah [4]

---

### **TEST 5: Pencarian Obat - Hasil Ditemukan**

**Tujuan**: Memverifikasi fungsi pencarian bekerja dengan benar

**Langkah**:
1. Di TextBox, ketik: `Paracetamol`
2. Klik tombol "Cari"

**Expected Result**:
- ✅ DataGridView menampilkan hanya 1 baris
- ✅ Baris menunjukkan: Paracetamol, 21, Rp 5.000, 15/06/2025, Available
- ✅ Warna baris: Hijau muda (Status: Available)

**Langkah Lanjutan** (Test pencarian lain):
1. Clear TextBox
2. Ketik: `Sanmol`
3. Klik "Cari"

**Expected Result**:
- ✅ DataGridView menampilkan hanya 1 baris
- ✅ Baris menunjukkan: Sanmol, 5, Rp 3.000, 31/12/2024, Expired
- ✅ Warna baris: Merah muda (Status: Expired)

**Jika Gagal**:
- [ ] Cek apakah input diterima dengan benar
- [ ] Pastikan method `TampilkanData()` dipanggil di button1_Click()
- [ ] Verifikasi logic pencarian menggunakan `.Contains()`

---

### **TEST 6: Pencarian Obat - Tidak Ditemukan**

**Tujuan**: Memverifikasi aplikasi handle kondisi obat tidak ditemukan

**Langkah**:
1. Clear TextBox
2. Ketik: `XYZ` (atau nama yang tidak ada)
3. Klik "Cari"

**Expected Result**:
- ✅ MessageBox muncul dengan pesan: `Obat tidak ada`
- ✅ Setelah klik OK, DataGridView kembali menampilkan semua 6 obat
- ✅ Warna baris kembali ke kondisi semula

**Jika Gagal**:
- [ ] Cek kondisi `if (hasil.Count > 0)`
- [ ] Pastikan fallback memanggil `TampilkanData(daftarObat)`

---

### **TEST 7: Pencarian - Input Kosong**

**Tujuan**: Memverifikasi validasi input kosong

**Langkah**:
1. Kosongkan TextBox (jika ada isi)
2. Klik tombol "Cari"

**Expected Result**:
- ✅ MessageBox muncul dengan pesan: `Masukan Nama Obat Dulu`
- ✅ DataGridView tetap menampilkan data sebelumnya

**Jika Gagal**:
- [ ] Cek kondisi `if (inputan == "")`

---

### **TEST 8: Case-Insensitive Search**

**Tujuan**: Memverifikasi pencarian tidak case-sensitive

**Langkah**:
1. Ketik: `PARACETAMOL` (huruf besar semua)
2. Klik "Cari"

**Expected Result**:
- ✅ DataGridView menampilkan Paracetamol
- ✅ Pencarian berhasil meskipun input huruf besar

**Langkah Lanjutan**:
1. Ketik: `paraceTaMol` (huruf campur)
2. Klik "Cari"

**Expected Result**:
- ✅ DataGridView menampilkan Paracetamol
- ✅ Pencarian berhasil

**Jika Gagal**:
- [ ] Pastikan menggunakan `.ToLower()` pada input dan nama obat

---

### **TEST 9: Partial Search**

**Tujuan**: Memverifikasi pencarian dengan substring

**Langkah**:
1. Ketik: `para` (hanya sebagian dari "Paracetamol")
2. Klik "Cari"

**Expected Result**:
- ✅ DataGridView menampilkan Paracetamol
- ✅ Pencarian menggunakan `.Contains()`

**Langkah Lanjutan**:
1. Ketik: `flu` (substring dari "Influenza")
2. Klik "Cari"

**Expected Result**:
- ✅ DataGridView menampilkan Influenza

**Jika Gagal**:
- [ ] Pastikan menggunakan `.Contains()` bukan `.Equals()`

---

### **TEST 10: Multiple Results**

**Tujuan**: Memverifikasi pencarian dengan hasil multiple

**Langkah**:
1. Ketik: `obat` (ada di "Nama Obat" tapi tidak di data)
   - Alternatif: Ketik `a` (akan cocok dengan: Paracetamol, Sanmol, HRIG, Jane Doe)
2. Klik "Cari"

**Expected Result**:
- ✅ DataGridView menampilkan beberapa baris sesuai kriteria
- ✅ Semua baris menampilkan warna berdasarkan statusnya

**Jika Gagal**:
- [ ] Cek logic loop pencarian
- [ ] Pastikan semua hasil ditambahkan ke list

---

## 🔧 Checklist Debugging

Jika ada TEST yang gagal, gunakan checklist ini:

```
□ Pastikan file Form1.cs sudah disave
□ Clean solution: Build > Clean Solution
□ Rebuild solution: Build > Rebuild Solution
□ Check Error List (Ctrl + \, then E)
□ Check Output window (Debug > Windows > Output)
□ Tambahkan breakpoint dan step through code
□ Verifikasi data sample sudah ter-update
□ Pastikan DateTime format sudah benar (tahun, bulan, hari)
□ Check apakah semua method dipanggil dengan urutan benar
```

---

## 📊 Tabel Perbandingan Status & Warna

| Data | Kondisi | Expected Status | Expected Color | Test Result |
|------|---------|-----------------|-----------------|-------------|
| Paracetamol | Stok 21, Exp 15/06/2025 | Available | Hijau | ☐ |
| Ibuprofen | Stok 17, Exp 20/08/2025 | Available | Hijau | ☐ |
| Sanmol | Stok 5, Exp 31/12/2024 | Expired | Merah | ☐ |
| HRIG | Stok 3, Exp 10/11/2024 | Expired | Merah | ☐ |
| Influenza | Stok 15, Exp 15/03/2025 | Available | Hijau | ☐ |
| Jane Doe | Stok 50, Exp 01/01/2026 | Available | Hijau | ☐ |

---

## 🎯 Summary Checklist

```
STARTUP & DISPLAY:
  □ Aplikasi berjalan tanpa error
  □ Form menampilkan title dengan statistik
  □ DataGridView menampilkan 6 baris
  □ Semua kolom terlihat lengkap

NOTIFIKASI:
  □ MessageBox obat expired muncul
  □ MessageBox obat low stock muncul
  □ Isi pesan sesuai dengan data

VISUAL INDIKATOR:
  □ Warna merah untuk Expired
  □ Warna kuning untuk LowStock (jika ada)
  □ Warna hijau untuk Available

PENCARIAN:
  □ Pencarian case-insensitive bekerja
  □ Partial search bekerja
  □ Multiple results ditampilkan
  □ Obat tidak ditemukan handled
  □ Input kosong divalidasi
  □ Reset ke semua data berfungsi

TOTAL: __ / 24 Test Passed
```

---

## 🎓 Tips Troubleshooting Lanjutan

### Problem: DataGridView kosong
```csharp
// Debug: Print ke console
Console.WriteLine($"Data count: {data.Count}");
foreach (var obat in data)
    Console.WriteLine($"- {obat.nama}");
```

### Problem: Status tidak sesuai
```csharp
// Debug: Cek masing-masing kondisi
var obat = daftarObat[0];
Console.WriteLine($"Expired: {obat.expiredDate < DateTime.Now}");
Console.WriteLine($"LowStock: {obat.stok < 10}");
Console.WriteLine($"Status: {obat.status}");
```

### Problem: Warna tidak muncul
```csharp
// Debug: Tambahkan di TerapkanWarnaStatus
Console.WriteLine($"Row {i}: Status = {status}");
```

---

## ✅ Final Approval

Setelah semua TEST selesai, tandai di bawah:

- [ ] TEST 1: Startup & Display ✓
- [ ] TEST 2: Notifikasi Expired ✓
- [ ] TEST 3: Notifikasi Low Stock ✓
- [ ] TEST 4: Visual Indikator ✓
- [ ] TEST 5: Pencarian Found ✓
- [ ] TEST 6: Pencarian Not Found ✓
- [ ] TEST 7: Input Validation ✓
- [ ] TEST 8: Case-Insensitive ✓
- [ ] TEST 9: Partial Search ✓
- [ ] TEST 10: Multiple Results ✓

**Status**: 🎉 **READY FOR PRODUCTION** 🎉

---

**Selamat! Fitur "Sistem Status Obat dan Notifikasi" sudah lulus semua testing!**
