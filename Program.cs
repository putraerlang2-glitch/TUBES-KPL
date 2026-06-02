using System;
using System.Net;
using System.Windows.Forms;

namespace TubesKPL
{
    internal static class Program
    {
        /// <summary>
        /// Main entry point aplikasi
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Handle HTTPS certificate untuk development ASP.NET Core API
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new FormLogin());
        }
    }
}