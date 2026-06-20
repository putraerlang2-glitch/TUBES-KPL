
using System;
using System.Data;
using System.Windows.Forms;

namespace TubesKPL
{
    public partial class FormHistory : Form
    {
        public FormHistory()
        {
            InitializeComponent();
            LoadHistory();
            ActivityHistoryService.LogActivity("VIEW_HISTORY", "Membuka halaman riwayat transaksi");
        }

        private void LoadHistory()
        {
            try
            {
                DataTable dt = TransactionHistoryService.GetAllHistory();
                BindDataToGrid(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindDataToGrid(DataTable dt)
        {
            tblHistory.DataSource = dt;
            if (dt.Columns.Count > 0)
            {
                SetColumnHeaders();
            }
        }

        private void SetColumnHeaders()
        {
            if (tblHistory.Columns.Contains("no_struk"))
                tblHistory.Columns["no_struk"].HeaderText = "No Struk";
            if (tblHistory.Columns.Contains("tanggal_transaksi"))
                tblHistory.Columns["tanggal_transaksi"].HeaderText = "Tanggal Transaksi";
            if (tblHistory.Columns.Contains("nama_kasir"))
                tblHistory.Columns["nama_kasir"].HeaderText = "Nama Kasir";
            if (tblHistory.Columns.Contains("nama_obat"))
                tblHistory.Columns["nama_obat"].HeaderText = "Nama Obat";
            if (tblHistory.Columns.Contains("kategori_obat"))
                tblHistory.Columns["kategori_obat"].HeaderText = "Kategori Obat";
            if (tblHistory.Columns.Contains("jumlah"))
                tblHistory.Columns["jumlah"].HeaderText = "Jumlah";
            if (tblHistory.Columns.Contains("harga_satuan"))
                tblHistory.Columns["harga_satuan"].HeaderText = "Harga Satuan";
            if (tblHistory.Columns.Contains("subtotal_item"))
                tblHistory.Columns["subtotal_item"].HeaderText = "Subtotal Item";
            if (tblHistory.Columns.Contains("subtotal_transaksi"))
                tblHistory.Columns["subtotal_transaksi"].HeaderText = "Subtotal Transaksi";
            if (tblHistory.Columns.Contains("persentase_diskon"))
                tblHistory.Columns["persentase_diskon"].HeaderText = "Diskon (%)";
            if (tblHistory.Columns.Contains("nominal_diskon"))
                tblHistory.Columns["nominal_diskon"].HeaderText = "Nominal Diskon";
            if (tblHistory.Columns.Contains("persentase_pajak"))
                tblHistory.Columns["persentase_pajak"].HeaderText = "Pajak (%)";
            if (tblHistory.Columns.Contains("nominal_pajak"))
                tblHistory.Columns["nominal_pajak"].HeaderText = "Nominal Pajak";
            if (tblHistory.Columns.Contains("total_akhir"))
                tblHistory.Columns["total_akhir"].HeaderText = "Total Akhir";
            if (tblHistory.Columns.Contains("uang_bayar"))
                tblHistory.Columns["uang_bayar"].HeaderText = "Uang Bayar";
            if (tblHistory.Columns.Contains("uang_kembalian"))
                tblHistory.Columns["uang_kembalian"].HeaderText = "Uang Kembalian";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadHistory();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCari_Click(object sender, EventArgs e)
        {
            try
            {
                string noStruk = txtNoStruk.Text.Trim();
                string namaObat = txtNamaObat.Text.Trim();
                string namaKasir = txtNamaKasir.Text.Trim();
                DateTime? tanggalFrom = dtpTanggalFrom.Checked ? dtpTanggalFrom.Value : (DateTime?)null;
                DateTime? tanggalTo = dtpTanggalTo.Checked ? dtpTanggalTo.Value : (DateTime?)null;

                DataTable dt = TransactionHistoryService.SearchHistory(noStruk, namaObat, namaKasir, tanggalFrom, tanggalTo);
                BindDataToGrid(dt);

                string desc = $"Mencari history: NoStruk={noStruk}, NamaObat={namaObat}, NamaKasir={namaKasir}";
                ActivityHistoryService.LogActivity("SEARCH_HISTORY", desc);
                if (tanggalFrom.HasValue || tanggalTo.HasValue)
                {
                    ActivityHistoryService.LogActivity("FILTER_HISTORY", $"Filter tanggal: Dari={tanggalFrom}, Ke={tanggalTo}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
