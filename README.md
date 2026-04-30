# 🏥 Sistem Status Obat dan Notifikasi - Aplikasi Apotek TubesKPL

Fitur manajemen status obat berbasis state dengan notifikasi otomatis untuk aplikasi desktop apotek menggunakan C# dan WinForms.

**NEW: Table-Driven Construction + API Simulasi (v2.0) - [AYONDI]**

---

## ✨ Fitur Utama

### 1. **Sistem Status Berbasis State**

- Status otomatis berdasarkan kondisi obat (Expired, LowStock, Available)
- Logika state-based programming untuk akurasi data
- **[NEW]** Table-Driven Construction dengan StatusConfig.cs

### 2. **Visual Indikator**

- 🔴 **Merah** - Obat expired (kadaluarsa)
- 🟡 **Kuning** - Obat low stock (stok rendah < 10)
- 🟢 **Hijau** - Obat available (tersedia)

### 3. **Sistem Notifikasi**

- Alert otomatis saat aplikasi dijalankan
- Peringatan untuk obat expired
- Peringatan untuk obat dengan stok rendah
- **[NEW]** NotifikasiHelper untuk berbagai tipe notifikasi

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
- **[NEW]** Mode indicator (API/Local) di title bar

### 7. **[NEW] API Simulasi (Tanpa Database)**

- ObatApiService.cs - Layer API untuk akses data
- Method: GetAll(), GetStatusSummary(), GetByStatus(), GetByKategori(), Search(), GetStats()
- Kompatibel dengan mode lokal dan API

### 8. **[NEW] HTTP API Server (Optional)**

- HttpApiServer.cs - Simple HTTP listener
- Endpoint: GET /obat, GET /obat/status, GET /obat/search?q=..., GET /obat/stats
- Kompatibel .NET Framework 4.7.2
- Tanpa dependency tambahan (hanya built-in HttpListener)

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

| Nama Obat   | Stok | Harga   | Expired Date | Status      |
| ----------- | ---- | ------- | ------------ | ----------- |
| Paracetamol | 21   | 5,000   | 15/06/2025   | Available ✓ |
| Ibuprofen   | 17   | 7,000   | 20/08/2025   | Available ✓ |
| Sanmol      | 5    | 3,000   | 31/12/2024   | Expired ✗   |
| HRIG        | 3    | 20,000  | 10/11/2024   | Expired ✗   |
| Influenza   | 15   | 2,000   | 15/03/2025   | Available ✓ |
| Jane Doe    | 50   | 500,000 | 01/01/2026   | Available ✓ |

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
- ✅ **[NEW]** Mode indicator di title bar ([LOCAL MODE] atau [API MODE])

---

## 🔌 [NEW] Cara Menggunakan API

### **Mode 1: Gunakan Lokal (Default)**

```csharp
// Form1 startup (default mode)
useApi = false;  // [AYONDI] Default

// Data diambil dari daftarObat List
var data = form.GetObatData();  // Return daftarObat
```

### **Mode 2: Gunakan API Simulasi**

```csharp
// Toggle ke mode API (programmatic)
form.ToggleApi();  // [AYONDI] Switch useApi = true

// Data sekarang diambil dari ObatApiService
var response = ObatApiService.GetAll();
var data = response.Data;  // Return dari API
```

### **Mode 3: Start HTTP Server (Optional)**

```csharp
// [AYONDI] Cara start HTTP server di Form1_Load atau menu

// Create server pada port 8080
HttpApiServer apiServer = new HttpApiServer(8080);
apiServer.Start();
MessageBox.Show("API Server started on " + apiServer.Prefix);

// Now accessible via:
// http://localhost:8080/obat
// http://localhost:8080/obat/status
// http://localhost:8080/obat/search?q=paracetamol
// http://localhost:8080/obat/stats

// Testing dengan browser atau Postman:
// GET http://localhost:8080/obat → JSON response dengan semua obat
// GET http://localhost:8080/obat/status → JSON response dengan summary

// Stop server
apiServer.Stop();
```

### **Contoh: Menggunakan API dalam Code**

```csharp
// [AYONDI] Contoh penggunaan ObatApiService

// 1. Initialize
ObatApiService.Initialize(daftarObat);

// 2. Get semua obat
var response = ObatApiService.GetAll();
int total = response.Count;  // Total obat
foreach (var obat in response.Data)
{
    // Process obat...
}

// 3. Get status summary
var summary = ObatApiService.GetStatusSummary();
MessageBox.Show($"Available: {summary.TotalAvailable}, " +
                $"Low: {summary.TotalLowStock}, " +
                $"Expired: {summary.TotalExpired}");

// 4. Search obat
var searchResult = ObatApiService.Search("paracetamol");
foreach (var obat in searchResult.Data)
{
    Console.WriteLine(obat.nama);
}

// 5. Get statistik
var stats = ObatApiService.GetStats();
decimal totalNilai = (decimal)stats["TotalNilai"];
int totalStok = (int)stats["TotalStok"];
```

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

