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
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using MetroFramework.Globals;

namespace DataCapturer
{
	public partial class DataCapturer : MetroForm
	{
		private delegate void Code(); //方便包裝程式碼片段
		private bool IsSetAxLim = true;

		#region Images
		private MetroImage ImageInput;
		private MetroImage ImageFilterW;
		private MetroImage ImageAxis;
		private MetroImage ImageFilterRGB;
		private MetroImage ImageErase;
		private MetroImage ImageOutput;
		#endregion

		#region Component Properties
		private double Xlo { get => double.Parse(TextBoxXlo.Text); }
		private double Xhi { get => double.Parse(TextBoxXhi.Text); }
		private double Ylo { get => double.Parse(TextBoxYlo.Text); }
		private double Yhi { get => double.Parse(TextBoxYhi.Text); }
		private double Xbase { get => double.Parse(TextBoxXBase.Text); }
		private double Ybase { get => double.Parse(TextBoxYBase.Text); }
		private int AxisLengthX { get => SliderAxLengthX.Value; }
		private int AxisLengthY { get => SliderAxLengthY.Value; }
		private int AxisOffset { get => SliderAxisOffset.Value; }
		private int FilterRMax { get => RangeSliderRed.RangeMax; }
		private int FilterRMin { get => RangeSliderRed.RangeMin; }
		private int FilterGMax { get => RangeSliderGreen.RangeMax; }
		private int FilterGMin { get => RangeSliderGreen.RangeMin; }
		private int FilterBMax { get => RangeSliderBlue.RangeMax; }
		private int FilterBMin { get => RangeSliderBlue.RangeMin; }
		#endregion

		#region Enter Point
		private List<Control> AllControls = new List<Control>();
		private static List<Control> GetAllControls(Control.ControlCollection controls)
		{
			List<Control> controlList = new List<Control>(); //初始化List
			foreach (Control control in controls)
				controlList.Add(control); // 將controls轉型成List				
			List<Control> allControls = GetAllControls(controlList); //真正開始getAllControls
			return allControls;
		}
		private static List<Control> GetAllControls(List<Control> controlList)
		{
			List<Control> opt = new List<Control>(); //不能opt = controlList!!! 會複製到參考型別!!!
			opt.AddRange(controlList);
			IEnumerable<Control> groupControls = from control in controlList
																					 where control is GroupBox | control is TabControl | control is Panel
																					 select control; //選出controls中的groupControls
			foreach (Control groupControl in groupControls)
				opt.AddRange(GetAllControls(groupControl.Controls)); //遞迴加入groupControls中的控制項
			return opt;
		}
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

			AllControls = GetAllControls(this.Controls);
			foreach (Control control in AllControls)
				control.Enabled = false;

			IEnumerable<Control> TabPages = from control in AllControls
																			where control is TabPage
																			select control;
			foreach (Control control in TabPages)
				control.Enabled = true;

			TabControlMain.Enabled = true;
			ButtonBrowse.Enabled = true;

			Console.WriteLine(SliderAxisOffset.Parent.ToString());
				
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
			//ImageInput = new MetroImage(new Bitmap(openFileDialog.FileName));
			ImageInput = new MetroImage(new Bitmap("C:\\Users\\alex\\Dropbox (Alex)\\SMCMLAB\\matlab\\DataCapturer\\images\\19451854599_fdc0d1a8d7_c.jpg"));
			ImageFilterW = new MetroImage(ImageInput.Size) { Pixel = FilterW(ImageInput, FilterWMax) };
			PictureBoxInput.Image = ImageInput.Bitmap;
			PictureBoxSetAxLim.Image = ImageInput.Bitmap;
			SliderAxLengthX.BarMax = ImageInput.Bitmap.Width;
			SliderAxLengthX.Value = SliderAxLengthX.BarMax / 2;
			SliderAxLengthY.BarMax = ImageInput.Bitmap.Height;
			SliderAxLengthY.Value = SliderAxLengthY.BarMax / 2;

			foreach (Control control in AllControls)
				control.Enabled = true;
			TextBoxXBase.Enabled = false;
			TextBoxYBase.Enabled = false;

			UpdateAxis();
		}
		#endregion

