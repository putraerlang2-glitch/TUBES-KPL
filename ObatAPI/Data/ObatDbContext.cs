using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using ObatAPI.Models;

namespace ObatAPI.Data
{
    public class ObatDbContext : DbContext
    {
        private readonly ILogger<ObatDbContext> _logger;

        public ObatDbContext(DbContextOptions<ObatDbContext> options, ILogger<ObatDbContext> logger = null!) : base(options)
        {
            _logger = logger;
        }

        public DbSet<Obat> Obat { get; set; } = null!;
        public DbSet<Transaksi> Transaksi { get; set; } = null!;
        public DbSet<TransaksiDetail> TransaksiDetail { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Obat>(entity =>
            {
                entity.ToTable("obat");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").IsRequired();
                entity.Property(e => e.Nama).HasColumnName("nama").HasColumnType("VARCHAR(255)").IsRequired();
                entity.Property(e => e.Kategori).HasColumnName("kategori").HasColumnType("VARCHAR(100)").HasDefaultValue("Tablet");
                entity.Property(e => e.Stok).HasColumnName("stok").HasDefaultValue(0).IsRequired();
                entity.Property(e => e.Harga).HasColumnName("harga").HasPrecision(10, 2).HasDefaultValue(0.00M).IsRequired();
                entity.Property(e => e.ExpiredDate).HasColumnName("expired_date").HasColumnType("DATETIME").IsRequired();
                entity.Property(e => e.Status).HasColumnName("status").HasColumnType("VARCHAR(50)").HasDefaultValue("Available");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

                entity.HasIndex(e => e.Status).HasDatabaseName("idx_status");
                entity.HasIndex(e => e.ExpiredDate).HasDatabaseName("idx_expired_date");
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("idx_created_at");
            });

            modelBuilder.Entity<Transaksi>(entity =>
            {
                entity.ToTable("transaksi");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.NoStruk).HasColumnName("no_struk").HasColumnType("VARCHAR(50)");
                entity.Property(e => e.TanggalTransaksi).HasColumnName("tanggal_transaksi").HasColumnType("DATETIME");
                
                entity.Property(e => e.Subtotal).HasColumnName("subtotal").HasPrecision(12, 2);
                entity.Property(e => e.PersentaseDiskon).HasColumnName("persentase_diskon").HasPrecision(5, 2);
                entity.Property(e => e.NominalDiskon).HasColumnName("nominal_diskon").HasPrecision(12, 2);
                entity.Property(e => e.PersentasePajak).HasColumnName("persentase_pajak").HasPrecision(5, 2);
                entity.Property(e => e.NominalPajak).HasColumnName("nominal_pajak").HasPrecision(12, 2);
                entity.Property(e => e.TotalAkhir).HasColumnName("total_akhir").HasPrecision(12, 2);
                
                entity.Property(e => e.UangBayar).HasColumnName("uang_bayar").HasPrecision(12, 2);
                entity.Property(e => e.UangKembalian).HasColumnName("uang_kembalian").HasPrecision(12, 2);
                
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TransaksiDetail>(entity =>
            {
                entity.ToTable("transaksi_detail");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TransaksiId).HasColumnName("transaksi_id");
                entity.Property(e => e.ObatId).HasColumnName("obat_id");
                
                entity.Property(e => e.Jumlah).HasColumnName("jumlah");
                entity.Property(e => e.HargaSatuan).HasColumnName("harga_satuan").HasPrecision(10, 2);
                entity.Property(e => e.Subtotal).HasColumnName("subtotal").HasPrecision(12, 2);
                
                entity.HasOne(d => d.Transaksi)
                      .WithMany(p => p.DetailList)
                      .HasForeignKey(d => d.TransaksiId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(d => d.Obat)
                      .WithMany()
                      .HasForeignKey(d => d.ObatId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is Obat obat)
                    _logger?.LogInformation("[DB] Obat '{ObatNama}' - State: {EntityState}", obat.Nama, entry.State);
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is Obat obat)
                    _logger?.LogInformation("[DB] Obat '{ObatNama}' - State: {EntityState}", obat.Nama, entry.State);
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
