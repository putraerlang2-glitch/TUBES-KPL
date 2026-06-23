using System;

namespace TubesKPL
{
    public static class ObatValidator
    {
        public static bool ValidateObatInput(string namaObat, int jumlahStok, decimal hargaObat)
        {
            if (string.IsNullOrWhiteSpace(namaObat)) return false;
            if (jumlahStok < 0) return false;
            if (hargaObat <= 0) return false;
            return true;
        }

        public static bool ValidateObatInput(
            string namaObat,
            string stokText,
            string hargaText,
            out string pesan,
            out int jumlahStok,
            out decimal hargaObat)
        {
            pesan = "";
            jumlahStok = 0;
            hargaObat = 0;

            namaObat = namaObat.Trim();

            if (string.IsNullOrWhiteSpace(namaObat))
            {
                pesan = "Nama obat tidak boleh kosong.";
                return false;
            }

            if (!int.TryParse(stokText, out jumlahStok))
            {
                pesan = "Stok harus berupa angka.";
                return false;
            }

            if (jumlahStok < 0)
            {
                pesan = "Stok tidak boleh negatif.";
                return false;
            }

            if (!decimal.TryParse(hargaText, out hargaObat))
            {
                pesan = "Harga harus berupa angka.";
                return false;
            }

            if (hargaObat <= 0)
            {
                pesan = "Harga harus lebih dari 0.";
                return false;
            }

            return true;
        }
    }
}   