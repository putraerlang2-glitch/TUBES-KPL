using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubesKPL
{
    public class ItemTransaksi
    {
        public Obat obat { get; set; }
        public int jumlah { get; set; }

        public decimal Subtotal()
        {
            return obat.Harga * jumlah;
        }
    }
}
