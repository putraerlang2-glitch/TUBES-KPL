using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static TubesKPL.Obat;

namespace TubesKPL
{
    // [KEEP IT SIMPLE] Minimal JSON parsing tanpa external dependencies
    // [DESIGN PATTERN] Service Locator pattern untuk data persistence
    public static class JsonDataManager
    {
        private static string _defaultFilePath = "data.json";

        public static void SetDataFilePath(string filePath) => _defaultFilePath = filePath;

        // [KEEP IT SIMPLE] Load dari JSON file dengan minimal logic
        public static List<Obat> LoadFromJson()
        {
            try
            {
                if (!File.Exists(_defaultFilePath))
                    return new List<Obat>();

                string json = File.ReadAllText(_defaultFilePath);
                
                // [KEEP IT SIMPLE] Extract obat array dari JSON
                var items = new List<Obat>();
                var matches = Regex.Matches(json, @"\{[^}]*""nama""[^}]*\}", RegexOptions.Singleline);
                
                foreach (Match match in matches)
                {
                    var obat = ParseObatObject(match.Value);
                    if (obat != null)
                        items.Add(obat);
                }
                
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Load JSON: {ex.Message}");
                return new List<Obat>();
            }
        }

        // [KEEP IT SIMPLE] Save ke JSON file
        public static bool SaveToJson(List<Obat> data)
        {
            try
            {
                if (data == null)
                    return false;

                var sb = new StringBuilder();
                sb.AppendLine("{");
                sb.AppendLine("  \"obat\": [");

                for (int i = 0; i < data.Count; i++)
                {
                    var o = data[i];
                    sb.Append("    {");
                    sb.Append($"\"nama\": \"{Escape(o.Nama)}\", ");
                    sb.Append($"\"stok\": {o.Stok}, ");
                    sb.Append($"\"harga\": {o.Harga}, ");
                    sb.Append($"\"expiredDate\": \"{o.ExpiredDate:yyyy-MM-dd}\", ");
                    sb.Append($"\"kategori\": \"{o.Kategori}\"");
                    sb.AppendLine(" }");
                    
                    if (i < data.Count - 1)
                        sb.Insert(sb.Length - 1, ",");
                }

                sb.AppendLine("  ]");
                sb.AppendLine("}");

                File.WriteAllText(_defaultFilePath, sb.ToString(), Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Save JSON: {ex.Message}");
                return false;
            }
        }

        // [KEEP IT SIMPLE] Parse single JSON object to Obat
        private static Obat ParseObatObject(string json)
        {
            try
            {
                string nama = ExtractField(json, "nama");
                if (string.IsNullOrEmpty(nama))
                    return null;

                int stok = int.Parse(ExtractField(json, "stok"));
                decimal harga = decimal.Parse(ExtractField(json, "harga"));
                var expDate = DateTime.Parse(ExtractField(json, "expiredDate"));
                var kategori = (KategoriObat)Enum.Parse(typeof(KategoriObat), ExtractField(json, "kategori"));

                return new Obat(nama, stok, harga, expDate, kategori);
            }
            catch { return null; }
        }

        // [KEEP IT SIMPLE] Extract field value dari JSON
        private static string ExtractField(string json, string fieldName)
        {
            var pattern = $@"""{fieldName}""\s*:\s*""([^""]*)""";
            var match = Regex.Match(json, pattern);
            if (match.Success)
                return match.Groups[1].Value;

            // Try numeric value (for stok, harga)
            pattern = $@"""{fieldName}""\s*:\s*([0-9.]+)";
            match = Regex.Match(json, pattern);
            return match.Success ? match.Groups[1].Value : "";
        }

        // [CLEAN CODE] Escape JSON special characters
        private static string Escape(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            return text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
        }
    }
}