## 🎯 [NEW] Table-Driven Construction & API Simulasi

### **StatusConfig.cs - Table-Driven Status Management**

```csharp
// [AYONDI] Mapping Status -> (Warna, Text, Deskripsi)
// Dictionary-based: STATUS → WARNA + TEXT
// Menghilangkan if-else panjang di TerapkanWarnaStatus()

StatusConfig config = StatusConfigManager.GetConfig(StatusObat.Expired);
// Dapatkan: Color, Text, Description secara konsisten
```

**Keuntungan:**

- ✅ Mudah di-maintain (ubah di satu tempat)
- ✅ Menghilangkan hardcoded if-else
- ✅ Scalable untuk tambah status baru
- ✅ Separation of concerns

### **ObatApiService.cs - API Simulasi Layer**

```csharp
// [AYONDI] Layer API untuk akses data tanpa HTTP server
// Simulasi API response tanpa database

// Initialize saat startup
ObatApiService.Initialize(daftarObat);

// API Methods:
var allObat = ObatApiService.GetAll();              // Get semua obat
var summary = ObatApiService.GetStatusSummary();    // Get ringkasan status
var expired = ObatApiService.GetByStatus(StatusObat.Expired);  // Filter by status
var results = ObatApiService.Search("paracetamol"); // Search obat
var stats = ObatApiService.GetStats();              // Get statistik lengkap
```

**Response Format:**

```csharp
// ObatListResponse
{
  "success": true,
  "count": 6,
  "message": "Retrieved 6 obat successfully",
  "data": [ { nama, stok, harga, ... } ],
  "responseTime": "2025-01-15 10:30:45"
}

// StatusSummaryResponse
{
  "totalAvailable": 4,
  "totalLowStock": 1,
  "totalExpired": 1,
  "totalObat": 6,
  "responseTime": "2025-01-15 10:30:45"
}
```

### **HttpApiServer.cs - HTTP API Server (Optional)**

```csharp
// [AYONDI] Simple HTTP listener untuk serve API
// Compatible .NET 4.7.2, tanpa ASP.NET Core

// Start server
HttpApiServer server = new HttpApiServer(8080);
server.Start();

// Available endpoints:
// GET http://localhost:8080/obat                    → Get semua obat
// GET http://localhost:8080/obat/status             → Get status summary
// GET http://localhost:8080/obat/search?q=nama      → Search obat
// GET http://localhost:8080/obat/stats              → Get statistik
```

### **[NEW] Toggle API Mode**

```csharp
// [AYONDI] Switch antara mode API dan mode lokal
// useApi = true  → Gunakan ObatApiService
// useApi = false → Gunakan List lokal (default)

Form1 form = new Form1();
form.ToggleApi();  // Switch mode
// Tampil notifikasi mode saat ini
```

**Title Bar Indicator:**

- `[LOCAL MODE]` - Gunakan daftarObat List
- `[API MODE]` - Gunakan ObatApiService

### **NotifikasiHelper.cs - Notification Utility**

```csharp
// [AYONDI] Utility class untuk berbagai tipe notifikasi

NotifikasiHelper.TampilkanNotifikasiExpired(daftarObat);     // Show expired obat
NotifikasiHelper.TampilkanNotifikasiLowStock(daftarObat);    // Show low stock obat
NotifikasiHelper.TampilkanNotifikasiSummary(daftarObat);     // Show summary
NotifikasiHelper.TampilkanNotifikasiDariApi(response);       // Show dari API response
NotifikasiHelper.TampilkanNotifikasiSukses("Message");       // Show success
NotifikasiHelper.TampilkanNotifikasiError("Message");        // Show error
NotifikasiHelper.TampilkanNotifikasiWarning("Message");      // Show warning
```

---

## 📦 Struktur File Baru

| File                  | Deskripsi                   | Status |
| --------------------- | --------------------------- | ------ |
| `StatusConfig.cs`     | Table-Driven status mapping | ✅     |
| `ObatApiService.cs`   | API simulasi layer          | ✅     |
| `HttpApiServer.cs`    | HTTP server (optional)      | ✅     |
| `NotifikasiHelper.cs` | Notification utility        | ✅     |

---

## 🔄 Architecture Improvements

### **Before (Hardcoded if-else)**

