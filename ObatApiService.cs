
using System;
using System.Collections.Generic;
using System.Linq;

namespace TubesKPL
{
    // [KISS] Service ini hanya menyimpan data obat sementara dari API agar Form tidak langsung memegang semua proses.
    public static class ObatApiService
    {
        private static List<Obat> _obatList = new List<Obat>();
        private static bool _initialized = false;

        public static void Initialize(List<Obat> sourceData)
        {
            _obatList.Clear();
            // Jika sourceData null, gunakan list kosong untuk keamanan
            if (sourceData != null)
            {
                // Tambahkan hanya item yang tidak null
                _obatList.AddRange(sourceData.Where(o => o != null));
            }
            _initialized = true;
        }

        public static List<Obat> GetAll()
        {
            foreach (var obat in _obatList)
            {
                // Hindari null reference jika ada item yang null di list
                if (obat != null)
                {
                    obat.UpdateStatus();
                }
            }
            return _obatList;
        }

        public static void GetStatusSummary(out int available, out int lowStock, out int expired) => StateMachine.GetStatusCounts(_obatList, out available, out lowStock, out expired);

        public static bool IsInitialized() => _initialized;

        public static void Reset()
        {
            _obatList.Clear();
            _initialized = false;
        }
    }
}
