
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
                    
                    // Dapatkan semua kolom dari activity_history
                    DataTable schema = new DataTable();
                    string getColumnsQuery = @"
                        SELECT COLUMN_NAME 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_SCHEMA = DATABASE() 
                        AND TABLE_NAME = 'activity_history'
                        ORDER BY ORDINAL_POSITION";

                    List<string> availableColumns = new List<string>();
                    using (var cmd = new MySqlCommand(getColumnsQuery, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            availableColumns.Add(reader.GetString(0));
                        }
                    }

                    // Bangun query SELECT dinamis berdasarkan kolom yang ada, dengan alias yang jelas
                    string selectColumns = string.Join(", ", availableColumns.Select(col => 
                    {
                        // Beri alias yang ramah pengguna
                        return col switch
                        {
                            "activity_id" => "activity_id AS 'ID'",
                            "created_at" => "created_at AS 'Waktu'",
                            "activity_type" => "activity_type AS 'Aktivitas'",
                            "activity_description" => "activity_description AS 'Deskripsi'",
                            "user_id" => "user_id AS 'User ID'",
                            "obat_id" => "obat_id AS 'Obat ID'",
                            "batch_id" => "batch_id AS 'Batch ID'",
                            "transaksi_id" => "transaksi_id AS 'Transaksi ID'",
                            _ => col
                        };
                    }));

                    string query = $@"
                        SELECT {selectColumns}
                        FROM activity_history 
                        ORDER BY created_at DESC";

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
