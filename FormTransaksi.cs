using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TubesKPL
{
    public partial class FormTransaksi : Form
    {
        List<Obat> daftarObat;
        Form1 mainform;

        public FormTransaksi(List<Obat> data, Form1 f1)
        {
            InitializeComponent();
            daftarObat = data;
            mainform = f1;

            BoxObat.DataSource = daftarObat;
            BoxObat.DisplayMember = "nama";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void BtnHitung_Click(object sender, EventArgs e)
        {
            int jumlah;
          
            Obat o = (Obat)BoxObat.SelectedItem;
            decimal harga = o.harga;

            if (string.IsNullOrWhiteSpace(TBjumlah.Text))
            {
                MessageBox.Show("Masukkan obat yang mau di beli ya Jane doe");
                return;
            }

            if(!int.TryParse(TBjumlah.Text, out jumlah))
            {
                MessageBox.Show("Tolong Masukkan Angka ya jane doe");
                return;
            }

            if (jumlah > o.stok)
            {
                MessageBox.Show("Stok tidak cukup");
                return;
            }

            o.stok -= jumlah;
            o.UpdateStatus();

            decimal total = jumlah * harga;
            MessageBox.Show("Nama obat: " + o + "Total Harga: " + total);

            mainform.RefreshData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }
    }
}
