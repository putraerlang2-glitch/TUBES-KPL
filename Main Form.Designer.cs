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
            ((System.ComponentModel.ISupportInitialize)(this.tblObat)).BeginInit();
            this.SuspendLayout();
            // 
            // tblObat
            // 
            this.tblObat.AllowUserToOrderColumns = true;
            this.tblObat.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tblObat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tblObat.Location = new System.Drawing.Point(37, 30);
            this.tblObat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tblObat.Name = "tblObat";
            this.tblObat.ReadOnly = true;
            this.tblObat.RowHeadersWidth = 51;
            this.tblObat.RowTemplate.Height = 24;
            this.tblObat.Size = new System.Drawing.Size(696, 235);
            this.tblObat.TabIndex = 0;
            this.tblObat.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // txtCariInputan
            // 
            this.txtCariInputan.Location = new System.Drawing.Point(37, 284);
            this.txtCariInputan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCariInputan.Multiline = true;
            this.txtCariInputan.Name = "txtCariInputan";
            this.txtCariInputan.Size = new System.Drawing.Size(326, 26);
            this.txtCariInputan.TabIndex = 1;
            this.txtCariInputan.TextChanged += new System.EventHandler(this.txtCariInputan_TextChanged);
            // 
            // btnCariObat
            // 
            this.btnCariObat.Location = new System.Drawing.Point(37, 316);
            this.btnCariObat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCariObat.Name = "btnCariObat";
            this.btnCariObat.Size = new System.Drawing.Size(326, 30);
            this.btnCariObat.TabIndex = 2;
            this.btnCariObat.Text = "Cari";
            this.btnCariObat.UseVisualStyleBackColor = true;
            this.btnCariObat.Click += new System.EventHandler(this.btnCariObat_Click);
            // 
            // btnTambahObat
            // 
            this.btnTambahObat.Location = new System.Drawing.Point(37, 350);
            this.btnTambahObat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTambahObat.Name = "btnTambahObat";
            this.btnTambahObat.Size = new System.Drawing.Size(326, 30);
            this.btnTambahObat.TabIndex = 3;
            this.btnTambahObat.Text = "Tambah";
            this.btnTambahObat.UseVisualStyleBackColor = true;
            this.btnTambahObat.Click += new System.EventHandler(this.btnTambahObat_Click);
            // 
            // btnHapus
            // 
            this.btnHapus.BackColor = System.Drawing.Color.LightCoral;
            this.btnHapus.Location = new System.Drawing.Point(391, 284);
            this.btnHapus.Margin = new System.Windows.Forms.Padding(4);
            this.btnHapus.Name = "btnHapus";
            this.btnHapus.Size = new System.Drawing.Size(249, 28);
            this.btnHapus.TabIndex = 4;
            this.btnHapus.Text = "Hapus";
            this.btnHapus.UseVisualStyleBackColor = false;
            this.btnHapus.Click += new System.EventHandler(this.btnHapus_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(37, 385);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(326, 26);
            this.btnUpdate.TabIndex = 5;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(56, 11);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(8, 8);
            this.button4.TabIndex = 6;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // btnTransaksi
            // 
            this.btnTransaksi.Location = new System.Drawing.Point(391, 320);
            this.btnTransaksi.Name = "btnTransaksi";
            this.btnTransaksi.Size = new System.Drawing.Size(249, 26);
            this.btnTransaksi.TabIndex = 7;
            this.btnTransaksi.Text = "Transaksi";
            this.btnTransaksi.UseVisualStyleBackColor = true;
            this.btnTransaksi.Click += new System.EventHandler(this.btnTransaksi_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 505);
            this.Controls.Add(this.btnTransaksi);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnHapus);
            this.Controls.Add(this.btnTambahObat);
            this.Controls.Add(this.btnCariObat);
            this.Controls.Add(this.txtCariInputan);
            this.Controls.Add(this.tblObat);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
    }
}

