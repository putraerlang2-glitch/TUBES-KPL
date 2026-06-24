using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TubesKPL;
using Xunit;

namespace TubesKPL.Test
{
    public class RuntimeConfigTests
    {
        [Fact]
        public void Test_UbahNilaiPajak_DanDiskon()
        {
            Decimal pajakBaru = 0.15m; // Pajak 15%
            Decimal diskonBaru = 0.5m; // Diskon 5%

            RuntimeConfig.UpdateConfig(pajakBaru, diskonBaru);

            Assert.Equal(0.15m, RuntimeConfig.PajakPPN);
            Assert.Equal(0.5m, RuntimeConfig.DiskonAktif);
        }
        [Fact]
        public void Test_PajakDiskon_Semua0()
        {
            Decimal pajakBaru = 0.0m; // Pajak 0%
            Decimal diskonBaru = 0.0m; // Diskon 0%

            RuntimeConfig.UpdateConfig(pajakBaru, diskonBaru);

            Assert.Equal(0.0m, RuntimeConfig.PajakPPN);
            Assert.Equal(0.0m, RuntimeConfig.DiskonAktif);
        }

        [Fact]
        public void Test_Pajak15_Diskon0()
        {
            Decimal pajakBaru = 15m; // Pajak 0%
            Decimal diskonBaru = 0m; // Diskon 0%

            RuntimeConfig.UpdateConfig(pajakBaru, diskonBaru);

            Assert.Equal(15m, RuntimeConfig.PajakPPN);
            Assert.Equal(0m, RuntimeConfig.DiskonAktif);
        }
        [Fact]
        public void Test_Pajak0_Diskon20()
        {
            Decimal pajakBaru = 0m; // Pajak 0%
            Decimal diskonBaru = 20m; // Diskon 20%

            RuntimeConfig.UpdateConfig(pajakBaru, diskonBaru);

            Assert.Equal(0m, RuntimeConfig.PajakPPN);
            Assert.Equal(20m, RuntimeConfig.DiskonAktif);
        }
    }
}
