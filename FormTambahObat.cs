using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static TubesKPL.Obat;

namespace TubesKPL
{
    public partial class FormTambahObat : Form
    {
        // ini untuk kirim data ke Form1
        public Obat obatBaru;

        public FormTambahObat()
        {
            InitializeComponent();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnSimpan_Click_Click(object sender, EventArgs e)
        {
            try
            {
                string nama = textBox1.Text;
                int stok = int.Parse(textBox2.Text);
                decimal harga = decimal.Parse(textBox3.Text);
                DateTime expired = dateTimePicker1.Value;

                if (nama == "")
                {
                    MessageBox.Show("Nama obat tidak boleh kosong!");
                    return;
                }

                obatBaru = new Obat(nama, stok, harga, expired, KategoriObat.Tablet) ;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch
            {
                MessageBox.Show("Input tidak valid! Pastikan angka benar.");
            }
        }


        private void btnBatal_Click_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}