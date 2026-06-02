using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using ObatAPI.Models;

namespace ObatAPI.Data
{
    /// <summary>
    /// ObatDbContext - Entity Framework Core DbContext untuk MySQL database
    /// Mengelola semua data entity dan database operations
    /// Database: tubes_kpl (MySQL 8.4.8 di Laragon)
    /// </summary>
    public class ObatDbContext : DbContext
    {
        public ObatDbContext(DbContextOptions<ObatDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet untuk table Obat
        /// </summary>
        public DbSet<Obat> Obat { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================
            // Configure Obat entity
            // =====================================================
            modelBuilder.Entity<Obat>(entity =>
            {
                // Table name
                entity.ToTable("obat");

                // Primary Key
                entity.HasKey(e => e.Id);

                // Column configurations
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .IsRequired();

                entity.Property(e => e.Nama)
                    .HasColumnName("nama")
                    .HasColumnType("VARCHAR(255)")
                    .IsRequired();

                entity.Property(e => e.Kategori)
                    .HasColumnName("kategori")
                    .HasColumnType("VARCHAR(100)")
                    .HasDefaultValue("Tablet");

                entity.Property(e => e.Stok)
                    .HasColumnName("stok")
                    .HasDefaultValue(0)
                    .IsRequired();

                entity.Property(e => e.Harga)
                    .HasColumnName("harga")
                    .HasPrecision(10, 2)
                    .HasDefaultValue(0.00M)
                    .IsRequired();

                entity.Property(e => e.ExpiredDate)
                    .HasColumnName("expired_date")
                    .HasColumnType("DATETIME")
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("VARCHAR(50)")
                    .HasDefaultValue("Available");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("TIMESTAMP")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("TIMESTAMP")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                // Indexes untuk performance
                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("idx_status");

                entity.HasIndex(e => e.ExpiredDate)
                    .HasDatabaseName("idx_expired_date");

                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("idx_created_at");
            });
        }

        /// <summary>
        /// Override SaveChanges untuk logging
        /// </summary>
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is Obat obat)
                {
                    var state = entry.State;
                    Console.WriteLine($"[DB] Obat '{obat.Nama}' - State: {state}");
                }
            }

            return base.SaveChanges();
        }

        /// <summary>
        /// Override SaveChangesAsync untuk logging
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is Obat obat)
                {
                    var state = entry.State;
                    Console.WriteLine($"[DB] Obat '{obat.Nama}' - State: {state}");
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
