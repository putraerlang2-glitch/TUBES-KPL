using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TubesKPL.Test
{
    public class AddObatTest
    {
        [Fact]
        public void Test_TambahObat_Berhasil()
        {
            var dataObat = new List<Obat>();

            var obatBaru = new Obat(
                "Vitamin C",
                20,
                13000,
                DateTime.Now.AddDays(30),
                "Vitamin"
                );

            dataObat.Add(obatBaru);

            Assert.Single(dataObat);
        }
        [Fact]
        public void Test_ListBerisi_ObatBaru()
        {
            var dataObat = new List<Obat>();

            var obatBaru = new Obat(
                "Vitamin C",
                20,
                13000,
                DateTime.Now.AddDays(30),
                "Vitamin"
                );

            dataObat.Add(obatBaru);

            Assert.Contains(dataObat, o => o.Nama == "Vitamin C");
        }
    }
}
