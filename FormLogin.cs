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
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        List<Akun> daftarAkun = new List<Akun>()
        {
            new Akun("admin", "admin123", "Admin Utama"),
            new Akun("langga", "langga123", "Apoteker1"),
            new Akun("kasir", "kasir123", "Apoteker2")
        };

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string inputUsername = txtUsername.Text;
            string inputPassword = txtPassword.Text;

            Akun akunValid = daftarAkun.FirstOrDefault(akun => akun.Username == inputUsername && akun.Password == inputPassword);

            if (akunValid != null)
            {
                MessageBox.Show($"Login Berhasil!\nSelamat datang, {akunValid.Username} ({akunValid.Role})",
                                "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Hide();
                Form1 formUtama = new Form1();
                formUtama.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Username atau Password salah!", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtUsername.Focus();
            }
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class Akun
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public Akun(string username, string password, string role)
        {
            Username = username;
            Password = password;
            Role = role;
        }
    }
}