using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubesKPL
{
    //Class DS Factory create item & Validaste
    public class ItemTransaksiFactory
    {
        public static ItemTransaksi CreateItem(Obat obat, int jumlah)
        {
            // Pakai validator untuk validasi
            TransaksiValidator.TambahKeranjangValidator(obat, jumlah);

            // Jika validasi lolos, buat item
            return new ItemTransaksi()
            {
                obat = obat,
                jumlah = jumlah
            };
        }
    }
}
