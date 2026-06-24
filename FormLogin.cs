using System;
using System.Windows.Forms;

namespace TubesKPL
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string inputUsername = txtUsername.Text;
            string inputPassword = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(inputUsername) || string.IsNullOrWhiteSpace(inputPassword))
            {
                MessageBox.Show("Username dan Password harus diisi!", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var client = new ObatApiClient())
                {
                    var user = await client.LoginAsync(inputUsername, inputPassword);
                    
                    Console.WriteLine($"[LOGIN ATTEMPT] Waktu: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Username: {inputUsername} | Status: BERHASIL");

                    MessageBox.Show($"Login Berhasil!\nSelamat datang, {user.Nama} ({user.Role})",
                                    "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();
                    Form1 formUtama = new Form1();
                    formUtama.ShowDialog();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOGIN ATTEMPT] Waktu: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Username: {inputUsername} | Status: GAGAL | Error: {ex.Message}");
                MessageBox.Show($"Login Gagal: {ex.Message}", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtUsername.Focus();
            }
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }
    }
}