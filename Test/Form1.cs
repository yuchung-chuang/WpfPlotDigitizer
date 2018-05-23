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
		Bitmap image, newImage, cloneImage;
		public Form1()
		{
			InitializeComponent();
			image = new Bitmap("C:\\Users\\alex\\Dropbox (Alex)\\SMCMLAB\\matlab\\DataCapturer\\images\\19451854599_fdc0d1a8d7_c.jpg");
			newImage = new Bitmap(image);
			cloneImage = (Bitmap)image.Clone();
			image = DrawEraser(image, new Point(10,10));
			
		}
		private Bitmap DrawEraser(Bitmap ImageTmp, Point pos)
		{
			int EraserL = 30;
			using (Graphics graphics = Graphics.FromImage(ImageTmp))//DrawImage會導致畫面閃爍
			{
				graphics.DrawRectangle(new Pen(Color.Black, 5), pos.X - EraserL / 2, pos.Y - EraserL / 2, EraserL, EraserL);
				graphics.DrawRectangle(new Pen(Color.Blue, 3), pos.X - EraserL / 2, pos.Y - EraserL / 2, EraserL, EraserL);
			};
			return ImageTmp;
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

		private int i = 0;
		private void pictureBox1_Click(object sender, EventArgs e)
		{
			if (i == 0)
			{
				pictureBox1.Image = image;

			}
			else if (i == 1)
			{
				pictureBox1.Image = newImage;

			}
			else
			{
				pictureBox1.Image = cloneImage;

			}
			i++;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			
		}
	}
}
