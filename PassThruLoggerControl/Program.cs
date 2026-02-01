using System;
using System.Windows.Forms;

namespace PassThruLoggerControl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new J2534LogController());
            } catch (System.Exception e)
            {
                MessageBox.Show(e.ToString(), "Uncatched exception");
            }
        }
    }
}
