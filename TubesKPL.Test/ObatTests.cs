using Xunit;
using TubesKPL;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TubesKPL.Test
{
    public class ObatTests
    {
        [Fact]
        public void Test_Expired_JadiExpired()
        {
            //Arrange
            var obat = new Obat(
                "Paracetamol",
                100,
                5000,
                DateTime.Now.AddDays(-1),
                "Tablet"
                );

            //Act
            obat.UpdateStatus();

            //Assert
            Assert.Equal("Expired", obat.Status);
        }
        [Fact]
        public void Test_StokRendah_jadiLowStock()
        {
            var obat = new Obat(
                "Paracetamol",
                2,
                5000,
                DateTime.Now.AddDays(30),
                "Tablet"
                );

            obat.UpdateStatus();
            Assert.Equal("LowStock", obat.Status);
        }
        [Fact]
        public void Test_StokBanyak_jadiAvailable()
        {
            var obat = new Obat(
                "Paracetamol",
                20,
                5000,
                DateTime.Now.AddDays(30),
                "Tablet"
                );

            obat.UpdateStatus();
            Assert.Equal("Available", obat.Status);
        }

        [Fact]
        public void Test_GetStatusEnum_Available_ReturnsAvailable()
        {
            // Arrange
            var obat = new Obat("Paracetamol", 20, 5000, DateTime.Now.AddDays(30));

            // Act
            var status = obat.GetStatusEnum();

            // Assert
            Assert.Equal(StatusObat.Available, status);
        }

        [Fact]
        public void Test_GetStatusEnum_LowStock_ReturnsLowStock()
        {
            // Arrange
            var obat = new Obat("Paracetamol", 2, 5000, DateTime.Now.AddDays(30));

            // Act
            var status = obat.GetStatusEnum();

            // Assert
            Assert.Equal(StatusObat.LowStock, status);
        }

        [Fact]
        public void Test_GetStatusEnum_Expired_ReturnsExpired()
        {
            // Arrange
            var obat = new Obat("Paracetamol", 100, 5000, DateTime.Now.AddDays(-1));

            // Act
            var status = obat.GetStatusEnum();

            // Assert
            Assert.Equal(StatusObat.Expired, status);
        }

        [Fact]
        public void Test_Konstruktor_KategoriEnum_Berjalan()
        {
            // Arrange
            var obat = new Obat("Salep", 10, 15000, DateTime.Now.AddDays(30), KategoriObat.Salep);

            // Act & Assert
            Assert.Equal("Salep", obat.Kategori);
            Assert.Equal("Salep", obat.Nama);
            Assert.Equal(10, obat.Stok);
            Assert.Equal(15000, obat.Harga);
        }
    }
}
