
using System;
using System.Collections.Generic;
using Xunit;

namespace TubesKPL.Test
{
    public class ObatApiServiceTests
    {
        public ObatApiServiceTests()
        {
            // Reset before each test
            ObatApiService.Reset();
        }

        [Fact]
        public void Initialize_NullSource_ShouldNotThrow()
        {
            // Act
            var exception = Record.Exception(() => ObatApiService.Initialize(null));

            // Assert
            Assert.Null(exception);
            Assert.True(ObatApiService.IsInitialized());
        }

        [Fact]
        public void Initialize_SourceWithNullItems_ShouldIgnoreNullItems()
        {
            // Arrange
            var source = new List<Obat>
            {
                new Obat("Paracetamol", 10, 5000, DateTime.Now.AddYears(1)),
                null,
                new Obat("Amoxicillin", 5, 10000, DateTime.Now.AddYears(1))
            };

            // Act
            ObatApiService.Initialize(source);
            var all = ObatApiService.GetAll();

            // Assert
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public void GetAll_ShouldCallUpdateStatus()
        {
            // Arrange
            var list = new List<Obat>
            {
                new Obat("Paracetamol", 10, 5000, DateTime.Now.AddYears(1)),
                new Obat("Obat Expired", 20, 5000, DateTime.Now.AddDays(-1))
            };
            ObatApiService.Initialize(list);

            // Act
            var result = ObatApiService.GetAll();

            // Assert
            Assert.Equal(StateMachine.STATUS_AVAILABLE, result[0].Status);
            Assert.Equal(StateMachine.STATUS_EXPIRED, result[1].Status);
        }

        [Fact]
        public void GetStatusSummary_ValidList_ShouldCountCorrectly()
        {
            // Arrange
            var list = new List<Obat>
            {
                new Obat("Obat 1", 20, 5000, DateTime.Now.AddYears(1)),
                new Obat("Obat 2", 3, 5000, DateTime.Now.AddYears(1)),
                new Obat("Obat 3", 5, 5000, DateTime.Now.AddDays(-1))
            };
            ObatApiService.Initialize(list);

            // Act
            ObatApiService.GetStatusSummary(out var available, out var lowStock, out var expired);

            // Assert
            Assert.Equal(1, available);
            Assert.Equal(1, lowStock);
            Assert.Equal(1, expired);
        }

        [Fact]
        public void Reset_ShouldSetInitializedFalse()
        {
            // Arrange
            ObatApiService.Initialize(new List<Obat>
            {
                new Obat("Paracetamol", 10, 5000, DateTime.Now.AddYears(1))
            });

            // Act
            ObatApiService.Reset();

            // Assert
            Assert.False(ObatApiService.IsInitialized());
            Assert.Empty(ObatApiService.GetAll());
        }
    }
}
