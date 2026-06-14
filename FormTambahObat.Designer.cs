namespace TubesKPL
{
    partial class FormTambahObat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblNama = new System.Windows.Forms.Label();
            this.lblStok = new System.Windows.Forms.Label();
            this.lblHarga = new System.Windows.Forms.Label();
            this.lblKategori = new System.Windows.Forms.Label();
            this.lblExpired = new System.Windows.Forms.Label();

            this.txtNamaObat = new System.Windows.Forms.TextBox();
            this.txtStok = new System.Windows.Forms.TextBox();
            this.txtHarga = new System.Windows.Forms.TextBox();

            this.cmbKategori = new System.Windows.Forms.ComboBox();
            this.dtpExpired = new System.Windows.Forms.DateTimePicker();

            this.btnSimpan = new System.Windows.Forms.Button();
            this.btnBatal = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // 
            // lblNama
            // 
            this.lblNama.AutoSize = true;
            this.lblNama.Location = new System.Drawing.Point(80, 60);
            this.lblNama.Name = "lblNama";
            this.lblNama.Size = new System.Drawing.Size(87, 16);
            this.lblNama.TabIndex = 0;
            this.lblNama.Text = "Nama Obat :";

            // 
            // lblStok
            // 
            this.lblStok.AutoSize = true;
            this.lblStok.Location = new System.Drawing.Point(80, 100);
            this.lblStok.Name = "lblStok";
            this.lblStok.Size = new System.Drawing.Size(43, 16);
            this.lblStok.TabIndex = 1;
            this.lblStok.Text = "Stok :";

            // 
            // lblHarga
            // 
            this.lblHarga.AutoSize = true;
            this.lblHarga.Location = new System.Drawing.Point(80, 140);
            this.lblHarga.Name = "lblHarga";
            this.lblHarga.Size = new System.Drawing.Size(50, 16);
            this.lblHarga.TabIndex = 2;
            this.lblHarga.Text = "Harga :";

            // 
            // lblKategori
            // 
            this.lblKategori.AutoSize = true;
            this.lblKategori.Location = new System.Drawing.Point(80, 180);
            this.lblKategori.Name = "lblKategori";
            this.lblKategori.Size = new System.Drawing.Size(64, 16);
            this.lblKategori.TabIndex = 3;
            this.lblKategori.Text = "Kategori :";

            // 
            // lblExpired
            // 
            this.lblExpired.AutoSize = true;
            this.lblExpired.Location = new System.Drawing.Point(80, 220);
            this.lblExpired.Name = "lblExpired";
            this.lblExpired.Size = new System.Drawing.Size(105, 16);
            this.lblExpired.TabIndex = 4;
            this.lblExpired.Text = "Tanggal Expired :";

            // 
            // txtNamaObat
            // 
            this.txtNamaObat.Location = new System.Drawing.Point(220, 57);
            this.txtNamaObat.Name = "txtNamaObat";
            this.txtNamaObat.Size = new System.Drawing.Size(220, 22);
            this.txtNamaObat.TabIndex = 5;

            // 
            // txtStok
            // 
            this.txtStok.Location = new System.Drawing.Point(220, 97);
            this.txtStok.Name = "txtStok";
            this.txtStok.Size = new System.Drawing.Size(220, 22);
            this.txtStok.TabIndex = 6;

            // 
            // txtHarga
            // 
            this.txtHarga.Location = new System.Drawing.Point(220, 137);
            this.txtHarga.Name = "txtHarga";
            this.txtHarga.Size = new System.Drawing.Size(220, 22);
            this.txtHarga.TabIndex = 7;

            // 
            // cmbKategori
            // 
            this.cmbKategori.FormattingEnabled = true;
            this.cmbKategori.Location = new System.Drawing.Point(220, 177);
            this.cmbKategori.Name = "cmbKategori";
            this.cmbKategori.Size = new System.Drawing.Size(220, 24);
            this.cmbKategori.TabIndex = 8;

            // 
            // dtpExpired
            // 
            this.dtpExpired.Location = new System.Drawing.Point(220, 217);
            this.dtpExpired.Name = "dtpExpired";
            this.dtpExpired.Size = new System.Drawing.Size(220, 22);
            this.dtpExpired.TabIndex = 9;

            // 
            // btnSimpan
            // 
            this.btnSimpan.Location = new System.Drawing.Point(120, 290);
            this.btnSimpan.Name = "btnSimpan";
            this.btnSimpan.Size = new System.Drawing.Size(100, 35);
            this.btnSimpan.TabIndex = 10;
            this.btnSimpan.Text = "Simpan";
            this.btnSimpan.UseVisualStyleBackColor = true;
            this.btnSimpan.Click += new System.EventHandler(this.btnSimpan_Click);

            // 
            // btnBatal
            // 
            this.btnBatal.Location = new System.Drawing.Point(300, 290);
            this.btnBatal.Name = "btnBatal";
            this.btnBatal.Size = new System.Drawing.Size(100, 35);
            this.btnBatal.TabIndex = 11;
            this.btnBatal.Text = "Batal";
            this.btnBatal.UseVisualStyleBackColor = true;
            this.btnBatal.Click += new System.EventHandler(this.btnBatal_Click);

            // 
            // FormTambahObat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 380);

            this.Controls.Add(this.lblNama);
            this.Controls.Add(this.lblStok);
            this.Controls.Add(this.lblHarga);
            this.Controls.Add(this.lblKategori);
            this.Controls.Add(this.lblExpired);

            this.Controls.Add(this.txtNamaObat);
            this.Controls.Add(this.txtStok);
            this.Controls.Add(this.txtHarga);

            this.Controls.Add(this.cmbKategori);
            this.Controls.Add(this.dtpExpired);

            this.Controls.Add(this.btnSimpan);
            this.Controls.Add(this.btnBatal);

            this.Name = "FormTambahObat";
            this.Text = "Tambah Obat";

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblNama;
        private System.Windows.Forms.Label lblStok;
        private System.Windows.Forms.Label lblHarga;
        private System.Windows.Forms.Label lblKategori;
        private System.Windows.Forms.Label lblExpired;

        private System.Windows.Forms.TextBox txtNamaObat;
        private System.Windows.Forms.TextBox txtStok;
        private System.Windows.Forms.TextBox txtHarga;

        private System.Windows.Forms.ComboBox cmbKategori;
        private System.Windows.Forms.DateTimePicker dtpExpired;

        private System.Windows.Forms.Button btnSimpan;
        private System.Windows.Forms.Button btnBatal;
    }
}
