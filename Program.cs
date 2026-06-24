using System;
using System.Net;
using System.Windows.Forms;

namespace TubesKPL
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Allow self-signed certificates for development only
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) =>
                {
                    // ⚠️ Only for development - check certificate CN matches
                    return cert?.Subject?.Contains("localhost") ?? false || 
                           sslPolicyErrors == System.Net.Security.SslPolicyErrors.None;
                };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormLogin());
        }
    }
}