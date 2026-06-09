using System;
using System.Collections.Generic;
using TubesKPL;
using Xunit;

namespace TubesKPL.Test
{
    public class ItemTransaksiTests
    {
        [Fact]
        public void Test_PerhitunganHarga()
        {
            //Arrange
            var obat = new Obat(
                "Vitamin C",
                100,
                5000,
                DateTime.Now.AddDays(30),
                "Vitamin"
                );

            var item = new ItemTransaksi()
            {
                obat = obat,
                jumlah = 7
            };

            decimal hasil = item.Subtotal();
            Assert.Equal(35000, hasil);
        }
        [Fact]
        public void Test_Hasil0_jikaobat0()
        {
            //Arrange
            var obat = new Obat(
                "Vitamin C",
                100,
                5000,
                DateTime.Now.AddDays(30),
                "Vitamin"
                );

            var item = new ItemTransaksi()
            {
                obat = obat,
                jumlah = 0
            };

            decimal hasil = item.Subtotal();
            Assert.Equal(0, hasil);
        }
        [Fact]
        public void Test_Haga0_hasilgratis0()
        {
            //Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                var obat = new Obat(
                    "Vitamin C",
                    100,
                    0,
                    DateTime.Now.AddDays(30),
                    "Vitamin"
                );
            });
        }
    }
}
