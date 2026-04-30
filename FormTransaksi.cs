using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TubesKPL;

namespace TubesKPL
{

    public partial class FormTransaksi : Form
    {
        List<Obat> daftarObat;
        Form1 mainform;
        List<ItemTransaksi> keranjang = new List<ItemTransaksi>();

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
            if(keranjang.Count == 0)
            {
                MessageBox.Show("Masukkan obat yang mau dibeli dulu");
                return;
            }
            decimal grandTotal = 0;

            foreach (var item in keranjang)
            {
                item.obat.stok -= item.jumlah;
                item.obat.UpdateStatus();

                grandTotal += item.Subtotal();
            }
            MessageBox.Show("Total Pembayaran: " + grandTotal.ToString("C"));
            keranjang.Clear();
            RefreshKeranjang();

            mainform.RefreshData();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void RefreshKeranjang()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Nama Obat");
            dt.Columns.Add("Harga");
            dt.Columns.Add("Jumlah");
            dt.Columns.Add("Sub total");

            foreach (var item in keranjang)
            {
                dt.Rows.Add(item.obat.nama,item.obat.harga.ToString("C"), item.jumlah,item.Subtotal().ToString("C"));
            }
            TabelKeranjang.DataSource = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            int jumlah;

            Obat o = (Obat)BoxObat.SelectedItem;
            decimal harga = o.harga;

            if (string.IsNullOrWhiteSpace(TBjumlah.Text))
            {
                MessageBox.Show("Masukkan obat yang mau di beli ya Jane doe");
                return;
            }

            if (!int.TryParse(TBjumlah.Text, out jumlah))
            {
                MessageBox.Show("Tolong Masukkan Angka ya jane doe");
                return;
            }

            if (jumlah > o.stok)
            {
                MessageBox.Show("Stok tidak cukup");
                return;
            }

            ItemTransaksi item = new ItemTransaksi()
            {
                obat = o,
                jumlah = jumlah
            };

            keranjang.Add(item);
            RefreshKeranjang();
        }
    }
}
public class ItemTransaksi
{
    public Obat obat { get; set; }
    public int jumlah { get; set; }

    public decimal Subtotal()
    {
        return obat.harga * jumlah;
    }
}
