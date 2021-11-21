using System;
using System.Globalization;
using System.Windows.Forms;

namespace BtcCraneBot
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {            
            DBConnect.DataBaseCreate();
            DBCommand.UserTableCreate();
            DBCommand.ServiceTableCreate();
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }
    }
}
