using System;
using System.Collections.Generic;
using System.Linq;
using static TubesKPL.Obat;

namespace TubesKPL
{
    // ============================================================
    // DESIGN PATTERN: Service Locator + Facade Pattern + Singleton
    // ============================================================
    // Service Locator: Centralized access point untuk data obat
    // Facade Pattern: Menyederhanakan interface untuk client
    //   - Client tidak perlu tahu tentang StateMachine internal
    //   - Hanya call GetAll(), GetStatusSummary(), dll
    // 
    // Singleton Pattern (implicit):
    //   - Static class dengan static fields = one instance globally
    //   - State (_obatList, _initialized) bersama di memory
    //
    // BENEFIT:
    // - Single point of initialization & reset
    // - Separation of concerns: Service layer tidak mix dengan UI
    // - Mudah di-test: bisa reset state anytime
    // ============================================================
    public static class ObatApiService
    {
        // [CLEAN CODE] Static fields untuk state management
        // [SECURE CODE] Private access: state tidak bisa diubah dari luar
        private static List<Obat> _obatList = new List<Obat>();
        private static bool _initialized = false;

        // [METHOD] Initialize service dengan data
        // [CLEAN CODE] Clear existing data sebelum initialize (prevents duplication)
        public static void Initialize(List<Obat> sourceData)
        {
            _obatList.Clear();  // [SECURE CODE] Clear terlebih dahulu
            if (sourceData != null)  // [SECURE CODE] Null check
            {
                foreach (var obat in sourceData)
                {
                    _obatList.Add(obat);
                }
            }
            _initialized = true;
        }

        // [METHOD] Get all obat dengan status updated
        // [FACADE PATTERN] Client hanya call 1 method, internal complexity hidden
        // [CLEAN CODE] Update status untuk semua obat sebelum return
        public static List<Obat> GetAll()
        {
            foreach (var obat in _obatList)
            {
                obat.UpdateStatus();  // [DELEGATION PATTERN] Update status via Obat method
            }
            return _obatList;
        }

        // [METHOD] Get status summary (wrapper untuk StateMachine)
        // [FACADE PATTERN] Menyederhanakan akses ke StateMachine logic
        // [CLEAN CODE] Single Responsibility: hanya delegate ke StateMachine
        public static void GetStatusSummary(out int available, out int lowStock, out int expired)
        {
            StateMachine.GetStatusCounts(_obatList, out available, out lowStock, out expired);
        }

        // [METHOD] Check apakah service sudah initialize
        // [CLEAN CODE] Simple getter untuk state
        public static bool IsInitialized()
        {
            return _initialized;
        }

        // [METHOD] Reset service ke state awal
        // [CLEAN CODE] Explicit reset method untuk cleanup
        // [SECURE CODE] Useful untuk testing dan state management
        public static void Reset()
        {
            _obatList.Clear();
            _initialized = false;
        }
    }
}

