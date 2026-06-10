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
    // ============================================================
    // DESIGN PATTERN: MVC Pattern + Delegation Pattern
    // ============================================================
    // MVC Pattern:
    // - Model: Obat class (data) + StateMachine (business logic)
    // - View: WinForms DataGridView (UI display)
    // - Controller: Form1 (orchestrate Model & View interaction)
    //
    // Delegation Pattern:
    // - Form1 TIDAK handle status logic sendiri
    // - DELEGATE ke StateMachine.EvaluateStatus()
    // - DELEGATE ke StateMachine.ApplyStatusColors()
    // - DELEGATE ke StateMachine.ShowNotifications()
    //
    // BENEFIT:
    // - Separation of concerns: UI logic terpisah dari business logic
    // - Mudah test business logic tanpa UI
    // - Mudah ganti UI tanpa ubah business logic
    // ============================================================
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // [CLEAN CODE] Data di level Form untuk local scope
        // [STANDARD CODE] Sample data untuk demo/testing
        List<Obat> daftarObat = new List<Obat>()
        {
            new Obat("Paracetamol", 21, 5000, new DateTime(2025, 6, 15)),
            new Obat("Ibuprofen", 17, 7000, new DateTime(2025, 8, 20)),
            new Obat("Sanmol", 5, 3000, new DateTime(2024, 12, 31)),
            new Obat("HRIG", 3, 20000, new DateTime(2024, 11, 10)),
            new Obat("Influenza", 15, 2000, new DateTime(2025, 3, 15)),
            new Obat("Jane Doe", 50, 500000, new DateTime(2026, 1, 1))
        };

        // [METHOD] Load data saat form initialize
        // [CLEAN CODE] Simple orchestration: load data, show data, show notifications
        private void Form1_Load(object sender, EventArgs e)
        {
            TampilkanData(daftarObat);
            TampilkanNotifikasi();
        }

        // [METHOD] Tampilkan data obat di DataGridView
        // [MVC CONTROLLER] Orchestrate Model (Obat) & View (DataGridView)
        private void TampilkanData(List<Obat> data)
        {
            // [DELEGATION PATTERN] Update status via Obat.UpdateStatus()
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

            // [DELEGATION PATTERN] Terapkan warna via StateMachine (business logic terpisah)
            TerapkanWarnaStatus();
        }

        // [METHOD] Terapkan warna status ke grid
        // [DELEGATION PATTERN] DELEGATE ke StateMachine, Form hanya call
        // [CLEAN CODE] Single Responsibility: hanya call StateMachine
        private void TerapkanWarnaStatus()
        {
            // [CLEAN CODE] Named parameter untuk clarity
            StateMachine.ApplyStatusColors(tblObat, statusColumnIndex: 4);
        }

        // [METHOD] Tampilkan notifikasi & statistik
        // [DELEGATION PATTERN] DELEGATE ke StateMachine & format title
        private void TampilkanNotifikasi()
        {
            // [DELEGATION PATTERN] Notifikasi logic di StateMachine
            StateMachine.ShowNotifications(daftarObat);
            TampilkanStatistik();
        }

        // [METHOD] Format & tampilkan statistik di title
        // [CLEAN CODE] Single line assignment menggunakan StateMachine helper
        private void TampilkanStatistik()
        {
            // [DELEGATION PATTERN] Format logic di StateMachine
            this.Text = StateMachine.FormatTitleWithStats("Form1", daftarObat);
        }


        // [METHOD] Search obat berdasarkan nama
        // [CLEAN CODE] Guard clause di awal
        // [CLEAN CODE] LINQ-like logic (filtering)
        private void button1_Click(object sender, EventArgs e)
        {
            string inputan = textBox1.Text.ToLower();
            // [SECURE CODE] Validate input tidak kosong
            if(inputan == "")
            {
                MessageBox.Show("Masukan Nama Obat Dulu");
                return;
            }
            List<Obat> hasil = new List<Obat>();

            // [CLEAN CODE] Simple loop untuk filter
            for(int i = 0; i < daftarObat.Count; i++)
            {
                // [CLEAN CODE] Case-insensitive search
                if (daftarObat[i].nama.ToLower().Contains(inputan))
                {
                    hasil.Add(daftarObat[i]);
                }
            }
            // [CLEAN CODE] Check jika hasil ada sebelum tampilkan
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
