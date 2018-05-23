using MyLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Test
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		public void Run()
		{

		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (BackgroundWorker1.IsBusy == true)
			{
				MessageBox.Show("背景作業執行中，請稍候!");
			}
			else
			{
				BackgroundWorker1.RunWorkerAsync();			
			}
		}

		private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			for (int i = 1; i < 1000; i++)
			{
				BackgroundWorker1.ReportProgress(i);
				System.Threading.Thread.Sleep(10);
			}
		}

		private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			ProgressBar1.Value = (int)(e.ProgressPercentage / 10f);
		}

		private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			ProgressBar1.Value = 100;
		}
	}
}
