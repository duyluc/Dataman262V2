using System;
using System.Windows.Forms;
using System.Threading;

namespace DATAMAN262
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static Mutex mutex  = null;
        [STAThread]
        static void Main()
        {
            bool isSingle;
            mutex = new Mutex(true, "Dataman", out isSingle);
            if (!isSingle) return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
