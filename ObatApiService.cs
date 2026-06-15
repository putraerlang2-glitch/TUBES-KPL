using System;
using System.Collections.Generic;
using System.Linq;

namespace TubesKPL
{
    public static class ObatApiService
    {
        private static List<Obat> _obatList = new List<Obat>();
        private static bool _initialized = false;

        public static void Initialize(List<Obat> sourceData)
        {
            _obatList.Clear();
            if (sourceData != null) _obatList.AddRange(sourceData);
            _initialized = true;
        }

        public static List<Obat> GetAll()
        {
            foreach (var obat in _obatList) obat.UpdateStatus();
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
