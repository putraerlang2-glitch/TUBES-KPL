using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static TubesKPL.Obat;

namespace TubesKPL
{
    // [AYONDI] JsonDataManager - Class untuk mengelola operasi JSON file
    // Menyediakan method untuk load dan save data obat ke file JSON
    // Compatible dengan .NET Framework 4.7.2 dan C# 7.3
    // Format JSON disesuaikan dengan struktur Obat class

    /// <summary>
    /// [AYONDI] Class JsonDataManager
    /// Menyediakan method-method untuk mengelola JSON file operations
    /// Tanpa menggunakan library JSON external (manual parsing)
    /// </summary>
    public static class JsonDataManager
    {
        // [AYONDI] Default file path untuk data JSON
        private static string _defaultFilePath = "data.json";

        // [AYONDI] Method untuk set custom file path
        /// <summary>
        /// [AYONDI] Set custom path untuk JSON data file
        /// Parameter: string filePath - path file JSON
        /// </summary>
        public static void SetDataFilePath(string filePath)
        {
            _defaultFilePath = filePath;
        }

        // [AYONDI] Method untuk load data dari JSON file
        /// <summary>
        /// [AYONDI] Load data obat dari file JSON
        /// Return: List<Obat> berisi semua data obat dari JSON
        /// Jika file tidak ditemukan, return empty list dengan warning
        /// </summary>
        public static List<Obat> LoadFromJson()
        {
            List<Obat> result = new List<Obat>();

            try
            {
                // [AYONDI] Check apakah file ada
                if (!File.Exists(_defaultFilePath))
                {
                    Console.WriteLine("[AYONDI] Warning: File " + _defaultFilePath + " not found. Returning empty list.");
                    return result;
                }

                // [AYONDI] Read entire file as string
                string jsonContent = File.ReadAllText(_defaultFilePath, Encoding.UTF8);

                // [AYONDI] Parse JSON manual (tanpa library)
                result = ParseJsonToObatList(jsonContent);

                Console.WriteLine("[AYONDI] Successfully loaded " + result.Count + " medicines from " + _defaultFilePath);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AYONDI] Error loading JSON: " + ex.Message);
                return result;
            }
        }

        // [AYONDI] Method untuk save data ke JSON file
        /// <summary>
        /// [AYONDI] Save data obat ke file JSON
        /// Parameter: List<Obat> data - list obat untuk disave
        /// </summary>
        public static bool SaveToJson(List<Obat> data)
        {
            try
            {
                // [AYONDI] Convert data ke JSON string
                string jsonContent = ConvertObatListToJson(data);

                // [AYONDI] Write ke file
                File.WriteAllText(_defaultFilePath, jsonContent, Encoding.UTF8);

                Console.WriteLine("[AYONDI] Successfully saved " + data.Count + " medicines to " + _defaultFilePath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AYONDI] Error saving JSON: " + ex.Message);
                return false;
            }
        }

