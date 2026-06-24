using System;
using System.Diagnostics;
using System.Windows.Forms;
using static TubesKPL.Obat;

namespace TubesKPL
{
    public partial class FormTambahObat : Form
    {
        public Obat ObatBaru { get; private set; }

        public FormTambahObat()
        {
            InitializeComponent();

            GenericHelper.LoadEnumToComboBox<KategoriObat>(cmbKategori);
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbKategori.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Kategori wajib dipilih!",
                        "Validasi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

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

                ObatBaru = ObatFactory.Create(
                    txtNamaObat.Text,
                    jumlahStok,
                    hargaObat,
                    dtpExpired.Value,
                    (KategoriObat)cmbKategori.SelectedItem);

                MessageBox.Show(
                    "Data obat berhasil ditambahkan!",
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
                    "Terjadi kesalahan saat menyimpan data.",
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