
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TubesKPL
{
    public partial class FormHistory : Form
    {
        public FormHistory()
        {
            InitializeComponent();
            
            // Sembunyikan komponen search/filter sesuai permintaan
            HideSearchControls();
            
            // Load activity history
            LoadActivityHistory();
            
            ActivityHistoryService.LogActivity("VIEW_HISTORY", "Membuka halaman riwayat aktivitas");
        }

        private void HideSearchControls()
        {
            lblNoStruk.Visible = false;
            txtNoStruk.Visible = false;
            lblNamaObat.Visible = false;
            txtNamaObat.Visible = false;
            lblNamaKasir.Visible = false;
            txtNamaKasir.Visible = false;
            lblTanggalFrom.Visible = false;
            dtpTanggalFrom.Visible = false;
            lblTanggalTo.Visible = false;
            dtpTanggalTo.Visible = false;
            btnCari.Visible = false;
            
            // Atur posisi tblHistory ke atas (karena search/filter disembunyikan)
            tblHistory.Location = new System.Drawing.Point(12, 12);
            tblHistory.Size = new System.Drawing.Size(1160, 380);
        }

        private void LoadActivityHistory()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    
                    // Cek apakah kolom batch_id ada di activity_history
                    bool hasBatchId = false;
                    string checkBatchIdQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_SCHEMA = DATABASE() 
                        AND TABLE_NAME = 'activity_history' 
                        AND COLUMN_NAME = 'batch_id'";
                    using (var checkCmd = new MySqlCommand(checkBatchIdQuery, conn))
                    {
                        hasBatchId = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                    }
                    
                    // Query sesuai apakah batch_id ada atau tidak
                    string query = hasBatchId
                        ? @"
                            SELECT 
                                ah.activity_id AS 'ID History',
                                ah.created_at AS 'Waktu',
                                ah.activity_type AS 'Aktivitas',
                                ah.activity_description AS 'Deskripsi',
                                ah.user_id AS 'User ID',
                                u.nama AS 'Nama User',
                                ah.obat_id AS 'Obat ID',
                                o.nama AS 'Nama Obat',
                                o.kategori AS 'Kategori Obat',
                                ah.batch_id AS 'Batch ID',
                                b.no_batch AS 'No Batch',
                                b.expired_date AS 'Tgl Kadaluarsa Batch',
                                b.jumlah_masuk AS 'Jumlah Masuk Batch',
                                ah.transaksi_id AS 'Transaksi ID',
                                t.no_struk AS 'No Struk',
                                t.total_akhir AS 'Total Transaksi'
                            FROM activity_history ah
                            LEFT JOIN user u ON ah.user_id = u.user_id
                            LEFT JOIN obat o ON ah.obat_id = o.obat_id
                            LEFT JOIN batch_input b ON ah.batch_id = b.batch_id
                            LEFT JOIN transaksi t ON ah.transaksi_id = t.transaksi_id
                            ORDER BY ah.created_at DESC"
                        : @"
                            SELECT 
                                ah.activity_id AS 'ID History',
                                ah.created_at AS 'Waktu',
                                ah.activity_type AS 'Aktivitas',
                                ah.activity_description AS 'Deskripsi',
                                ah.user_id AS 'User ID',
                                u.nama AS 'Nama User',
                                ah.obat_id AS 'Obat ID',
                                o.nama AS 'Nama Obat',
                                o.kategori AS 'Kategori Obat',
                                ah.transaksi_id AS 'Transaksi ID',
                                t.no_struk AS 'No Struk',
                                t.total_akhir AS 'Total Transaksi'
                            FROM activity_history ah
                            LEFT JOIN user u ON ah.user_id = u.user_id
                            LEFT JOIN obat o ON ah.obat_id = o.obat_id
                            LEFT JOIN transaksi t ON ah.transaksi_id = t.transaksi_id
                            ORDER BY ah.created_at DESC";

                    using (var adapter = new MySqlDataAdapter(query, conn))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        tblHistory.DataSource = dataTable;
                        // Pastikan kolom menyesuaikan konten
                        tblHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        // Set kolom hanya baca
                        tblHistory.ReadOnly = true;
                        // Tidak bisa menambah baris baru
                        tblHistory.AllowUserToAddRows = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadActivityHistory();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Dummy method to keep Designer happy
        private void btnCari_Click(object sender, EventArgs e)
        {
            // Tidak digunakan
        }
    }
}
