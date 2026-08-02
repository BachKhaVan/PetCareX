using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PETCAREX
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
            //Application.Run(new RegisterForm());
            //Application.Run(new FormDanhSachChiNhanh());
            //Application.Run(new FormMuaHang());
            //Application.Run(new FormQLDT_ALL());
            //Application.Run(new FormQuanLyDoanhThu());
        }
    }
}
