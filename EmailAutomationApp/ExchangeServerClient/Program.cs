using System;
using System.Windows.Forms;

namespace ExchangeServerClient
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //string _uri = "https://outlook.office365.com/EWS/Exchange.asmx";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
