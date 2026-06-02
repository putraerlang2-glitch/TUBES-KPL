# 🏥 Sistem Status Obat dan Notifikasi - Aplikasi Apotek TubesKPL

Fitur manajemen status obat berbasis state dengan notifikasi otomatis untuk aplikasi desktop apotek menggunakan C# dan WinForms.

---

## ✨ Fitur Utama

### 1. **Sistem Status Berbasis State**
- Status otomatis berdasarkan kondisi obat (Expired, LowStock, Available)
- Logika state-based programming untuk akurasi data

### 2. **Visual Indikator**
- 🔴 **Merah** - Obat expired (kadaluarsa)
- 🟡 **Kuning** - Obat low stock (stok rendah < 10)
- 🟢 **Hijau** - Obat available (tersedia)

### 3. **Sistem Notifikasi**
- Alert otomatis saat aplikasi dijalankan
- Peringatan untuk obat expired
- Peringatan untuk obat dengan stok rendah

### 4. **Tampilan Data Lengkap**
- Nama Obat
- Stok
- Harga
- Tanggal Expired (format: dd/MM/yyyy)
- Status (Available/LowStock/Expired)

### 5. **Fitur Pencarian**
- Cari obat berdasarkan nama
- Case-insensitive dan partial match
- Hasil pencarian menampilkan status dan warna

### 6. **Statistik Real-Time**
- Jumlah obat available
- Jumlah obat low stock
- Jumlah obat expired
- Ditampilkan di title bar

---

## 📦 Struktur Implementasi

### **Enum: StatusObat**
```csharp
public enum StatusObat
{
    Available,    // Stok >= 10 dan belum expired
    LowStock,     // Stok < 10
    Expired       // TanggalExpired < DateTime.Now
}
```

### **Class: Obat**
```csharp
public class Obat
{
    public string nama { get; set; }
    public int stok { get; set; }
    public decimal harga { get; set; }
    public DateTime expiredDate { get; set; }
    public StatusObat status { get; set; }
    
    public StatusObat HitungStatus() { ... }
    public void UpdateStatus() { ... }
}
```

### **Method Utama**
- `TampilkanData()` - Menampilkan data dengan status
- `TerapkanWarnaStatus()` - Apply warna berdasarkan status
- `TampilkanNotifikasi()` - Show alert saat startup
- `TampilkanStatistik()` - Update title bar dengan summary

---

## 🎯 Logika State-Based

```
Kondisi                          │ Status
─────────────────────────────────┼──────────────
ExpiredDate < DateTime.Now       │ Expired
Stok < 10                        │ LowStock
Selain itu                       │ Available
```

Status **TIDAK** diset manual, tapi **DIHITUNG OTOMATIS** melalui method `HitungStatus()`.

---

## 📊 Data Sample

| Nama Obat | Stok | Harga | Expired Date | Status |
|-----------|------|-------|--------------|--------|
| Paracetamol | 21 | 5,000 | 15/06/2025 | Available ✓ |
| Ibuprofen | 17 | 7,000 | 20/08/2025 | Available ✓ |
| Sanmol | 5 | 3,000 | 31/12/2024 | Expired ✗ |
| HRIG | 3 | 20,000 | 10/11/2024 | Expired ✗ |
| Influenza | 15 | 2,000 | 15/03/2025 | Available ✓ |
| Jane Doe | 50 | 500,000 | 01/01/2026 | Available ✓ |

---

## 🚀 Cara Menjalankan

### 1. **Build**
```
Build → Build Solution (Ctrl + Shift + B)
```

### 2. **Run**
```
Debug → Start Without Debugging (Ctrl + F5)
```

### 3. **Hasil**
- ✅ Form menampilkan tabel obat dengan warna
- ✅ Notifikasi obat expired (jika ada)
- ✅ Notifikasi obat low stock (jika ada)
- ✅ Statistik di title bar

---

## 📚 File Dokumentasi

