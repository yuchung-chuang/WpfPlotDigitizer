using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Drawing;
using MetroFramework.Forms;
using MyLibrary;
using static MyLibrary.MyMethods;

namespace DataCapturer
{
	public partial class DataCapturer : MetroForm
	{
		private bool IsSetAxLim = true;

		#region Images
		private PixelImage ImageInput;
		private PixelImage ImageFilterW;
		private PixelImage ImageAxis;
		private PixelImage ImageFilterRGB;
		private PixelImage ImageErase => ImageEraseList[EraseIdx];
		private List<PixelImage> ImageEraseList = new List<PixelImage>();
		private PixelImage ImageOutput;
		#endregion

		#region Component Properties
		private double Xlo => double.Parse(TextBoxXlo.Text);
		private double Xhi => double.Parse(TextBoxXhi.Text);
		private double Ylo => double.Parse(TextBoxYlo.Text);
		private double Yhi => double.Parse(TextBoxYhi.Text);
		private double Xbase => double.Parse(TextBoxXBase.Text);
		private double Ybase => double.Parse(TextBoxYBase.Text);
		private int AxisLengthX => SliderAxLengthX.Value;
		private int AxisLengthY => SliderAxLengthY.Value;
		private int AxisOffset => SliderAxisOffset.Value;
		private int FilterRMax => RangeSliderRed.RangeMax;
		private int FilterRMin => RangeSliderRed.RangeMin;
		private int FilterGMax => RangeSliderGreen.RangeMax;
		private int FilterGMin => RangeSliderGreen.RangeMin;
		private int FilterBMax => RangeSliderBlue.RangeMax;
		private int FilterBMin => RangeSliderBlue.RangeMin;
		#endregion

		#region Enter Point
		private List<Control> AllControls = new List<Control>();
		private string StringArrow(string direction)
		{
			switch (direction)
			{
				case ("left"):
					return char.ConvertFromUtf32(0x2190);
				case ("right"):
					return char.ConvertFromUtf32(0x2192);
				case ("up"):
					return char.ConvertFromUtf32(0x2191);
				case ("down"):
					return char.ConvertFromUtf32(0x2193);
				default:
					return null;
			}
		}
		public DataCapturer()
		{
			InitializeComponent();
			ButtonNext.Text = "Next" + " " + StringArrow("right");
			ButtonBack.Text = StringArrow("left") + " " + "Back";

			AllControls = RecursiveGetControls(this);
			foreach (Control control in AllControls)
				control.Enabled = false;

			IEnumerable<Control> TabPages = from control in AllControls
																			where control is TabPage
																			select control;
			foreach (Control control in TabPages)
				control.Enabled = true;

			TabControlMain.Enabled = true;
			ButtonBrowse.Enabled = true;

			Initialize();
		}
		#endregion

		#region Step 1: Browse
		private void ButtonBrowse_Click(object sender, EventArgs e)
		{
			DialogResult result = openFileDialog.ShowDialog();
			if (result != DialogResult.OK) { return; }
			Initialize();
		}
		private void Initialize()
		{
			//ImageInput = new PixelImage(new Bitmap(openFileDialog.FileName));
			ImageInput = new PixelImage(new Bitmap("C:\\Users\\alex\\Dropbox (Alex)\\SMCMLAB\\matlab\\DataCapturer\\images\\19451854599_fdc0d1a8d7_c.jpg"));
			PictureBoxInput.Image = ImageInput.Bitmap;

			//Enable Constrols
			foreach (Control control in AllControls)
				control.Enabled = true;
			TextBoxXBase.Enabled = false;
			TextBoxYBase.Enabled = false;

			UpdateImageSetAxLim();
		}
		#endregion

