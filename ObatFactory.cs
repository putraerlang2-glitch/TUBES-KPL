using System;

namespace TubesKPL
{
    public static class ObatFactory
    {
        public static Obat Create(
            string namaObat,
            int jumlahStok,
            decimal hargaObat,
            DateTime tanggalExpired,
            KategoriObat kategoriObat)
        {
            namaObat = namaObat.Trim();

            return new Obat(
                namaObat,
                jumlahStok,
                hargaObat,
                tanggalExpired,
                kategoriObat);
        }
    }
}