using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TubesKPL
{
    /// <summary>HTTP client for ObatAPI backend service</summary>
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

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = ValidateCertificate;

            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromSeconds(DefaultTimeoutSeconds);
        }

        private bool ValidateCertificate(HttpRequestMessage message, System.Security.Cryptography.X509Certificates.X509Certificate2 cert, 
            System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            if (errors == System.Net.Security.SslPolicyErrors.None)
                return true;

            // Accept self-signed certs only for localhost development
            return _baseUrl.Contains("localhost") || _baseUrl.Contains("127.0.0.1");
        }

        public async Task<List<Obat>> GetAllObatAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/obat");
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"HTTP {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();
                return ParseObatListFromJson(json) ?? new List<Obat>();
            }
            catch (TaskCanceledException)
            {
                throw new Exception("API connection timeout");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch medicines: {ex.Message}", ex);
            }
        }

        public async Task<Obat> GetObatByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("ID must be positive");

                var response = await _httpClient.GetAsync($"{_baseUrl}/api/obat/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new Exception($"Medicine not found");

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"HTTP {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();
                return ParseObatFromJson(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch medicine: {ex.Message}", ex);
            }
        }

        public async Task<Obat> AddObatAsync(Obat obat)
        {
            try
            {
                if (obat == null)
                    throw new ArgumentNullException(nameof(obat));

                var json = SerializeObat(obat);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/obat", content);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"HTTP {response.StatusCode}");

                var responseJson = await response.Content.ReadAsStringAsync();
                return ParseObatFromJson(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add medicine: {ex.Message}", ex);
            }
        }

        public async Task<Obat> UpdateObatAsync(int id, Obat obat)
        {
            try
            {
                if (id <= 0 || obat == null)
                    throw new ArgumentException("Invalid parameters");

                var json = SerializeObat(obat);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/api/obat/{id}", content);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"HTTP {response.StatusCode}");

                var responseJson = await response.Content.ReadAsStringAsync();
                return ParseObatFromJson(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update medicine: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteObatAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("ID must be positive");

                var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/obat/{id}");
                return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete medicine: {ex.Message}", ex);
            }
        }

        public async Task<bool> CheckoutTransaksiAsync(TransaksiDTO transaksi)
        {
            try
            {
                if (transaksi?.DetailList?.Count == 0)
                    throw new ArgumentException("Transaction data required");

                var json = SerializeTransaksi(transaksi);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/transaksi", content);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"HTTP {response.StatusCode}");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to process transaction: {ex.Message}", ex);
            }
        }

        public async Task<string> GetStatusRulesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/obat/status/rules");
                return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
            }
            catch { return null; }
        }

        public async Task<string> GetStatusSummaryAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/obat/status/summary");
                return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
            }
            catch { return null; }
        }

        private List<Obat> ParseObatListFromJson(string json)
        {
            var result = new List<Obat>();
            if (string.IsNullOrEmpty(json)) return result;

            try
            {
                json = json.Replace("\n", "").Replace("\r", "").Replace("\t", "");
                int start = json.IndexOf("[");
                int end = json.LastIndexOf("]");

                if (start == -1 || end == -1 || start >= end)
                    return result;

                string content = json.Substring(start + 1, end - start - 1);
                int pos = 0;

                while (pos < content.Length)
                {
                    int objStart = content.IndexOf("{", pos);
                    if (objStart == -1) break;

                    int objEnd = FindClosingBracket(content, objStart);
                    if (objEnd == -1) break;

                    var obat = ParseObatObject(content.Substring(objStart + 1, objEnd - objStart - 1));
                    if (obat != null) result.Add(obat);

                    pos = objEnd + 1;
                }
            }
            catch { }

            return result;
        }

        private Obat ParseObatFromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;

            try
            {
                json = json.Replace("\n", "").Replace("\r", "").Replace("\t", "");
                int start = json.IndexOf("{");
                int end = FindClosingBracket(json, start);

                if (start == -1 || end == -1) return null;

                return ParseObatObject(json.Substring(start + 1, end - start - 1));
            }
            catch { return null; }
        }

        private Obat ParseObatObject(string content)
        {
            try
            {
                string nama = GetJsonStringValue(content, "nama");
                if (string.IsNullOrEmpty(nama)) return null;

                int id = GetJsonIntValue(content, "id");
                int stok = GetJsonIntValue(content, "stok");
                decimal harga = GetJsonDecimalValue(content, "harga");
                string kategori = GetJsonStringValue(content, "kategori") ?? "Tablet";
                string status = GetJsonStringValue(content, "status") ?? "Available";
                string dateStr = GetJsonStringValue(content, "expiredDate");

                DateTime expiredDate = DateTime.Now.AddYears(1);
                if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out var parsed))
                    expiredDate = parsed;

                return new Obat(nama, stok, harga, expiredDate, kategori, id) { Status = status };
            }
            catch { return null; }
        }

        private string SerializeObat(Obat obat)
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "{{\"nama\":\"{0}\",\"kategori\":\"{1}\",\"stok\":{2},\"harga\":{3},\"expiredDate\":\"{4:yyyy-MM-ddTHH:mm:ss}\"}}",
                EscapeJson(obat.Nama), EscapeJson(obat.Kategori), obat.Stok, obat.Harga, obat.ExpiredDate);
        }

        private string SerializeTransaksi(TransaksiDTO transaksi)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("{\"noStruk\":\"").Append(EscapeJson(transaksi.NoStruk)).Append("\",");
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "\"tanggalTransaksi\":\"{0:yyyy-MM-ddTHH:mm:ss}\",\"subtotal\":{1},",
                transaksi.TanggalTransaksi, transaksi.Subtotal);
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "\"persentaseDiskon\":{0},\"nominalDiskon\":{1},\"persentasePajak\":{2},\"nominalPajak\":{3},\"totalAkhir\":{4},",
                transaksi.PersentaseDiskon, transaksi.NominalDiskon, transaksi.PersentasePajak, transaksi.NominalPajak, transaksi.TotalAkhir);
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "\"uangBayar\":{0},\"uangKembalian\":{1},\"detailList\":[", transaksi.UangBayar, transaksi.UangKembalian);

            for (int i = 0; i < transaksi.DetailList.Count; i++)
            {
                var detail = transaksi.DetailList[i];
                if (i > 0) sb.Append(",");
                sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                    "{{\"obatId\":{0},\"jumlah\":{1},\"hargaSatuan\":{2},\"subtotal\":{3}}}",
                    detail.ObatId, detail.Jumlah, detail.HargaSatuan, detail.Subtotal);
            }

            sb.Append("]}");
            return sb.ToString();
        }

        private int FindClosingBracket(string json, int openPos)
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

        private string GetJsonStringValue(string json, string key)
        {
            try
            {
                int pos = json.IndexOf("\"" + key + "\"", StringComparison.OrdinalIgnoreCase);
                if (pos == -1) return string.Empty;

                int colonPos = json.IndexOf(":", pos);
                if (colonPos == -1) return string.Empty;

                int quoteStart = json.IndexOf("\"", colonPos);
                if (quoteStart == -1) return string.Empty;

                int quoteEnd = json.IndexOf("\"", quoteStart + 1);
                if (quoteEnd == -1) return string.Empty;

                return json.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
            }
            catch { return string.Empty; }
        }

        private int GetJsonIntValue(string json, string key)
        {
            try
            {
                int pos = json.IndexOf("\"" + key + "\"");
                if (pos == -1) return 0;

                int colonPos = json.IndexOf(":", pos);
                if (colonPos == -1) return 0;

                int start = colonPos + 1;
                while (start < json.Length && (json[start] == ' ' || json[start] == ':')) start++;

                int end = start;
                while (end < json.Length && char.IsDigit(json[end])) end++;

                return end > start && int.TryParse(json.Substring(start, end - start), out int result) ? result : 0;
            }
            catch { return 0; }
        }

        private decimal GetJsonDecimalValue(string json, string key)
        {
            try
            {
                int pos = json.IndexOf("\"" + key + "\"");
                if (pos == -1) return 0;

                int colonPos = json.IndexOf(":", pos);
                if (colonPos == -1) return 0;

                int start = colonPos + 1;
                while (start < json.Length && (json[start] == ' ' || json[start] == ':')) start++;

                int end = start;
                while (end < json.Length && (char.IsDigit(json[end]) || json[end] == '.')) end++;

                return end > start && decimal.TryParse(json.Substring(start, end - start), out decimal result) ? result : 0;
            }
            catch { return 0; }
        }

        private string EscapeJson(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : str.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
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

        public List<TransaksiDetailDTO> DetailList { get; set; } = new List<TransaksiDetailDTO>();
    }

    public class TransaksiDetailDTO
    {
        public int ObatId { get; set; }
        public int Jumlah { get; set; }
        public decimal HargaSatuan { get; set; }
        public decimal Subtotal { get; set; }
    }
}
