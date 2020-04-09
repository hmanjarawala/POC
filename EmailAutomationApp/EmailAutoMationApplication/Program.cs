using EmailAutoMationApplication.Presentor;
using EmailAutoMationApplication.View;
using System;
using System.Windows.Forms;

namespace EmailAutoMationApplication
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
            var presentor = new EmailAutomationPresentor();
            Application.Run(new frmEmailAutoMation(presentor));
        }
    }
}
