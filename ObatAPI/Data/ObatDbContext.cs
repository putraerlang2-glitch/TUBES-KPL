using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ObatAPI.Models;

namespace ObatAPI.Data
{
    public class ObatDbContext : DbContext
    {
        public DbSet<Obat> Obat { get; set; } = null!;
        public DbSet<Transaksi> Transaksi { get; set; } = null!;
        public DbSet<TransaksiDetail> TransaksiDetail { get; set; } = null!;
        public DbSet<User> User { get; set; } = null!;

        public ObatDbContext(DbContextOptions<ObatDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Converter untuk ObatStatus (handle spasi di SQL)
            var obatStatusConverter = new ValueConverter<ObatStatus, string>(
                v => v == ObatStatus.LowStock ? "Low Stock" : v == ObatStatus.OutOfStock ? "Out of Stock" : v.ToString(),
                v => v == "Low Stock" ? ObatStatus.LowStock : v == "Out of Stock" ? ObatStatus.OutOfStock : Enum.Parse<ObatStatus>(v.Replace(" ", ""))
            );

            // --- TABEL USER ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired().ValueGeneratedOnAdd();
                entity.Property(e => e.Username).HasColumnName("username").HasColumnType("VARCHAR(50)").IsRequired();
                entity.Property(e => e.Nama).HasColumnName("nama").HasColumnType("VARCHAR(100)").IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasColumnType("VARCHAR(255)").IsRequired();
                entity.Property(e => e.Role).HasColumnName("role").HasColumnType("VARCHAR(50)").IsRequired().HasDefaultValue("Kasir");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

                entity.HasIndex(e => e.Username).IsUnique().HasDatabaseName("username");
            });

            // --- TABEL OBAT ---
            modelBuilder.Entity<Obat>(entity =>
            {
                entity.ToTable("obat");
                entity.HasKey(e => e.ObatId);

                entity.Property(e => e.ObatId).HasColumnName("obat_id").ValueGeneratedOnAdd();
                entity.Property(e => e.Nama).HasColumnName("nama").HasColumnType("VARCHAR(255)").IsRequired();
                entity.Property(e => e.Kategori).HasColumnName("kategori").HasColumnType("VARCHAR(100)").HasDefaultValue("Tablet");
                entity.Property(e => e.Stok).HasColumnName("stok").HasDefaultValue(0);
                entity.Property(e => e.Harga).HasColumnName("harga").HasPrecision(18, 2).HasDefaultValue(0.00);
                entity.Property(e => e.ExpiredDate).HasColumnName("expired_date").HasColumnType("DATETIME").IsRequired();
                entity.Property(e => e.Status)
                      .HasColumnName("status")
                      .HasConversion(obatStatusConverter)
                      .HasDefaultValue(ObatStatus.Available);

                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

                entity.HasIndex(e => e.Status).HasDatabaseName("idx_obat_status");
                entity.HasIndex(e => e.ExpiredDate).HasDatabaseName("idx_obat_expired");
            });

            // --- TABEL TRANSAKSI ---
            modelBuilder.Entity<Transaksi>(entity =>
            {
                entity.ToTable("transaksi");
                entity.HasKey(e => e.TransaksiId);

                entity.Property(e => e.TransaksiId).HasColumnName("transaksi_id").ValueGeneratedOnAdd();
                entity.Property(e => e.NoStruk).HasColumnName("no_struk").HasColumnType("VARCHAR(50)").IsRequired();
                entity.Property(e => e.TanggalTransaksi).HasColumnName("tanggal_transaksi").HasColumnType("DATETIME").IsRequired();

                entity.Property(e => e.Subtotal).HasColumnName("subtotal").HasPrecision(18, 2).HasDefaultValue(0.00);
                entity.Property(e => e.PersentaseDiskon).HasColumnName("persentase_diskon").HasPrecision(5, 2).HasDefaultValue(0.00);
                entity.Property(e => e.NominalDiskon).HasColumnName("nominal_diskon").HasPrecision(18, 2).HasDefaultValue(0.00);
                entity.Property(e => e.PersentasePajak).HasColumnName("persentase_pajak").HasPrecision(5, 2).HasDefaultValue(0.00);
                entity.Property(e => e.NominalPajak).HasColumnName("nominal_pajak").HasPrecision(18, 2).HasDefaultValue(0.00);
                entity.Property(e => e.TotalAkhir).HasColumnName("total_akhir").HasPrecision(18, 2).HasDefaultValue(0.00);
                entity.Property(e => e.UangBayar).HasColumnName("uang_bayar").HasPrecision(18, 2).HasDefaultValue(0.00);
                entity.Property(e => e.UangKembalian).HasColumnName("uang_kembalian").HasPrecision(18, 2).HasDefaultValue(0.00);

                entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

                // Relasi User → Transaksi
                entity.HasOne(t => t.User)
                      .WithMany(u => u.TransaksiList)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("fk_transaksi_user");

                entity.HasIndex(e => e.NoStruk).IsUnique().HasDatabaseName("no_struk");
                entity.HasIndex(e => e.TanggalTransaksi).HasDatabaseName("idx_transaksi_tanggal");
                entity.HasIndex(e => e.UserId).HasDatabaseName("idx_transaksi_user");
            });

            // --- TABEL TRANSAKSI DETAIL ---
            modelBuilder.Entity<TransaksiDetail>(entity =>
            {
                entity.ToTable("transaksi_detail");
                entity.HasKey(e => e.TransaksiDetailId);

                entity.Property(e => e.TransaksiDetailId).HasColumnName("transaksi_detail_id").ValueGeneratedOnAdd();
                entity.Property(e => e.TransaksiId).HasColumnName("transaksi_id").IsRequired();
                entity.Property(e => e.ObatId).HasColumnName("obat_id").IsRequired();

                entity.Property(e => e.Jumlah).HasColumnName("jumlah").IsRequired();
                entity.Property(e => e.HargaSatuan).HasColumnName("harga_satuan").HasPrecision(18, 2).IsRequired();
                entity.Property(e => e.Subtotal).HasColumnName("subtotal").HasPrecision(18, 2).IsRequired();

                // Relasi Transaksi → TransaksiDetail
                entity.HasOne(d => d.Transaksi)
                      .WithMany(p => p.DetailList)
                      .HasForeignKey(d => d.TransaksiId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("fk_transaksi_detail_transaksi");

                // Relasi Obat → TransaksiDetail
                entity.HasOne(d => d.Obat)
                      .WithMany()
                      .HasForeignKey(d => d.ObatId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("fk_transaksi_detail_obat");

                entity.HasIndex(e => e.TransaksiId).HasDatabaseName("idx_transaksi_detail_transaksi");
                entity.HasIndex(e => e.ObatId).HasDatabaseName("idx_transaksi_detail_obat");
            });
        }
    }
}
