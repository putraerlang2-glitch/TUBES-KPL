using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static TubesKPL.Obat;

namespace TubesKPL
{
    public static class JsonDataManager
    {
        private static string _defaultFilePath = "data.json";

        public static void SetDataFilePath(string filePath)
        {
            _defaultFilePath = filePath;
        }

        public static List<Obat> LoadFromJson()
        {
            var result = new List<Obat>();

            try
            {
                if (!File.Exists(_defaultFilePath))
                    return result;

                string jsonContent = File.ReadAllText(_defaultFilePath, Encoding.UTF8);
                result = ParseJsonToObatList(jsonContent);

                return result;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[ERROR] Failed to read JSON file: {ex.Message}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Unexpected error loading JSON: {ex.Message}");
                return result;
            }
        }

        public static bool SaveToJson(List<Obat> data)
        {
            try
            {
                if (data == null)
                {
                    Console.WriteLine("[ERROR] Cannot save null data to JSON");
                    return false;
                }

                string jsonContent = ConvertObatListToJson(data);
                File.WriteAllText(_defaultFilePath, jsonContent, Encoding.UTF8);
                return true;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[ERROR] Failed to write JSON file: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Unexpected error saving JSON: {ex.Message}");
                return false;
            }
        }

        private static List<Obat> ParseJsonToObatList(string jsonContent)
        {
            var result = new List<Obat>();

            try
            {
                if (string.IsNullOrWhiteSpace(jsonContent))
                    return result;

                jsonContent = jsonContent.Replace("\n", "").Replace("\r", "").Replace("\t", "");

                int arrayStart = jsonContent.IndexOf("\"obat\"");
                if (arrayStart == -1)
                    return result;

                arrayStart = jsonContent.IndexOf("[", arrayStart);
                int arrayEnd = jsonContent.LastIndexOf("]");

                if (arrayStart == -1 || arrayEnd == -1 || arrayStart >= arrayEnd)
                    return result;

                string arrayContent = jsonContent.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);
                int currentPos = 0;

                while (currentPos < arrayContent.Length)
                {
                    int objStart = arrayContent.IndexOf("{", currentPos);
                    if (objStart == -1) break;

                    int objEnd = arrayContent.IndexOf("}", objStart);
                    if (objEnd == -1) break;

                    var obat = ParseJsonObject(arrayContent.Substring(objStart + 1, objEnd - objStart - 1));
                    if (obat != null)
                        result.Add(obat);

                    currentPos = objEnd + 1;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to parse JSON array: {ex.Message}");
                return result;
            }
        }

        private static Obat ParseJsonObject(string objContent)
        {
            try
            {
                string nama = ExtractJsonField(objContent, "nama");
                if (string.IsNullOrEmpty(nama))
                    return null;

                string stokStr = ExtractJsonField(objContent, "stok");
                string hargaStr = ExtractJsonField(objContent, "harga");
                string expiredDateStr = ExtractJsonField(objContent, "expiredDate");
                string kategoriStr = ExtractJsonField(objContent, "kategori");

                if (string.IsNullOrEmpty(stokStr) || string.IsNullOrEmpty(hargaStr) || 
                    string.IsNullOrEmpty(expiredDateStr) || string.IsNullOrEmpty(kategoriStr))
                    return null;

                int stok = int.Parse(stokStr);
                decimal harga = decimal.Parse(hargaStr);
                DateTime expiredDate = DateTime.Parse(expiredDateStr);
                var kategori = (KategoriObat)Enum.Parse(typeof(KategoriObat), kategoriStr);

                return new Obat(nama, stok, harga, expiredDate, kategori);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[WARNING] Failed to parse JSON object: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Unexpected error parsing JSON object: {ex.Message}");
                return null;
            }
        }

        private static string ExtractJsonField(string content, string fieldName)
        {
            try
            {
                string searchPattern = "\"" + fieldName + "\":";
                int fieldPos = content.IndexOf(searchPattern);

                if (fieldPos == -1)
                    return "";

                int valueStart = fieldPos + searchPattern.Length;

                while (valueStart < content.Length && (content[valueStart] == ' ' || content[valueStart] == '\t'))
                    valueStart++;

                if (content[valueStart] == '"')
                {
                    int valueEnd = content.IndexOf("\"", valueStart + 1);
                    return valueEnd == -1 ? "" : content.Substring(valueStart + 1, valueEnd - valueStart - 1);
                }
                else
                {
                    int valueEnd = content.IndexOf(",", valueStart);
                    if (valueEnd == -1)
                        valueEnd = content.Length;
                    return content.Substring(valueStart, valueEnd - valueStart).Trim();
                }
            }
            catch
            {
                return "";
            }
        }

        private static string ConvertObatListToJson(List<Obat> obatList)
        {
            var sb = new StringBuilder();

            sb.Append("{\n");
            sb.Append("  \"obat\": [\n");

            for (int i = 0; i < obatList.Count; i++)
            {
                var obat = obatList[i];

                sb.Append("    {\n");
                sb.Append("      \"nama\": \"" + EscapeJsonString(obat.Nama) + "\",\n");
                sb.Append("      \"stok\": " + obat.Stok + ",\n");
                sb.Append("      \"harga\": " + obat.Harga + ",\n");
                sb.Append("      \"expiredDate\": \"" + obat.ExpiredDate.ToString("yyyy-MM-dd") + "\",\n");
                sb.Append("      \"kategori\": \"" + obat.Kategori + "\"\n");
                sb.Append("    }");

                if (i < obatList.Count - 1)
                    sb.Append(",");

                sb.Append("\n");
            }

            sb.Append("  ]\n");
            sb.Append("}");
            return sb.ToString();
        }

        private static string EscapeJsonString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }
}
