
using System;
using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace TubesKPL.Test
{
    public class StateMachineTests
    {
        [Fact]
        public void EvaluateStatus_ExpiredDatePast_ShouldSetExpired()
        {
            // Arrange
            var obat = new Obat("Paracetamol", 10, 5000, DateTime.Now.AddDays(-1));

            // Act
            StateMachine.EvaluateStatus(obat);

            // Assert
            Assert.Equal(StateMachine.STATUS_EXPIRED, obat.Status);
        }

        [Fact]
        public void EvaluateStatus_StockBelowThreshold_ShouldSetLowStock()
        {
            // Arrange
            var obat = new Obat("Paracetamol", 3, 5000, DateTime.Now.AddYears(1));

            // Act
            StateMachine.EvaluateStatus(obat);

            // Assert
            Assert.Equal(StateMachine.STATUS_LOW_STOCK, obat.Status);
        }

        [Fact]
        public void EvaluateStatus_StockAboveThresholdNotExpired_ShouldSetAvailable()
        {
            // Arrange
            var obat = new Obat("Paracetamol", 20, 5000, DateTime.Now.AddYears(1));

            // Act
            StateMachine.EvaluateStatus(obat);

            // Assert
            Assert.Equal(StateMachine.STATUS_AVAILABLE, obat.Status);
        }

        [Fact]
        public void EvaluateStatus_NullObat_ShouldNotThrow()
        {
            // Arrange, Act, Assert
            var exception = Record.Exception(() => StateMachine.EvaluateStatus(null));
            Assert.Null(exception);
        }

        [Fact]
        public void GetStatusColor_ExpiredStatus_ShouldReturnRed()
        {
            // Act
            var color = StateMachine.GetStatusColor(StateMachine.STATUS_EXPIRED);

            // Assert
            Assert.Equal(Color.FromArgb(220, 53, 69), color);
        }

        [Fact]
        public void GetStatusColor_LowStockStatus_ShouldReturnYellow()
        {
            // Act
            var color = StateMachine.GetStatusColor(StateMachine.STATUS_LOW_STOCK);

            // Assert
            Assert.Equal(Color.FromArgb(255, 193, 7), color);
        }

        [Fact]
        public void GetStatusColor_AvailableStatus_ShouldReturnGreen()
        {
            // Act
            var color = StateMachine.GetStatusColor(StateMachine.STATUS_AVAILABLE);

            // Assert
            Assert.Equal(Color.FromArgb(40, 167, 69), color);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void GetStatusColor_EmptyOrNullStatus_ShouldReturnWhite(string status)
        {
            // Act
            var color = StateMachine.GetStatusColor(status);

            // Assert
            Assert.Equal(Color.White, color);
        }

        [Fact]
        public void GetStatusCounts_ValidList_ShouldCountCorrectly()
        {
            // Arrange
            var list = new List<Obat>
            {
                new Obat("Obat 1", 20, 5000, DateTime.Now.AddYears(1)), // Available
                new Obat("Obat 2", 3, 5000, DateTime.Now.AddYears(1)),   // Low Stock
                new Obat("Obat 3", 5, 5000, DateTime.Now.AddDays(-1)),   // Expired
                new Obat("Obat 4", 10, 5000, DateTime.Now.AddDays(-2))   // Expired
            };
            // Evaluate all statuses first
            foreach (var o in list) StateMachine.EvaluateStatus(o);

            // Act
            StateMachine.GetStatusCounts(list, out var available, out var lowStock, out var expired);

            // Assert
            Assert.Equal(1, available);
            Assert.Equal(1, lowStock);
            Assert.Equal(2, expired);
        }

        [Fact]
        public void GetStatusCounts_NullList_ShouldReturnAllZero()
        {
            // Act
            StateMachine.GetStatusCounts(null, out var available, out var lowStock, out var expired);

            // Assert
            Assert.Equal(0, available);
            Assert.Equal(0, lowStock);
            Assert.Equal(0, expired);
        }

        [Fact]
        public void FormatTitleWithStats_NullList_ShouldReturnBaseTitle()
        {
            // Act
            var title = StateMachine.FormatTitleWithStats("Test", null);

            // Assert
            Assert.Equal("Test", title);
        }

        [Fact]
        public void FormatTitleWithStats_ValidList_ShouldIncludeStats()
        {
            // Arrange
            var list = new List<Obat>
            {
                new Obat("Obat 1", 20, 5000, DateTime.Now.AddYears(1)),
                new Obat("Obat 2", 3, 5000, DateTime.Now.AddYears(1)),
                new Obat("Obat 3", 5, 5000, DateTime.Now.AddDays(-1))
            };
            foreach (var o in list) StateMachine.EvaluateStatus(o);

            // Act
            var title = StateMachine.FormatTitleWithStats("Daftar Obat", list);

            // Assert
            Assert.Contains("🟢 1", title);
            Assert.Contains("🟡 1", title);
            Assert.Contains("🔴 1", title);
        }

        [Fact]
        public void FormatTitleWithStats_BaseTitleNull_ShouldReturnEmptyString()
        {
            // Act
            var title = StateMachine.FormatTitleWithStats(null, new List<Obat>());

            // Assert
            Assert.Equal(" - 🟢 0 | 🟡 0 | 🔴 0", title);
        }
    }
}
