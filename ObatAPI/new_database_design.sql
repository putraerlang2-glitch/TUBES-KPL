-- ========================================
-- DATABASE: tubes_kpl
-- DESAIN BARU DENGAN RELASI JELAS
-- ========================================

-- Hapus tabel jika sudah ada (untuk clean start)
DROP TABLE IF EXISTS `transaksi_detail`;
DROP TABLE IF EXISTS `transaksi`;
DROP TABLE IF EXISTS `obat`;
DROP TABLE IF EXISTS `user`;

-- ========================================
-- 1. TABEL USER (PENGGUNA APLIKASI)
-- PK: user_id
-- ========================================
CREATE TABLE `user` (
    `user_id` INT PRIMARY KEY AUTO_INCREMENT COMMENT 'ID unik pengguna',
    `username` VARCHAR(50) UNIQUE NOT NULL COMMENT 'Username login (unik)',
    `nama` VARCHAR(100) NOT NULL COMMENT 'Nama lengkap pengguna',
    `password_hash` VARCHAR(255) NOT NULL COMMENT 'Hash password (tidak plaintext)',
    `role` ENUM('Admin', 'Apoteker', 'Kasir') NOT NULL DEFAULT 'Kasir' COMMENT 'Peran pengguna',
    `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Waktu akun dibuat'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Tabel data pengguna';

-- ========================================
-- 2. TABEL OBAT (DATA OBAT)
-- PK: obat_id
-- ========================================
CREATE TABLE `obat` (
    `obat_id` INT PRIMARY KEY AUTO_INCREMENT COMMENT 'ID unik obat',
    `nama` VARCHAR(255) NOT NULL COMMENT 'Nama obat',
    `kategori` VARCHAR(100) DEFAULT 'Tablet' COMMENT 'Kategori obat (Tablet, Sirup, Kapsul, dll)',
    `stok` INT NOT NULL DEFAULT 0 COMMENT 'Jumlah stok tersedia',
    `harga` DECIMAL(10,2) NOT NULL DEFAULT 0.00 COMMENT 'Harga jual per unit',
    `expired_date` DATETIME NOT NULL COMMENT 'Tanggal kadaluarsa',
    `status` VARCHAR(50) NOT NULL DEFAULT 'Available' COMMENT 'Status obat',
    `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Waktu data obat dibuat',
    `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Waktu data obat terakhir diupdate'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Tabel data obat';

-- Index untuk pencarian cepat
CREATE INDEX `idx_obat_status` ON `obat`(`status`);
CREATE INDEX `idx_obat_expired` ON `obat`(`expired_date`);

-- ========================================
-- 3. TABEL TRANSAKSI (DATA PENJUALAN)
-- PK: transaksi_id
-- FK: user_id → user.user_id
-- ========================================
CREATE TABLE `transaksi` (
    `transaksi_id` INT PRIMARY KEY AUTO_INCREMENT COMMENT 'ID unik transaksi',
    `no_struk` VARCHAR(50) UNIQUE NOT NULL COMMENT 'Nomor struk (unik)',
    `tanggal_transaksi` DATETIME NOT NULL COMMENT 'Tanggal dan waktu transaksi',
    `subtotal` DECIMAL(12,2) NOT NULL DEFAULT 0.00 COMMENT 'Total sebelum diskon dan pajak',
    `persentase_diskon` DECIMAL(5,2) NOT NULL DEFAULT 0.00 COMMENT 'Persentase diskon (%)',
    `nominal_diskon` DECIMAL(12,2) NOT NULL DEFAULT 0.00 COMMENT 'Nominal diskon (Rp)',
    `persentase_pajak` DECIMAL(5,2) NOT NULL DEFAULT 0.00 COMMENT 'Persentase pajak (%)',
    `nominal_pajak` DECIMAL(12,2) NOT NULL DEFAULT 0.00 COMMENT 'Nominal pajak (Rp)',
    `total_akhir` DECIMAL(12,2) NOT NULL DEFAULT 0.00 COMMENT 'Total akhir yang harus dibayar',
    `uang_bayar` DECIMAL(12,2) NOT NULL DEFAULT 0.00 COMMENT 'Uang yang dibayarkan customer',
    `uang_kembalian` DECIMAL(12,2) NOT NULL DEFAULT 0.00 COMMENT 'Uang kembalian',
    `user_id` INT NOT NULL COMMENT 'ID pengguna yang melakukan transaksi',
    `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Waktu transaksi dibuat',
    FOREIGN KEY (`user_id`) REFERENCES `user`(`user_id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Tabel data transaksi penjualan';

-- Index untuk pencarian cepat
CREATE INDEX `idx_transaksi_no_struk` ON `transaksi`(`no_struk`);
CREATE INDEX `idx_transaksi_tanggal` ON `transaksi`(`tanggal_transaksi`);
CREATE INDEX `idx_transaksi_user` ON `transaksi`(`user_id`);

-- ========================================
-- 4. TABEL TRANSAKSI DETAIL (ITEM PER TRANSAKSI)
-- PK: transaksi_detail_id
-- FK: transaksi_id → transaksi.transaksi_id
-- FK: obat_id → obat.obat_id
-- ========================================
CREATE TABLE `transaksi_detail` (
    `transaksi_detail_id` INT PRIMARY KEY AUTO_INCREMENT COMMENT 'ID unik detail transaksi',
    `transaksi_id` INT NOT NULL COMMENT 'ID transaksi',
    `obat_id` INT NOT NULL COMMENT 'ID obat yang dibeli',
    `jumlah` INT NOT NULL COMMENT 'Jumlah obat yang dibeli',
    `harga_satuan` DECIMAL(18,2) NOT NULL COMMENT 'Harga satuan obat saat transaksi',
    `subtotal` DECIMAL(18,2) NOT NULL COMMENT 'Subtotal item (jumlah × harga_satuan',
    FOREIGN KEY (`transaksi_id`) REFERENCES `transaksi`(`transaksi_id`) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (`obat_id`) REFERENCES `obat`(`obat_id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Tabel detail item per transaksi';

-- Index untuk pencarian cepat
CREATE INDEX `idx_transaksi_detail_transaksi` ON `transaksi_detail`(`transaksi_id`);
CREATE INDEX `idx_transaksi_detail_obat` ON `transaksi_detail`(`obat_id`);
