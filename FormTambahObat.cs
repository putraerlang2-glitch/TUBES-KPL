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
                string namaObat = txtNamaObat.Text.Trim();
                if (!int.TryParse(txtStok.Text, out int jumlahStok))
                {
                    MessageBox.Show(
                        "Stok harus berupa angka!",
                        "Validasi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (!decimal.TryParse(txtHarga.Text, out decimal hargaObat))
                {
                    MessageBox.Show(
                        "Harga harus berupa angka!",
                        "Validasi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                DateTime tanggalExpired = dtpExpired.Value;

                if (cmbKategori.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Kategori wajib dipilih!",
                        "Validasi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                KategoriObat kategoriObat =
                    (KategoriObat)cmbKategori.SelectedItem;

                if (!ObatValidator.ValidateObatInput(
                    namaObat,
                    jumlahStok,
                    hargaObat))
                {
                    MessageBox.Show(
                        "Input obat tidak valid!",
                        "Validasi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                ObatBaru = ObatFactory.Create(
                    namaObat,
                    jumlahStok,
                    hargaObat,
                    tanggalExpired,
                    kategoriObat
                );

                MessageBox.Show(
                    "Data obat berhasil ditambahkan!",
                    "Sukses",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");

                MessageBox.Show(
                    "Terjadi kesalahan saat menyimpan data.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
    public static class ObatValidator
    {
        public static bool ValidateObatInput(
            string namaObat,
            int jumlahStok,
            decimal hargaObat)
        {
            if (string.IsNullOrWhiteSpace(namaObat))
                return false;

            if (jumlahStok < 0)
                return false;

            if (hargaObat < 0)
                return false;

            return true;
        }
    }
    public static class ObatFactory
    {
        public static Obat Create(
            string namaObat,
            int jumlahStok,
            decimal hargaObat,
            DateTime tanggalExpired,
            KategoriObat kategoriObat)
        {
            namaObat = namaObat.Trim();

            return new Obat(
                namaObat,
                jumlahStok,
                hargaObat,
                tanggalExpired,
                kategoriObat
            );
        }
    }

    public static class GenericHelper
    {
        public static void LoadEnumToComboBox<T>(ComboBox comboBox)
            where T : Enum
        {
            comboBox.DataSource = Enum.GetValues(typeof(T));
        }
    }
}

