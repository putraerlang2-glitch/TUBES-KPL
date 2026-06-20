using System;
using System.Diagnostics;
using System.Windows.Forms;
using static TubesKPL.Obat;

namespace TubesKPL
{
    public partial class FormUpdateObat : Form
    {
        private Obat editingObat;

        public FormUpdateObat(Obat obatToEdit)
        {
            InitializeComponent();

            // Muat daftar kategori ke ComboBox
            GenericHelper.LoadEnumToComboBox<KategoriObat>(cmbKategori);

            editingObat = obatToEdit;

            // Isi kontrol input di UI dengan data dari objek obat yang dikirim
            txtNamaObat.Text = editingObat.Nama;
            txtStok.Text = editingObat.Stok.ToString();
            txtHarga.Text = editingObat.Harga.ToString();
            dtpExpired.Value = editingObat.ExpiredDate;
            cmbKategori.SelectedItem = editingObat.Kategori;
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                string namaObat = txtNamaObat.Text.Trim();

                // Validasi Tipe Data & Guard Clauses (Clean Code)
                if (!int.TryParse(txtStok.Text, out int newStok))
                {
                    MessageBox.Show("Stok harus berupa angka!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtHarga.Text, out decimal newHarga))
                {
                    MessageBox.Show("Harga harus berupa angka!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbKategori.SelectedItem == null)
                {
                    MessageBox.Show("Kategori wajib dipilih!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                KategoriObat kategoriBaru = (KategoriObat)cmbKategori.SelectedItem;

                // Memakai validasi yang sama persis dengan form tambah (Code Reuse)
                if (!ObatValidator.ValidateObatInput(namaObat, newStok, newHarga))
                {
                    MessageBox.Show("Input obat tidak valid! Pastikan nama terisi dan angka tidak negatif.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Terapkan perubahan nilai ke objek
                editingObat.Nama = namaObat;
                editingObat.Stok = newStok;
                editingObat.Harga = newHarga;
                editingObat.ExpiredDate = dtpExpired.Value;
                editingObat.Kategori = kategoriBaru.ToString();
                editingObat.UpdateStatus();

                MessageBox.Show("Data obat berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // Menyembunyikan error teknis dari user (Secure Code)
                Debug.WriteLine($"Error saat update data: {ex.Message}");
                MessageBox.Show("Terjadi kesalahan sistem saat memperbarui data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}