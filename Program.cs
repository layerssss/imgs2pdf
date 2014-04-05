using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace imgs2pdf
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0)
            {
                var alertKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes", true).CreateSubKey("imgs2pdf");
                alertKey.SetValue("", "URL:Pdf2imgs Protocol");
                alertKey.SetValue("URL Protocol", "");
                var shellKey = alertKey.CreateSubKey("shell");
                var openKey = shellKey.CreateSubKey("open");
                var commandKey = openKey.CreateSubKey("command");
                commandKey.SetValue("", "\"" + Application.ExecutablePath + "\" \"%1\"");
                MessageBox.Show("imgs2pdf installed.", "imgs2pdf", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var frags = args[0].Remove(0, "imgs2pdf:".Length).Split('|');
                var mainForm = new MainForm()
                {
                    Filename = frags[0],
                    DPI = Convert.ToInt32(frags[1]),
                    Imgs = frags.Skip(2).ToArray()
                };
                Application.Run(mainForm);
            }
        }
    }
}
