using System;
using Newtonsoft.Json;

namespace TubesKPL
{
    public class Obat
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nama")]
        public string Nama { get; set; } = string.Empty;

        [JsonProperty("kategori")]
        public string Kategori { get; set; } = "Tablet";

        [JsonProperty("stok")]
        public int Stok { get; set; }

        [JsonProperty("harga")]
        public decimal Harga { get; set; }

        [JsonProperty("expiredDate")]
        public DateTime ExpiredDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } = "Available";

        public Obat() => ExpiredDate = DateTime.Now.AddYears(1);

        public Obat(string nama, int stok, decimal harga, DateTime expiredDate, string kategori = "Tablet", int id = 0)
        {
            if (string.IsNullOrWhiteSpace(nama)) throw new ArgumentException("Nama obat tidak boleh kosong");
            if (stok < 0) throw new ArgumentException("Stok tidak boleh negatif");
            if (harga <= 0) throw new ArgumentException("Harga harus lebih dari 0");

            Id = id;
            Nama = nama.Trim();
            Stok = stok;
            Harga = harga;
            ExpiredDate = expiredDate;
            Kategori = kategori ?? "Tablet";
            UpdateStatus();
        }

        public Obat(string nama, int stok, decimal harga, DateTime expiredDate, KategoriObat kategoriEnum, int id = 0)
            : this(nama, stok, harga, expiredDate, kategoriEnum.ToString(), id) { }

        public void UpdateStatus()
        {
            Status = ExpiredDate < DateTime.Now ? "Expired" : (Stok <= 5 ? "LowStock" : "Available");
        }

        public StatusObat GetStatusEnum()
        {
            return Status?.ToLower() switch
            {
                "lowstock" => StatusObat.LowStock,
                "expired" => StatusObat.Expired,
                _ => StatusObat.Available
            };
        }

        public override string ToString() => $"[{Id}] {Nama} | Stok: {Stok} | Status: {Status} | Exp: {ExpiredDate:yyyy-MM-dd}";
    }

    public enum StatusObat
    {
        Available,
        LowStock,
        Expired
    }

    public enum KategoriObat
    {
        Tablet,
        Salep,
        Sirup,
        Vitamin,
        Antibiotik,
        AntiJamur
    }
}
