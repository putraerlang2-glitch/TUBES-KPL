-- =====================================================
-- MySQL Database Script untuk TUBES-KPL Application
-- Database: tubes_kpl
-- MySQL Version: 8.4.8 (Laragon)
-- Created for: Apotek Management System with StateMachine
-- =====================================================

-- Drop existing database (if needed)
DROP DATABASE IF EXISTS tubes_kpl;

-- Create database
CREATE DATABASE tubes_kpl 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE tubes_kpl;

-- =====================================================
-- Table: obat (Obat/Medicine)
-- =====================================================
CREATE TABLE obat (
	id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Unique identifier for obat',
	nama VARCHAR(255) NOT NULL COMMENT 'Nama obat',
	kategori VARCHAR(100) DEFAULT 'Tablet' COMMENT 'Kategori obat (Tablet, Sirup, Kapsul, dll)',
	stok INT NOT NULL DEFAULT 0 COMMENT 'Jumlah stok obat',
	harga DECIMAL(10, 2) NOT NULL DEFAULT 0.00 COMMENT 'Harga per unit',
	expired_date DATETIME NOT NULL COMMENT 'Tanggal kadaluarsa obat',
	status VARCHAR(50) DEFAULT 'Available' COMMENT 'Status obat (Available, LowStock, Expired)',
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Waktu dibuat',
	updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Waktu terakhir diupdate',

	-- Indexes untuk performance
	INDEX idx_status (status) COMMENT 'Index untuk query by status',
	INDEX idx_expired_date (expired_date) COMMENT 'Index untuk query by expired date',
	INDEX idx_created_at (created_at) COMMENT 'Index untuk query by created date'
) ENGINE=InnoDB 
DEFAULT CHARSET=utf8mb4 
COLLATE utf8mb4_unicode_ci
COMMENT='Table untuk menyimpan data obat dengan status management';

-- =====================================================
-- Sample Data untuk Testing
-- =====================================================
INSERT INTO obat (nama, kategori, stok, harga, expired_date, status) VALUES
('Paracetamol', 'Tablet', 100, 5000.00, '2025-12-31 23:59:59', 'Available'),
('Ibuprofen', 'Tablet', 12, 7000.00, '2024-07-20 23:59:59', 'Available'),
('Sanmol', 'Sirup', 15, 3000.00, '2025-04-05 23:59:59', 'Available'),
('HRIG', 'Injeksi', 12, 20000.00, '2024-03-10 23:59:59', 'Expired'),
('Amoxicillin', 'Tablet', 5, 10000.00, '2025-06-30 23:59:59', 'LowStock'),
('Vitamin C', 'Tablet', 0, 500.00, '2026-08-01 23:59:59', 'Available');

-- =====================================================
-- Verify Data
-- =====================================================
SELECT COUNT(*) as total_obat FROM obat;
SELECT * FROM obat ORDER BY id;

-- =====================================================
-- Optional: Trigger untuk auto-update timestamp
-- =====================================================
-- (InnoDB sudah support ON UPDATE CURRENT_TIMESTAMP, jadi trigger tidak perlu)

-- =====================================================
-- End of Script
-- =====================================================