        // [AYONDI] Helper: Parse JSON string ke List<Obat>
        /// <summary>
        /// [AYONDI] Parse JSON manual ke List<Obat>
        /// Format JSON yang didukung:
        /// {
        ///   "obat": [
        ///     { "nama": "...", "stok": ..., "harga": ..., "expiredDate": "yyyy-MM-dd", "kategori": "..." }
        ///   ]
        /// }
        /// </summary>
        private static List<Obat> ParseJsonToObatList(string jsonContent)
        {
            List<Obat> result = new List<Obat>();

            try
            {
                // [AYONDI] Remove whitespace untuk simplify parsing
                jsonContent = jsonContent.Replace("\n", "").Replace("\r", "").Replace("\t", "");

                // [AYONDI] Find "obat" array dalam JSON
                int arrayStart = jsonContent.IndexOf("\"obat\"");
                if (arrayStart == -1)
                {
                    return result;
                }

                // [AYONDI] Find opening bracket [
                arrayStart = jsonContent.IndexOf("[", arrayStart);
                int arrayEnd = jsonContent.LastIndexOf("]");

                if (arrayStart == -1 || arrayEnd == -1 || arrayStart >= arrayEnd)
                {
                    return result;
                }

                // [AYONDI] Extract array content
                string arrayContent = jsonContent.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);

                // [AYONDI] Split by object delimiter }{ untuk setiap item
                // Simple approach: find each { ... } pair
                int currentPos = 0;
                while (currentPos < arrayContent.Length)
                {
                    int objStart = arrayContent.IndexOf("{", currentPos);
                    if (objStart == -1) break;

                    int objEnd = arrayContent.IndexOf("}", objStart);
                    if (objEnd == -1) break;

                    string objContent = arrayContent.Substring(objStart + 1, objEnd - objStart - 1);
                    Obat obat = ParseJsonObject(objContent);

                    if (obat != null)
                    {
                        result.Add(obat);
                    }

                    currentPos = objEnd + 1;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AYONDI] Error parsing JSON: " + ex.Message);
                return result;
            }
        }

        // [AYONDI] Helper: Parse single JSON object ke Obat
        /// <summary>
        /// [AYONDI] Parse single JSON object string ke Obat instance
        /// Parameter: string objContent - JSON object content (without {})
        /// Return: Obat instance atau null jika parsing gagal
        /// </summary>
        private static Obat ParseJsonObject(string objContent)
        {
            try
            {
                // [AYONDI] Extract field values menggunakan simple string parsing
                string nama = ExtractJsonField(objContent, "nama");
                string stokStr = ExtractJsonField(objContent, "stok");
                string hargaStr = ExtractJsonField(objContent, "harga");
                string expiredDateStr = ExtractJsonField(objContent, "expiredDate");
                string kategoriStr = ExtractJsonField(objContent, "kategori");

                // [AYONDI] Validate extracted values
                if (string.IsNullOrEmpty(nama) || string.IsNullOrEmpty(stokStr) || string.IsNullOrEmpty(hargaStr) || string.IsNullOrEmpty(expiredDateStr) || string.IsNullOrEmpty(kategoriStr))
                {
                    return null;
                }

                // [AYONDI] Parse numeric values
                int stok = int.Parse(stokStr);
                decimal harga = decimal.Parse(hargaStr);
                DateTime expiredDate = DateTime.Parse(expiredDateStr);

                // [AYONDI] Parse kategori enum
                KategoriObat kategori = (KategoriObat)Enum.Parse(typeof(KategoriObat), kategoriStr);

                // [AYONDI] Create dan return Obat instance
                return new Obat(nama, stok, harga, expiredDate, kategori);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AYONDI] Error parsing JSON object: " + ex.Message);
                return null;
            }
        }

        // [AYONDI] Helper: Extract JSON field value
        /// <summary>
        /// [AYONDI] Extract nilai field dari JSON object string
        /// Parameter: string content - JSON object content
        /// Parameter: string fieldName - nama field yang ingin diambil
        /// Return: string value dari field (tanpa quotes untuk string, atau raw number)
        /// </summary>
        private static string ExtractJsonField(string content, string fieldName)
        {
            try
            {
                // [AYONDI] Find field name dalam format "fieldName":
                string searchPattern = "\"" + fieldName + "\":";
                int fieldPos = content.IndexOf(searchPattern);

                if (fieldPos == -1)
                {
                    return "";
                }

                // [AYONDI] Move position ke setelah ": "
                int valueStart = fieldPos + searchPattern.Length;

                // [AYONDI] Skip whitespace
                while (valueStart < content.Length && (content[valueStart] == ' ' || content[valueStart] == '\t'))
                {
                    valueStart++;
                }

                // [AYONDI] Check apakah value adalah string (dimulai dengan ") atau number
                if (content[valueStart] == '"')
                {
                    // [AYONDI] String value: cari closing quote
                    int valueEnd = content.IndexOf("\"", valueStart + 1);
                    if (valueEnd == -1) return "";
                    return content.Substring(valueStart + 1, valueEnd - valueStart - 1);
                }
                else
                {
                    // [AYONDI] Number value: cari delimiter (comma atau closing brace)
                    int valueEnd = content.IndexOf(",", valueStart);
                    if (valueEnd == -1)
                    {
                        valueEnd = content.Length;
                    }
                    return content.Substring(valueStart, valueEnd - valueStart).Trim();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AYONDI] Error extracting field '" + fieldName + "': " + ex.Message);
                return "";
            }
        }

        // [AYONDI] Helper: Convert List<Obat> ke JSON string
        /// <summary>
        /// [AYONDI] Convert List<Obat> ke format JSON string
        /// Format output:
        /// {
        ///   "obat": [
        ///     { "nama": "...", "stok": ..., "harga": ..., "expiredDate": "yyyy-MM-dd", "kategori": "..." },
        ///     ...
        ///   ]
        /// }
        /// </summary>
        private static string ConvertObatListToJson(List<Obat> obatList)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{\n");
            sb.Append("  \"obat\": [\n");

            for (int i = 0; i < obatList.Count; i++)
            {
                var obat = obatList[i];

                sb.Append("    {\n");
                sb.Append("      \"nama\": \"" + EscapeJsonString(obat.nama) + "\",\n");
                sb.Append("      \"stok\": " + obat.stok + ",\n");
                sb.Append("      \"harga\": " + obat.harga + ",\n");
                sb.Append("      \"expiredDate\": \"" + obat.expiredDate.ToString("yyyy-MM-dd") + "\",\n");
                sb.Append("      \"kategori\": \"" + obat.kategori.ToString() + "\"\n");
                sb.Append("    }");

                // [AYONDI] Add comma jika bukan item terakhir
                if (i < obatList.Count - 1)
                {
                    sb.Append(",");
                }

                sb.Append("\n");
            }

            sb.Append("  ]\n");
            sb.Append("}");

            return sb.ToString();
        }

        // [AYONDI] Helper: Escape special characters untuk JSON string
        /// <summary>
        /// [AYONDI] Escape special characters dalam string untuk JSON compliance
        /// Mengganti: " -> \", \ -> \\, newline -> \n
        /// </summary>
        private static string EscapeJsonString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }
}
