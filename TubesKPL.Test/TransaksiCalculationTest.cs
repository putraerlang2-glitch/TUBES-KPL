using Xunit;
using TubesKPL;
using System.Xml.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TubesKPL.Test
{
    public class TransaksiCalculationTest
    {
        [Fact]
        public void Test_TotalTanpa_PajakDiskon()
        {
            Decimal subtotal = 100000m;
            decimal pajak = 0m;
            decimal diskon = 0m;

            decimal nominalDiskon = subtotal * diskon;
            decimal subTotalSetelahDiskon = subtotal - nominalDiskon;
            decimal nominalPajak = subTotalSetelahDiskon * pajak;
            decimal total = subTotalSetelahDiskon + nominalPajak;

            Assert.Equal(100000m, total);
        }

        [Fact]
        public void Test_Kembalian_UangPas()
        {
            decimal totalBiaya = 5000m;
            decimal uangBayar = 5000m;

            decimal kembalian = uangBayar - totalBiaya;

            Assert.Equal(0m, kembalian);
        }

        [Fact]
        public void Test_Kembalian_UangLebih()
        {
            decimal totalBiaya = 15000m;
            decimal uangBayar = 20000;

            decimal kembalian = uangBayar - totalBiaya;

            Assert.Equal(5000m, kembalian);
        }

        [Fact]
        public void Test_TotalDengan_PajakDiskon()
        {
            Decimal subtotal = 100000m;
            decimal pajak = 0.15m; //Pajak 15%
            decimal diskon = 0.05m; //Diskon 50%

            decimal nominalDiskon = subtotal * diskon;
            decimal subTotalSetelahDiskon = subtotal - nominalDiskon;
            decimal nominalPajak = subTotalSetelahDiskon * pajak;
            decimal total = subTotalSetelahDiskon + nominalPajak;

            Assert.Equal(109250m, total);
        }
        [Fact]
        public void Test_UangKurang()
        {
            decimal totalBiaya = 15000m;
            decimal uangBayar = 5000m;

            bool uangCukup = uangBayar > totalBiaya;

            Assert.False(uangCukup);
        }
    }
}
