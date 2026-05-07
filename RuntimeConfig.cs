using System;

namespace TubesKPL
{
    // [LANGGA - RUNTIME CONFIGURATOR]
    public static class RuntimeConfig
    {
        // Default Pajak PPN 11%
        public static decimal PajakPPN { get; set; } = 0.11m;

        // Default Diskon 0%
        public static decimal DiskonAktif { get; set; } = 0.00m;

        // Method untuk mengubah nilai saat aplikasi berjalan (Runtime)
        public static void UpdateConfig(decimal pajakBaru, decimal diskonBaru)
        {
            PajakPPN = pajakBaru;
            DiskonAktif = diskonBaru;
        }
    }
}