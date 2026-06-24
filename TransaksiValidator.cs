using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubesKPL
{
    //Class DS Factory Validator
    public class TransaksiValidator
    {
        public static void TambahKeranjangValidator(Obat obat, int jumlah)
        {
            if (obat == null)
            {
                throw new ArgumentException("Obat harus dipilih");
            }

            // Validasi 2: Jumlah harus > 0
            if (jumlah <= 0)
            {
                throw new ArgumentException("Jumlah harus lebih dari 0");
            }

            // Validasi 3: Stok harus cukup
            if (jumlah > obat.Stok)
            {
                throw new ArgumentException("Stok tidak cukup");
            }
        }
        public static void HapusValidator(int index, int totalItems)
        {
            if (index < 0 || index >= totalItems)
            {
                throw new ArgumentException("Item yang dipilih tidak valid");
            }
        }
        public static bool ValidasiJumlah(string quantityInput, out int quantity)
        {
            quantity = 0;

            // Cek kosong
            if (string.IsNullOrWhiteSpace(quantityInput))
            {
                return false;
            }

            // Cek format angka
            if (!int.TryParse(quantityInput, out quantity))
            {
                return false;
            }

            return true;
        }
    }
}
