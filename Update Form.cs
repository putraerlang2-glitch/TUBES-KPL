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

            // Isi kontrol input dengan data dari objek
            txtNama.Text = editingObat.nama;
            txtStok.Text = editingObat.stok.ToString();
            txtHarga.Text = editingObat.harga.ToString();
            dtpExpired.Value = editingObat.expiredDate;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SimpanData();
        }

        // --- SOLUSI ERROR CS1061 ---
        // Jika Designer mencari 'button1_Click', kita arahkan ke fungsi Simpan
        private void button1_Click(object sender, EventArgs e)
        {
            SimpanData();
        }

        private void SimpanData()
        {
            try
            {
                editingObat.nama = txtNama.Text.Trim();

                if (int.TryParse(txtStok.Text, out int newStok))
                    editingObat.stok = newStok;

                if (decimal.TryParse(txtHarga.Text, out decimal newHarga))
                    editingObat.harga = newHarga;

                editingObat.expiredDate = dtpExpired.Value;
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

        // Biarkan method kosong ini tetap ada jika Designer masih merujuk ke sini
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
    }
}