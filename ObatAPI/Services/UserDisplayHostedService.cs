using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace ObatAPI.Services
{
    /// <summary>
    /// Hosted Service untuk menampilkan daftar akun dummy secara berkala (setiap 10 menit)
    /// </summary>
    public class UserDisplayHostedService : BackgroundService
    {
        // Data dummy user (sama dengan DatabaseSeeder)
        private static readonly (string Username, string Nama, string Role)[] DummyUsers = new[]
        {
            ("admin", "Admin Utama", "Admin"),
            ("apoteker1", "Apoteker Satu", "Apoteker"),
            ("kasir1", "Kasir Satu", "Kasir"),
            ("kasir2", "Kasir Dua", "Kasir")
        };

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Tunggu 5 detik dulu agar startup selesai
            await Task.Delay(5000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                TampilkanDaftarAkun();

                // Tunggu 10 menit sebelum menampilkan lagi
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private void TampilkanDaftarAkun()
        {
            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("        📋 DAFTAR AKUN DUMMY           ");
            Console.WriteLine("========================================");

            foreach (var (username, nama, role) in DummyUsers)
            {
                Console.WriteLine($" • Username : {username}");
                Console.WriteLine($"   Password : {username}_123");
                Console.WriteLine($"   Nama     : {nama}");
                Console.WriteLine($"   Role     : {role}");
                Console.WriteLine("----------------------------------------");
            }

            Console.WriteLine("📌 Catat: Password selalu = [username]_123");
            Console.WriteLine("========================================");
            Console.WriteLine();
        }
    }
}
