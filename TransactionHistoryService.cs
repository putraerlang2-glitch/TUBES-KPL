
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace TubesKPL
{
    public static class TransactionHistoryService
    {
        private static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TubesKPL");
        private static readonly string HistoryFile = Path.Combine(AppFolder, "transaction_history.json");

        // For backward compatibility: keep old methods that use JSON file
        public static void EnsureStorage()
        {
            if (!Directory.Exists(AppFolder)) Directory.CreateDirectory(AppFolder);
            if (!File.Exists(HistoryFile)) File.WriteAllText(HistoryFile, "[]");
        }

        public static void AppendTransaction(TransaksiDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            EnsureStorage();
            var list = GetAllTransactions();
            list.Add(dto);
            File.WriteAllText(HistoryFile, JsonConvert.SerializeObject(list, Formatting.Indented));
        }

        public static List<TransaksiDTO> GetAllTransactions()
        {
            try
            {
                EnsureStorage();
                var json = File.ReadAllText(HistoryFile);
                var list = JsonConvert.DeserializeObject<List<TransaksiDTO>>(json);
                return list ?? new List<TransaksiDTO>();
            }
            catch
            {
                return new List<TransaksiDTO>();
            }
        }

        // New methods that use direct MySQL connection for FormHistory
        public static DataTable GetAllHistory()
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    const string query = "SELECT * FROM vw_history_transaksi";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                ActivityHistoryService.LogActivity("ERROR", $"Gagal memuat history: {ex.Message}");
                throw new Exception("Terjadi kesalahan saat memuat riwayat transaksi.");
            }

            return dt;
        }

        public static DataTable SearchHistory(string noStruk = null, string namaObat = null, string namaKasir = null, DateTime? tanggalFrom = null, DateTime? tanggalTo = null)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT * FROM vw_history_transaksi WHERE 1=1";
                    
                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;

                        if (!string.IsNullOrWhiteSpace(noStruk))
                        {
                            query += " AND no_struk LIKE @noStruk";
                            cmd.Parameters.AddWithValue("@noStruk", "%" + noStruk + "%");
                        }

                        if (!string.IsNullOrWhiteSpace(namaObat))
                        {
                            query += " AND nama_obat LIKE @namaObat";
                            cmd.Parameters.AddWithValue("@namaObat", "%" + namaObat + "%");
                        }

                        if (!string.IsNullOrWhiteSpace(namaKasir))
                        {
                            query += " AND nama_kasir LIKE @namaKasir";
                            cmd.Parameters.AddWithValue("@namaKasir", "%" + namaKasir + "%");
                        }

                        if (tanggalFrom.HasValue)
                        {
                            query += " AND tanggal_transaksi >= @tanggalFrom";
                            cmd.Parameters.AddWithValue("@tanggalFrom", tanggalFrom.Value.Date);
                        }

                        if (tanggalTo.HasValue)
                        {
                            query += " AND tanggal_transaksi <= @tanggalTo";
                            cmd.Parameters.AddWithValue("@tanggalTo", tanggalTo.Value.Date.AddDays(1).AddTicks(-1));
                        }

                        cmd.CommandText = query;

                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ActivityHistoryService.LogActivity("ERROR", $"Gagal mencari history: {ex.Message}");
                throw new Exception("Terjadi kesalahan saat mencari riwayat transaksi.");
            }

            return dt;
        }
    }
}
