using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace imgs2pdf
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        public string[] Imgs;
        public int DPI;
        public string Filename;
        private Process process;
        public string ImgListFilename;

        private void MainForm_Load(object sender, EventArgs e)
        {
            var args = "-verbose -density " + this.DPI + " ";
            var convertBin = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\')) + "\\convert.exe";
            if (this.Imgs != null)
            {
                foreach (var img in this.Imgs)
                {
                    args += img + " ";
                }
            }
            else
            {
                args += "\"@" + this.ImgListFilename + "\" ";
            }
            this.saveFileDialog1.FileName = this.Filename;
            DialogResult result;
            if ((result =this.saveFileDialog1.ShowDialog()) == DialogResult.OK)
            {
                this.Text = "Converting " + this.Filename + "...";
                args += "\"" + this.saveFileDialog1.FileName + "\""; 
                this.Activate();
                this.process = new Process();
                this.process.StartInfo = new ProcessStartInfo(convertBin, args);
                this.process.StartInfo.UseShellExecute = false;
                this.process.StartInfo.CreateNoWindow = true;
                this.process.StartInfo.RedirectStandardError = true;
                this.process.StartInfo.RedirectStandardOutput = true;
                this.process.EnableRaisingEvents = true;
                this.process.Exited += new EventHandler(this.process_Exited);
                this.process.ErrorDataReceived += process_ErrorDataReceived;
                this.process.OutputDataReceived += process_OutputDataReceived;
                this.textBox1.Text += "\"" + convertBin + "\" " + args + "\r\nss";
                this.process.Start();

            }
            else {
                this.DialogResult = result;
                this.Close();
            }
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.textBox1.Text += e.Data;
        }

        void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.textBox1.Text += e.Data;
        }

        void process_Exited(object sender, EventArgs e)
        {
            if (this.process.ExitCode == 0)
            {
                MessageBox.Show("PDF Saved to " + this.saveFileDialog1.FileName, "imgs2pdf", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(this.textBox1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Abort;
            }
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.process.Kill();
            }
            catch { }
        }
    }
}
