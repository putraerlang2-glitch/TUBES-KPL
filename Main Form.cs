using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static TubesKPL.Obat;

namespace TubesKPL
{
    public partial class Form1 : Form
    {

        List<Obat> daftarObat = new List<Obat>()
        {
            new Obat("Paracetamol", 21, 5000, new DateTime(2025, 1, 15), KategoriObat.Tablet),
            new Obat("Ibuprofen", 12, 7000, new DateTime(2024, 7, 20), KategoriObat.Tablet),
            new Obat("Sanmol", 15, 3000, new DateTime(2025, 4, 5), KategoriObat.Sirup),
            new Obat("HRIG", 12, 20000, new DateTime(2024, 3, 10), KategoriObat.AntiJamur),
            new Obat("Influenza", 10, 2000, new DateTime(2025, 3, 15), KategoriObat.Tablet),
            new Obat("Jane Doe", 11, 500000, new DateTime(2026, 8, 1), KategoriObat.Vitamin)
        };

        public void RefreshData()
        {
            TampilkanData(daftarObat);
            TampilkanStatistik();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TampilkanData(daftarObat);
            TampilkanNotifikasi();

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
                dt.Rows.Add(obat.nama,obat.kategori.ToString(), obat.stok, obat.harga.ToString("C"),
                    obat.expiredDate.ToString("dd/MM/yyyy"), obat.status.ToString());
            }

            tblObat.DataSource = dt;
            TerapkanWarnaStatus();
        }

       
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (tblObat.CurrentRow == null)
            {
                MessageBox.Show("Klik dulu baris obat yang mau diubah di tabel!");
                return;
            }

            int selectedIndex = tblObat.CurrentRow.Index;

            
            if (selectedIndex < 0 || selectedIndex >= daftarObat.Count)
            {
                MessageBox.Show("Baris tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void TerapkanWarnaStatus()
        {
            for (int i = 0; i < tblObat.Rows.Count; i++)
            {
                string status = tblObat.Rows[i].Cells[5].Value?.ToString();
                DataGridViewRow row = tblObat.Rows[i];

                if (status == "Expired") row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                else if (status == "LowStock") row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200);
                else if (status == "Available") row.DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 200);
            }
        }

        private void TampilkanNotifikasi()
        {
            var obatExpired = daftarObat.Where(o => o.status == StatusObat.Expired).ToList();
            if (obatExpired.Count > 0)
            {
                string pesan = "⚠️ PERINGATAN: Obat expired:\n" + string.Join("\n", obatExpired.Select(o => $"- {o.nama}"));
                MessageBox.Show(pesan, "Obat Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            TampilkanStatistik();
        }

        private void TampilkanStatistik()
        {
            int available = daftarObat.Count(o => o.status == StatusObat.Available);
            int low = daftarObat.Count(o => o.status == StatusObat.LowStock);
            int expired = daftarObat.Count(o => o.status == StatusObat.Expired);
            this.Text = $"Apotek - Avail: {available} | Low: {low} | Expired: {expired}";
        }

        private void button1_Click(object sender, EventArgs e) 
        {
            string inputan = textBox1.Text.ToLower();
            var hasil = daftarObat.Where(o => o.nama.ToLower().Contains(inputan)).ToList();

            if (hasil.Count > 0) TampilkanData(hasil);
            else MessageBox.Show("Obat tidak ditemukan");
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

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (tblObat.CurrentRow != null)
            {
                string nama = tblObat.CurrentRow.Cells[0].Value.ToString();
                if (MessageBox.Show($"Hapus {nama}?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    daftarObat.RemoveAll(o => o.nama == nama);
                    TampilkanData(daftarObat);
                    TampilkanStatistik();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void button3_Click(object sender, EventArgs e)
        {

            FormTransaksi ft = new FormTransaksi(daftarObat, this);
            ft.Show();
        }
    }

    public enum StatusObat { Available, LowStock, Expired }

    public class Obat
    {
        public string nama { get; set; }
        public int stok { get; set; }
        public decimal harga { get; set; }
        public DateTime expiredDate { get; set; }
        public StatusObat status { get; set; }
        public KategoriObat kategori { get; set; }

        private static Dictionary<KategoriObat, int> batasMinimumStok = new Dictionary<KategoriObat, int>()
{
        { KategoriObat.Tablet, 10 },
        { KategoriObat.Salep, 8 },
        { KategoriObat.Sirup, 11 },
        { KategoriObat.Vitamin, 8 },
        { KategoriObat.Antibiotik, 15 },
        { KategoriObat.AntiJamur, 7 }
};

        public Obat(string nama, int stok, decimal harga, DateTime expiredDate, KategoriObat kategori)
        {
            this.nama = nama;
            this.stok = stok;
            this.harga = harga;
            this.expiredDate = expiredDate;
            this.kategori = kategori;
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            if (expiredDate < DateTime.Now)
                status = StatusObat.Expired;
            else if (stok < batasMinimumStok[kategori])
                status = StatusObat.LowStock; 
            else status = StatusObat.Available;
        }
        public enum KategoriObat
        {
            Tablet,
            Salep,
            Sirup,
            Vitamin,
            Antibiotik,
            AntiJamur
        }
    }
}
