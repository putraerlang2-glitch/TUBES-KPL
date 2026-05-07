using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TubesKPL
{
    /// <summary>
    /// ObatApiClient adalah HTTP client untuk komunikasi dengan ObatAPI
    /// Menggunakan manual JSON parsing (sama seperti JsonDataManager)
    /// 
    /// API Server: ObatAPI (.NET 6) - project terpisah
    /// Base URL: https://localhost:7103 (default HTTPS untuk .NET 6)
    /// 
    /// ⚠️ PENTING: 
    /// - Pastikan ObatAPI sudah running sebelum TubesKPL
    /// - Jalankan set Multiple Startup Projects
    /// 
    /// API Response Format (dari server .NET 6):
    /// {
    ///   "id": 1,
    ///   "nama": "Paracetamol",
    ///   "kategori": "Tablet",
    ///   "stok": 100,
    ///   "harga": 5000,
    ///   "expiredDate": "2025-01-15T00:00:00",
    ///   "status": "Available"
    /// }
    /// </summary>
    public class ObatApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        // Default port untuk ASP.NET Core 6 development
        public ObatApiClient(string baseUrl = "https://localhost:7103")
        {
            _baseUrl = baseUrl;

            // Allow self-signed certificate untuk development HTTPS
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Ambil semua data obat dari API ObatAPI
        /// GET /api/obat
        /// </summary>
        public async Task<List<Obat>> GetAllObatAsync()
        {
            try
            {
                string url = $"{_baseUrl}/api/obat";
                System.Console.WriteLine($"[API CLIENT] GET {url}");

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    System.Console.WriteLine($"[API CLIENT] Response: {json.Substring(0, Math.Min(100, json.Length))}...");

                    List<Obat> obatList = ParseObatListFromJson(json);
                    System.Console.WriteLine($"[API CLIENT] Parsed {obatList.Count} items");

                    return obatList ?? new List<Obat>();
                }
                else
                {
                    throw new Exception($"API Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP Error: {ex.Message}\nPastikan ObatAPI sudah running di {_baseUrl}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new Exception($"Connection Timeout: {ex.Message}\nObatAPI server tidak merespons", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching obat data: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Ambil obat berdasarkan ID
        /// GET /api/obat/{id}
        /// </summary>
        public async Task<Obat> GetObatByIdAsync(int id)
        {
            try
            {
                string url = $"{_baseUrl}/api/obat/{id}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return ParseObatFromJson(json);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception($"Obat dengan ID {id} tidak ditemukan");
                }
                else
                {
                    throw new Exception($"API Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching obat: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tambah obat baru ke API
        /// POST /api/obat
        /// </summary>
        public async Task<Obat> AddObatAsync(Obat obat)
        {
            try
            {
                string url = $"{_baseUrl}/api/obat";
                string json = SerializeObat(obat);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    return ParseObatFromJson(responseJson);
                }
                else
                {
                    throw new Exception($"API Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding obat: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update obat di API
        /// PUT /api/obat/{id}
        /// </summary>
        public async Task<Obat> UpdateObatAsync(int id, Obat obat)
        {
            try
            {
                string url = $"{_baseUrl}/api/obat/{id}";
                string json = SerializeObat(obat);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    return ParseObatFromJson(responseJson);
                }
                else
                {
                    throw new Exception($"API Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating obat: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hapus obat dari API
        /// DELETE /api/obat/{id}
        /// </summary>
        public async Task<bool> DeleteObatAsync(int id)
        {
            try
            {
                string url = $"{_baseUrl}/api/obat/{id}";
                HttpResponseMessage response = await _httpClient.DeleteAsync(url);

                return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting obat: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Helper: Parse JSON array ke List<Obat>
        /// Format: [{"id":1,"nama":"...", ...}, ...]
        /// </summary>
        private List<Obat> ParseObatListFromJson(string json)
        {
            List<Obat> result = new List<Obat>();

            try
            {
                // Remove whitespace
                json = json.Replace("\n", "").Replace("\r", "").Replace("\t", "");

                // Find array brackets [ ... ]
                int arrayStart = json.IndexOf("[");
                int arrayEnd = json.LastIndexOf("]");

                if (arrayStart == -1 || arrayEnd == -1 || arrayStart >= arrayEnd)
                {
                    return result;
                }

                string arrayContent = json.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);

                // Parse setiap object dalam array
                int currentPos = 0;
                while (currentPos < arrayContent.Length)
                {
                    int objStart = arrayContent.IndexOf("{", currentPos);
                    if (objStart == -1) break;

                    int objEnd = FindMatchingBracket(arrayContent, objStart);
                    if (objEnd == -1) break;

                    string objContent = arrayContent.Substring(objStart + 1, objEnd - objStart - 1);
                    Obat obat = ParseObatObject(objContent);

                    if (obat != null)
                    {
                        result.Add(obat);
                    }

                    currentPos = objEnd + 1;
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[ERROR] ParseObatListFromJson: {ex.Message}");
                return result;
            }
        }

        /// <summary>
        /// Helper: Parse single JSON object ke Obat
        /// </summary>
        private Obat ParseObatFromJson(string json)
        {
            try
            {
                json = json.Replace("\n", "").Replace("\r", "").Replace("\t", "");

                int objStart = json.IndexOf("{");
                int objEnd = FindMatchingBracket(json, objStart);

                if (objStart == -1 || objEnd == -1)
                {
                    return null;
                }

                string objContent = json.Substring(objStart + 1, objEnd - objStart - 1);
                return ParseObatObject(objContent);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[ERROR] ParseObatFromJson: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Helper: Parse object content ke Obat instance
        /// Menghandle response dari ASP.NET Core API dengan PascalCase properties
        /// Content format: "id":1,"nama":"Paracetamol","kategori":"Tablet","stok":100,"harga":5000,"expiredDate":"2025-01-15","status":"Available"
        /// </summary>
        private Obat ParseObatObject(string objContent)
        {
            try
            {
                // Parse ID
                int id = ExtractIntValue(objContent, "\"id\"");

                // Parse nama (required)
                string nama = ExtractStringValue(objContent, "\"nama\"");
                if (string.IsNullOrEmpty(nama)) 
                    return null;

                // Parse stok
                int stok = ExtractIntValue(objContent, "\"stok\"");

                // Parse harga
                decimal harga = ExtractDecimalValue(objContent, "\"harga\"");

                // Parse expiredDate - API server return ISO format: "2025-01-15T00:00:00" atau "2025-01-15"
                DateTime expiredDate = DateTime.Now.AddYears(1); // default 1 tahun dari sekarang
                string dateStr = ExtractStringValue(objContent, "\"expiredDate\"");
                if (!string.IsNullOrEmpty(dateStr))
                {
                    // Try parse dengan berbagai format ISO 8601
                    if (DateTime.TryParse(dateStr, out DateTime parsed))
                    {
                        expiredDate = parsed;
                    }
                    else
                    {
                        System.Console.WriteLine($"[WARN] Could not parse date: {dateStr}");
                    }
                }

                // Parse kategori (string dari API)
                string kategori = ExtractStringValue(objContent, "\"kategori\"");
                if (string.IsNullOrEmpty(kategori))
                    kategori = "Tablet";

                // Parse status (string dari API)
                string status = ExtractStringValue(objContent, "\"status\"");
                if (string.IsNullOrEmpty(status))
                    status = "Available";

                // Create Obat instance dengan ID
                Obat obat = new Obat(nama, stok, harga, expiredDate, kategori, id)
                {
                    Status = status
                };

                System.Console.WriteLine($"[PARSE] Obat parsed: {obat}");
                return obat;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[ERROR] ParseObatObject: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Helper: Serialize Obat ke JSON string untuk POST/PUT
        /// Format disesuaikan dengan API server (.NET 6)
        /// </summary>
        private string SerializeObat(Obat obat)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"nama\":\"{EscapeJsonString(obat.Nama)}\",");
            sb.Append($"\"kategori\":\"{EscapeJsonString(obat.Kategori)}\",");
            sb.Append($"\"stok\":{obat.Stok},");
            sb.Append($"\"harga\":{obat.Harga},");
            sb.Append($"\"expiredDate\":\"{obat.ExpiredDate:yyyy-MM-ddTHH:mm:ss}\"");
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Helper: Extract string value dari JSON
        /// Mendukung baik lowercase maupun PascalCase keys
        /// </summary>
        private string ExtractStringValue(string json, string key)
        {
            try
            {
                int keyPos = json.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (keyPos == -1) return string.Empty;

                int colonPos = json.IndexOf(":", keyPos);
                if (colonPos == -1) return string.Empty;

                int quoteStart = json.IndexOf("\"", colonPos);
                if (quoteStart == -1) return string.Empty;

                quoteStart++; // Move past the opening quote
                int quoteEnd = json.IndexOf("\"", quoteStart);

                if (quoteStart > 0 && quoteEnd > quoteStart)
                {
                    string value = json.Substring(quoteStart, quoteEnd - quoteStart);
                    return UnescapeJsonString(value);
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Helper: Extract int value dari JSON
        /// </summary>
        private int ExtractIntValue(string json, string key)
        {
            try
            {
                int keyPos = json.IndexOf(key);
                if (keyPos == -1) return 0;

                int colonPos = json.IndexOf(":", keyPos);
                if (colonPos == -1) return 0;

                int numberStart = colonPos + 1;
                while (numberStart < json.Length && (json[numberStart] == ' ' || json[numberStart] == ':'))
                    numberStart++;

                int numberEnd = numberStart;
                while (numberEnd < json.Length && char.IsDigit(json[numberEnd]))
                    numberEnd++;

                if (numberEnd > numberStart)
                {
                    string numStr = json.Substring(numberStart, numberEnd - numberStart);
                    if (int.TryParse(numStr, out int result))
                        return result;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Helper: Extract decimal value dari JSON
        /// </summary>
        private decimal ExtractDecimalValue(string json, string key)
        {
            try
            {
                int keyPos = json.IndexOf(key);
                if (keyPos == -1) return 0;

                int colonPos = json.IndexOf(":", keyPos);
                if (colonPos == -1) return 0;

                int numberStart = colonPos + 1;
                while (numberStart < json.Length && (json[numberStart] == ' ' || json[numberStart] == ':'))
                    numberStart++;

                int numberEnd = numberStart;
                while (numberEnd < json.Length && (char.IsDigit(json[numberEnd]) || json[numberEnd] == '.'))
                    numberEnd++;

                if (numberEnd > numberStart)
                {
                    string numStr = json.Substring(numberStart, numberEnd - numberStart);
                    if (decimal.TryParse(numStr, out decimal result))
                        return result;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Helper: Find matching closing bracket
        /// </summary>
        private int FindMatchingBracket(string json, int openPos)
        {
            int count = 1;
            for (int i = openPos + 1; i < json.Length; i++)
            {
                if (json[i] == '{') count++;
                else if (json[i] == '}') count--;

                if (count == 0) return i;
            }
            return -1;
        }

        /// <summary>
        /// Helper: Escape string untuk JSON
        /// </summary>
        private string EscapeJsonString(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            return str.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        /// <summary>
        /// Helper: Unescape string dari JSON
        /// </summary>
        private string UnescapeJsonString(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            return str.Replace("\\\"", "\"").Replace("\\\\", "\\");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
