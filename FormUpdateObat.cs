using System;
using System.Diagnostics;
using System.Windows.Forms;
using static TubesKPL.Obat;

namespace TubesKPL
{
    public partial class FormUpdateObat : Form
    {
        private readonly Obat editingObat;

        public FormUpdateObat(Obat obatToEdit)
        {
            InitializeComponent();

            // Memuat daftar kategori ke ComboBox
            GenericHelper.LoadEnumToComboBox<KategoriObat>(cmbKategori);

            editingObat = obatToEdit;

            // Menampilkan data obat yang akan diedit
            txtNamaObat.Text = editingObat.Nama;
            txtStok.Text = editingObat.Stok.ToString();
            txtHarga.Text = editingObat.Harga.ToString();
            dtpExpired.Value = editingObat.ExpiredDate;

            // Konversi string kategori menjadi enum
            cmbKategori.SelectedItem =
            (KategoriObat)Enum.Parse(typeof(KategoriObat), editingObat.Kategori);
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                // Pastikan kategori dipilih
                if (cmbKategori.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Kategori wajib dipilih!",
                        "Validasi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // Seluruh validasi dilakukan oleh ObatValidator
                if (!ObatValidator.ValidateObatInput(
                    txtNamaObat.Text,
                    txtStok.Text,
                    txtHarga.Text,
                    out string pesan,
                    out int jumlahStok,
                    out decimal hargaObat))
                {
                    MessageBox.Show(
                        pesan,
                        "Validasi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // Update data obat
                editingObat.Nama = txtNamaObat.Text.Trim();
                editingObat.Stok = jumlahStok;
                editingObat.Harga = hargaObat;
                editingObat.ExpiredDate = dtpExpired.Value;
                editingObat.Kategori = ((KategoriObat)cmbKategori.SelectedItem).ToString();

                editingObat.UpdateStatus();

                MessageBox.Show(
                    "Data obat berhasil diperbarui!",
                    "Sukses",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                MessageBox.Show(
                    "Terjadi kesalahan saat memperbarui data.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}