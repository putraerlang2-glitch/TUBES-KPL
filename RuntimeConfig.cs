using System;

namespace TubesKPL
{
    // Menyimpan nilai pengaturan pajak dan diskon yang bisa diubah saat aplikasi berjalan.
    public static class RuntimeConfig
    {
        // Pajak PPN bawaan adalah 11%.
        public static decimal PajakPPN { get; set; } = 0.11m;

        // Diskon bawaan adalah 0%.
        public static decimal DiskonAktif { get; set; } = 0.00m;

        // Menyimpan perubahan nilai pajak dan diskon baru ke dalam sistem.
        public static void UpdateConfig(decimal pajakBaru, decimal diskonBaru)
        {
            PajakPPN = pajakBaru;
            DiskonAktif = diskonBaru;
        }
    }
}