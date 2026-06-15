using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubesKPL
{
    public class ItemTransaksi
    {
        public Obat Obat { get; set; }
        public int Jumlah { get; set; }

        public decimal Subtotal()
        {
            return Obat.Harga * Jumlah;
        }
    }
}
