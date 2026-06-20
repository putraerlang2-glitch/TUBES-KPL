
-- ============================================
-- DATABASE UPGRADE SCRIPT FOR HISTORY FEATURE
-- ============================================
-- This script adds:
-- 1. activity_history table
-- 2. vw_history_transaksi view
-- ============================================

-- 1. Create activity_history table
CREATE TABLE IF NOT EXISTS activity_history (
    activity_id INT PRIMARY KEY AUTO_INCREMENT COMMENT 'ID unik aktivitas',
    user_id INT NULL COMMENT 'ID user yang melakukan aktivitas',
    transaksi_id INT NULL COMMENT 'ID transaksi terkait (opsional)',
    obat_id INT NULL COMMENT 'ID obat terkait (opsional)',
    activity_type VARCHAR(50) NOT NULL COMMENT 'Jenis aktivitas (VIEW_HISTORY, SEARCH_HISTORY, dll)',
    activity_description TEXT NULL COMMENT 'Deskripsi aktivitas',
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Waktu aktivitas',
    
    FOREIGN KEY (user_id) REFERENCES user(user_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (transaksi_id) REFERENCES transaksi(transaksi_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (obat_id) REFERENCES obat(obat_id) ON DELETE SET NULL ON UPDATE CASCADE,
    
    INDEX idx_activity_user (user_id),
    INDEX idx_activity_transaksi (transaksi_id),
    INDEX idx_activity_obat (obat_id),
    INDEX idx_activity_type (activity_type),
    INDEX idx_activity_created (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Tabel log aktivitas sistem';

-- 2. Create vw_history_transaksi view
CREATE OR REPLACE VIEW vw_history_transaksi AS
SELECT
    t.transaksi_id,
    t.no_struk,
    t.tanggal_transaksi,
    u.user_id,
    u.nama AS nama_kasir,
    o.obat_id,
    o.nama AS nama_obat,
    o.kategori AS kategori_obat,
    td.jumlah,
    td.harga_satuan,
    td.subtotal AS subtotal_item,
    t.subtotal AS subtotal_transaksi,
    t.persentase_diskon,
    t.nominal_diskon,
    t.persentase_pajak,
    t.nominal_pajak,
    t.total_akhir,
    t.uang_bayar,
    t.uang_kembalian
FROM transaksi t
INNER JOIN user u ON t.user_id = u.user_id
INNER JOIN transaksi_detail td ON t.transaksi_id = td.transaksi_id
INNER JOIN obat o ON td.obat_id = o.obat_id
ORDER BY t.tanggal_transaksi DESC, t.transaksi_id DESC;
