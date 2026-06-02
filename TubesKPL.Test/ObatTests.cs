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

    }
}
