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
            this.SuspendLayout();
            // 
            // BoxObat
            // 
            this.BoxObat.FormattingEnabled = true;
            this.BoxObat.Location = new System.Drawing.Point(123, 41);
            this.BoxObat.Name = "BoxObat";
            this.BoxObat.Size = new System.Drawing.Size(155, 24);
            this.BoxObat.TabIndex = 0;
            this.BoxObat.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 44);
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
            this.TBjumlah.Multiline = true;
            this.TBjumlah.Name = "TBjumlah";
            this.TBjumlah.Size = new System.Drawing.Size(155, 22);
            this.TBjumlah.TabIndex = 3;
            this.TBjumlah.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // BtnHitung
            // 
            this.BtnHitung.Location = new System.Drawing.Point(184, 120);
            this.BtnHitung.Name = "BtnHitung";
            this.BtnHitung.Size = new System.Drawing.Size(94, 26);
            this.BtnHitung.TabIndex = 4;
            this.BtnHitung.Text = "Hitung ";
            this.BtnHitung.UseVisualStyleBackColor = true;
            this.BtnHitung.Click += new System.EventHandler(this.BtnHitung_Click);
            // 
            // FormTransaksi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.BtnHitung);
            this.Controls.Add(this.TBjumlah);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BoxObat);
            this.Name = "FormTransaksi";
            this.Text = "Form3";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox BoxObat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TBjumlah;
        private System.Windows.Forms.Button BtnHitung;
    }
}