namespace TubesKPL
{
    partial class FormTransaksi
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
            this.BoxObat = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TBjumlah = new System.Windows.Forms.TextBox();
            this.BtnHitung = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.TabelKeranjang = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TBUangBayar = new System.Windows.Forms.TextBox();
            this.BtnUbahConfig = new System.Windows.Forms.Button();
            this.TBPajak = new System.Windows.Forms.TextBox();
            this.TBDiskon = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.BtnHapus = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.TabelKeranjang)).BeginInit();
            this.SuspendLayout();
            // 
            // BoxObat
            // 
            this.BoxObat.FormattingEnabled = true;
            this.BoxObat.Location = new System.Drawing.Point(123, 41);
            this.BoxObat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BoxObat.Name = "BoxObat";
            this.BoxObat.Size = new System.Drawing.Size(155, 24);
            this.BoxObat.TabIndex = 0;
            this.BoxObat.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Cari Obat";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Jumlah Obat";
            // 
            // TBjumlah
            // 
            this.TBjumlah.Location = new System.Drawing.Point(123, 79);
            this.TBjumlah.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TBjumlah.Multiline = true;
            this.TBjumlah.Name = "TBjumlah";
            this.TBjumlah.Size = new System.Drawing.Size(155, 22);
            this.TBjumlah.TabIndex = 3;
            this.TBjumlah.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // BtnHitung
            // 
            this.BtnHitung.Location = new System.Drawing.Point(184, 210);
            this.BtnHitung.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BtnHitung.Name = "BtnHitung";
            this.BtnHitung.Size = new System.Drawing.Size(93, 26);
            this.BtnHitung.TabIndex = 4;
            this.BtnHitung.Text = "Checkout";
            this.BtnHitung.UseVisualStyleBackColor = true;
            this.BtnHitung.Click += new System.EventHandler(this.BtnHitung_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(133, 169);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(145, 26);
            this.button1.TabIndex = 5;
            this.button1.Text = "Tambah Pesanan";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TabelKeranjang
            // 
            this.TabelKeranjang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TabelKeranjang.Location = new System.Drawing.Point(309, 43);
            this.TabelKeranjang.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TabelKeranjang.Name = "TabelKeranjang";
            this.TabelKeranjang.RowHeadersWidth = 51;
            this.TabelKeranjang.RowTemplate.Height = 24;
            this.TabelKeranjang.Size = new System.Drawing.Size(479, 193);
            this.TabelKeranjang.TabIndex = 6;
            this.TabelKeranjang.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(315, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Keranjang";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(310, 254);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Total: Rp.0";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // TBUangBayar
            // 
            this.TBUangBayar.Location = new System.Drawing.Point(123, 124);
            this.TBUangBayar.Margin = new System.Windows.Forms.Padding(4);
            this.TBUangBayar.Name = "TBUangBayar";
            this.TBUangBayar.Size = new System.Drawing.Size(155, 22);
            this.TBUangBayar.TabIndex = 9;
            // 
            // BtnUbahConfig
            // 
            this.BtnUbahConfig.Location = new System.Drawing.Point(514, 247);
            this.BtnUbahConfig.Margin = new System.Windows.Forms.Padding(4);
            this.BtnUbahConfig.Name = "BtnUbahConfig";
            this.BtnUbahConfig.Size = new System.Drawing.Size(100, 28);
            this.BtnUbahConfig.TabIndex = 10;
            this.BtnUbahConfig.Text = "Ubah Config";
            this.BtnUbahConfig.UseVisualStyleBackColor = true;
            this.BtnUbahConfig.Click += new System.EventHandler(this.BtnUbahConfig_Click);
            // 
            // TBPajak
            // 
            this.TBPajak.Location = new System.Drawing.Point(481, 283);
            this.TBPajak.Margin = new System.Windows.Forms.Padding(4);
            this.TBPajak.Name = "TBPajak";
            this.TBPajak.Size = new System.Drawing.Size(132, 22);
            this.TBPajak.TabIndex = 11;
            // 
            // TBDiskon
            // 
            this.TBDiskon.Location = new System.Drawing.Point(479, 316);
            this.TBDiskon.Margin = new System.Windows.Forms.Padding(4);
            this.TBDiskon.Name = "TBDiskon";
            this.TBDiskon.Size = new System.Drawing.Size(132, 22);
            this.TBDiskon.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(314, 290);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "Pajak % :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(313, 324);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 16);
            this.label6.TabIndex = 14;
            this.label6.Text = "Diskon % :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(37, 128);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 16);
            this.label7.TabIndex = 15;
            this.label7.Text = "Uang Tunai";
            // 
            // BtnHapus
            // 
            this.BtnHapus.Location = new System.Drawing.Point(184, 240);
            this.BtnHapus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BtnHapus.Name = "BtnHapus";
            this.BtnHapus.Size = new System.Drawing.Size(93, 26);
            this.BtnHapus.TabIndex = 16;
            this.BtnHapus.Text = "Hapus";
            this.BtnHapus.UseVisualStyleBackColor = true;
            this.BtnHapus.Click += new System.EventHandler(this.BtnHapus_Click);
            // 
            // FormTransaksi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.BtnHapus);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TBDiskon);
            this.Controls.Add(this.TBPajak);
            this.Controls.Add(this.BtnUbahConfig);
            this.Controls.Add(this.TBUangBayar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TabelKeranjang);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.BtnHitung);
            this.Controls.Add(this.TBjumlah);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BoxObat);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormTransaksi";
            this.Text = "Form3";
            ((System.ComponentModel.ISupportInitialize)(this.TabelKeranjang)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox BoxObat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TBjumlah;
        private System.Windows.Forms.Button BtnHitung;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView TabelKeranjang;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TBUangBayar;
        private System.Windows.Forms.Button BtnUbahConfig;
        private System.Windows.Forms.TextBox TBPajak;
        private System.Windows.Forms.TextBox TBDiskon;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button BtnHapus;
    }
}