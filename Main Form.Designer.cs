namespace TubesKPL
{
    partial class Form1
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
            this.tblObat = new System.Windows.Forms.DataGridView();
            this.txtCariInputan = new System.Windows.Forms.TextBox();
            this.btnCariObat = new System.Windows.Forms.Button();
            this.btnTambahObat = new System.Windows.Forms.Button();
            this.btnHapus = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btnTransaksi = new System.Windows.Forms.Button();
            this.btnHistory = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tblObat)).BeginInit();
            this.SuspendLayout();
            // 
            // tblObat
            // 
            this.tblObat.AllowUserToOrderColumns = true;
            this.tblObat.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tblObat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tblObat.Location = new System.Drawing.Point(28, 62);
            this.tblObat.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tblObat.Name = "tblObat";
            this.tblObat.ReadOnly = true;
            this.tblObat.RowHeadersWidth = 51;
            this.tblObat.RowTemplate.Height = 24;
            this.tblObat.Size = new System.Drawing.Size(522, 191);
            this.tblObat.TabIndex = 0;
            this.tblObat.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // txtCariInputan
            // 
            this.txtCariInputan.Location = new System.Drawing.Point(91, 36);
            this.txtCariInputan.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtCariInputan.Multiline = true;
            this.txtCariInputan.Name = "txtCariInputan";
            this.txtCariInputan.Size = new System.Drawing.Size(148, 22);
            this.txtCariInputan.TabIndex = 1;
            this.txtCariInputan.TextChanged += new System.EventHandler(this.txtCariInputan_TextChanged);
            // 
            // btnCariObat
            // 
            this.btnCariObat.Location = new System.Drawing.Point(243, 36);
            this.btnCariObat.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCariObat.Name = "btnCariObat";
            this.btnCariObat.Size = new System.Drawing.Size(43, 24);
            this.btnCariObat.TabIndex = 2;
            this.btnCariObat.Text = "Cari";
            this.btnCariObat.UseVisualStyleBackColor = true;
            this.btnCariObat.Click += new System.EventHandler(this.btnCariObat_Click);
            // 
            // btnTambahObat
            // 
            this.btnTambahObat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnTambahObat.Location = new System.Drawing.Point(343, 258);
            this.btnTambahObat.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnTambahObat.Name = "btnTambahObat";
            this.btnTambahObat.Size = new System.Drawing.Size(66, 23);
            this.btnTambahObat.TabIndex = 3;
            this.btnTambahObat.Text = "Tambah";
            this.btnTambahObat.UseVisualStyleBackColor = false;
            this.btnTambahObat.Click += new System.EventHandler(this.btnTambahObat_Click);
            // 
            // btnHapus
            // 
            this.btnHapus.BackColor = System.Drawing.Color.LightCoral;
            this.btnHapus.Location = new System.Drawing.Point(484, 258);
            this.btnHapus.Name = "btnHapus";
            this.btnHapus.Size = new System.Drawing.Size(66, 23);
            this.btnHapus.TabIndex = 4;
            this.btnHapus.Text = "Hapus";
            this.btnHapus.UseVisualStyleBackColor = false;
            this.btnHapus.Click += new System.EventHandler(this.btnHapus_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnUpdate.Location = new System.Drawing.Point(413, 258);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(66, 23);
            this.btnUpdate.TabIndex = 5;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(42, 9);
            this.button4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(6, 6);
            this.button4.TabIndex = 6;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // btnTransaksi
            // 
            this.btnTransaksi.Location = new System.Drawing.Point(27, 353);
            this.btnTransaksi.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnTransaksi.Name = "btnTransaksi";
            this.btnTransaksi.Size = new System.Drawing.Size(117, 21);
            this.btnTransaksi.TabIndex = 7;
            this.btnTransaksi.Text = "Transaksi";
            this.btnTransaksi.UseVisualStyleBackColor = true;
            this.btnTransaksi.Click += new System.EventHandler(this.btnTransaksi_Click);
            // 
            // btnHistory
            // 
            this.btnHistory.Location = new System.Drawing.Point(148, 353);
            this.btnHistory.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(116, 21);
            this.btnHistory.TabIndex = 8;
            this.btnHistory.Text = "Histori";
            this.btnHistory.UseVisualStyleBackColor = true;
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Nama Obat";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(342, 283);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(208, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "*Pilih Obat terlebih dahulu baru di eksekusi";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 410);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnTransaksi);
            this.Controls.Add(this.btnHistory);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnHapus);
            this.Controls.Add(this.btnTambahObat);
            this.Controls.Add(this.btnCariObat);
            this.Controls.Add(this.txtCariInputan);
            this.Controls.Add(this.tblObat);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tblObat)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView tblObat;
        private System.Windows.Forms.TextBox txtCariInputan;
        private System.Windows.Forms.Button btnCariObat;
        private System.Windows.Forms.Button btnTambahObat;
        private System.Windows.Forms.Button btnHapus;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnTransaksi;
        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

