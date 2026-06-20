namespace TubesKPL
{
    partial class FormUpdateObat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
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
            this.lblNama.Location = new System.Drawing.Point(220, 94);
            this.lblNama.Name = "lblNama";
            this.lblNama.Size = new System.Drawing.Size(82, 16);
            this.lblNama.TabIndex = 12;
            this.lblNama.Text = "Nama Obat :";
            // 
            // lblStok
            // 
            this.lblStok.AutoSize = true;
            this.lblStok.Location = new System.Drawing.Point(220, 134);
            this.lblStok.Name = "lblStok";
            this.lblStok.Size = new System.Drawing.Size(40, 16);
            this.lblStok.TabIndex = 13;
            this.lblStok.Text = "Stok :";
            // 
            // lblHarga
            // 
            this.lblHarga.AutoSize = true;
            this.lblHarga.Location = new System.Drawing.Point(220, 174);
            this.lblHarga.Name = "lblHarga";
            this.lblHarga.Size = new System.Drawing.Size(51, 16);
            this.lblHarga.TabIndex = 14;
            this.lblHarga.Text = "Harga :";
            // 
            // lblKategori
            // 
            this.lblKategori.AutoSize = true;
            this.lblKategori.Location = new System.Drawing.Point(220, 214);
            this.lblKategori.Name = "lblKategori";
            this.lblKategori.Size = new System.Drawing.Size(63, 16);
            this.lblKategori.TabIndex = 15;
            this.lblKategori.Text = "Kategori :";
            // 
            // lblExpired
            // 
            this.lblExpired.AutoSize = true;
            this.lblExpired.Location = new System.Drawing.Point(220, 254);
            this.lblExpired.Name = "lblExpired";
            this.lblExpired.Size = new System.Drawing.Size(113, 16);
            this.lblExpired.TabIndex = 16;
            this.lblExpired.Text = "Tanggal Expired :";
            // 
            // txtNamaObat
            // 
            this.txtNamaObat.Location = new System.Drawing.Point(360, 91);
            this.txtNamaObat.Name = "txtNamaObat";
            this.txtNamaObat.Size = new System.Drawing.Size(220, 22);
            this.txtNamaObat.TabIndex = 17;
            // 
            // txtStok
            // 
            this.txtStok.Location = new System.Drawing.Point(360, 131);
            this.txtStok.Name = "txtStok";
            this.txtStok.Size = new System.Drawing.Size(220, 22);
            this.txtStok.TabIndex = 18;
            // 
            // txtHarga
            // 
            this.txtHarga.Location = new System.Drawing.Point(360, 171);
            this.txtHarga.Name = "txtHarga";
            this.txtHarga.Size = new System.Drawing.Size(220, 22);
            this.txtHarga.TabIndex = 19;
            // 
            // cmbKategori
            // 
            this.cmbKategori.FormattingEnabled = true;
            this.cmbKategori.Location = new System.Drawing.Point(360, 211);
            this.cmbKategori.Name = "cmbKategori";
            this.cmbKategori.Size = new System.Drawing.Size(220, 24);
            this.cmbKategori.TabIndex = 20;
            // 
            // dtpExpired
            // 
            this.dtpExpired.Location = new System.Drawing.Point(360, 251);
            this.dtpExpired.Name = "dtpExpired";
            this.dtpExpired.Size = new System.Drawing.Size(220, 22);
            this.dtpExpired.TabIndex = 21;
            // 
            // btnSimpan
            // 
            this.btnSimpan.Location = new System.Drawing.Point(260, 324);
            this.btnSimpan.Name = "btnSimpan";
            this.btnSimpan.Size = new System.Drawing.Size(100, 35);
            this.btnSimpan.TabIndex = 22;
            this.btnSimpan.Text = "Simpan";
            this.btnSimpan.UseVisualStyleBackColor = true;
            this.btnSimpan.Click += new System.EventHandler(this.btnSimpan_Click);
            // 
            // btnBatal
            // 
            this.btnBatal.Location = new System.Drawing.Point(440, 324);
            this.btnBatal.Name = "btnBatal";
            this.btnBatal.Size = new System.Drawing.Size(100, 35);
            this.btnBatal.TabIndex = 23;
            this.btnBatal.Text = "Batal";
            this.btnBatal.UseVisualStyleBackColor = true;
            this.btnBatal.Click += new System.EventHandler(this.btnBatal_Click);
            // 
            // FormUpdateObat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
            this.Name = "FormUpdateObat";
            this.Text = "Form Update Obat";
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