# Dokumentasi Fitur: Sistem Status Obat dan Notifikasi

## Gambaran Umum
Fitur ini mengimplementasikan sistem manajemen status obat berbasis state dengan notifikasi otomatis untuk obat yang expired atau memiliki stok rendah.

---

## 1. Enum StatusObat

```csharp
public enum StatusObat
{
    Available,      // Obat tersedia dan stok cukup (>= 10)
    LowStock,       // Stok rendah (< 10 unit)
    Expired         // Obat sudah kadaluarsa (ExpiredDate < DateTime.Now)
}
```

---

## 2. Class Obat

### Properti:
- `nama` (string): Nama obat
- `stok` (int): Jumlah stok tersedia
- `harga` (decimal): Harga obat per unit
- `expiredDate` (DateTime): Tanggal kadaluarsa
- `status` (StatusObat): Status otomatis obat

### Method:

#### `HitungStatus()`
Menghitung status obat berdasarkan logika:
```
IF expiredDate < DateTime.Now
    → Status = Expired
ELSE IF stok < 10
    → Status = LowStock
ELSE
    → Status = Available
```

#### `UpdateStatus()`
Mengupdate status obat (dipanggil sebelum menampilkan data)

---

## 3. Logika State-Based Programming

Status obat ditentukan secara otomatis melalui:

1. **Pengecekan Tanggal Expired**: Jika tanggal expired sudah lewat dari tanggal hari ini
2. **Pengecekan Stok Minimum**: Jika stok kurang dari 10 unit
3. **Status Default**: Jika kedua kondisi di atas tidak terpenuhi

Setiap kali data ditampilkan, status diperbarui untuk memastikan data selalu akurat.

---

## 4. Fitur Tampilan Data

### DataGridView Columns:
1. **Nama Obat**: Nama produk
2. **Stok**: Jumlah unit tersedia
3. **Harga**: Harga per unit (format currency)
4. **Tanggal Expired**: Format dd/MM/yyyy
5. **Status**: Available, LowStock, atau Expired

### Indikator Warna (Visual Feedback):
- **Merah Muda** (RGB: 255, 200, 200): Status Expired ⚠️
- **Kuning Muda** (RGB: 255, 255, 200): Status LowStock ⚠️
- **Hijau Muda** (RGB: 200, 255, 200): Status Available ✓

---

## 5. Sistem Notifikasi

### Notifikasi Expired
Menampilkan MessageBox dengan daftar semua obat yang sudah kadaluarsa beserta tanggal expirenya.

### Notifikasi Low Stock
Menampilkan MessageBox dengan daftar semua obat yang memiliki stok rendah beserta jumlah stoknya.

### Statistik
Judul form menampilkan ringkasan:
```
Form1 - Available: X | Low Stock: Y | Expired: Z
```

Notifikasi muncul otomatis saat aplikasi dimulai (Form Load event).

---

## 6. Method Utama

### `Form1_Load()`
- Memanggil `TampilkanData()` untuk menampilkan semua obat
- Memanggil `TampilkanNotifikasi()` untuk menampilkan peringatan

### `TampilkanData(List<Obat> data)`
- Mengupdate status semua obat
- Membuat DataTable dengan kolom sesuai kebutuhan
- Menampilkan data di DataGridView
- Menerapkan warna berdasarkan status

### `TerapkanWarnaStatus()`
- Mengubah background color setiap row berdasarkan status obat

### `TampilkanNotifikasi()`
- Menampilkan peringatan obat expired
- Menampilkan peringatan obat low stock
- Memanggil `TampilkanStatistik()`

### `TampilkanStatistik()`
- Menghitung jumlah obat per status
- Menampilkan statistik di title bar aplikasi

### `button1_Click()` (Tombol Cari)
- Mencari obat berdasarkan nama
- Menampilkan hasil pencarian dengan status dan warna
- Menampilkan pesan jika obat tidak ditemukan

---

## 7. Data Sample

Aplikasi sudah dilengkapi dengan 6 data obat sample:

| Nama Obat | Stok | Harga | Expired Date | Status |
|-----------|------|-------|--------------|--------|
| Paracetamol | 21 | 5,000 | 15/06/2025 | Available |
| Ibuprofen | 17 | 7,000 | 20/08/2025 | Available |
| Sanmol | 5 | 3,000 | 31/12/2024 | Expired |
| HRIG | 3 | 20,000 | 10/11/2024 | Expired |
| Influenza | 15 | 2,000 | 15/03/2025 | Available |
| Jane Doe | 50 | 500,000 | 01/01/2026 | Available |

*Note: Status contoh di atas berlaku untuk tanggal hari ini. Sesuaikan tanggal sesuai kebutuhan testing.*

---

## 8. Event Handler

### `dataGridView1_CellContentClick()`
- Siap untuk menambahkan logika custom saat cell diklik

### `textBox1_TextChanged()`
- Siap untuk menambahkan live search atau auto-complete

---

## 9. Fitur Bonus: Perhitungan Statistik

Method `TampilkanStatistik()` menghitung:
- Jumlah obat dengan status Available
- Jumlah obat dengan status LowStock
- Jumlah obat dengan status Expired

Hasil ditampilkan di title bar aplikasi untuk monitoring real-time.

---

## 10. Alur Program

```
[Aplikasi Dimulai]
        ↓
[Form1_Load Event]
        ↓
[TampilkanData()] → Update status, buat DataTable, terapkan warna
        ↓
[TampilkanNotifikasi()] → Cek & tampilkan peringatan
        ↓
[TampilkanStatistik()] → Hitung & tampilkan di title bar
        ↓
[Aplikasi Siap Digunakan]
        ↓
[User Klik "Cari"] → button1_Click() → TampilkanData(hasil pencarian)
```

---

## 11. Cara Penggunaan

### Menampilkan Semua Obat
1. Aplikasi langsung menampilkan semua obat saat startup
2. Peringatan otomatis ditampilkan jika ada obat expired atau low stock

### Mencari Obat
1. Ketik nama obat di TextBox
2. Klik tombol "Cari"
3. Hasil pencarian ditampilkan dengan status dan warna sesuai

### Interpretasi Warna
- **Merah**: Obat sudah expired, harus dihapus dari inventaris
- **Kuning**: Stok rendah, perlu order ulang
- **Hijau**: Obat dalam kondisi baik, stok mencukupi

---

## 12. Pengembangan Lebih Lanjut

Fitur-fitur yang bisa ditambahkan:
1. **Fungsi Edit**: Memungkinkan user mengubah stok atau tanggal expired
2. **Fungsi Delete**: Menghapus obat dari list
3. **Export Data**: Menyimpan data ke file Excel/CSV
4. **Database Integration**: Menyimpan data ke database
5. **Advanced Search**: Filter berdasarkan status atau range harga
6. **Reorder Alert**: Otomatis mengingatkan saat stok mendekati minimum
7. **History Log**: Mencatat perubahan stok dan status

---

**Versi**: 1.0  
**Dibuat**: 2024  
**Status**: Production Ready
