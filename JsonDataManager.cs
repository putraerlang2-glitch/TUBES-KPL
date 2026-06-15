using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TubesKPL
{
    public static class JsonDataManager
    {
        private static string _defaultFilePath = "data.json";
        public static void SetDataFilePath(string filePath) => _defaultFilePath = filePath;

        public static List<Obat> LoadFromJson()
        {
            try
            {
                if (!File.Exists(_defaultFilePath)) return new List<Obat>();
                string json = File.ReadAllText(_defaultFilePath);
                var jObject = JObject.Parse(json);
                return jObject["obat"]?.ToObject<List<Obat>>() ?? new List<Obat>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Load JSON: {ex.Message}");
                return new List<Obat>();
            }
        }

        public static bool SaveToJson(List<Obat> data)
        {
            try
            {
                if (data == null) return false;
                var json = JsonConvert.SerializeObject(new { obat = data }, Formatting.Indented);
                File.WriteAllText(_defaultFilePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Save JSON: {ex.Message}");
                return false;
            }
        }
    }
}
