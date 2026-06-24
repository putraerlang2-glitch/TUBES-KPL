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

        public Obat obat
        {
            get => Obat;
            set => Obat = value;
        }

        public int jumlah
        {
            get => Jumlah;
            set => Jumlah = value;
        }

        public decimal Subtotal()
        {
            if (Obat == null) throw new ArgumentNullException(nameof(Obat));
            return Obat.Harga * Jumlah;
        }
    }
}