		#region Step 2: Set Axis
		private void SliderAxLengthY_Scroll(object sender, ScrollEventArgs e)
		{
			UpdateAxis();
		}
		private void SliderAxLengthX_Scroll(object sender, ScrollEventArgs e)
		{
			UpdateAxis();
		}
		private void SliderAxisOffset_Scroll(object sender, ScrollEventArgs e)
		{
			UpdateOffset();
		}
		private int FilterWMax = 200;
		private Point AxPos = new Point();
		private Size AxSize = new Size();
		private Point OffsetPos = new Point();
		private Size OffsetSize = new Size();
		private bool IsGetAxis { get => (AxSize.Width > 0 && AxSize.Height > 0) ? true : false; }
		private bool IsOffset { get => (OffsetSize.Width > 0 && OffsetSize.Height > 0) ? true : false; }
		private byte[] FilterW(MetroImage iptImage, int white = 200)
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
		private bool IsColor(byte[] pixel, int i)
		{
			return pixel[i + 3] != 0;  //A 
		}
		private void GetAxis(MetroImage image)
		{
			int L = 0, xTmp = 0, yTmp = 0, idx;
			AxSize.Width = 0;
			AxSize.Height = 0; //歸零

			for (int x = 0; x < image.Bitmap.Width; x++)
			{
				L = 0; yTmp = 0;
				for (int y = 0; y < image.Bitmap.Height; y++)
				{
					//右移x個像素(*一個字節的長度)下移y個像素(*一整行字節的長度)
					idx = x * image.Byte + y * image.Stride;
					if (IsColor(image.Pixel, idx))
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
			for (int y = 0; y < image.Bitmap.Height; y++)
			{
				L = 0; xTmp = 0;
				for (int x = 0; x < image.Bitmap.Width; x++)
				{
					// 右移x個像素(*一個字節的長度)下移y個像素(*一整行字節的長度)
					idx = x * image.Byte + y * image.Stride;
					if (IsColor(image.Pixel, idx)) //繼續記錄
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
		}
		private void UpdateAxis()
		{
			GetAxis(ImageFilterW); // get AxPos, AxSize
			if (!IsGetAxis)
			{
				PictureBoxGetAxis.Image = null;
				Console.WriteLine("GetAxis Error!");
				return;
			}
			UpdateOffset();
		}
		private void OffsetAxis()
		{
			OffsetPos.X = AxPos.X + AxisOffset;
			OffsetPos.Y = AxPos.Y + AxisOffset;
			OffsetSize.Width = AxSize.Width - AxisOffset * 2;
			OffsetSize.Height = AxSize.Height - AxisOffset * 2;
		}
		private void UpdateOffset()
		{
			OffsetAxis();
			if (!IsOffset)
			{
				PictureBoxGetAxis.Image = null;
				Console.WriteLine("GetAxis Error!");
				return;
			}

			UpdateImageAxis();
		}
		private void Crop()
		{
			Rectangle rect = new Rectangle(OffsetPos, OffsetSize);
			ImageAxis = new MetroImage(ImageInput.Bitmap.Clone(rect, MetroImage.PixelFormat));
		}
		private void UpdateImageAxis()
		{
			Crop(); //Crop ImageAxis
			PictureBoxGetAxis.Image = ImageAxis.Bitmap;

			ImageFilterRGB = new MetroImage(ImageAxis.Bitmap); //預先設定ImageFilterRGB
			UpdateImageFilter();
		}
		#endregion

		#region Step 3: Filter
		private void RangeSliderRed_Scroll(object sender, EventArgs e)
		{
			Code code = () => { ImageFilterRGB.Pixel = FilterR(ImageFilterRGB); };
			TimeIt(code);
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
		private byte[] FilterR(MetroImage iptImage)
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
		private byte[] FilterG(MetroImage iptImage)
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
		private byte[] FilterB(MetroImage iptImage)
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

			ImageErase = new MetroImage(ImageFilterRGB.Bitmap);
			UpdateImageErase();
		}
		#endregion

		#region Step 4: Erase
		private void PictureBoxEraser_MouseEnter(object sender, EventArgs e)
		{
			Cursor.Hide();
		}
		private void PictureBoxEraser_MouseLeave(object sender, EventArgs e)
		{
			Cursor.Show();
		}
		private bool IsErasing = false;
		private void PictureBoxEraser_MouseDown(object sender, MouseEventArgs e)
		{
			IsErasing = true;
		}
		private void PictureBoxEraser_MouseUp(object sender, MouseEventArgs e)
		{
			IsErasing = false;
		}
		private int EraserL = 20;
		private Graphics GraphicsEraser;
		private void PictureBoxEraser_MouseMove(object sender, MouseEventArgs e)
		{
			float ScaleX = (float)ImageErase.Width / PictureBoxEraser.Width;
			float ScaleY = (float)ImageErase.Height / PictureBoxEraser.Height;
			Console.WriteLine("X:" + e.X.ToString() + "  Y:" + e.Y.ToString());
			Bitmap ImageTmp = (Bitmap)ImageErase.Bitmap.Clone();
			GraphicsEraser = Graphics.FromImage(ImageTmp); //DrawImage會導致畫面閃爍

			GraphicsEraser.DrawRectangle(new Pen(MetroColors.Black, 5), ScaleX * e.X - EraserL / 2, ScaleY * e.Y - EraserL / 2, EraserL, EraserL);
			GraphicsEraser.DrawRectangle(new Pen(MetroColors.Blue, 3), ScaleX * e.X - EraserL / 2, ScaleY * e.Y - EraserL / 2, EraserL, EraserL);
			
			GraphicsEraser.Dispose();
			PictureBoxEraser.Image = ImageTmp;
		}
		private void UpdateImageErase()
		{
			PictureBoxEraser.Image = ImageErase.Bitmap;
		}
		#endregion


		#region Other User Activities
		private void ButtonNext_Click(object sender, EventArgs e)
		{
			TabControlMain.SelectTab(MetroMethods.clamp(TabControlMain.SelectedIndex + 1, TabControlMain.TabCount - 1, 0));
		}
		private void ButtonBack_Click(object sender, EventArgs e)
		{
			TabControlMain.SelectTab(MetroMethods.clamp(TabControlMain.SelectedIndex - 1, TabControlMain.TabCount - 1, 0));
		}
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
		#endregion
		
		dajisajdoaisjdoisajdosa
		private void UpdateData()
		{

		}

		private void UpdateImageOutput()
		{

		}

		private void UpdateWarning()
		{
			PictureBoxWarnSetAxLim.Visible = (IsSetAxLim) ? false : true;
			PictureBoxWarnGetAxis.Visible = (IsGetAxis) ? false : true;
		}



		private void TimeIt(Code code)
		{
			Stopwatch sw = new Stopwatch();//引用stopwatch物件
			sw.Reset();//碼表歸零
			sw.Start();//碼表開始計時
								 //-----目標程式-----//
			code.Invoke();
			//-----目標程式-----//
			sw.Stop();//碼錶停止
			string result = sw.Elapsed.TotalMilliseconds.ToString();
			Console.WriteLine(result);
		}


		private void CheckBoxXLog_CheckedChanged(object sender, EventArgs e)
		{
			TextBoxXBase.Enabled = (CheckBoxXLog.Checked) ? true : false;
		}
		private void CheckBoxYLog_CheckedChanged(object sender, EventArgs e)
		{
			TextBoxYBase.Enabled = (CheckBoxYLog.Checked) ? true : false;
		}
		private void ButtonSave_Click(object sender, EventArgs e)
		{

		}
		private void TextBoxXhi_TextChanged(object sender, EventArgs e)
		{
			UpdateData();
		}
		private void TextBoxXlo_TextChanged(object sender, EventArgs e)
		{
			UpdateData();
		}
		private void TextBoxYlo_TextChanged(object sender, EventArgs e)
		{
			UpdateData();
		}
		private void TextBoxYhi_TextChanged(object sender, EventArgs e)
		{
			UpdateData();
		}
		private void TextBoxXBase_TextChanged(object sender, EventArgs e)
		{
			UpdateData();
		}
		private void TextBoxYBase_TextChanged(object sender, EventArgs e)
		{
			UpdateData();
		}
		private void Tooltip_Popup(object sender, PopupEventArgs e)
		{

		}
		private void PictureBoxEraser_LoadCompleted(object sender, AsyncCompletedEventArgs e)
		{

		}	
	}
}