```csharp
// ❌ TerapkanWarnaStatus() - Hardcoded
if (status == "Expired")
    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
else if (status == "LowStock")
    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200);
// ... repeated if-else ...
```

### **After (Table-Driven)**

```csharp
// ✅ StatusConfigManager - Dictionary-based
Color color = StatusConfigManager.GetColor(status);
row.DefaultCellStyle.BackColor = color;
// Clean, maintainable, scalable
```

---

## 📚 File Dokumentasi

| File                      | Deskripsi                                    |
| ------------------------- | -------------------------------------------- |
| `DOKUMENTASI_FITUR.md`    | Dokumentasi lengkap fitur dan konsep         |
| `PANDUAN_IMPLEMENTASI.md` | Panduan step-by-step implementasi            |
| `CONTOH_PENGGUNAAN.cs`    | Contoh kode dan penjelasan detail            |
| `TESTING_GUIDE.md`        | Panduan testing lengkap dengan 10 test cases |
| `README.md`               | File ini - Overview ringkas                  |

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
- ✅ **[NEW]** Table-Driven Construction (StatusConfig)
- ✅ **[NEW]** API Simulasi (ObatApiService)
- ✅ **[NEW]** HTTP API Server (HttpApiServer)
- ✅ **[NEW]** Toggle API mode
- ✅ **[NEW]** NotifikasiHelper utility

Bisa dikembangkan lebih lanjut:

- [ ] Edit/Delete obat
- [ ] Real database integration
- [ ] Export ke Excel
- [ ] Advanced search filter
- [ ] Auto-reorder alert
- [ ] History tracking
- [ ] Authentication untuk API
- [ ] Database migration dari API

---

## 🛠️ Tech Stack

- **Bahasa**: C# 7.3
- **Framework**: .NET Framework 4.7.2
- **UI**: Windows Forms (WinForms)
- **API**: HttpListener (built-in)
- **Database**: None (in-memory List)

**Kompatibilitas:**

- ✅ C# 7.3 (No modern features)
- ✅ .NET Framework 4.7.2
- ✅ Windows Forms
- ✅ Tanpa dependency tambahan

---

## 📝 Lisensi & Kontribusi

Ini adalah project pembelajaran untuk sistem manajemen apotek.

**[AYONDI] Kontribusi:**

- ✅ Table-Driven Construction (StatusConfig.cs)
- ✅ API Simulasi Layer (ObatApiService.cs)
- ✅ HTTP API Server (HttpApiServer.cs)
- ✅ API Mode Toggle (Form1.cs)
- ✅ Notification Helper (NotifikasiHelper.cs)
- ✅ Code Analysis Comments ([AYONDI - ANALISIS])
- ✅ Complete Documentation

---

## 🎉 Status

✅ **PRODUCTION READY v2.0** - Siap digunakan dengan Table-Driven + API

---

## 📞 Bantuan

Jika ada pertanyaan:

1. **Baca file dokumentasi**:
   - `DOKUMENTASI_FITUR.md` - Konsep & alur
   - `PANDUAN_IMPLEMENTASI.md` - Step-by-step
   - `CONTOH_PENGGUNAAN.cs` - Code examples
   - `README.md` - Overview (file ini)

2. **Lakukan testing**:
   - Ikuti `TESTING_GUIDE.md`
   - Perhatikan error message di Output window

3. **Debug menggunakan breakpoint**:
   - Tekan F5 (Debug mode)
   - Klik kiri pada line number untuk set breakpoint
   - Step through code dengan F10/F11

4. **Test API (jika menggunakan HTTP Server)**:
   - Gunakan browser: `http://localhost:8080/obat`
   - Atau gunakan Postman untuk test endpoint
   - Response dalam format JSON

---

## 📌 Catatan Penting

⚠️ **PERHATIAN:**

- Tanggal expired yang digunakan adalah data sample
- Untuk production, data harus dari database
- Sesuaikan tanggal expired sesuai kebutuhan testing
- API berjalan in-memory (data hilang saat aplikasi ditutup)
- HTTP API (HttpApiServer) optional, gunakan hanya jika diperlukan

---

## 📊 Version History

| Version | Tanggal | Changes                                                |
| ------- | ------- | ------------------------------------------------------ |
| 1.0     | 2024    | Initial release dengan status dan notifikasi           |
| 2.0     | 2025    | Table-Driven Construction + API Simulasi + HTTP Server |

---

**Dibuat dengan ❤️ untuk TubesKPL - Sistem Manajemen Apotek**
**Version**: 2.0  
**Status**: Production Ready ✅
**[AYONDI] Kontribusi**: Table-Driven + API Implementation
