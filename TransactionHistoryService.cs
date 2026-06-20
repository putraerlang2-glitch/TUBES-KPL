using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TubesKPL
{
    public static class TransactionHistoryService
    {
        private static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TubesKPL");
        private static readonly string HistoryFile = Path.Combine(AppFolder, "transaction_history.json");

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
    }
}
