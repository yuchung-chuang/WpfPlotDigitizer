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
		private PixelImage pixelImage;
		private bool isdone = true;
		public void Run()
		{
			if (!isdone)
			{
				return;
			}
			isdone = false;
			pixelImage = new PixelImage(new Bitmap("C:\\Users\\alex\\Dropbox (Alex)\\SMCMLAB\\matlab\\DataCapturer\\images\\19451854599_fdc0d1a8d7_c.jpg"));
			pictureBox1.Image = pixelImage.Bitmap;

			Point AxPos;
			Size AxSize;
			var t1 = new Thread(() => GetAxis(pixelImage, new Point(10,10), out AxPos, out AxSize));
			t1.IsBackground = true;
			t1.Start();
		}

		private bool IsColor(byte[] pixel, int i)
		{
			return pixel[i + 3] != 0;  //A 
		}
		private void GetAxis(PixelImage ImageFilterW, Point AxPosOld, out Point AxPos, out Size AxSize)
		{
			Console.WriteLine("!!!");
			int L = 0, xTmp = 0, yTmp = 0, idx;
			AxSize = new Size(0, 0);
			AxPos = AxPosOld;
			lock (ImageFilterW)
			{
				int Width = ImageFilterW.Bitmap.Width;
				int Height = ImageFilterW.Bitmap.Height;
				int Byte = ImageFilterW.Byte;
				int Stride = ImageFilterW.Stride;
				byte[] Pixel = ImageFilterW.Pixel;
				for (int x = 0; x < Width; x++)
				{
					L = 0; yTmp = 0;
					for (int y = 0; y < Height; y++)
					{
						//右移x個像素(*一個字節的長度)下移y個像素(*一整行字節的長度)
						idx = x * Byte + y * Stride;
						if (IsColor(Pixel, idx))
						{
							if (L == 0) //如果長度歸零
								yTmp = y; //更新lo索引
							L++; //長度+1
						}
						else //中斷紀錄
						{
							if (L > AxSize.Height) //確認是否比紀錄的最大值還大
							{
								AxPos.Y = yTmp;
								AxSize.Height = L;
							}
							L = 0; //長度歸零
						}
					}
					if (AxSize.Height > 10)
						break;
				}
				for (int y = 0; y < Height; y++)
				{
					L = 0; xTmp = 0;
					for (int x = 0; x < Width; x++)
					{
						// 右移x個像素(*一個字節的長度)下移y個像素(*一整行字節的長度)
						idx = x * Byte + y * Stride;
						if (IsColor(Pixel, idx)) //繼續記錄
						{
							if (L == 0) //如果長度歸零
								xTmp = x; //更新lo索引
							L++; //長度+1
						}
						else //中斷紀錄
						{
							if (L > AxSize.Width) //確認是否比紀錄的最大值還大
							{
								AxPos.X = xTmp;
								AxSize.Width = L;
							}
							L = 0; //長度歸零
						}
					}
					if (AxSize.Width > 10)
						break;
				}
			}
			Thread.Sleep(10000);
			isdone = true;
		}


		private void AddToCart(PixelImage pixelImageObj)
		{
			Console.WriteLine("Thread{0}.AddToCart().Start()", Thread.CurrentThread.ManagedThreadId);
			PixelImage pixelImage = (PixelImage)pixelImageObj;
			pixelImage.Bitmap = new Bitmap(openFileDialog1.FileName);
			int x = pixelImage.Width + pixelImage.Height;
			PixelImage xxx = new PixelImage(new Bitmap(openFileDialog1.FileName));

			if (InvokeRequired)
			{
				Console.WriteLine("InvokeRequired");
				UpdateUIHandle handle = new UpdateUIHandle(UpdateUI);
				handle.Invoke(pictureBox1, pixelImage);
			}
			else
			{
				Console.WriteLine("NoInvoke");
				UpdateUI(pictureBox1, pixelImage);
			}
			Console.WriteLine("Thread{0}.AddToCat().End()", Thread.CurrentThread.ManagedThreadId);
		}
		delegate void UpdateUIHandle(PictureBox pictureBox, PixelImage pixelImage);
		private void UpdateUI(PictureBox pictureBox, PixelImage pixelImage)
		{
			Console.WriteLine("Thread{0}.UpdateUI().Start()", Thread.CurrentThread.ManagedThreadId);
			Thread.Sleep(100000);
			pictureBox.Image = pixelImage.Bitmap;
			Console.WriteLine("Thread{0}.UpdateUI().End()", Thread.CurrentThread.ManagedThreadId);
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			Run();
		}

		private void Form1_MouseMove(object sender, MouseEventArgs e)
		{
			Run();
		}
	}
}
