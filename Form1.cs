using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TubesKPL
{
    public partial class Form1 : Form
    {
        List<Obat> daftarObat = new List<Obat>();

        public Form1() => InitializeComponent();
        public void RefreshData() => TampilkanData(daftarObat);

        private async void Form1_Load(object sender, EventArgs e) => await LoadFromApi();

        private async Task LoadFromApi()
        {
            try
            {
                using (var client = new ObatApiClient())
                {
                    Console.WriteLine("[Form1] Fetching data from API...");
                    daftarObat = await client.GetAllObatAsync() ?? new List<Obat>();
                    Console.WriteLine($"[Form1] Received {daftarObat.Count} items from API");
                }

                ObatApiService.Initialize(daftarObat);
                TampilkanData(daftarObat);
                StateMachine.ShowNotifications(daftarObat);
                this.Text = StateMachine.FormatTitleWithStats("Form1", daftarObat);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Form1 Error] {ex.Message}");
                MessageBox.Show($"Gagal memuat data dari API.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                daftarObat.Clear();
                TampilkanData(daftarObat);
            }
        }

        private void TampilkanData(List<Obat> data)
        {
            foreach (var obat in data) obat.UpdateStatus();
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("Nama Obat"),
                new DataColumn("Stok"),
                new DataColumn("Harga"),
                new DataColumn("Tanggal Expired"),
                new DataColumn("Status")
            });

            foreach (var obat in data)
                dt.Rows.Add(obat.Nama, obat.Stok, obat.Harga.ToString("C"), obat.ExpiredDate.ToString("dd/MM/yyyy"), obat.Status);

            tblObat.DataSource = dt;
            StateMachine.ApplyStatusColors(tblObat, 4);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inputan = textBox1.Text.ToLower();
            if (string.IsNullOrWhiteSpace(inputan))
            {
                TampilkanData(daftarObat);
                return;
            }
            
            var hasil = daftarObat.Where(o => o.Nama.ToLower().Contains(inputan)).ToList();
            if (hasil.Count > 0) TampilkanData(hasil);
            else
            {
                MessageBox.Show("Obat tidak ada");
                TampilkanData(daftarObat);
            }
        }
    }
}
