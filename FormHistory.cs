using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace TubesKPL
{
    public partial class FormHistory : Form
    {
        public FormHistory()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            var list = TransactionHistoryService.GetAllTransactions();
            DataTable dt = new DataTable();
            dt.Columns.Add("No Struk");
            dt.Columns.Add("Tanggal");
            dt.Columns.Add("Total");
            dt.Columns.Add("UserId");

            foreach (var t in list.OrderByDescending(x => x.TanggalTransaksi))
            {
                dt.Rows.Add(t.NoStruk, t.TanggalTransaksi.ToString("g"), t.TotalAkhir.ToString("C"), t.UserId);
            }

            tblHistory.DataSource = dt;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadHistory();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