| File | Deskripsi |
|------|-----------|
| `DOKUMENTASI_FITUR.md` | Dokumentasi lengkap fitur dan konsep |
| `PANDUAN_IMPLEMENTASI.md` | Panduan step-by-step implementasi |
| `CONTOH_PENGGUNAAN.cs` | Contoh kode dan penjelasan detail |
| `TESTING_GUIDE.md` | Panduan testing lengkap dengan 10 test cases |
| `README.md` | File ini - Overview ringkas |

---

## 🧪 Testing

Lihat `TESTING_GUIDE.md` untuk:
- 10 test cases lengkap
- Langkah-langkah testing detail
- Expected result untuk setiap test
- Checklist debugging

---

## 🎓 Konsep Pembelajaran

### **State-Based Programming**
Status obat ditentukan berdasarkan **kondisi saat ini**, bukan **nilai hardcoded**.

```csharp
// ❌ SALAH - Hardcoded
status = StatusObat.Available;  // Tidak dinamis!

// ✅ BENAR - Berdasarkan kondisi
if (expiredDate < DateTime.Now)
    status = StatusObat.Expired;
else if (stok < 10)
    status = StatusObat.LowStock;
else
    status = StatusObat.Available;
```

### **Separation of Concerns**
- **Logic**: Class `Obat` dengan method `HitungStatus()`
- **UI**: Form1 dengan DataGridView dan warna
- **Business**: Notifikasi dan statistik

---

## 🔄 Alur Program

```
[START]
  ↓
[Form1_Load]
  ├─ TampilkanData(daftarObat)
  │   ├─ UpdateStatus() setiap obat
  │   ├─ Buat DataTable
  │   └─ TerapkanWarnaStatus()
  │
  └─ TampilkanNotifikasi()
      ├─ Alert Expired
      ├─ Alert LowStock
      └─ TampilkanStatistik()
  ↓
[APLIKASI SIAP]
  ↓
[User Klik "Cari"]
  ├─ Validasi input
  ├─ Filter data
  └─ TampilkanData(hasil)
  ↓
[END]
```

---

## 💡 Fitur Bonus

Sudah diimplementasikan:
- ✅ Perhitungan statistik otomatis
- ✅ Update status real-time
- ✅ Visual indikator dengan 3 warna
- ✅ Notifikasi dual (expired + lowstock)

Bisa dikembangkan lebih lanjut:
- [ ] Edit/Delete obat
- [ ] Database integration
- [ ] Export ke Excel
- [ ] Advanced search filter
- [ ] Auto-reorder alert
- [ ] History tracking

---

## 🛠️ Tech Stack

- **Bahasa**: C# 7.0+
- **Framework**: .NET Framework 4.7.2
- **UI**: Windows Forms (WinForms)
- **Database**: (Optional) SQL Server / SQLite

---

## 📝 Lisensi & Kontribusi

Ini adalah project pembelajaran untuk sistem manajemen apotek.

---

## 🎉 Status

✅ **PRODUCTION READY** - Siap digunakan untuk production

---

## 📞 Bantuan

Jika ada pertanyaan:

1. **Baca file dokumentasi**:
   - `DOKUMENTASI_FITUR.md` - Konsep & alur
   - `PANDUAN_IMPLEMENTASI.md` - Step-by-step
   - `CONTOH_PENGGUNAAN.cs` - Code examples

2. **Lakukan testing**:
   - Ikuti `TESTING_GUIDE.md`
   - Perhatikan error message di Output window

3. **Debug menggunakan breakpoint**:
   - Tekan F5 (Debug mode)
   - Klik kiri pada line number untuk set breakpoint
   - Step through code dengan F10/F11

---

## 📌 Catatan Penting

⚠️ **PERHATIAN:**
- Tanggal expired yang digunakan adalah data sample
- Untuk production, data harus dari database
- Sesuaikan tanggal expired sesuai kebutuhan testing

---

**Dibuat dengan ❤️ untuk TubesKPL - Sistem Manajemen Apotek**

**Version**: 1.0  
**Status**: Production Ready ✅
