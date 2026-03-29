using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TubesKPL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        List<Obat> daftarObat = new List<Obat>()
        {
            new Obat("Paracetamol", 21, 5000, new DateTime(2025, 6, 15)),
            new Obat("Ibuprofen", 17, 7000, new DateTime(2025, 8, 20)),
            new Obat("Sanmol", 5, 3000, new DateTime(2024, 12, 31)),
            new Obat("HRIG", 3, 20000, new DateTime(2024, 11, 10)),
            new Obat("Influenza", 15, 2000, new DateTime(2025, 3, 15)),
            new Obat("Jane Doe", 50, 500000, new DateTime(2026, 1, 1))
        };

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
            dt.Columns.Add("Stok");
            dt.Columns.Add("Harga");
            dt.Columns.Add("Tanggal Expired");
            dt.Columns.Add("Status");

            foreach (var obat in data)
            {
                dt.Rows.Add(obat.nama, obat.stok, obat.harga.ToString("C"), 
                    obat.expiredDate.ToString("dd/MM/yyyy"), obat.status.ToString());
            }

            tblObat.DataSource = dt;

            TerapkanWarnaStatus();
        }

        private void TerapkanWarnaStatus()
        {
            if (tblObat.DataSource is DataTable dt)
            {
                for (int i = 0; i < tblObat.Rows.Count; i++)
                {
                    string status = tblObat.Rows[i].Cells[4].Value?.ToString();
                    DataGridViewRow row = tblObat.Rows[i];

                    switch (status)
                    {
                        case "Expired":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200); 
                            break;
                        case "LowStock":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200); 
                            break;
                        case "Available":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 200); 
                            break;
                    }
                }
            }
        }

        private void TampilkanNotifikasi()
        {
            List<Obat> obatExpired = daftarObat.Where(o => o.status == StatusObat.Expired).ToList();
            List<Obat> obatLowStock = daftarObat.Where(o => o.status == StatusObat.LowStock).ToList();


            if (obatExpired.Count > 0)
            {
                string pesan = "⚠️ PERINGATAN: Ada obat yang sudah expired:\n\n";
                foreach (var obat in obatExpired)
                {
                    pesan += $"- {obat.nama} (Expired: {obat.expiredDate:dd/MM/yyyy})\n";
                }
                MessageBox.Show(pesan, "Obat Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (obatLowStock.Count > 0)
            {
                string pesan = " PERHATIAN: Ada obat dengan stok rendah:\n\n";
                foreach (var obat in obatLowStock)
                {
                    pesan += $"- {obat.nama} (Stok: {obat.stok})\n";
                }
                MessageBox.Show(pesan, "Stok Rendah", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            TampilkanStatistik();
        }

        private void TampilkanStatistik()
        {
            int jumlahAvailable = daftarObat.Count(o => o.status == StatusObat.Available);
            int jumlahLowStock = daftarObat.Count(o => o.status == StatusObat.LowStock);
            int jumlahExpired = daftarObat.Count(o => o.status == StatusObat.Expired);

            this.Text = $"Form1 - Available: {jumlahAvailable} | Low Stock: {jumlahLowStock} | Expired: {jumlahExpired}";
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inputan = textBox1.Text.ToLower();
            if(inputan == "")
            {
                MessageBox.Show("Masukan Nama Obat Dulu");
                return;
            }
            List<Obat> hasil = new List<Obat>();

            for(int i = 0; i < daftarObat.Count; i++)
            {
                if (daftarObat[i].nama.ToLower().Contains(inputan))
                {
                    hasil.Add(daftarObat[i]);
                }
            }
            if(hasil.Count > 0)
            {
                TampilkanData(hasil);
            }
            else
            {
                MessageBox.Show("Obat tidak ada");
                TampilkanData(daftarObat);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormTambahObat formTambah = new FormTambahObat();

            // kalau user klik simpan (OK)
            if (formTambah.ShowDialog() == DialogResult.OK)
            {
                Obat obatBaru = formTambah.obatBaru;

                daftarObat.Add(obatBaru);

                TampilkanData(daftarObat);
                TampilkanStatistik();
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (tblObat.CurrentCell != null && tblObat.Rows.Count > 0)
            {
                int selectedIndex = tblObat.CurrentCell.RowIndex;

                string namaObat = tblObat.Rows[selectedIndex].Cells[0].Value?.ToString();

                DialogResult dialogResult = MessageBox.Show(
                    $"Apakah kamu yakin ingin menghapus obat '{namaObat}'?",
                    "Konfirmasi Hapus",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    Obat obatDihapus = daftarObat.FirstOrDefault(o => o.nama == namaObat);

                    if (obatDihapus != null)
                    {
                        daftarObat.Remove(obatDihapus);

                        TampilkanData(daftarObat);
                        TampilkanStatistik();

                        MessageBox.Show("Data obat berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Silakan pilih data obat di tabel terlebih dahulu yang ingin dihapus.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
    public enum StatusObat
    {
        Available,
        LowStock,
        Expired
    }

    public class Obat
    {
        public string nama { get; set; }
        public int stok { get; set; }
        public decimal harga { get; set; }
        public DateTime expiredDate { get; set; }
        public StatusObat status { get; set; }

        public Obat(string nama, int stok, decimal harga, DateTime expiredDate)
        {
            this.nama = nama;
            this.stok = stok;
            this.harga = harga;
            this.expiredDate = expiredDate;
            this.status = HitungStatus();
        }

        public StatusObat HitungStatus()
        {
            if (expiredDate < DateTime.Now)
            {
                return StatusObat.Expired;
            }
            else if (stok < 10)
            {
                return StatusObat.LowStock;
            }
            else
            {
                return StatusObat.Available;
            }
        }

        public void UpdateStatus()
        {
            status = HitungStatus();
        }
    }
}
