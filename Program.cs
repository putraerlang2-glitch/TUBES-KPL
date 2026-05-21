using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace TubesKPL
{
    internal static class Program
    {
        /// <summary>
        /// ⚠️ CATATAN INTEGRASI:
        /// 
        /// DEPRECATED: HttpApiServer.cs tidak digunakan lagi!
        /// 
        /// Alasan:
        /// - API sekarang di-host di project ObatAPI (ASP.NET Core 6) terpisah
        /// - Menggunakan architecture Client-Server yang lebih modern
        /// - Swagger UI untuk dokumentasi dan testing API
        /// - CORS enabled untuk komunikasi cross-project
        /// 
        /// Komunikasi:
        /// - Client (TubesKPL/WinForms) → HttpClient
        /// - Server (ObatAPI) → ASP.NET Core Controller
        /// - Transport: HTTPS (https://localhost:7245)
        /// - Data Format: JSON
        /// 
        /// Setup:
        /// 1. Set Multiple Startup Projects di Solution Properties
        ///    - TubesKPL → Start
        ///    - ObatAPI → Start
        /// 2. Press F5 untuk jalankan kedua projects
        /// 3. Buka Swagger di https://localhost:7245/swagger
        /// 
        /// Lihat: INTEGRATION_SETUP.md untuk dokumentasi lengkap
        /// </summary>

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // [IMPORTANT] Handle self-signed HTTPS certificate untuk development
            // ASP.NET Core 6 menggunakan self-signed cert di development environment
            // Ini diperlukan agar WinForms client bisa connect ke API via HTTPS
            ServicePointManager.ServerCertificateValidationCallback += 
                (sender, cert, chain, sslPolicyErrors) => true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // [INTEGRATION] Application entry point
            // FormLogin akan load, kemudian Main Form akan connect ke ObatAPI
            Application.Run(new FormLogin());
        }
    }
}
