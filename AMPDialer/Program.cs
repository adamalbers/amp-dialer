using System;
using System.Windows.Forms;

//            HotKeys.GlobalHotkey ghk;
//            ghk = new HotKeys.GlobalHotkey(Constants.ALT + Constants.SHIFT, Keys.O, this);

namespace AMPDialer
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
            /// Application.Run(new Form1());
            using (ProcessIcon pi = new ProcessIcon())
            {
                pi.Display();
                Application.Run();
            }
        }
    }
}
