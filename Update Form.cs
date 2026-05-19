using System;
using System.Windows.Forms;

namespace TubesKPL
{
    public partial class Form2 : Form
    {
        private Obat editingObat;

        public Form2(Obat obatToEdit)
        {
            InitializeComponent();

            editingObat = obatToEdit;

            // Menampilkan data obat ke dalam form isian.
            txtNama.Text = editingObat.Nama;
            txtStok.Text = editingObat.Stok.ToString();
            txtHarga.Text = editingObat.Harga.ToString();
            dtpExpired.Value = editingObat.ExpiredDate;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SimpanData();
        }

        // Mencegah error tampilan desain dengan mengarahkan tombol ke fungsi penyimpan.
        private void button1_Click(object sender, EventArgs e)
        {
            SimpanData();
        }

        private void SimpanData()
        {
            try
            {
                editingObat.Nama = txtNama.Text.Trim();

                if (int.TryParse(txtStok.Text, out int newStok))
                    editingObat.Stok = newStok;

                if (decimal.TryParse(txtHarga.Text, out decimal newHarga))
                    editingObat.Harga = newHarga;

                editingObat.ExpiredDate = dtpExpired.Value;
                editingObat.UpdateStatus();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan saat menyimpan: " + ex.Message);
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Dibiarkan kosong agar tampilan desain UI tidak error.
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }

        private void dtpExpired_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}