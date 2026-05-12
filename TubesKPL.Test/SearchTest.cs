using Xunit;
using TubesKPL;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TubesKPL.Test
{
    public class SearchTest
    {
        private List<Obat> GetSampleData()
        {
            return new List<Obat>()
            {
                new Obat("Paracetamol", 10, 5000, DateTime.Now.AddDays(30), "Tablet"),
                new Obat("Ibuprofen", 20, 7000, DateTime.Now.AddDays(30), "Tablet"),
                new Obat("Vitamin C", 15, 3000, DateTime.Now.AddDays(30), "Vitamin")
            };
        }

        [Fact]
        public void test_Data_Ditemukan()
        {
            var dataObat = GetSampleData();
            string keyword = "Paracetamol";

            var hasil = dataObat.Where(o => o.Nama.ToLower().Contains(keyword.ToLower())).ToList();

            Assert.Single(hasil);
            Assert.Equal("Paracetamol", hasil[0].nama);
        }
        [Fact]
        public void test_Data_TidakDitemukan()
        {
            var dataObat = GetSampleData();
            string keyword = "Jane Doe Mandi";

            var hasil = dataObat.Where(o => o.Nama.ToLower().Contains(keyword.ToLower())).ToList();

            Assert.Empty(hasil);
        }

    }
}
