using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using static TubesKPL.Obat;

namespace TubesKPL
{
    public partial class FormTambahObat : Form
    {
        public Obat obatBaru;

        public FormTambahObat()
        {
            InitializeComponent();

            ApplyRuntimeConfig();

            GenericHelper.GenericComboBox<KategoriObat>(comboBox1);
        }

        private void ApplyRuntimeConfig()
        {
            try
            {
                if (File.Exists("config.txt"))
                {
                    string appName = File.ReadAllText("config.txt");
                    this.Text = "Tambah Data - " + appName;
                }
            }
            catch {
            }
        }

        private void btnSimpan_Click_Click(object sender, EventArgs e)
        {
            try
            {
                string nama = textBox1.Text;
                int stok = int.Parse(textBox2.Text);
                decimal harga = decimal.Parse(textBox3.Text);
                DateTime expired = dateTimePicker1.Value;

                if (!ObatValidator.IsValidInput(nama, stok))
                {
                    MessageBox.Show("Input Nama atau Stok tidak valid!");
                    return;
                }

                KategoriObat kategoriTerpilih = (KategoriObat)comboBox1.SelectedItem;

                obatBaru = new Obat(nama, stok, harga, expired, kategoriTerpilih);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi Error: " + ex.Message);
            }
        }

        private void btnBatal_Click_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox4_TextChanged(object sender, EventArgs e) { 
        }
        private void textBox1_TextChanged(object sender, EventArgs e) { 
        }
        private void textBox2_TextChanged(object sender, EventArgs e) { 
        }
        private void textBox3_TextChanged(object sender, EventArgs e) { 
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { 
        }
        private void label5_Click(object sender, EventArgs e) { 
        }
        private void label4_Click(object sender, EventArgs e) { 
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { 
        }
    }

    public static class ObatValidator
    {
        public static bool IsValidInput(string nama, int stok)
        {
            if (string.IsNullOrWhiteSpace(nama)) return false;
            if (stok < 0) return false;
            return true;
        }
    }

    public static class GenericHelper
    {
        public static void GenericComboBox<T>(ComboBox cmb) where T : Enum
        {
            cmb.DataSource = Enum.GetValues(typeof(T));
        }
    }
}