using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static TubesKPL.Obat;

namespace TubesKPL
{
    public partial class Form1 : Form
    {
        // List data obat
        List<Obat> daftarObat = new List<Obat>();

        public Form1()
        {
            InitializeComponent();
        }

        public void RefreshData()
        {
            TampilkanData(daftarObat);
            TampilkanStatistik();
        }

        /// <summary>
        /// Sample data fallback jika API gagal
        /// </summary>
        private List<Obat> GetSampleData()
        {
            return new List<Obat>()
            {
                new Obat("Paracetamol", 21, 5000, new DateTime(2025, 1, 15), "Tablet"),
                new Obat("Ibuprofen", 12, 7000, new DateTime(2024, 7, 20), "Tablet"),
                new Obat("Sanmol", 15, 3000, new DateTime(2025, 4, 5), "Sirup"),
                new Obat("HRIG", 12, 20000, new DateTime(2024, 3, 10), "AntiJamur"),
                new Obat("Influenza", 10, 2000, new DateTime(2025, 3, 15), "Tablet"),
                new Obat("Vitamin C", 11, 500000, new DateTime(2026, 8, 1), "Vitamin")
            };
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            ObatApiClient client = null;

            try
            {
                client = new ObatApiClient("https://localhost:7245");

                Console.WriteLine("[MAIN FORM] Fetching data from API...");

                daftarObat = await client.GetAllObatAsync();

                Console.WriteLine($"[MAIN FORM] Received {(daftarObat?.Count ?? 0)} items from API");

                if (daftarObat == null || daftarObat.Count == 0)
                {
                    MessageBox.Show(
                        "Tidak ada data dari API.\nMenggunakan sample data.",
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    daftarObat = GetSampleData();
                }

                ObatApiService.Initialize(daftarObat);

                TampilkanData(daftarObat);
                TampilkanNotifikasi();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");

                MessageBox.Show(
                    $"Gagal mengambil data API.\n\n{ex.Message}\n\nMenggunakan sample data.",
                    "API Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                daftarObat = GetSampleData();

                ObatApiService.Initialize(daftarObat);

                TampilkanData(daftarObat);
                TampilkanNotifikasi();
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        private void TampilkanData(List<Obat> data)
        {
            // Update status untuk setiap obat menggunakan StateMachine
            foreach (var obat in data)
            {
                StateMachine.EvaluateStatus(obat);
            }

            DataTable dt = new DataTable();

            dt.Columns.Add("Nama Obat");
            dt.Columns.Add("Kategori");
            dt.Columns.Add("Stok");
            dt.Columns.Add("Harga");
            dt.Columns.Add("Tanggal Expired");
            dt.Columns.Add("Status");

            foreach (var obat in data)
            {
                dt.Rows.Add(
                    obat.Nama,
                    obat.Kategori,
                    obat.Stok,
                    obat.Harga.ToString("C"),
                    obat.ExpiredDate.ToString("dd/MM/yyyy"),
                    obat.Status
                );
            }

            tblObat.DataSource = dt;

            TerapkanWarnaStatus();
        }

        private void TerapkanWarnaStatus()
        {
            // Gunakan StateMachine untuk apply colors
            StateMachine.ApplyStatusColors(tblObat, statusColumnIndex: 5);
        }

        private void TampilkanNotifikasi()
        {
            // Gunakan StateMachine untuk show notifications
            StateMachine.ShowNotifications(daftarObat);
            TampilkanStatistik();
        }

        private void TampilkanStatistik()
        {
            // Gunakan StateMachine untuk format title dengan stats
            string baseTitle = "Apotek";
            this.Text = StateMachine.FormatTitleWithStats(baseTitle, daftarObat);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inputan = textBox1.Text.ToLower();

            var hasil = daftarObat
                .Where(o => o.Nama.ToLower().Contains(inputan))
                .ToList();

            if (hasil.Count > 0)
            {
                TampilkanData(hasil);
            }
            else
            {
                MessageBox.Show("Obat tidak ditemukan");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            FormTambahObat formTambah = new FormTambahObat();

            if (formTambah.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (ObatApiClient client = new ObatApiClient("https://localhost:7245"))
                    {
                        var obatBaru = await client.AddObatAsync(formTambah.obatBaru);
                        daftarObat.Add(obatBaru);

                        TampilkanData(daftarObat);
                        TampilkanStatistik();
                        MessageBox.Show("Obat berhasil disimpan ke Database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Gagal menyimpan ke database:\n{ex.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (tblObat.CurrentRow == null)
            {
                MessageBox.Show("Klik dulu obat yang ingin diubah!");
                return;
            }

            int selectedIndex = tblObat.CurrentRow.Index;

            if (selectedIndex < 0 || selectedIndex >= daftarObat.Count)
            {
                MessageBox.Show(
                    "Baris tidak valid.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            Obat selectedObat = daftarObat[selectedIndex];

            Form2 f2 = new Form2(selectedObat);

            if (f2.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (ObatApiClient client = new ObatApiClient("https://localhost:7245"))
                    {
                        // Update to API
                        var updatedObat = await client.UpdateObatAsync(selectedObat.Id, selectedObat);
                        daftarObat[selectedIndex] = updatedObat;

                        TampilkanData(daftarObat);
                        TampilkanStatistik();
                        MessageBox.Show("Obat berhasil diubah di Database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Gagal merubah data di database:\n{ex.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnHapus_Click(object sender, EventArgs e)
        {
            if (tblObat.CurrentRow != null)
            {
                int selectedIndex = tblObat.CurrentRow.Index;
                if (selectedIndex < 0 || selectedIndex >= daftarObat.Count) return;

                Obat selectedObat = daftarObat[selectedIndex];
                string nama = selectedObat.Nama;

                if (
                    MessageBox.Show(
                        $"Hapus {nama} dari database secara permanen?",
                        "Konfirmasi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    ) == DialogResult.Yes
                )
                {
                    try
                    {
                        using (ObatApiClient client = new ObatApiClient("https://localhost:7245"))
                        {
                            bool sukses = await client.DeleteObatAsync(selectedObat.Id);
                            if (sukses)
                            {
                                daftarObat.RemoveAt(selectedIndex);

                                TampilkanData(daftarObat);
                                TampilkanStatistik();
                                MessageBox.Show("Obat berhasil dihapus dari Database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Gagal menghapus data dari server.", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Gagal menghapus dari database:\n{ex.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormTransaksi ft = new FormTransaksi(daftarObat, this);
            ft.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}