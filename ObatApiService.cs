using System;
using System.Collections.Generic;
using System.Linq;
using static TubesKPL.Obat;

namespace TubesKPL
{
    // [AYONDI - REFACTOR] ObatApiService adalah service layer sederhana untuk status obat
    // Hanya menyimpan metode yang benar-benar digunakan di Main Form
    // Menghapus method yang tidak terpakai dan response classes yang kompleks

    public static class ObatApiService
    {
        private static List<Obat> _obatList = new List<Obat>();
        private static bool _initialized = false;

        // [AYONDI - REFACTOR] Initialize data obat dari Form
        public static void Initialize(List<Obat> sourceData)
        {
            _obatList.Clear();
            if (sourceData != null)
            {
                foreach (var obat in sourceData)
                {
                    _obatList.Add(obat);
                }
            }
            _initialized = true;
        }

        // [AYONDI - REFACTOR] GetAll - return semua obat dengan status updated
        public static List<Obat> GetAll()
        {
            foreach (var obat in _obatList)
            {
                obat.UpdateStatus();
            }
            return _obatList;
        }

        // [AYONDI - REFACTOR] GetStatusSummary - hitung ringkasan status sederhana
        public static void GetStatusSummary(out int available, out int lowStock, out int expired)
        {
            available = 0;
            lowStock = 0;
            expired = 0;

            foreach (var obat in _obatList)
            {
                obat.UpdateStatus();

                if (obat.status == StatusObat.Expired)
                    expired++;
                else if (obat.status == StatusObat.LowStock)
                    lowStock++;
                else
                    available++;
            }
        }

        public static bool IsInitialized()
        {
            return _initialized;
        }

        public static void Reset()
        {
            _obatList.Clear();
            _initialized = false;
        }
    }
}

