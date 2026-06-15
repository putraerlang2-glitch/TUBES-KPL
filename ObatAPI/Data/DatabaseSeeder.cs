using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ObatAPI.Helpers;
using ObatAPI.Models;

namespace ObatAPI.Data
{
    /// <summary>
    /// Seeder untuk membuat data dummy user otomatis ketika aplikasi berjalan
    /// Menerapkan: Clean Code, KISS, Secure Code (hash password otomatis)
    /// </summary>
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Data dummy user (username mudah diingat, password default [username]_123)
        /// </summary>
        private static readonly List<(string Username, string Nama, string Role)> DummyUsers = new()
        {
            ("admin", "Admin Utama", "Admin"),
            ("apoteker1", "Apoteker Satu", "Apoteker"),
            ("kasir1", "Kasir Satu", "Kasir"),
            ("kasir2", "Kasir Dua", "Kasir")
        };

        /// <summary>
        /// Jalankan seeding: cek user, jika belum ada insert dengan password hash!
        /// </summary>
        public static async Task SeedAsync(ObatDbContext context)
        {
            // Pastikan database dibuat (jika belum ada)
            await context.Database.EnsureCreatedAsync();

            Console.WriteLine("========================================");
            Console.WriteLine("        DATA DUMMY USER LOGIN          ");
            Console.WriteLine("========================================");

            foreach (var (username, nama, role) in DummyUsers)
            {
                // Cek apakah user dengan username ini sudah ada
                var existingUser = await context.User.FirstOrDefaultAsync(u => u.Username == username);

                if (existingUser == null)
                {
                    // Buat password default: [username]_123
                    string defaultPassword = $"{username}_123";

                    // Hash password dengan helper PasswordHasher (aman!)
                    string passwordHash = PasswordHasher.HashPassword(defaultPassword);

                    // Insert user baru ke database
                    var newUser = new User
                    {
                        Username = username,
                        Nama = nama,
                        PasswordHash = passwordHash,
                        Role = role,
                        CreatedAt = DateTime.Now
                    };

                    await context.User.AddAsync(newUser);

                    // Tampilkan di terminal!
                    Console.WriteLine($" [+] Username: {username}");
                    Console.WriteLine($"     Password: {defaultPassword}");
                    Console.WriteLine($"     Nama    : {nama}");
                    Console.WriteLine($"     Role    : {role}");
                    Console.WriteLine("----------------------------------------");
                }
                else
                {
                    // User sudah ada, tampilkan juga untuk refresh memory
                    Console.WriteLine($" [✓] Username: {username} (sudah ada di database)");
                    Console.WriteLine($"     Password: {username}_123");
                    Console.WriteLine($"     Nama    : {existingUser.Nama}");
                    Console.WriteLine($"     Role    : {existingUser.Role}");
                    Console.WriteLine("----------------------------------------");
                }
            }

            // Simpan perubahan ke database
            await context.SaveChangesAsync();

            Console.WriteLine("========================================");
            Console.WriteLine("   DATA DUMMY USER SIAP DIGUNAKAN!     ");
            Console.WriteLine("   CATAT: Password = [username]_123    ");
            Console.WriteLine("========================================");
            Console.WriteLine();
        }
    }
}
