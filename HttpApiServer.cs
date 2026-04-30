using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace TubesKPL
{
    // [AYONDI] HttpListener API Server - Optional ringan tanpa ASP.NET Core
    // File ini menyediakan HTTP API server menggunakan HttpListener
    // Compatible dengan .NET Framework 4.7.2
    // Endpoint tersedia:
    //   GET /obat - Mengambil semua data obat
    //   GET /obat/status - Mengambil ringkasan status
    //   GET /obat/search?q=nama - Mencari obat berdasarkan nama
    //   GET /obat/stats - Mengambil statistik lengkap

    /// <summary>
    /// [AYONDI] Class HttpApiServer
    /// Simple HTTP listener server untuk menyediakan API endpoint
    /// Menggunakan HttpListener dari System.Net (built-in .NET Framework)
    /// </summary>
    public class HttpApiServer
    {
        // [AYONDI] HttpListener object untuk menangani HTTP requests
        private HttpListener _httpListener;

        // [AYONDI] Thread untuk menjalankan server di background
        private Thread _listenerThread;

        // [AYONDI] Flag untuk mengontrol status server (running/stopped)
        private bool _isRunning;

        // [AYONDI] Port tempat server akan listen
        private int _port;

        // [AYONDI] Prefix URL yang akan di-listen
        private string _prefix;

        // [AYONDI] Constructor
        /// <summary>
        /// [AYONDI] Inisialisasi HttpApiServer dengan port tertentu
        /// Parameter: int port - port untuk listen (default: 8080)
        /// </summary>
        public HttpApiServer(int port = 8080)
        {
            _port = port;
            _prefix = "http://localhost:" + _port + "/";
            _httpListener = new HttpListener();
            _isRunning = false;
        }

        // [AYONDI] Method untuk start server
        /// <summary>
        /// [AYONDI] Memulai HTTP server di background thread
        /// Server akan listen pada http://localhost:port/
        /// </summary>
        public void Start()
        {
            if (_isRunning)
            {
                return;  // [AYONDI] Server sudah running
            }

            try
            {
                // [AYONDI] Add prefix untuk HttpListener
                _httpListener.Prefixes.Add(_prefix);

                // [AYONDI] Start HttpListener
                _httpListener.Start();

                // [AYONDI] Set flag running
                _isRunning = true;

                // [AYONDI] Create dan start listener thread
                _listenerThread = new Thread(ListenForRequests);
                _listenerThread.IsBackground = true;
                _listenerThread.Start();

                Console.WriteLine("[AYONDI] HTTP API Server started on " + _prefix);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AYONDI] Error starting HTTP server: " + ex.Message);
                _isRunning = false;
            }
        }

        // [AYONDI] Method untuk stop server
        /// <summary>
        /// [AYONDI] Menghentikan HTTP server
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
            {
                return;  // [AYONDI] Server sudah stopped
            }

            try
            {
                // [AYONDI] Set flag stopped
                _isRunning = false;

                // [AYONDI] Stop HttpListener
                _httpListener.Stop();
                _httpListener.Close();

                Console.WriteLine("[AYONDI] HTTP API Server stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AYONDI] Error stopping HTTP server: " + ex.Message);
            }
        }

        // [AYONDI] Method private: ListenForRequests() - Background thread untuk handle requests
        /// <summary>
        /// [AYONDI] Loop untuk menangani incoming HTTP requests
        /// Dijalankan di background thread
        /// </summary>
        private void ListenForRequests()
        {
            while (_isRunning)
            {
                try
                {
                    // [AYONDI] Wait untuk incoming request
                    HttpListenerContext context = _httpListener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    // [AYONDI] Get request path
                    string path = request.Url.AbsolutePath.ToLower();
                    string query = request.Url.Query;

                    // [AYONDI] Route handling: switch-case untuk different endpoints
                    if (path == "/obat" || path == "/obat/")
                    {
                        HandleGetObat(response);
                    }
                    else if (path == "/obat/status")
                    {
                        HandleGetStatus(response);
                    }
                    else if (path == "/obat/search")
                    {
                        HandleSearch(response, query);
                    }
                    else if (path == "/obat/stats")
                    {
                        HandleGetStats(response);
                    }
                    else
                    {
                        Handle404(response);
                    }

                    // [AYONDI] Close response
                    response.Close();
                }
                catch (HttpListenerException)
                {
                    // [AYONDI] Ignore HttpListenerException ketika server di-stop
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[AYONDI] Error handling request: " + ex.Message);
                }
            }
        }

        // [AYONDI] Handler: GET /obat
        /// <summary>
        /// [AYONDI] Handle GET /obat - Mengambil semua data obat
        /// Return JSON berisi array obat
        /// </summary>
        private void HandleGetObat(HttpListenerResponse response)
        {
            // [AYONDI] Call API service untuk get semua obat
            var apiResponse = ObatApiService.GetAll();

            // [AYONDI] Convert ke JSON dan kirim response
            string json = ConvertObatListToJson(apiResponse);
            SendJsonResponse(response, json);
        }

        // [AYONDI] Handler: GET /obat/status
        /// <summary>
        /// [AYONDI] Handle GET /obat/status - Mengambil ringkasan status
        /// Return JSON berisi jumlah obat per status
        /// </summary>
        private void HandleGetStatus(HttpListenerResponse response)
        {
            // [AYONDI] Call API service untuk get status summary
            var apiResponse = ObatApiService.GetStatusSummary();

            // [AYONDI] Convert ke JSON dan kirim response
            string json = ConvertStatusSummaryToJson(apiResponse);
            SendJsonResponse(response, json);
        }

        // [AYONDI] Handler: GET /obat/search?q=nama
        /// <summary>
        /// [AYONDI] Handle GET /obat/search - Mencari obat berdasarkan nama
        /// Parameter query: ?q=nama_obat
        /// Return JSON berisi hasil pencarian
        /// </summary>
        private void HandleSearch(HttpListenerResponse response, string query)
        {
            // [AYONDI] Parse query parameter untuk mendapatkan search term
            string searchTerm = ExtractQueryParameter(query, "q");

            if (string.IsNullOrEmpty(searchTerm))
            {
                SendErrorResponse(response, "Missing 'q' parameter");
                return;
            }

            // [AYONDI] Call API service untuk search
            var apiResponse = ObatApiService.Search(searchTerm);

            // [AYONDI] Convert ke JSON dan kirim response
            string json = ConvertObatListToJson(apiResponse);
            SendJsonResponse(response, json);
        }

        // [AYONDI] Handler: GET /obat/stats
        /// <summary>
        /// [AYONDI] Handle GET /obat/stats - Mengambil statistik lengkap
        /// Return JSON berisi berbagai statistik
        /// </summary>
        private void HandleGetStats(HttpListenerResponse response)
        {
            // [AYONDI] Call API service untuk get stats
            var stats = ObatApiService.GetStats();

            // [AYONDI] Convert ke JSON dan kirim response
            string json = ConvertStatsToJson(stats);
            SendJsonResponse(response, json);
        }

        // [AYONDI] Handler: 404 Not Found
        /// <summary>
        /// [AYONDI] Handle 404 - Endpoint tidak ditemukan
        /// </summary>
        private void Handle404(HttpListenerResponse response)
        {
            SendErrorResponse(response, "Endpoint not found. Available: /obat, /obat/status, /obat/search?q=..., /obat/stats");
        }

        // [AYONDI] Helper: Convert ObatListResponse ke JSON
        /// <summary>
        /// [AYONDI] Convert data obat ke format JSON sederhana
        /// Kompatibel C# 7.3 (tanpa library JSON)
        /// </summary>
        private string ConvertObatListToJson(ObatListResponse apiResponse)
        {
            // [AYONDI] Manual JSON construction (kompatibel dengan C# 7.3)
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"success\":true,");
            sb.Append("\"count\":" + apiResponse.Count + ",");
            sb.Append("\"message\":\"" + EscapeJson(apiResponse.Message) + "\",");
            sb.Append("\"data\":[");

            // [AYONDI] Loop untuk setiap obat
            for (int i = 0; i < apiResponse.Data.Count; i++)
            {
                var obat = apiResponse.Data[i];
                sb.Append("{");
                sb.Append("\"nama\":\"" + EscapeJson(obat.nama) + "\",");
                sb.Append("\"stok\":" + obat.stok + ",");
                sb.Append("\"harga\":" + obat.harga + ",");
                sb.Append("\"expiredDate\":\"" + obat.expiredDate.ToString("yyyy-MM-dd") + "\",");
                sb.Append("\"status\":\"" + obat.status.ToString() + "\",");
                sb.Append("\"kategori\":\"" + obat.kategori.ToString() + "\"");
                sb.Append("}");

                // [AYONDI] Add comma jika bukan item terakhir
                if (i < apiResponse.Data.Count - 1)
                {
                    sb.Append(",");
                }
            }

            sb.Append("],");
            sb.Append("\"responseTime\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"");
            sb.Append("}");

            return sb.ToString();
        }

        // [AYONDI] Helper: Convert StatusSummaryResponse ke JSON
        /// <summary>
        /// [AYONDI] Convert status summary ke format JSON sederhana
        /// </summary>
        private string ConvertStatusSummaryToJson(StatusSummaryResponse statusResponse)
        {
            // [AYONDI] Manual JSON construction
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"success\":true,");
            sb.Append("\"totalAvailable\":" + statusResponse.TotalAvailable + ",");
            sb.Append("\"totalLowStock\":" + statusResponse.TotalLowStock + ",");
            sb.Append("\"totalExpired\":" + statusResponse.TotalExpired + ",");
            sb.Append("\"totalObat\":" + statusResponse.TotalObat + ",");
            sb.Append("\"responseTime\":\"" + statusResponse.ResponseTime.ToString("yyyy-MM-dd HH:mm:ss") + "\"");
            sb.Append("}");

            return sb.ToString();
        }

        // [AYONDI] Helper: Convert Dictionary stats ke JSON
        /// <summary>
        /// [AYONDI] Convert statistics dictionary ke format JSON sederhana
        /// </summary>
        private string ConvertStatsToJson(Dictionary<string, object> stats)
        {
            // [AYONDI] Manual JSON construction
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"success\":true,");

            int index = 0;
            foreach (var key in stats.Keys)
            {
                var value = stats[key];

                // [AYONDI] Handle different value types
                if (value is string)
                {
                    sb.Append("\"" + key + "\":\"" + EscapeJson(value.ToString()) + "\"");
                }
                else if (value is DateTime)
                {
                    sb.Append("\"" + key + "\":\"" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "\"");
                }
                else if (value is decimal || value is double)
                {
                    sb.Append("\"" + key + "\":" + value.ToString());
                }
                else if (value is int || value is long)
                {
                    sb.Append("\"" + key + "\":" + value.ToString());
                }
                else
                {
                    sb.Append("\"" + key + "\":\"" + value.ToString() + "\"");
                }

                // [AYONDI] Add comma jika bukan item terakhir
                if (index < stats.Keys.Count - 1)
                {
                    sb.Append(",");
                }
                index++;
            }

            sb.Append("}");

            return sb.ToString();
        }

        // [AYONDI] Helper: Kirim JSON response
        /// <summary>
        /// [AYONDI] Send JSON response dengan content-type application/json
        /// </summary>
        private void SendJsonResponse(HttpListenerResponse response, string json)
        {
            // [AYONDI] Set status dan content-type
            response.StatusCode = 200;
            response.ContentType = "application/json; charset=utf-8";

            // [AYONDI] Convert string ke bytes
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            // [AYONDI] Set content length dan write response
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        // [AYONDI] Helper: Kirim error response
        /// <summary>
        /// [AYONDI] Send error response JSON
        /// </summary>
        private void SendErrorResponse(HttpListenerResponse response, string message)
        {
            // [AYONDI] Manual JSON construction untuk error response
            string json = "{\"success\":false,\"error\":\"" + EscapeJson(message) + "\"}";

            // [AYONDI] Set status 400 Bad Request
            response.StatusCode = 400;
            response.ContentType = "application/json; charset=utf-8";

            // [AYONDI] Write response
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        // [AYONDI] Helper: Extract query parameter dari query string
        /// <summary>
        /// [AYONDI] Parse query parameter dari URL query string
        /// Contoh: ?q=paracetamol -> return "paracetamol"
        /// </summary>
        private string ExtractQueryParameter(string query, string paramName)
        {
            // [AYONDI] Remove leading ? jika ada
            if (query.StartsWith("?"))
            {
                query = query.Substring(1);
            }

            // [AYONDI] Split by & untuk multiple parameters
            string[] parameters = query.Split('&');

            // [AYONDI] Loop untuk find matching parameter
            foreach (string param in parameters)
            {
                if (param.StartsWith(paramName + "="))
                {
                    // [AYONDI] Extract value setelah =
                    string value = param.Substring((paramName + "=").Length);
                    // [AYONDI] Decode URL encoding jika diperlukan
                    value = System.Net.WebUtility.UrlDecode(value);
                    return value;
                }
            }

            return null;
        }

        // [AYONDI] Helper: Escape special characters untuk JSON
        /// <summary>
        /// [AYONDI] Escape string agar valid di JSON
        /// Replace quote, newline, carriage return, dll
        /// </summary>
        private string EscapeJson(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            // [AYONDI] Replace special characters
            text = text.Replace("\\", "\\\\");  // [AYONDI] Escape backslash
            text = text.Replace("\"", "\\\""); // [AYONDI] Escape quote
            text = text.Replace("\n", "\\n");  // [AYONDI] Escape newline
            text = text.Replace("\r", "\\r");  // [AYONDI] Escape carriage return
            text = text.Replace("\t", "\\t");  // [AYONDI] Escape tab

            return text;
        }

        // [AYONDI] Property untuk cek apakah server running
        /// <summary>
        /// [AYONDI] Get status server (running atau tidak)
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
        }

        // [AYONDI] Property untuk get port
        /// <summary>
        /// [AYONDI] Get port tempat server listen
        /// </summary>
        public int Port
        {
            get { return _port; }
        }

        // [AYONDI] Property untuk get prefix URL
        /// <summary>
        /// [AYONDI] Get URL prefix untuk server
        /// </summary>
        public string Prefix
        {
            get { return _prefix; }
        }
    }
}
