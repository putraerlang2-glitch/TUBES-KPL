using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubesKPL
{
    public class ItemTransaksiFactory
    {
        public static ItemTransaksi CreateItem(Obat obat, int jumlah)
        {
            TransaksiValidator.TambahKeranjangValidator(obat, jumlah);

            return new ItemTransaksi()
            {
                Obat = obat,
                Jumlah = jumlah
            };
        }
    }
}
