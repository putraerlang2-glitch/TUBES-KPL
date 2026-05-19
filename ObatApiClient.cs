using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace TubesKPL
{
    /// <summary>
    /// HTTP client untuk komunikasi dengan ObatAPI.
    /// Menggunakan robust JSON parsing dengan comprehensive error handling.
    /// </summary>
    public class ObatApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        /// <summary>
        /// Constructor dengan base URL validation
        /// </summary>
        public ObatApiClient(string baseUrl = "https://localhost:7103")
        {
            // Defensive: Null/empty check
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl), "Base URL tidak boleh kosong");

            _baseUrl = baseUrl;
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errs) => true;
            _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };
        }

        /// <summary>
        /// Mengambil semua data obat dari API dengan error handling
        /// </summary>
        public async Task<List<Obat>> GetAllObatAsync()
        {
            try
            {
                // Defensive: Validate base URL
                if (string.IsNullOrWhiteSpace(_baseUrl))
                    throw new Exception("Base URL tidak valid");

                string url = $"{_baseUrl}/api/obat";
                var response = await _httpClient.GetAsync(url);

                // Defensive: Check response status
                if (!response.IsSuccessStatusCode)
                    throw new ApiException($"API failed with status {response.StatusCode}", response.StatusCode);

                // Defensive: Ensure content exists
                string json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine("[WARN] API returned empty response");
                    return new List<Obat>();
                }

                var result = ParseObatListFromJson(json);
                return result ?? new List<Obat>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"❌ Connection Error: API tidak dapat diakses di {_baseUrl}\nDetail: {ex.Message}", ex);
            }
            catch (TaskCanceledException)
            {
                throw new Exception($"❌ Timeout: ObatAPI tidak merespons dalam 30 detik");
            }
            catch (Exception ex)
            {
                throw new Exception($"❌ Error GetAllObat: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mengambil data obat berdasarkan ID dengan parameter validation
        /// </summary>
        public async Task<Obat> GetObatByIdAsync(int id)
        {
            // Defensive: Parameter validation
            if (id <= 0)
                throw new ArgumentException("ID harus lebih besar dari 0", nameof(id));

            try
            {
                string url = $"{_baseUrl}/api/obat/{id}";
                var response = await _httpClient.GetAsync(url);

                // Defensive: Handle 404 specifically
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new Exception($"❌ Obat dengan ID {id} tidak ditemukan di API");

                if (!response.IsSuccessStatusCode)
                    throw new ApiException($"GET {id} failed", response.StatusCode);

                string json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                return ParseObatFromJson(json);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"❌ Connection Error saat fetch ID {id}: {ex.Message}", ex);
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                throw new Exception($"❌ Error GetObatById({id}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Menambahkan obat baru dengan model validation
        /// </summary>
        public async Task<Obat> AddObatAsync(Obat obat)
        {
            // Defensive: Null & model validation
            if (obat == null)
                throw new ArgumentNullException(nameof(obat), "Obat tidak boleh null");

            ValidateObatModel(obat);

            try
            {
                string url = $"{_baseUrl}/api/obat";
                string json = SerializeObatToJson(obat);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    throw new ApiException($"POST add obat failed", response.StatusCode);

                string responseJson = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseJson))
                    return null;

                return ParseObatFromJson(responseJson);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"❌ Connection Error saat POST: {ex.Message}", ex);
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                throw new Exception($"❌ Error AddObat: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Memperbarui data obat dengan parameter & model validation
        /// </summary>
        public async Task<Obat> UpdateObatAsync(int id, Obat obat)
        {
            // Defensive: Parameter validation
            if (id <= 0)
                throw new ArgumentException("ID harus lebih besar dari 0", nameof(id));

            // Defensive: Null & model validation
            if (obat == null)
                throw new ArgumentNullException(nameof(obat), "Obat tidak boleh null");

            ValidateObatModel(obat);

            try
            {
                string url = $"{_baseUrl}/api/obat/{id}";
                string json = SerializeObatToJson(obat);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    throw new ApiException($"PUT update ID {id} failed", response.StatusCode);

                string responseJson = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseJson))
                    return null;

                return ParseObatFromJson(responseJson);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"❌ Connection Error saat PUT: {ex.Message}", ex);
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                throw new Exception($"❌ Error UpdateObat({id}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Menghapus data obat dengan parameter validation
        /// </summary>
        public async Task<bool> DeleteObatAsync(int id)
        {
            // Defensive: Parameter validation
            if (id <= 0)
                throw new ArgumentException("ID harus lebih besar dari 0", nameof(id));

            try
            {
                string url = $"{_baseUrl}/api/obat/{id}";
                var response = await _httpClient.DeleteAsync(url);

                return response.IsSuccessStatusCode || 
                       response.StatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"❌ Connection Error saat DELETE: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Parse JSON list response menjadi List{Obat} dengan error recovery
        /// </summary>
        private List<Obat> ParseObatListFromJson(string json)
        {
            var result = new List<Obat>();
            try
            {
                // Defensive: Null/empty check
                if (string.IsNullOrWhiteSpace(json))
                    return result;

                // Extract data array dari ApiResponse format: { data: [...], success: true }
                int dataStart = json.IndexOf("\"data\":");
                if (dataStart == -1) return result;

                int arrayStart = json.IndexOf("[", dataStart);
                int arrayEnd = json.LastIndexOf("]");

                if (arrayStart == -1 || arrayEnd == -1 || arrayStart >= arrayEnd)
                    return result;

                string arrayContent = json.Substring(arrayStart + 1, arrayEnd - arrayStart - 1).Trim();

                // Defensive: Empty array check
                if (arrayContent.Length == 0)
                    return result;

                int currentPos = 0;
                while (currentPos < arrayContent.Length)
                {
                    int objStart = arrayContent.IndexOf("{", currentPos);
                    if (objStart == -1) break;

                    int objEnd = FindMatchingBrace(arrayContent, objStart);
                    if (objEnd == -1) break;

                    string objContent = arrayContent.Substring(objStart, objEnd - objStart + 1);
                    var obat = ParseObatObject(objContent);

                    // Defensive: Only add valid obat
                    if (obat != null)
                        result.Add(obat);

                    currentPos = objEnd + 1;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ParseObatListFromJson: {ex.Message}");
                return result; // Return empty list instead of throwing
            }
        }

        /// <summary>
        /// Parse single JSON obat response dengan error recovery
        /// </summary>
        private Obat ParseObatFromJson(string json)
        {
            try
            {
                // Defensive: Null/empty check
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                // Extract data object dari ApiResponse
                int dataStart = json.IndexOf("\"data\":");
                if (dataStart == -1) return null;

                int objStart = json.IndexOf("{", dataStart);
                int objEnd = FindMatchingBrace(json, objStart);

                if (objStart == -1 || objEnd == -1)
                    return null;

                string objContent = json.Substring(objStart, objEnd - objStart + 1);
                return ParseObatObject(objContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ParseObatFromJson: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Parse individual obat JSON object dengan defensive parsing
        /// </summary>
        private Obat ParseObatObject(string jsonObj)
        {
            try
            {
                // Defensive: Null/empty check
                if (string.IsNullOrWhiteSpace(jsonObj))
                    return null;

                int id = ExtractIntValue(jsonObj, "id");
                string nama = ExtractStringValue(jsonObj, "nama");

                // Defensive: Name validation (required field)
                if (string.IsNullOrEmpty(nama))
                    return null;

                int stok = ExtractIntValue(jsonObj, "stok");
                decimal harga = ExtractDecimalValue(jsonObj, "harga");

                // Defensive: Negative value check
                if (stok < 0 || harga < 0)
                {
                    Console.WriteLine($"[WARN] Invalid stok/harga for {nama}");
                }

                DateTime expiredDate = DateTime.Now.AddYears(1); // Safe default
                string dateStr = ExtractStringValue(jsonObj, "expiredDate");
                if (!string.IsNullOrEmpty(dateStr))
                {
                    // Defensive: Parse with TryParse
                    if (DateTime.TryParse(dateStr, out DateTime parsed))
                        expiredDate = parsed;
                }

                string kategori = ExtractStringValue(jsonObj, "kategori");
                if (string.IsNullOrEmpty(kategori))
                    kategori = "Tablet"; // Safe default

                // Defensive: Status parsing with fallback
                string statusStr = ExtractStringValue(jsonObj, "status");
                StatusObat status = StatusObat.Available;
                if (!string.IsNullOrEmpty(statusStr))
                {
                    status = ObatStateMachine.GetStatusEnum(statusStr);
                }

                var obat = new Obat(nama, stok, harga, expiredDate, kategori, id)
                {
                    Status = status
                };

                return obat;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ParseObatObject: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Serialize Obat ke JSON string dengan defensive escaping
        /// </summary>
        private string SerializeObatToJson(Obat obat)
        {
            try
            {
                // Defensive: Null check
                if (obat == null)
                    throw new ArgumentNullException(nameof(obat));

                var sb = new StringBuilder();
                sb.Append("{");
                sb.Append($"\"nama\":\"{EscapeJson(obat.Nama ?? "")}\",");
                sb.Append($"\"kategori\":\"{EscapeJson(obat.Kategori ?? "Tablet")}\",");
                sb.Append($"\"stok\":{Math.Max(0, obat.Stok)},"); // Ensure non-negative
                sb.Append($"\"harga\":{Math.Max(0, obat.Harga)},"); // Ensure non-negative
                sb.Append($"\"expiredDate\":\"{obat.ExpiredDate:yyyy-MM-ddTHH:mm:ss}\"");
                sb.Append("}");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SerializeObatToJson: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extract string value dari JSON dengan defensive parsing
        /// </summary>
        private string ExtractStringValue(string json, string key)
        {
            try
            {
                // Defensive: Input validation
                if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(key))
                    return string.Empty;

                string searchKey = $"\"{key}\":";
                int keyPos = json.IndexOf(searchKey, StringComparison.OrdinalIgnoreCase);
                if (keyPos == -1) return string.Empty;

                int quoteStart = json.IndexOf("\"", keyPos + searchKey.Length);
                if (quoteStart == -1) return string.Empty;

                int quoteEnd = json.IndexOf("\"", quoteStart + 1);
                if (quoteEnd == -1) return string.Empty;

                // Defensive: Bounds check
                if (quoteStart + 1 > quoteEnd)
                    return string.Empty;

                return UnescapeJson(json.Substring(quoteStart + 1, quoteEnd - quoteStart - 1));
            }
            catch
            {
                return string.Empty; // Return empty string on any error
            }
        }

        /// <summary>
        /// Extract int value dari JSON dengan defensive parsing
        /// </summary>
        private int ExtractIntValue(string json, string key)
        {
            try
            {
                // Defensive: Input validation
                if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(key))
                    return 0;

                string searchKey = $"\"{key}\":";
                int keyPos = json.IndexOf(searchKey, StringComparison.OrdinalIgnoreCase);
                if (keyPos == -1) return 0;

                int numStart = keyPos + searchKey.Length;

                // Skip whitespace
                while (numStart < json.Length && (json[numStart] == ' ' || json[numStart] == ':' || json[numStart] == '\t'))
                    numStart++;

                int numEnd = numStart;

                // Collect digits
                while (numEnd < json.Length && char.IsDigit(json[numEnd]))
                    numEnd++;

                // Defensive: Range check
                if (numEnd > numStart && numEnd <= json.Length)
                {
                    if (int.TryParse(json.Substring(numStart, numEnd - numStart), out int result))
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
        /// Extract decimal value dari JSON dengan defensive parsing
        /// </summary>
        private decimal ExtractDecimalValue(string json, string key)
        {
            try
            {
                // Defensive: Input validation
                if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(key))
                    return 0;

                string searchKey = $"\"{key}\":";
                int keyPos = json.IndexOf(searchKey, StringComparison.OrdinalIgnoreCase);
                if (keyPos == -1) return 0;

                int numStart = keyPos + searchKey.Length;

                // Skip whitespace
                while (numStart < json.Length && (json[numStart] == ' ' || json[numStart] == ':' || json[numStart] == '\t'))
                    numStart++;

                int numEnd = numStart;

                // Collect digits and decimal point
                while (numEnd < json.Length && (char.IsDigit(json[numEnd]) || json[numEnd] == '.'))
                    numEnd++;

                // Defensive: Range check
                if (numEnd > numStart && numEnd <= json.Length)
                {
                    if (decimal.TryParse(json.Substring(numStart, numEnd - numStart), out decimal result))
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
        /// Find matching closing brace dengan defensive bounds checking
        /// </summary>
        private int FindMatchingBrace(string json, int openPos)
        {
            try
            {
                // Defensive: Bounds check
                if (openPos < 0 || openPos >= json.Length)
                    return -1;

                int count = 1;
                for (int i = openPos + 1; i < json.Length; i++)
                {
                    if (json[i] == '{') 
                        count++;
                    else if (json[i] == '}') 
                        count--;

                    if (count == 0) 
                        return i;
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Escape JSON string dengan defensive null handling
        /// </summary>
        private string EscapeJson(string str)
        {
            // Defensive: Null check
            if (string.IsNullOrEmpty(str)) 
                return string.Empty;

            return str.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        /// <summary>
        /// Unescape JSON string dengan defensive null handling
        /// </summary>
        private string UnescapeJson(string str)
        {
            // Defensive: Null check
            if (string.IsNullOrEmpty(str)) 
                return string.Empty;

            return str.Replace("\\\"", "\"").Replace("\\\\", "\\");
        }

        /// <summary>
        /// Custom exception untuk API errors
        /// </summary>
        private class ApiException : Exception
        {
            public ApiException(string operation, System.Net.HttpStatusCode statusCode)
                : base($"❌ API Error ({(int)statusCode}) saat {operation}") { }
        }

        /// <summary>
        /// Cleanup HttpClient resources
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        /// <summary>
        /// Validate Obat model sebelum dikirim ke API
        /// </summary>
        private void ValidateObatModel(Obat obat)
        {
            // Defensive: Comprehensive validation
            if (string.IsNullOrWhiteSpace(obat.Nama))
                throw new ArgumentException("Nama obat tidak boleh kosong");

            if (obat.Stok < 0)
                throw new ArgumentException("Stok obat tidak boleh negatif");

            if (obat.Harga < 0)
                throw new ArgumentException("Harga obat tidak boleh negatif");

            if (obat.ExpiredDate == default(DateTime))
                throw new ArgumentException("Tanggal kadaluarsa harus diisi");
        }
    }
}
