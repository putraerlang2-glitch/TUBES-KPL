using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TubesKPL.Test
{
    public class ObatStateMachineTableDrivenTests
    {
        [Fact]
        public void Test_StatusExpired_WhenDatePassed()
        {
            var obat = new Obat(
                "Paracetamol Expired",
                50,
                5000,
                DateTime.Now.AddDays(-1),
                "Tablet"
            );

            obat.UpdateStatus();

            Assert.Equal(StatusObat.Expired, obat.Status);
        }

        [Fact]
        public void Test_StatusLowStock_WhenStokRendah()
        {
            var obat = new Obat(
                "Ibuprofen Low",
                3,
                7000,
                DateTime.Now.AddDays(60),
                "Tablet"
            );

            obat.UpdateStatus();

            Assert.Equal(StatusObat.LowStock, obat.Status);
        }

        [Fact]
        public void Test_StatusLowStock_AtThreshold5()
        {
            var obat = new Obat(
                "Sanmol Threshold",
                5,
                3000,
                DateTime.Now.AddDays(30),
                "Sirup"
            );

            obat.UpdateStatus();

            Assert.Equal(StatusObat.LowStock, obat.Status);
        }

        [Fact]
        public void Test_StatusAvailable_WhenStokGood()
        {
            var obat = new Obat(
                "Vitamin C",
                20,
                500,
                DateTime.Now.AddDays(90),
                "Vitamin"
            );

            obat.UpdateStatus();

            Assert.Equal(StatusObat.Available, obat.Status);
        }

        [Fact]
        public void Test_PriorityExpiredFirst_IgnoreLowStock()
        {
            var obat = new Obat(
                "Multi Issue",
                2,
                5000,
                DateTime.Now.AddDays(-5),
                "Tablet"
            );

            obat.UpdateStatus();

            Assert.Equal(StatusObat.Expired, obat.Status);
            Assert.NotEqual(StatusObat.LowStock, obat.Status);
        }

        [Fact]
        public void Test_ColorGreen_ForAvailable()
        {
            var color = ObatStateMachine.GetStatusColor("Available");
            var expectedColor = Color.FromArgb(200, 255, 200);

            Assert.Equal(expectedColor, color);
        }

        [Fact]
        public void Test_ColorYellow_ForLowStock()
        {
            var color = ObatStateMachine.GetStatusColor("LowStock");
            var expectedColor = Color.FromArgb(255, 255, 200);

            Assert.Equal(expectedColor, color);
        }

        [Fact]
        public void Test_ColorRed_ForExpired()
        {
            var color = ObatStateMachine.GetStatusColor("Expired");
            var expectedColor = Color.FromArgb(255, 200, 200);

            Assert.Equal(expectedColor, color);
        }

        [Fact]
        public void Test_EnumAvailable_FromString()
        {
            var result = ObatStateMachine.GetStatusEnum("available");

            Assert.Equal(StatusObat.Available, result);
        }

        [Fact]
        public void Test_EnumLowStock_FromString()
        {
            var result = ObatStateMachine.GetStatusEnum("lowstock");

            Assert.Equal(StatusObat.LowStock, result);
        }

        [Fact]
        public void Test_EnumExpired_FromString()
        {
            var result = ObatStateMachine.GetStatusEnum("expired");

            Assert.Equal(StatusObat.Expired, result);
        }

        [Fact]
        public void Test_EnumCaseInsensitive_Uppercase()
        {
            var result = ObatStateMachine.GetStatusEnum("AVAILABLE");

            Assert.Equal(StatusObat.Available, result);
        }

        [Fact]
        public void Test_BatchUpdateStatus_MultipleObat()
        {
            var daftarObat = new List<Obat>
            {
                new Obat("Obat1", 20, 5000, DateTime.Now.AddDays(30), "Tablet"),
                new Obat("Obat2", 3, 7000, DateTime.Now.AddDays(30), "Tablet"),
                new Obat("Obat3", 15, 3000, DateTime.Now.AddDays(-1), "Sirup")
            };

            ObatStateMachine.UpdateAllStatus(daftarObat);

            Assert.Equal(StatusObat.Available, daftarObat[0].Status);
            Assert.Equal(StatusObat.LowStock, daftarObat[1].Status);
            Assert.Equal(StatusObat.Expired, daftarObat[2].Status);
        }

        [Fact]
        public void Test_StatusSummary_CountsCorrectly()
        {
            var daftarObat = new List<Obat>
            {
                new Obat("A", 20, 5000, DateTime.Now.AddDays(30), "Tablet"),
                new Obat("B", 3, 7000, DateTime.Now.AddDays(30), "Tablet"),
                new Obat("C", 15, 3000, DateTime.Now.AddDays(-1), "Sirup"),
                new Obat("D", 2, 2000, DateTime.Now.AddDays(30), "Tablet")
            };

            ObatStateMachine.GetStatusSummary(daftarObat, out int available, out int lowStock, out int expired, out int soonToExpire);

            Assert.Equal(1, available);
            Assert.Equal(2, lowStock);
            Assert.Equal(1, expired);
            Assert.Equal(0, soonToExpire);
        }

        [Fact]
        public void Test_CalculateStatus_DirectCall()
        {
            var result = ObatStateMachine.CalculateStatus(4, DateTime.Now.AddDays(30));

            Assert.Equal(StatusObat.LowStock, ObatStateMachine.GetStatusEnum(result));
        }

        [Fact]
        public void Test_EdgeCase_ZeroStok()
        {
            var obat = new Obat("Zero Stock", 0, 5000, DateTime.Now.AddDays(30), "Tablet");

            obat.UpdateStatus();

            Assert.Equal(StatusObat.LowStock, obat.Status);
        }

        [Fact]
        public void Test_EdgeCase_NegativeStok()
        {
            var result = ObatStateMachine.CalculateStatus(-5, DateTime.Now.AddDays(30));

            Assert.Equal(StatusObat.LowStock, ObatStateMachine.GetStatusEnum(result));
        }

        [Fact]
        public void Test_BoundaryAboveThreshold()
        {
            var obat = new Obat("Above Threshold", 6, 5000, DateTime.Now.AddDays(30), "Tablet");

            obat.UpdateStatus();

            Assert.Equal(StatusObat.Available, obat.Status);
        }

        [Fact]
        public void Test_StatusEnum_InvalidFallback()
        {
            var result = ObatStateMachine.GetStatusEnum("UnknownStatus");

            Assert.Equal(StatusObat.Available, result);
        }

        [Fact]
        public void Test_ColorInvalid_DefaultToGreen()
        {
            var color = ObatStateMachine.GetStatusColor("UnknownStatus");
            var expectedGreen = Color.FromArgb(200, 255, 200);

            Assert.Equal(expectedGreen, color);
        }
    }
}
