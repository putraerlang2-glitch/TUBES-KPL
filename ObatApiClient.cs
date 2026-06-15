using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TubesKPL
{
    public class ObatApiClient : IDisposable
    {
        private const string DefaultBaseUrl = "https://localhost:7245";
        private const int DefaultTimeoutSeconds = 10;

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ObatApiClient(string baseUrl = DefaultBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be empty", nameof(baseUrl));

            _baseUrl = baseUrl;
            var handler = new HttpClientHandler
            {
                // ⚠️ HANYA UNTUK DEVELOPMENT! Hapus baris ini untuk production!
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                    errors == System.Net.Security.SslPolicyErrors.None || _baseUrl.Contains("localhost") || _baseUrl.Contains("127.0.0.1")
            };

            _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(DefaultTimeoutSeconds) };
        }

        public async Task<List<Obat>> GetAllObatAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/obat?pageSize=1000");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Obat>>>(json);
                return apiResponse?.Data ?? new List<Obat>();
            }
            catch (TaskCanceledException) { throw new Exception("API connection timeout"); }
            catch (Exception ex) { throw new Exception($"Failed to fetch medicines: {ex.Message}", ex); }
        }

        public async Task<Obat> GetObatByIdAsync(int obatId)
        {
            try
            {
                if (obatId <= 0) throw new ArgumentException("ID must be positive");

                var response = await _httpClient.GetAsync($"{_baseUrl}/api/obat/{obatId}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) throw new Exception("Medicine not found");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Obat>(json);
            }
            catch (Exception ex) { throw new Exception($"Failed to fetch medicine: {ex.Message}", ex); }
        }

        public async Task<Obat> AddObatAsync(Obat obat)
        {
            try
            {
                if (obat == null) throw new ArgumentNullException(nameof(obat));
                var json = JsonConvert.SerializeObject(obat);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/obat", content);
                response.EnsureSuccessStatusCode();

                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Obat>(resultJson);
            }
            catch (Exception ex) { throw new Exception($"Failed to add medicine: {ex.Message}", ex); }
        }

        public async Task<Obat> UpdateObatAsync(int obatId, Obat obat)
        {
            try
            {
                if (obatId <= 0 || obat == null) throw new ArgumentException("Invalid parameters");
                var json = JsonConvert.SerializeObject(obat);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/api/obat/{obatId}", content);
                response.EnsureSuccessStatusCode();

                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Obat>(resultJson);
            }
            catch (Exception ex) { throw new Exception($"Failed to update medicine: {ex.Message}", ex); }
        }

        public async Task<bool> DeleteObatAsync(int obatId)
        {
            try
            {
                if (obatId <= 0) throw new ArgumentException("ID must be positive");
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/obat/{obatId}");
                return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (Exception ex) { throw new Exception($"Failed to delete medicine: {ex.Message}", ex); }
        }

        public async Task<bool> CheckoutTransaksiAsync(TransaksiDTO transaksi)
        {
            try
            {
                if (transaksi?.DetailList?.Count == 0) throw new ArgumentException("Transaction data required");
                var json = JsonConvert.SerializeObject(transaksi);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/transaksi", content);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex) { throw new Exception($"Failed to process transaction: {ex.Message}", ex); }
        }

        public async Task<string> GetStatusRulesAsync()
        {
            try { var response = await _httpClient.GetAsync($"{_baseUrl}/api/obat/status/rules"); return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null; }
            catch { return null; }
        }

        public async Task<string> GetStatusSummaryAsync()
        {
            try { var response = await _httpClient.GetAsync($"{_baseUrl}/api/obat/status/summary"); return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null; }
            catch { return null; }
        }

        public async Task<UserDTO> LoginAsync(string username, string password)
        {
            try
            {
                var request = new { Username = username, Password = password };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/user/login", content);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new Exception("Invalid username or password");
                
                response.EnsureSuccessStatusCode();
                
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserDTO>(resultJson);
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}", ex);
            }
        }

        public async Task<UserDTO> RegisterAsync(string username, string password, string nama, string? role = null)
        {
            try
            {
                var request = new { Username = username, Password = password, Nama = nama, Role = role };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/user/register", content);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    throw new Exception("Username already exists");
                
                response.EnsureSuccessStatusCode();
                
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserDTO>(resultJson);
            }
            catch (Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}", ex);
            }
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/user");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<UserDTO>>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch users: {ex.Message}", ex);
            }
        }

        public void Dispose() => _httpClient?.Dispose();
    }

    public class UserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class TransaksiDTO
    {
        public string NoStruk { get; set; } = string.Empty;
        public DateTime TanggalTransaksi { get; set; }
        public decimal Subtotal { get; set; }
        public decimal PersentaseDiskon { get; set; }
        public decimal NominalDiskon { get; set; }
        public decimal PersentasePajak { get; set; }
        public decimal NominalPajak { get; set; }
        public decimal TotalAkhir { get; set; }
        public decimal UangBayar { get; set; }
        public decimal UangKembalian { get; set; }
        public int UserId { get; set; }
        public List<TransaksiDetailDTO> DetailList { get; set; } = new List<TransaksiDetailDTO>();
    }

    public class TransaksiDetailDTO
    {
        public int ObatId { get; set; }
        public int Jumlah { get; set; }
        public decimal HargaSatuan { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public object Pagination { get; set; }
    }
}