		#region Step 2: Set Axis Limits
		private void CheckBoxXLog_CheckedChanged(object sender, EventArgs e)
		{
			TextBoxXBase.Enabled = (CheckBoxXLog.Checked) ? true : false;
		}
		private void CheckBoxYLog_CheckedChanged(object sender, EventArgs e)
		{
			TextBoxYBase.Enabled = (CheckBoxYLog.Checked) ? true : false;
		}
		private void TextBoxXhi_TextChanged(object sender, EventArgs e)
		{
			if (this.Text != null)
				this.BackColor = DefaultBackColor;
			else
				this.BackColor = Color.LightPink;
		}
		private void TextBoxXlo_TextChanged(object sender, EventArgs e)
		{
			if (this.Text != null)
				this.BackColor = DefaultBackColor;
			else
				this.BackColor = Color.LightPink;
		}
		private void TextBoxYlo_TextChanged(object sender, EventArgs e)
		{
			if (this.Text != null)
				this.BackColor = DefaultBackColor;
			else
				this.BackColor = Color.LightPink;
		}
		private void TextBoxYhi_TextChanged(object sender, EventArgs e)
		{
			if (this.Text != null)
				this.BackColor = DefaultBackColor;
			else
				this.BackColor = Color.LightPink;
		}
		private void TextBoxXBase_TextChanged(object sender, EventArgs e)
		{
			if (this.Text != null)
				this.BackColor = DefaultBackColor;
			else
				this.BackColor = Color.LightPink;
		}
		private void TextBoxYBase_TextChanged(object sender, EventArgs e)
		{
			if (this.Text != null)
				this.BackColor = DefaultBackColor;
			else
				this.BackColor = Color.LightPink;
		}
		private void UpdateImageSetAxLim()
		{
			ImageViewerSetAxLim.Image = ImageInput.Bitmap;

			SliderAxLengthX.BarMax = ImageInput.Bitmap.Width;
			SliderAxLengthX.Value = SliderAxLengthX.BarMax / 2;
			SliderAxLengthY.BarMax = ImageInput.Bitmap.Height;
			SliderAxLengthY.Value = SliderAxLengthY.BarMax / 2;

			byte[] Pixel = FilterW(ImageInput, FilterWMax);
			ImageFilterW = new PixelImage(Pixel, ImageInput.Size);
			GetAxis(ImageFilterW, AxPos, out AxPos, out AxSize);
		}
		#endregion

