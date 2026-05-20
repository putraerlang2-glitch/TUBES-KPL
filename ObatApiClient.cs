using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TubesKPL
{
    /// <summary>
    /// HTTP client untuk komunikasi dengan ObatAPI.
    /// Menggunakan Newtonsoft.Json untuk serialization/deserialization yang robust.
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

                // Deserialize ApiResponse wrapper menggunakan Newtonsoft.Json
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Obat>>>(json);
                return apiResponse?.Data ?? new List<Obat>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"❌ Connection Error: API tidak dapat diakses di {_baseUrl}\nDetail: {ex.Message}", ex);
            }
            catch (TaskCanceledException)
            {
                throw new Exception($"❌ Timeout: ObatAPI tidak merespons dalam 30 detik");
            }
            catch (JsonException ex)
            {
                throw new Exception($"❌ Error parsing JSON response: {ex.Message}", ex);
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

                // Deserialize ApiResponse wrapper menggunakan Newtonsoft.Json
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<Obat>>(json);
                return apiResponse?.Data;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"❌ Connection Error saat fetch ID {id}: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"❌ Error parsing JSON response: {ex.Message}", ex);
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
                // Serialize menggunakan Newtonsoft.Json
                string json = JsonConvert.SerializeObject(obat, Formatting.None, 
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    throw new ApiException($"POST add obat failed", response.StatusCode);

                string responseJson = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseJson))
                    return null;

                // Deserialize ApiResponse wrapper menggunakan Newtonsoft.Json
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<Obat>>(responseJson);
                return apiResponse?.Data;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"❌ Connection Error saat POST: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"❌ Error serializing/parsing JSON: {ex.Message}", ex);
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
                // Serialize menggunakan Newtonsoft.Json
                string json = JsonConvert.SerializeObject(obat, Formatting.None,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    throw new ApiException($"PUT update ID {id} failed", response.StatusCode);

                string responseJson = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseJson))
                    return null;

                // Deserialize ApiResponse wrapper menggunakan Newtonsoft.Json
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<Obat>>(responseJson);
                return apiResponse?.Data;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"❌ Connection Error saat PUT: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"❌ Error serializing/parsing JSON: {ex.Message}", ex);
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

        /// <summary>
        /// Cleanup HttpClient resources
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        /// <summary>
        /// Generic API response wrapper untuk deserialization
        /// </summary>
        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class ApiResponse<T>
        {
            [JsonProperty("data")]
            public T Data { get; set; }

            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }

        /// <summary>
        /// Custom exception untuk API errors
        /// </summary>
        private class ApiException : Exception
        {
            public ApiException(string operation, System.Net.HttpStatusCode statusCode)
                : base($"❌ API Error ({(int)statusCode}) saat {operation}") { }
        }
    }
}
