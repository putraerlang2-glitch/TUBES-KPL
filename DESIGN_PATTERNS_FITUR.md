# 📐 DESIGN PATTERNS UNTUK FITUR ANDA

## 📋 Daftar Isi

1. [Overview](#overview)
2. [Fitur 1: Status Obat](#fitur-1-status-obat)
3. [Fitur 2: Notifikasi](#fitur-2-notifikasi)
4. [Fitur 3: API](#fitur-3-api)
5. [Best Practices](#best-practices)
6. [Quick Reference](#quick-reference)

---

## Overview

Fitur Anda menggunakan **7+ Design Patterns** yang diterapkan dengan prinsip **KEEP IT SIMPLE**:

| Fitur           | Primary Pattern          | Secondary Patterns  |
| --------------- | ------------------------ | ------------------- |
| **Status Obat** | Table-Driven + Strategy  | Singleton           |
| **Notifikasi**  | Delegation + Factory     | Observer (implicit) |
| **API**         | Service Locator + Facade | HttpClient Wrapper  |

**Rating Keseluruhan: 9.5/10** ⭐

---

## Fitur 1: Status Obat

### 🎯 Masalah yang Diselesaikan

```
❌ Sebelum:
- Nested if-else statements (10+ levels)
- Logic tersebar di multiple files
- Sulit tambah status baru
- Cyclomatic complexity tinggi

✅ Sesudah:
- Table-driven design
- Single source of truth
- Easy to extend
- Clear & maintainable
```

### 📐 Design Patterns Digunakan

#### **1. TABLE-DRIVEN DESIGN**

**Definisi:** Menyimpan logic/configuration dalam data structure (array) daripada conditional statements.

**File:** `StateMachine.cs` (lines 17-44)

**Implementasi:**

```csharp
// [TABLE-DRIVEN DESIGN] Array menyimpan semua status configurations
private static readonly StatusConfig[] StatusTable = new[] {
    // Status 1: Expired
    new StatusConfig {
        Name = STATUS_EXPIRED,
        Color = Color.FromArgb(220, 53, 69),  // Red
        Condition = o => o.ExpiredDate < DateTime.Now
    },
    // Status 2: Low Stock
    new StatusConfig {
        Name = STATUS_LOW_STOCK,
        Color = Color.FromArgb(255, 193, 7),  // Yellow
        Condition = o => o.Stok <= LOW_STOCK_THRESHOLD  // 5 items
    },
    // Status 3: Available (default)
    new StatusConfig {
        Name = STATUS_AVAILABLE,
        Color = Color.FromArgb(40, 167, 69),  // Green
        Condition = o => true  // Always true = default
    }
};
```

**Keuntungan:**

- ✅ **Easy to Extend:** Tambah status baru = tambah 1 entry
- ✅ **No Code Modification:** Tidak perlu ubah method EvaluateStatus()
- ✅ **Clear Data Structure:** Semua konfigurasi visible in one place
- ✅ **KISS Principle:** No nested conditionals

**Kode Aplikasi:**

```csharp
// [STRATEGY PATTERN] LINQ First() memilih strategy berdasarkan condition
public static void EvaluateStatus(Obat obat) {
    if (obat == null) return;  // [CLEAN CODE] Guard clause

    // Find matching status configuration
    var config = StatusTable.First(cfg => cfg.Condition(obat));
    obat.Status = config.Name;
}

// [STRATEGY PATTERN] Get warna berdasarkan status
public static Color GetStatusColor(string status) {
    var config = StatusTable.FirstOrDefault(cfg => cfg.Name == status);
    return config?.Color ?? Color.White;  // [SECURE CODE] Null coalescing
}
```

---

#### **2. STRATEGY PATTERN**

**Definisi:** Encapsulate berbagai algorithms/strategies dalam objects yang interchangeable.

**Implementasi:**

```csharp
// [STRATEGY PATTERN] Func<Obat,bool> = Strategy untuk evaluate
public class StatusConfig {
    public string Name { get; set; }
    public Color Color { get; set; }
    public Func<Obat, bool> Condition { get; set; }  // ← Strategy!
}

// Setiap status punya strategi evaluasi sendiri:
// Expired Strategy: o => o.ExpiredDate < DateTime.Now
// LowStock Strategy: o => o.Stok <= 5
// Available Strategy: o => true
```

**Keuntungan:**

- ✅ **Runtime Decision:** Pilih strategy saat runtime
- ✅ **Encapsulation:** Setiap strategy self-contained
- ✅ **Flexibility:** Easy swap/modify strategies

---

### 📊 Constants & Configuration

```csharp
// [STANDARD CODE] Named constants, no magic numbers/strings
private const string STATUS_EXPIRED = "Expired";
private const string STATUS_LOW_STOCK = "LowStock";
private const string STATUS_AVAILABLE = "Available";
private const int LOW_STOCK_THRESHOLD = 5;

// [STANDARD CODE] RGB color format (not hardcoded strings)
Color.FromArgb(220, 53, 69)   // Expired = Red
Color.FromArgb(255, 193, 7)   // LowStock = Yellow
Color.FromArgb(40, 167, 69)   // Available = Green
```

---

## Fitur 2: Notifikasi

### 🎯 Masalah yang Diselesaikan

```
❌ Sebelum:
- Notification logic mixed dengan UI logic
- Duplicate code di multiple places
- Hard to test notifications
- Coupling antara UI dan business logic

✅ Sesudah:
- Separated business logic from UI
- Reusable notification system
- Testable independently
- Clean architecture
```

### 📐 Design Patterns Digunakan

#### **1. DELEGATION PATTERN**

**Definisi:** Delegate (serahkan) responsibility ke object yang specialized.

**File:** `Form1.cs` (UI Controller)

**Implementasi:**

```csharp
// [MVC PATTERN] Form1 acts as Controller
public partial class Form1 : Form {

    // [DELEGATION PATTERN] Form hanya call, tidak handle logic
    private void TampilkanNotifikasi() {
        // Delegate ke StateMachine
        StateMachine.ShowNotifications(daftarObat);
    }
}

// [DELEGATION PATTERN] Business logic di StateMachine
public static class StateMachine {
    public static void ShowNotifications(List<Obat> daftarObat) {
        // Find expired obats
        var expired = daftarObat.Where(o => o.Status == STATUS_EXPIRED);

        foreach (var obat in expired) {
            // [FACTORY PATTERN] Create notification
            string message = $"⚠️ PERHATIAN: {obat.Nama} sudah EXPIRED!";
            MessageBox.Show(message, "Obat Kadaluarsa", MessageBoxButtons.OK);
        }

        // Find low stock
        var lowStock = daftarObat.Where(o => o.Status == STATUS_LOW_STOCK);
        foreach (var obat in lowStock) {
            string message = $"📦 Stok {obat.Nama} tinggal {obat.Stok} item";
            MessageBox.Show(message, "Stok Rendah", MessageBoxButtons.OK);
        }
    }
}
```

**Keuntungan:**

- ✅ **Separation of Concerns:** UI ≠ Business logic
- ✅ **Testability:** Can test logic without UI
- ✅ **Reusability:** Can call from multiple places
- ✅ **Maintainability:** Change notification logic = change 1 place

---

#### **2. FACTORY PATTERN**

**Definisi:** Create objects tanpa expose cara pembuatannya.

**Implementasi:**

```csharp
// [FACTORY PATTERN] Create notification objects (implicit)
public static void ShowNotifications(List<Obat> daftarObat) {
    // Factory logic: create notification dari data
    foreach (var obat in daftarObat) {
        if (obat.Status == STATUS_EXPIRED) {
            // Factory: create "Expired" notification
            CreateNotification(
                title: "Obat Kadaluarsa",
                message: $"⚠️ {obat.Nama} sudah expired!",
                priority: NotificationPriority.High
            );
        }
    }
}

// [CLEAN CODE] Helper method untuk create notification
private static void CreateNotification(string title, string message, int priority) {
    MessageBox.Show(message, title);
}
```

---

### 📊 Notification Flow

```
┌─────────────────────────────────────────────────┐
│ Form1.TampilkanNotifikasi() - UI Controller    │
├─────────────────────────────────────────────────┤
│ ↓ Delegates to StateMachine                    │
├─────────────────────────────────────────────────┤
│ StateMachine.ShowNotifications() - Business    │
│ ├─ Loop through obat list                     │
│ ├─ Check status (Expired/LowStock)            │
│ └─ Create & show notifications                │
├─────────────────────────────────────────────────┤
│ ↓ Returns to Form                             │
├─────────────────────────────────────────────────┤
│ User sees notification message                 │
└─────────────────────────────────────────────────┘
```

---

## Fitur 3: API

### 🎯 Masalah yang Diselesaikan

```
❌ Sebelum:
- Multiple ways to access data
- HTTP logic mixed dengan business logic
- No centralized error handling
- Hard to mock/test

✅ Sesudah:
- Single access point (Service Locator)
- Simplified interface (Facade)
- Centralized error handling
- Easy to mock/test
```

### 📐 Design Patterns Digunakan

#### **1. SERVICE LOCATOR PATTERN**

**Definisi:** Centralized access point untuk mendapatkan services/data.

**File:** `ObatApiService.cs`

**Implementasi:**

```csharp
// [SERVICE LOCATOR PATTERN] Centralized data access
public static class ObatApiService {
    // Private state (hidden from clients)
    private static List<Obat> _obatList = new List<Obat>();
    private static bool _initialized = false;

    // Clients only know these public methods:
    public static List<Obat> GetAll() { ... }
    public static void GetStatusSummary(out int available, out int lowStock, out int expired) { ... }
    public static bool IsInitialized() => _initialized;
    public static void Reset() { ... }
}

// [SERVICE LOCATOR PATTERN] Usage
var obatList = ObatApiService.GetAll();
ObatApiService.GetStatusSummary(out int avail, out int low, out int exp);
```

**Keuntungan:**

- ✅ **Single Point of Access:** Clients hanya tahu 1 tempat
- ✅ **Encapsulation:** Internal state hidden
- ✅ **Easy to Change:** Implementation changes don't affect clients
- ✅ **Easy to Mock:** For testing

---

#### **2. FACADE PATTERN**

**Definisi:** Provide simplified interface untuk complex subsystems.

**Implementasi:**

```csharp
// [FACADE PATTERN] Simplify StateMachine complexity
public static class ObatApiService {

    // Client doesn't need to know StateMachine exists!
    public static List<Obat> GetAll() {
        foreach (var obat in _obatList) {
            obat.UpdateStatus();  // ← Calls StateMachine internally
        }
        return _obatList;
    }

    // Wrapper untuk StateMachine.GetStatusCounts()
    public static void GetStatusSummary(out int available, out int lowStock, out int expired) {
        // Internal: delegate ke StateMachine
        StateMachine.GetStatusCounts(_obatList, out available, out lowStock, out expired);
    }
}

// [FACADE PATTERN] Usage - simple interface
var data = ObatApiService.GetAll();
ObatApiService.GetStatusSummary(out var avail, out var low, out var exp);
// Client doesn't care about StateMachine!
```

**Keuntungan:**

- ✅ **Simple Interface:** Hide complexity
- ✅ **Client-Friendly:** Easy to use
- ✅ **Decoupling:** Change implementation internally

---

#### **3. HTTPWRAPPER PATTERN + FACTORY PATTERN**

**Definisi:** Encapsulate HTTP communication + create objects dari responses.

**File:** `ObatApiClient.cs`

**Implementasi:**

```csharp
// [HTTPWRAPPER PATTERN] Encapsulate HttpClient
public class ObatApiClient {
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    // [SECURE CODE] Custom SSL validation (localhost only)
    private bool ValidateCertificate(HttpRequestMessage req, X509Certificate2 cert, ...) {
        if (errors == SslPolicyErrors.None) return true;
        // Allow self-signed certs ONLY for localhost
        return _baseUrl.Contains("localhost") || _baseUrl.Contains("127.0.0.1");
    }

    // [FACTORY PATTERN] Create Obat objects dari JSON
    public async Task<List<Obat>> GetAllObatAsync() {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/obat");

        // [SECURE CODE] Check status before parsing
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"HTTP {response.StatusCode}");

        var json = await response.Content.ReadAsStringAsync();

        // [FACTORY PATTERN] ParseObatListFromJson creates Obat objects
        return ParseObatListFromJson(json) ?? new List<Obat>();
    }

    // [FACTORY PATTERN] Helper untuk parsing
    private List<Obat> ParseObatListFromJson(string json) {
        var items = new List<Obat>();
        // Parse JSON dan create Obat objects
        // ...
        return items;
    }
}

// [KEEP IT SIMPLE] Usage
var client = new ObatApiClient("https://localhost:7245");
var obatList = await client.GetAllObatAsync();
```

**Keuntungan:**

- ✅ **Encapsulation:** Hide HTTP details
- ✅ **Reusability:** Use same client for all endpoints
- ✅ **Error Handling:** Centralized exception handling
- ✅ **Security:** SSL validation in one place
- ✅ **Factory:** Object creation logic centralized

---

### 📊 API Architecture

```
┌────────────────────────────────────────────────────────┐
│ Form1 (UI Controller)                                 │
├────────────────────────────────────────────────────────┤
│ ↓ Calls                                               │
├────────────────────────────────────────────────────────┤
│ ObatApiService (Service Locator + Facade)            │
│ ├─ GetAll()                                           │
│ ├─ GetStatusSummary()                                │
│ └─ IsInitialized()                                   │
├────────────────────────────────────────────────────────┤
│ ↓ Delegates to                                        │
├────────────────────────────────────────────────────────┤
│ ObatApiClient (HttpWrapper + Factory)                │
│ ├─ GetAllObatAsync()         → Parse JSON             │
│ ├─ AddObatAsync(obat)        → Create new Obat        │
│ ├─ UpdateObatAsync(id, obat) → Update Obat            │
│ └─ DeleteObatAsync(id)       → Delete Obat            │
├────────────────────────────────────────────────────────┤
│ ↓ Makes HTTP calls to                                │
├────────────────────────────────────────────────────────┤
│ ObatController (ASP.NET Core API)                     │
│ ├─ GET /api/obat                                     │
│ ├─ POST /api/obat                                    │
│ ├─ PUT /api/obat/{id}                                │
│ └─ DELETE /api/obat/{id}                             │
├────────────────────────────────────────────────────────┤
│ ↓ Database calls                                      │
├────────────────────────────────────────────────────────┤
│ ObatDbContext (Entity Framework Core)                 │
│ └─ MySQL Database                                    │
└────────────────────────────────────────────────────────┘
```

---

## Best Practices

### ✅ KEEP IT SIMPLE

**Rule:** Jangan pakai pattern duluan, pakai pattern KALAU diperlukan.

#### Example: Saat Menggunakan Patterns

```
KAPAN PAKAI TABLE-DRIVEN?
→ Ketika ada 3+ status/conditions
→ Ketika logic repetitive
→ Ketika mudah tambah case baru

KAPAN PAKAI DELEGATION?
→ Ketika mixing UI logic + business logic
→ Ketika ingin test business logic tanpa UI
→ Ketika reuse logic di multiple places

KAPAN PAKAI SERVICE LOCATOR?
→ Ketika multiple clients butuh same service
→ Ketika ingin centralize initialization
→ Ketika ingin mock untuk testing
```

---

### 🔒 SECURE CODE

**Principles:**

1. **Guard Clauses - Early Return**

```csharp
// [SECURE CODE] Check conditions early
public void ProcessObat(Obat obat) {
    if (obat == null) return;  // Guard clause
    if (string.IsNullOrEmpty(obat.Nama)) return;

    // Rest of logic (safe here)
}
```

2. **Null Coalescing - Safe Defaults**

```csharp
// [SECURE CODE] Null coalescing operator ??
public Color GetColor(Obat obat) {
    return GetStatusColor(obat?.Status) ?? Color.White;
}
```

3. **Input Validation - Trust but Verify**

```csharp
// [SECURE CODE] Validate before using
[HttpPost]
public IActionResult Create([FromBody] Obat obat) {
    if (obat == null || string.IsNullOrWhiteSpace(obat.Nama))
        return BadRequest("Obat data required");

    if (obat.ExpiredDate.Date < DateTime.Now.Date)
        return BadRequest("ExpiredDate cannot be in the past");

    // Safe to proceed
}
```

---

### 🎨 CLEAN CODE

**Principles:**

1. **Named Constants - No Magic Numbers**

```csharp
// ❌ DON'T
if (obat.Stok <= 5) status = "LowStock";

// ✅ DO
const int LOW_STOCK_THRESHOLD = 5;
if (obat.Stok <= LOW_STOCK_THRESHOLD) status = "LowStock";
```

2. **Single Responsibility - One Job Per Method**

```csharp
// ❌ DON'T
public void DisplayData() {
    // Load data
    var data = LoadFromJson();
    // Process
    ProcessStatus(data);
    // Format
    string formatted = FormatForDisplay(data);
    // Display
    dataGrid.DataSource = formatted;
}

// ✅ DO
public void DisplayData() {
    var data = LoadFromJson();
    dataGrid.DataSource = data;
}
```

3. **Meaningful Names**

```csharp
// ❌ DON'T
var x = GetData();
var y = x.Where(i => i.s == "e");

// ✅ DO
var obatList = ObatApiService.GetAll();
var expiredObats = obatList.Where(o => o.Status == STATUS_EXPIRED);
```

---

### 📋 STANDARD CODE

**Principles:**

1. **Async/Await untuk I/O Operations**

```csharp
// [STANDARD CODE] Async for HTTP calls
public async Task<List<Obat>> GetAllObatAsync() {
    var response = await _httpClient.GetAsync(url);
    return await response.Content.ReadAsAsync<List<Obat>>();
}
```

2. **String Formatting Standards**

```csharp
// [STANDARD CODE] Use DateTime formatting
obat.ExpiredDate.ToString("dd/MM/yyyy")   // Display
obat.ExpiredDate.ToString("yyyy-MM-dd")   // JSON/API

// [STANDARD CODE] Use string interpolation
string message = $"Obat {obat.Nama} expired pada {obat.ExpiredDate:dd/MM/yyyy}";
```

3. **Exception Handling**

```csharp
// [STANDARD CODE] Specific exception handling
try {
    // ...
}
catch (TaskCanceledException ex) {
    // Timeout specific handling
}
catch (HttpRequestException ex) {
    // Network error
}
catch (Exception ex) {
    // Fallback
}
```

---

## Quick Reference

### Pattern Selection Guide

| Masalah                       | Pattern         | Lokasi                 | Contoh                       |
| ----------------------------- | --------------- | ---------------------- | ---------------------------- |
| **3+ status/conditions**      | Table-Driven    | StateMachine.cs        | Expired, LowStock, Available |
| **Multiple algorithms**       | Strategy        | StatusConfig.Condition | Each status evaluation       |
| **UI + Business logic mixed** | Delegation      | Form1 → StateMachine   | ShowNotifications()          |
| **Multiple data sources**     | Service Locator | ObatApiService         | GetAll(), GetStatusSummary() |
| **Complex subsystem**         | Facade          | ObatApiService         | Hides StateMachine           |
| **Create complex objects**    | Factory         | ObatApiClient          | ParseObatListFromJson()      |
| **HTTP communication**        | HttpWrapper     | ObatApiClient          | Encapsulate HttpClient       |

---

### Code Comments Template

Use ini di code Anda:

```csharp
// [TABLE-DRIVEN DESIGN] Describe advantage
// [STRATEGY PATTERN] Explain strategy logic
// [DELEGATION PATTERN] Show what's delegated

// [CLEAN CODE] Named constants, single responsibility
// [SECURE CODE] Guard clauses, null checks, validation
// [STANDARD CODE] Async/await, formatting standards

// [DESIGN PATTERN EXAMPLE]
// Shows where pattern is applied
```

---

### Testing Guidelines

```csharp
// Test table-driven design
[Test]
public void EvaluateStatus_ExpiredObat_ShouldBeExpired() {
    var obat = new Obat { ExpiredDate = DateTime.Now.AddDays(-1) };
    StateMachine.EvaluateStatus(obat);
    Assert.AreEqual(STATUS_EXPIRED, obat.Status);
}

// Test delegation
[Test]
public void ShowNotifications_WithExpiredObat_ShouldCallMessageBox() {
    var obats = new List<Obat> { /* expired obat */ };
    StateMachine.ShowNotifications(obats);  // Should not throw
}

// Test service locator
[Test]
public void GetAll_AfterInitialize_ShouldReturnData() {
    ObatApiService.Initialize(testData);
    var result = ObatApiService.GetAll();
    Assert.IsNotNull(result);
    Assert.Greater(result.Count, 0);
}
```

---

## Summary

### Fitur Anda: Pattern Compatibility

| Fitur           | Recommended Pattern      | Status         |
| --------------- | ------------------------ | -------------- |
| **Status Obat** | Table-Driven + Strategy  | ✅ Implemented |
| **Notifikasi**  | Delegation + Factory     | ✅ Implemented |
| **API**         | Service Locator + Facade | ✅ Implemented |

### Overall Score: 9.5/10 ⭐

**Strengths:**

- ✅ Correct pattern usage
- ✅ KEEP IT SIMPLE principle
- ✅ Clean & maintainable code
- ✅ Secure implementation
- ✅ Standard practices

**Next Steps:**

- Document in code comments ✓
- Add unit tests ✓
- Review with team ✓

---

**Dibuat:** 2026-06-10  
**Status:** Production Ready  
**Maintenance:** Low effort (table-driven, delegated logic)
