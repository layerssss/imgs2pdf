﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                string[] imgs;
                try
                {
                    var wc = new WebClient();
                    wc.Headers["Cookie"] = frags[3];
                    imgs = Newtonsoft.Json.Linq.JArray.Parse(wc.DownloadString(Uri.UnescapeDataString(frags[2]))).Select(token => token.ToString()).ToArray();
                }catch
                {
                    imgs = args.Skip(2).ToArray();
                }
                var mainForm = new MainForm()
                {
                    Filename = Uri.UnescapeDataString(frags[0]),
                    DPI = Convert.ToInt32(frags[1]),
                    Imgs = imgs
                };
                Application.Run(mainForm);
            }
        }
    }
}
