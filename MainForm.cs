using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using ImageMagick;

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
        Thread thread;
        Exception ex;
        int processed = 0;

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.progressBar1.Maximum = this.Imgs.Length;
            var convertBin = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\')) + "\\convert.exe";
            this.saveFileDialog1.FileName = this.Filename;
            DialogResult result;
            if ((result =this.saveFileDialog1.ShowDialog()) == DialogResult.OK)
            {
                this.Text = "Converting " + this.Filename + "...";
                this.Activate();
                this.thread = new Thread(this.threadStart);
                this.thread.Start();
            }
            else {
                this.DialogResult = result;
                this.Close();
            }
        }

        void threadStart()
        {
            try
            {
                using (var image = new MagickImageCollection())
                {
                    foreach (var img in this.Imgs)
                    {
                        var mimg = new MagickImage(img);
                        mimg.Density = new MagickGeometry(300, 300);
                        image.Add(mimg);
                        this.processed += 1;
                    }
                    image.Write(this.saveFileDialog1.FileName);
                }
            }
            catch (Exception e)
            {
                this.ex = e;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.thread.Abort();
            }
            catch { }
            this.timer1.Stop();
            if (this.ex == null)
            {
                this.DialogResult = DialogResult.OK;
                MessageBox.Show("Saved to " + this.saveFileDialog1.FileName, "imgs2pdf", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.progressBar1.Value = this.processed;
            if (this.processed == this.Imgs.Length)
            {
                this.progressBar1.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                this.progressBar1.Style = ProgressBarStyle.Continuous;
            }
            if (this.thread != null && !this.thread.IsAlive)
            {
                this.Close();
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

    }
}
