-- Create obat table
CREATE TABLE IF NOT EXISTS `obat` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nama` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `kategori` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT 'Tablet',
  `stok` int NOT NULL DEFAULT 0,
  `harga` decimal(10,2) NOT NULL DEFAULT 0.00,
  `expired_date` datetime NOT NULL,
  `status` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT 'Available',
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_status` (`status`),
  KEY `idx_expired_date` (`expired_date`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create transaksi table
CREATE TABLE IF NOT EXISTS `transaksi` (
  `id` int NOT NULL AUTO_INCREMENT,
  `no_struk` varchar(50) COLLATE utf8mb4_unicode_ci,
  `tanggal_transaksi` datetime(6) NOT NULL,
  `subtotal` decimal(10,2) NOT NULL,
  `persentase_diskon` decimal(5,2) NOT NULL,
  `nominal_diskon` decimal(10,2) NOT NULL,
  `persentase_pajak` decimal(5,2) NOT NULL,
  `nominal_pajak` decimal(10,2) NOT NULL,
  `total_akhir` decimal(10,2) NOT NULL,
  `uang_bayar` decimal(10,2) NOT NULL,
  `uang_kembalian` decimal(10,2) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create transaksi_detail table
CREATE TABLE IF NOT EXISTS `transaksi_detail` (
  `id` int NOT NULL AUTO_INCREMENT,
  `transaksi_id` int NOT NULL,
  `obat_id` int NOT NULL,
  `jumlah` int NOT NULL,
  `harga_satuan` decimal(10,2) NOT NULL,
  `subtotal` decimal(12,2) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `IX_transaksi_detail_transaksi_id` (`transaksi_id`),
  KEY `IX_transaksi_detail_obat_id` (`obat_id`),
  CONSTRAINT `FK_transaksi_detail_transaksi_transaksi_id` FOREIGN KEY (`transaksi_id`) REFERENCES `transaksi` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_transaksi_detail_obat_obat_id` FOREIGN KEY (`obat_id`) REFERENCES `obat` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create __EFMigrationsHistory table (untuk tracking migrations)
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
  `MigrationId` nvarchar(150) NOT NULL,
  `ProductVersion` nvarchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insert migration history
INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240115000000_InitialCreate', '6.0.28');
