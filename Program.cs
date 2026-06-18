using System;
using System.Windows.Forms;
using NSC.Winlator.Forms;
using NSC.Winlator.Infrastructure;

namespace NSC.Winlator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            // Initialize application bootstrap
            AppBootstrap.Initialize();
            
            // Show main form
            Application.Run(new MainForm());
        }
    }
}
