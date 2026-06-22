
using System;
using System.Threading.Tasks;
using Xunit;

namespace TubesKPL.Test
{
    public class ObatApiClientTests
    {
        [Fact]
        public void Constructor_EmptyBaseUrl_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ObatApiClient(""));
        }

        [Fact]
        public async Task GetObatByIdAsync_InvalidId_ShouldThrowArgumentException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(() => client.GetObatByIdAsync(0));
            }
        }

        [Fact]
        public async Task AddObatAsync_NullObat_ShouldThrowArgumentNullException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                // Act & Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => client.AddObatAsync(null));
            }
        }

        [Fact]
        public async Task UpdateObatAsync_InvalidId_ShouldThrowArgumentException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                var obat = new Obat("Paracetamol", 10, 5000, DateTime.Now.AddYears(1));

                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateObatAsync(0, obat));
            }
        }

        [Fact]
        public async Task UpdateObatAsync_NullObat_ShouldThrowArgumentNullException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                // Act & Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => client.UpdateObatAsync(1, null));
            }
        }

        [Fact]
        public async Task DeleteObatAsync_InvalidId_ShouldThrowArgumentException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(() => client.DeleteObatAsync(0));
            }
        }

        [Fact]
        public async Task CheckoutTransaksiAsync_NullTransaction_ShouldThrowArgumentNullException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                // Act & Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => client.CheckoutTransaksiAsync(null));
            }
        }

        [Fact]
        public async Task CheckoutTransaksiAsync_NullDetailList_ShouldThrowArgumentException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                var transaksi = new TransaksiDTO
                {
                    NoStruk = "001",
                    TanggalTransaksi = DateTime.Now,
                    UserId = 1,
                    DetailList = null
                };

                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(() => client.CheckoutTransaksiAsync(transaksi));
            }
        }

        [Fact]
        public async Task CheckoutTransaksiAsync_EmptyDetailList_ShouldThrowArgumentException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                var transaksi = new TransaksiDTO
                {
                    NoStruk = "001",
                    TanggalTransaksi = DateTime.Now,
                    UserId = 1,
                    DetailList = new System.Collections.Generic.List<TransaksiDetailDTO>()
                };

                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(() => client.CheckoutTransaksiAsync(transaksi));
            }
        }

        [Fact]
        public async Task LoginAsync_EmptyUsername_ShouldThrowArgumentException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(() => client.LoginAsync("", "password"));
            }
        }

        [Fact]
        public async Task LoginAsync_EmptyPassword_ShouldThrowArgumentException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(() => client.LoginAsync("admin", ""));
            }
        }

        [Fact]
        public async Task RegisterAsync_EmptyUsername_ShouldThrowArgumentException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(() => client.RegisterAsync("", "password", "Test User"));
            }
        }

        [Fact]
        public async Task RegisterAsync_EmptyPassword_ShouldThrowArgumentException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(() => client.RegisterAsync("testuser", "", "Test User"));
            }
        }

        [Fact]
        public async Task RegisterAsync_EmptyNama_ShouldThrowArgumentException()
        {
            // Arrange
            using (var client = new ObatApiClient())
            {
                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(() => client.RegisterAsync("testuser", "password", ""));
            }
        }
    }
}
