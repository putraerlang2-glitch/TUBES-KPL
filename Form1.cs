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
            // Update status semua obat
            foreach (var obat in data)
            {
                obat.UpdateStatus();
            }

            // Buat DataTable untuk menampilkan data
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

            // Terapkan warna berdasarkan status
            TerapkanWarnaStatus();
        }

        private void TerapkanWarnaStatus()
        {
            StateMachine.ApplyStatusColors(tblObat, statusColumnIndex: 4);
        }

        private void TampilkanNotifikasi()
        {
            StateMachine.ShowNotifications(daftarObat);
            TampilkanStatistik();
        }

        private void TampilkanStatistik()
        {
            this.Text = StateMachine.FormatTitleWithStats("Form1", daftarObat);
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
    }
}
