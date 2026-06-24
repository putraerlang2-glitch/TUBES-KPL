
namespace TubesKPL
{
    partial class FormHistory
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
            this.tblHistory = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblNoStruk = new System.Windows.Forms.Label();
            this.txtNoStruk = new System.Windows.Forms.TextBox();
            this.lblNamaObat = new System.Windows.Forms.Label();
            this.txtNamaObat = new System.Windows.Forms.TextBox();
            this.lblNamaKasir = new System.Windows.Forms.Label();
            this.txtNamaKasir = new System.Windows.Forms.TextBox();
            this.lblTanggalFrom = new System.Windows.Forms.Label();
            this.dtpTanggalFrom = new System.Windows.Forms.DateTimePicker();
            this.lblTanggalTo = new System.Windows.Forms.Label();
            this.dtpTanggalTo = new System.Windows.Forms.DateTimePicker();
            this.btnCari = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tblHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // tblHistory
            // 
            this.tblHistory.AllowUserToAddRows = false;
            this.tblHistory.AllowUserToDeleteRows = false;
            this.tblHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.tblHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tblHistory.Location = new System.Drawing.Point(12, 80);
            this.tblHistory.Name = "tblHistory";
            this.tblHistory.ReadOnly = true;
            this.tblHistory.RowHeadersWidth = 51;
            this.tblHistory.RowTemplate.Height = 24;
            this.tblHistory.Size = new System.Drawing.Size(1160, 320);
            this.tblHistory.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 410);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 30);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(1052, 410);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 30);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblNoStruk
            // 
            this.lblNoStruk.AutoSize = true;
            this.lblNoStruk.Location = new System.Drawing.Point(12, 15);
            this.lblNoStruk.Name = "lblNoStruk";
            this.lblNoStruk.Size = new System.Drawing.Size(55, 17);
            this.lblNoStruk.TabIndex = 3;
            this.lblNoStruk.Text = "No Struk";
            // 
            // txtNoStruk
            // 
            this.txtNoStruk.Location = new System.Drawing.Point(73, 12);
            this.txtNoStruk.Name = "txtNoStruk";
            this.txtNoStruk.Size = new System.Drawing.Size(150, 22);
            this.txtNoStruk.TabIndex = 4;
            // 
            // lblNamaObat
            // 
            this.lblNamaObat.AutoSize = true;
            this.lblNamaObat.Location = new System.Drawing.Point(240, 15);
            this.lblNamaObat.Name = "lblNamaObat";
            this.lblNamaObat.Size = new System.Drawing.Size(72, 17);
            this.lblNamaObat.TabIndex = 5;
            this.lblNamaObat.Text = "Nama Obat";
            // 
            // txtNamaObat
            // 
            this.txtNamaObat.Location = new System.Drawing.Point(318, 12);
            this.txtNamaObat.Name = "txtNamaObat";
            this.txtNamaObat.Size = new System.Drawing.Size(150, 22);
            this.txtNamaObat.TabIndex = 6;
            // 
            // lblNamaKasir
            // 
            this.lblNamaKasir.AutoSize = true;
            this.lblNamaKasir.Location = new System.Drawing.Point(485, 15);
            this.lblNamaKasir.Name = "lblNamaKasir";
            this.lblNamaKasir.Size = new System.Drawing.Size(74, 17);
            this.lblNamaKasir.TabIndex = 7;
            this.lblNamaKasir.Text = "Nama Kasir";
            // 
            // txtNamaKasir
            // 
            this.txtNamaKasir.Location = new System.Drawing.Point(565, 12);
            this.txtNamaKasir.Name = "txtNamaKasir";
            this.txtNamaKasir.Size = new System.Drawing.Size(150, 22);
            this.txtNamaKasir.TabIndex = 8;
            // 
            // lblTanggalFrom
            // 
            this.lblTanggalFrom.AutoSize = true;
            this.lblTanggalFrom.Location = new System.Drawing.Point(12, 45);
            this.lblTanggalFrom.Name = "lblTanggalFrom";
            this.lblTanggalFrom.Size = new System.Drawing.Size(84, 17);
            this.lblTanggalFrom.TabIndex = 9;
            this.lblTanggalFrom.Text = "Tanggal Dari";
            // 
            // dtpTanggalFrom
            // 
            this.dtpTanggalFrom.Checked = false;
            this.dtpTanggalFrom.Location = new System.Drawing.Point(102, 42);
            this.dtpTanggalFrom.Name = "dtpTanggalFrom";
            this.dtpTanggalFrom.ShowCheckBox = true;
            this.dtpTanggalFrom.Size = new System.Drawing.Size(200, 22);
            this.dtpTanggalFrom.TabIndex = 10;
            // 
            // lblTanggalTo
            // 
            this.lblTanggalTo.AutoSize = true;
            this.lblTanggalTo.Location = new System.Drawing.Point(320, 45);
            this.lblTanggalTo.Name = "lblTanggalTo";
            this.lblTanggalTo.Size = new System.Drawing.Size(62, 17);
            this.lblTanggalTo.TabIndex = 11;
            this.lblTanggalTo.Text = "Sampai Ke";
            // 
            // dtpTanggalTo
            // 
            this.dtpTanggalTo.Checked = false;
            this.dtpTanggalTo.Location = new System.Drawing.Point(388, 42);
            this.dtpTanggalTo.Name = "dtpTanggalTo";
            this.dtpTanggalTo.ShowCheckBox = true;
            this.dtpTanggalTo.Size = new System.Drawing.Size(200, 22);
            this.dtpTanggalTo.TabIndex = 12;
            // 
            // btnCari
            // 
            this.btnCari.Location = new System.Drawing.Point(610, 37);
            this.btnCari.Name = "btnCari";
            this.btnCari.Size = new System.Drawing.Size(100, 30);
            this.btnCari.TabIndex = 13;
            this.btnCari.Text = "Cari";
            this.btnCari.UseVisualStyleBackColor = true;
            this.btnCari.Click += new System.EventHandler(this.btnCari_Click);
            // 
            // FormHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 450);
            this.Controls.Add(this.btnCari);
            this.Controls.Add(this.dtpTanggalTo);
            this.Controls.Add(this.lblTanggalTo);
            this.Controls.Add(this.dtpTanggalFrom);
            this.Controls.Add(this.lblTanggalFrom);
            this.Controls.Add(this.txtNamaKasir);
            this.Controls.Add(this.lblNamaKasir);
            this.Controls.Add(this.txtNamaObat);
            this.Controls.Add(this.lblNamaObat);
            this.Controls.Add(this.txtNoStruk);
            this.Controls.Add(this.lblNoStruk);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.tblHistory);
            this.Name = "FormHistory";
            this.Text = "Histori";
            ((System.ComponentModel.ISupportInitialize)(this.tblHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView tblHistory;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblNoStruk;
        private System.Windows.Forms.TextBox txtNoStruk;
        private System.Windows.Forms.Label lblNamaObat;
        private System.Windows.Forms.TextBox txtNamaObat;
        private System.Windows.Forms.Label lblNamaKasir;
        private System.Windows.Forms.TextBox txtNamaKasir;
        private System.Windows.Forms.Label lblTanggalFrom;
        private System.Windows.Forms.DateTimePicker dtpTanggalFrom;
        private System.Windows.Forms.Label lblTanggalTo;
        private System.Windows.Forms.DateTimePicker dtpTanggalTo;
        private System.Windows.Forms.Button btnCari;
    }
}
