using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Test
{
	public partial class LayerControl : UserControl
	{
		private Image image;
		private Graphics graphics;
		public LayerControl(int width, int height)
		{
			this.Width = width;
			this.Height = height;
			//建一个新的指定大小的位图
			image = new Bitmap(width, height);
			graphics = Graphics.FromImage(image);
			// 设置空间风格为:双缓冲|减少闪烁|用户绘制
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
		}
		// 重写OnPaint，每当绘画时发生
		protected override void OnPaint(PaintEventArgs e)
		{
			//做一张背景透明的位图当做绘制内容
			//若已有Image属性被指定，则画该图
			//由于e的绘制区域总是在变，这里不需要考虑绘制区域，系统会自己算，超出区域的不予以绘制
			//简单刷原理就是，每当系统要绘制时，先画背景，我们自行处理就需要计算应该画哪个区域（多数都是不规则的区域并集）
			//每当OnPaint时，绘制自己的Image图（若属性存在），或者是纯透明的位图以提供完全透明效果
			var bitMap = new Bitmap(image);
			bitMap.MakeTransparent(Color.White);
			image = bitMap;
			//绘制模式指定
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			g.CompositingQuality = CompositingQuality.GammaCorrected;
			//透明色变换
			float[][] mtxItens = {
						new float[] {1,0,0,0,0},
						new float[] {0,1,0,0,0},
						new float[] {0,0,1,0,0},
						new float[] {0,0,0,1,0},
						new float[] {0,0,0,0,1}};
			ColorMatrix colorMatrix = new ColorMatrix(mtxItens);
			ImageAttributes imgAtb = new ImageAttributes();
			imgAtb.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			//画
			g.DrawImage(image, ClientRectangle, 0.0f, 0.0f, image.Width, image.Height, GraphicsUnit.Pixel, imgAtb);
		}
		//控件背景绘制
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
			Graphics g = e.Graphics;
			//以下代码提取控件原来应有的背景（视觉上的位图，可能会包括其他空间的部分图形）到位图，进行变换后绘制
			if (Parent != null)
			{
				BackColor = Color.Transparent;
				//本控件在父控件的子集中的index
				int index = Parent.Controls.GetChildIndex(this);
				for (int i = Parent.Controls.Count - 1; i > index; i--)
				{
					//对每一个父控件的可视子控件，进行绘制区域交集运算，得到应该绘制的区域
					Control c = Parent.Controls[i];
					//如果有交集且可见
					if (c.Bounds.IntersectsWith(Bounds) && c.Visible)
					{
						//矩阵变换
						Bitmap bmp = new Bitmap(c.Width, c.Height, g);
						c.DrawToBitmap(bmp, c.ClientRectangle);
						g.TranslateTransform(c.Left - Left, c.Top - Top);
						//画图
						g.DrawImageUnscaled(bmp, Point.Empty);
						g.TranslateTransform(Left - c.Left, Top - c.Top);
						bmp.Dispose();
					}
				}
			}
			else
			{
				g.Clear(Parent.BackColor);
				g.FillRectangle(new SolidBrush(Color.FromArgb(255, Color.Transparent)), this.ClientRectangle);
			}
		}
		//示例代码，请自行修改。
		//在此控件上的绘图工作请写在此处。
		public void DrawCircles()
		{
			using (Brush b = new SolidBrush(Color.Red))
			{
				using (Pen p = new Pen(Color.Green, 3))
				{
					this.graphics.DrawEllipse(p, 50, 40, 30, 30);
				}
			}
		}
		//示例代码，请自行修改。
		public void DrawRectangle()
		{
			using (Brush b = new SolidBrush(Color.Red))
			{
				using (Pen p = new Pen(Color.Red, 3))
				{
					this.graphics.DrawRectangle(p, 50, 50, 40, 40);
				}
			}
		}
		//Image属性，每当赋值会引发Invalidate
		public Image Image
		{
			get
			{
				return image;
			}
			set
			{
				image = value;

				this.Invalidate();
			}
		}
	}

}
