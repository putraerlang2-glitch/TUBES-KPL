
using System;
using System.Data;
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
                    
                    string query = @"
                        SELECT 
                            created_at AS 'Waktu',
                            activity_type AS 'Aktivitas',
                            activity_description AS 'Deskripsi',
                            user_id AS 'User ID',
                            obat_id AS 'Obat ID',
                            transaksi_id AS 'Transaksi ID'
                        FROM activity_history 
                        ORDER BY created_at DESC";

                    using (var adapter = new MySqlDataAdapter(query, conn))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        tblHistory.DataSource = table;
                        tblHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
