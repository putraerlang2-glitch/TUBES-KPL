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
                client = new ObatApiClient("https://localhost:7103");

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
            foreach (var obat in data)
            {
                obat.UpdateStatus();
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
            for (int i = 0; i < tblObat.Rows.Count; i++)
            {
                string status = tblObat.Rows[i].Cells[5].Value?.ToString();

                DataGridViewRow row = tblObat.Rows[i];

                if (status == "Expired")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                }
                else if (status == "LowStock")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 200);
                }
            }
        }

        private void TampilkanNotifikasi()
        {
            var obatExpired = daftarObat
                .Where(o => o.Status == "Expired")
                .ToList();

            if (obatExpired.Count > 0)
            {
                string pesan =
                    "⚠️ PERINGATAN: Obat expired:\n" +
                    string.Join("\n", obatExpired.Select(o => "- " + o.Nama));

                MessageBox.Show(
                    pesan,
                    "Obat Expired",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }

            TampilkanStatistik();
        }

        private void TampilkanStatistik()
        {
            int available = daftarObat.Count(o => o.Status == "Available");
            int low = daftarObat.Count(o => o.Status == "LowStock");
            int expired = daftarObat.Count(o => o.Status == "Expired");

            this.Text =
                $"Apotek - Avail: {available} | Low: {low} | Expired: {expired}";
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

        private void button2_Click(object sender, EventArgs e)
        {
            FormTambahObat formTambah = new FormTambahObat();

            if (formTambah.ShowDialog() == DialogResult.OK)
            {
                daftarObat.Add(formTambah.obatBaru);

                TampilkanData(daftarObat);
                TampilkanStatistik();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
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
                TampilkanData(daftarObat);
                TampilkanStatistik();
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (tblObat.CurrentRow != null)
            {
                string nama =
                    tblObat.CurrentRow.Cells[0].Value.ToString();

                if (
                    MessageBox.Show(
                        $"Hapus {nama}?",
                        "Konfirmasi",
                        MessageBoxButtons.YesNo
                    ) == DialogResult.Yes
                )
                {
                    daftarObat.RemoveAll(o => o.Nama == nama);

                    TampilkanData(daftarObat);
                    TampilkanStatistik();
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