		#region Step 3: Get Frame
		private int FilterWMax = 200;
		private Point AxPos = new Point();
		private Size AxSize = new Size();
		private Point OffsetPos = new Point();
		private Size OffsetSize = new Size();
		private bool IsGetAxis => (AxSize.Width > 0 && AxSize.Height > 0) ? true : false;
		private bool IsOffset => (OffsetSize.Width > 0 && OffsetSize.Height > 0) ? true : false;
		private bool IsColor(byte[] pixel, int i)
		{
			return pixel[i + 3] != 0;  //A 
		}
		private byte[] FilterW(PixelImage iptImage, int white = 200)
		{
			byte blue, green, red;
			byte[] optPixel = (byte[])iptImage.Pixel.Clone(); // 複製值
			for (int i = 0; i < iptImage.Pixel.Length; i += iptImage.Byte)
			{
				blue = iptImage.Pixel[i];   //B
				green = iptImage.Pixel[i + 1]; //G
				red = iptImage.Pixel[i + 2];  //R
				optPixel[i + 3] = (red < white || green < white || blue < white) ? (byte)255 : (byte)0; //A
			}
			return optPixel;
		}
		private void SliderAxLengthY_Scroll(object sender, ScrollEventArgs e)
		{
			GetAxis(ImageFilterW, AxPos, out AxPos, out AxSize);
		}
		private void SliderAxLengthX_Scroll(object sender, ScrollEventArgs e)
		{
			if (!IsUpdateDone)
			{
				return;
			}
			IsUpdateDone = false;
			Thread t1 = new Thread(() => GetAxis(ImageFilterW, AxPos, out AxPos, out AxSize));
			t1.IsBackground = true;
			t1.Start();
			//GetAxis(ImageFilterW, AxPos, out AxPos, out AxSize);
		}
		private void SliderAxisOffset_Scroll(object sender, ScrollEventArgs e)
		{
			OffsetAxis(AxPos, AxSize, out OffsetPos, out OffsetSize);
		}
		private bool IsUpdateDone = true;
		private void GetAxis(PixelImage ImageFilterW, Point AxPosOld, out Point AxPos, out Size AxSize)
		{
			int L = 0, xTmp = 0, yTmp = 0, idx;
			AxSize = new Size(0, 0);
			AxPos = AxPosOld;
			for (int x = 0; x < ImageFilterW.Bitmap.Width; x++)
			{
				L = 0; yTmp = 0;
				for (int y = 0; y < ImageFilterW.Bitmap.Height; y++)
				{
					//右移x個像素(*一個字節的長度)下移y個像素(*一整行字節的長度)
					idx = x * ImageFilterW.Byte + y * ImageFilterW.Stride;
					if (IsColor(ImageFilterW.Pixel, idx))
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
				if (AxSize.Height > AxisLengthY)
					break;
			}
			for (int y = 0; y < ImageFilterW.Bitmap.Height; y++)
			{
				L = 0; xTmp = 0;
				for (int x = 0; x < ImageFilterW.Bitmap.Width; x++)
				{
					// 右移x個像素(*一個字節的長度)下移y個像素(*一整行字節的長度)
					idx = x * ImageFilterW.Byte + y * ImageFilterW.Stride;
					if (IsColor(ImageFilterW.Pixel, idx)) //繼續記錄
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
				if (AxSize.Width > AxisLengthX)
					break;
			}

			if (!IsGetAxis)
			{
				PictureBoxGetAxis.Image = null;
				Console.WriteLine("GetAxis Error!");
				return;
			}
			OffsetAxis(AxPos, AxSize, out OffsetPos, out OffsetSize);
		}
		private void OffsetAxis(Point AxPos, Size AxSize, out Point OffsetPos, out Size OffsetSize)
		{
			OffsetPos = new Point(AxPos.X + AxisOffset, AxPos.Y + AxisOffset);
			OffsetSize = new Size(AxSize.Width - AxisOffset * 2, AxSize.Height - AxisOffset * 2);
			if (!IsOffset)
			{
				PictureBoxGetAxis.Image = null;
				Console.WriteLine("GetAxis Error!");
				return;
			}

			UpdateImageAxis(out ImageAxis,(PixelImage)ImageInput.Clone(), OffsetPos, OffsetSize);
		}
		private void UpdateImageAxis(out PixelImage ImageAxis, PixelImage ImageInput, Point OffsetPos, Size OffsetSize)
		{
			ImageAxis = new PixelImage(Crop(ImageInput.Bitmap, new Rectangle(OffsetPos, OffsetSize)));
			PictureBoxGetAxis.Image = ImageAxis.Bitmap;
			IsUpdateDone = true;
			//ImageFilterRGB = new PixelImage(ImageAxis.Bitmap);
			//UpdateImageFilter();
		}
		#endregion

		#region Step 4: Filter
		private void RangeSliderRed_Scroll(object sender, EventArgs e)
		{
			ImageFilterRGB.Pixel = FilterR(ImageFilterRGB);
			UpdateImageFilter();
		}
		private void RangeSliderGreen_Scroll(object sender, EventArgs e)
		{
			ImageFilterRGB.Pixel = FilterG(ImageFilterRGB);
			UpdateImageFilter();
		}
		private void RangeSliderBlue_Scroll(object sender, EventArgs e)
		{
			ImageFilterRGB.Pixel = FilterB(ImageFilterRGB);
			UpdateImageFilter();
		}
		private bool IsRGBFilted(byte R, byte G, byte B)
		{
			return (R <= FilterRMax && R >= FilterRMin &&
						G <= FilterGMax && G >= FilterGMin &&
							B <= FilterBMax && B >= FilterBMin) ? true : false;
		}
		private byte[] FilterR(PixelImage iptImage)
		{
			byte R, G, B;
			byte[] optPixel = (byte[])iptImage.Pixel.Clone(); // 複製值

			for (int i = 0; i < iptImage.Pixel.Length; i += iptImage.Byte)
			{
				if (IsColor(optPixel, i))
				{
					R = iptImage.Pixel[i + 2];  //R
					if (!(R <= FilterRMax && R >= FilterRMin))
						optPixel[i + 3] = 0; //A
				}
				else
				{
					B = iptImage.Pixel[i];
					G = iptImage.Pixel[i + 1];
					R = iptImage.Pixel[i + 2];
					if (IsRGBFilted(R, G, B))
						optPixel[i + 3] = 255; //A
				}
			}
			return optPixel;
		}
		private byte[] FilterG(PixelImage iptImage)
		{
			byte R, G, B;
			byte[] optPixel = (byte[])iptImage.Pixel.Clone(); // 複製值

			for (int i = 0; i < iptImage.Pixel.Length; i += iptImage.Byte)
			{
				if (IsColor(optPixel, i))
				{
					G = iptImage.Pixel[i + 1]; //G
					if (!(G <= FilterGMax && G >= FilterGMin))
						optPixel[i + 3] = 0; //A
				}
				else
				{
					B = iptImage.Pixel[i];
					G = iptImage.Pixel[i + 1];
					R = iptImage.Pixel[i + 2];
					if (IsRGBFilted(R, G, B))
						optPixel[i + 3] = 255; //A
				}
			}
			return optPixel;
		}
		private byte[] FilterB(PixelImage iptImage)
		{
			byte R, G, B;
			byte[] optPixel = (byte[])iptImage.Pixel.Clone(); // 複製值

			for (int i = 0; i < iptImage.Pixel.Length; i += iptImage.Byte)
			{
				if (IsColor(optPixel, i))
				{
					B = iptImage.Pixel[i]; //B
					if (!(B <= FilterBMax && B >= FilterBMin))
						optPixel[i + 3] = 0; //A
				}
				else
				{
					B = iptImage.Pixel[i];
					G = iptImage.Pixel[i + 1];
					R = iptImage.Pixel[i + 2];
					if (IsRGBFilted(R, G, B))
						optPixel[i + 3] = 255; //A
				}
			}
			return optPixel;
		}
		private void UpdateImageFilter()
		{ 	
			PictureBoxFilter.Image = ImageFilterRGB.Bitmap;

			ImageEraseList.Clear();
			ImageEraseList.Add(new PixelImage(ImageFilterRGB.Bitmap));
			EraseIdx = 0;
			UpdateImageErase();
		}
		#endregion

		#region Step 5: Erase
		private int EraseIdx = 0;
		private int EraserL = 20;
		private bool IsErasing = false;
		private Bitmap DrawEraser(PixelImage image, Point pos)
		{
			Bitmap ImageTmp = (Bitmap)image.Bitmap.Clone();
			using (Graphics graphics = Graphics.FromImage(ImageTmp))//DrawImage會導致畫面閃爍
			{
				graphics.DrawRectangle(new Pen(MetroColors.Black, 5), pos.X - EraserL / 2, pos.Y - EraserL / 2, EraserL, EraserL);
				graphics.DrawRectangle(new Pen(MetroColors.Blue, 3), pos.X - EraserL / 2, pos.Y - EraserL / 2, EraserL, EraserL);
			};
			return ImageTmp;
		}
		private void EraseImage(Point pos)
		{
			byte[] pixel = ImageErase.Pixel;
			int idx;
			int x_ini = pos.X - EraserL / 2;
			int x_fin = pos.X + EraserL / 2;
			int y_ini = pos.Y - EraserL / 2;
			int y_fin = pos.Y + EraserL / 2;
			for (int x = x_ini; x < x_fin; x++)
			{
				for (int y = y_ini; y < y_fin; y++)
				{
					if (x < 0 || y < 0 || x >= ImageErase.Width || y >= ImageErase.Height)
						continue;
					idx = x * ImageErase.Byte + y * ImageErase.Stride;
					pixel[idx + 3] = 0; // A
				}
			}
			ImageErase.Pixel = pixel;
		}
		private void Undo()
		{
			if (EraseIdx > 0)
				EraseIdx -= 1;
			UpdateImageErase();
		}
		private void Redo()
		{
			if (EraseIdx < ImageEraseList.Count - 1)
				EraseIdx += 1;
			UpdateImageErase();
		}
		private void ImageViewerErase_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				IsErasing = true;
				ImageEraseList.RemoveRange(EraseIdx + 1, ImageEraseList.Count - EraseIdx - 1); //清除所有原先的Redo
				ImageEraseList.Add(new PixelImage(ImageErase.Bitmap));//或許需要設置ImageEraseList的儲存上限
				EraseIdx += 1;

				ImageViewerErase_MouseMove(sender, e);
			}
		}
		private void ImageViewerErase_MouseMove(object sender, MouseEventArgs e)
		{
			Point EffectiveMouseLocation = ImageViewerErase.GetEffectiveMouseLocation(e.Location);
			Point pos = new Point(ImageViewerErase.ImageBoxPos.X + EffectiveMouseLocation.X, ImageViewerErase.ImageBoxPos.Y + EffectiveMouseLocation.Y);
			if (IsErasing)
				EraseImage(pos);
			ImageViewerErase.Image = DrawEraser(ImageErase, pos);
		}
		private void ImageViewerErase_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				IsErasing = false;
				UpdateImageErase();
			}
		}
		private void ImageViewerErase_MouseEnter(object sender, EventArgs e)
		{
			Cursor.Hide();
		}
		private void ImageViewerErase_MouseLeave(object sender, EventArgs e)
		{
			Cursor.Show();
		}
		private void UpdateImageErase()
		{
			ImageViewerErase.Image = ImageErase.Bitmap;
			UpdateUndoButtonColor();
			UpdateRedoButtonColor();

			ImageOutput = new PixelImage(ImageErase.Bitmap);
			UpdateImageOutput();
		}
		#endregion

		#region 
		private void UpdateImageOutput()
		{
			PictureBoxOutput.Image = ImageOutput.Bitmap;

			UpdateData();
		}

		private bool IsDataOK()
		{
			if (TextBoxXlo.Text == null || TextBoxYlo.Text == null || TextBoxXhi.Text == null || TextBoxYhi.Text == null || (CheckBoxXLog.Checked && TextBoxXBase.Text == null) || (CheckBoxYLog.Checked && TextBoxYBase.Text == null))
			{
				TabControlMain.SelectedIndex = 1;
				MessageBox.Show("Please fill in all the axis limits.");
				return false;
			}
			if (!IsGetAxis || !IsOffset)
			{
				TabControlMain.SelectedIndex = 2;
				MessageBox.Show("Please adjust the scrollbars to capture axis properly.");
				return false;
			}
			return true;
		}
		private void UpdateData()
		{
			if (!IsDataOK())
				return;

		}

		private void Warning()
		{

		}


		private void ButtonSave_Click(object sender, EventArgs e)
		{

		}
		#endregion

		#region Other User Activities
		private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (TabControlMain.SelectedIndex == 0)
				ButtonBack.Hide();
			else
				ButtonBack.Show();
			if (TabControlMain.SelectedIndex == TabControlMain.TabCount - 1)
				ButtonNext.Hide();
			else
				ButtonNext.Show();
		}
		private void ButtonNext_Click(object sender, EventArgs e)
		{
			TabControlMain.SelectTab(CustomMethods.Clamp(TabControlMain.SelectedIndex + 1, TabControlMain.TabCount - 1, 0));
		}
		private void ButtonBack_Click(object sender, EventArgs e)
		{
			TabControlMain.SelectTab(CustomMethods.Clamp(TabControlMain.SelectedIndex - 1, TabControlMain.TabCount - 1, 0));
		}
		#endregion

		#region Undo/Redo Buttons
		private bool UndoButtonIsEnter = false;
		private bool UndoButtonIsPress = false;
		private void UndoButton_MouseEnter(object sender, EventArgs e)
		{
			UndoButtonIsEnter = true;
			UpdateUndoButtonColor();
		}
		private void UndoButton_MouseLeave(object sender, EventArgs e)
		{
			UndoButtonIsEnter = false;
			UpdateUndoButtonColor();
		}
		private void UndoButton_MouseDown(object sender, MouseEventArgs e)
		{
			UndoButtonIsPress = true;
			UpdateUndoButtonColor();
		}
		private void UndoButton_MouseUp(object sender, MouseEventArgs e)
		{
			UndoButtonIsPress = false;
			UpdateUndoButtonColor();

			if (e.X <= UndoButton.Width && e.Y <= UndoButton.Height)
				Undo();
		}
		private void UpdateUndoButtonColor()
		{
			if (EraseIdx == 0) //無法Undo
			{
				UndoButton.BackColor = Control.DefaultBackColor;
				UndoButton.Image = Properties.Resources.Undo_icon_black_;
			}
			else
			{
				UndoButton.Image = Properties.Resources.Undo_icon;
				if (UndoButtonIsPress)
					UndoButton.BackColor = MetroColors.Blue;
				else if (UndoButtonIsEnter)
					UndoButton.BackColor = Color.LightGray;
				else
					UndoButton.BackColor = Control.DefaultBackColor;
			}

		}

		private bool RedoButtonIsEnter = false;
		private bool RedoButtonIsPress = false;
		private void RedoButton_MouseEnter(object sender, EventArgs e)
		{
			RedoButtonIsEnter = true;
			UpdateRedoButtonColor();
		}
		private void RedoButton_MouseLeave(object sender, EventArgs e)
		{
			RedoButtonIsEnter = false;
			UpdateRedoButtonColor();
		}
		private void RedoButton_MouseDown(object sender, MouseEventArgs e)
		{
			RedoButtonIsPress = true;
			UpdateRedoButtonColor();
		}
		private void RedoButton_MouseUp(object sender, MouseEventArgs e)
		{
			RedoButtonIsPress = false;
			UpdateRedoButtonColor();

			if (e.X <= RedoButton.Width && e.Y <= RedoButton.Height)
				Redo();
		}
		private void UpdateRedoButtonColor()
		{
			if (EraseIdx == ImageEraseList.Count - 1) //無法Redo
			{
				RedoButton.BackColor = Control.DefaultBackColor;
				RedoButton.Image = Properties.Resources.Redo_icon_black_;
			}
			else
			{
				RedoButton.Image = Properties.Resources.Redo_icon;
				if (RedoButtonIsPress)
					RedoButton.BackColor = MetroColors.Blue;
				else if (RedoButtonIsEnter)
					RedoButton.BackColor = Color.LightGray;
				else
					RedoButton.BackColor = Control.DefaultBackColor;
			}

		}
		#endregion

		#region AutoResizeControls
		private void DataCapturer_Load(object sender, EventArgs e)
		{
			this.Tag = new ControlAnchor()
			{
				Height = this.Height,
				Width = this.Width
			};
			foreach (Control control in AllControls)
			{
				control.Tag = new ControlAnchor()
				{
					Top = control.Top,
					Left = control.Left,
					Height = control.Height,
					Width = control.Width,
				};
			}
		}
		private void DataCapturer_Resize(object sender, EventArgs e)
		{
			ControlAnchor formAnchor = (ControlAnchor)this.Tag;
			float WidthRatio = (float)this.Width / formAnchor.Width;
			float HeightRatio = (float)this.Height / formAnchor.Height;
			foreach (Control control in AllControls)
			{
				ControlAnchor controlAnchor = (ControlAnchor)control.Tag;
				control.Width = (int)(controlAnchor.Width * WidthRatio);
				control.Height = (int)(controlAnchor.Height * HeightRatio);
				control.Left = (int)(controlAnchor.Left * WidthRatio);
				control.Top = (int)(controlAnchor.Top * HeightRatio);
			}
		}
		#endregion

		
	}
}
