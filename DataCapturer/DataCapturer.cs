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
using MetroFramework.Drawing;
using MetroFramework.Forms;


namespace DataCapturer
{
	public partial class DataCapturer : MetroForm
	{
		private delegate void Code(); //方便包裝程式碼片段
		private bool IsSetAxLim = true;

		#region Images
		private PixelImage ImageInput;
		private PixelImage ImageFilterW;
		private PixelImage ImageAxis;
		private PixelImage ImageFilterRGB;
		private PixelImage ImageErase;
		private List<PixelImage> ImageEraseList = new List<PixelImage>();
		private PixelImage ImageOutput;
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
			ImageFilterW = new PixelImage(ImageInput.Size) { Pixel = FilterW(ImageInput, FilterWMax) };
			PictureBoxInput.Image = ImageInput.Bitmap;
			ImageViewerSetAxLim.Image = ImageInput.Bitmap;
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
		private bool IsGetAxis => (AxSize.Width > 0 && AxSize.Height > 0) ? true : false; 
		private bool IsOffset => (OffsetSize.Width > 0 && OffsetSize.Height > 0) ? true : false; 
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
		private bool IsColor(byte[] pixel, int i)
		{
			return pixel[i + 3] != 0;  //A 
		}
		private void GetAxis(PixelImage image)
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
			ImageAxis = new PixelImage(ImageInput.Bitmap.Clone(rect, PixelImage.PixelFormat));
		}
		private void UpdateImageAxis()
		{
			Crop(); //Crop ImageAxis
			PictureBoxGetAxis.Image = ImageAxis.Bitmap;

			ImageFilterRGB = new PixelImage(ImageAxis.Bitmap); //預先設定ImageFilterRGB
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

			//ImageErase = new PixelImage(ImageFilterRGB.Bitmap);
			ImageEraseList.Clear();
			ImageEraseList.Add(new PixelImage(ImageFilterRGB.Bitmap));
			EraseIdx = 0;
			UpdateImageErase();
		}
		#endregion

		#region Step 4: Erase
		private int EraseIdx = 0;
		private void UpdateImageErase()
		{
			//PictureBoxEraser.Image = ImageErase.Bitmap;
			PictureBoxEraser.Image = ImageEraseList[EraseIdx].Bitmap;
			UpdateUndoButtonColor();
			UpdateRedoButtonColor();
		}

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
			ImageEraseList.RemoveRange(EraseIdx + 1, ImageEraseList.Count - EraseIdx - 1); //清除所有原先的Redo
			ImageEraseList.Add(new PixelImage(ImageEraseList[EraseIdx].Bitmap));//或許需要設置ImageEraseList的儲存上限
			EraseIdx += 1;
			
			PictureBoxEraser_MouseMove(sender, e);
		}
		private void PictureBoxEraser_MouseUp(object sender, MouseEventArgs e)
		{
			IsErasing = false;
			UpdateImageErase();
		}
		private int EraserL = 20;
		private Graphics GraphicsEraser;
		private void PictureBoxEraser_MouseMove(object sender, MouseEventArgs e)
		{
			float ScaleX = (float)ImageEraseList[EraseIdx].Width / PictureBoxEraser.Width;
			float ScaleY = (float)ImageEraseList[EraseIdx].Height / PictureBoxEraser.Height;

			Point pos = new Point((int)(e.X * ScaleX), (int)(e.Y * ScaleY));
			if (IsErasing)
			{
				EraseImage(pos);
			}
			DrawEraser(pos);
		}

		
		private void DrawEraser(Point pos)
		{
			
			Bitmap ImageTmp = (Bitmap)ImageEraseList[EraseIdx].Bitmap.Clone();
			GraphicsEraser = Graphics.FromImage(ImageTmp); //DrawImage會導致畫面閃爍

			GraphicsEraser.DrawRectangle(new Pen(MetroColors.Black, 5), pos.X - EraserL / 2, pos.Y - EraserL / 2, EraserL, EraserL);
			GraphicsEraser.DrawRectangle(new Pen(MetroColors.Blue, 3), pos.X - EraserL / 2, pos.Y - EraserL / 2, EraserL, EraserL);

			GraphicsEraser.Dispose();
			PictureBoxEraser.Image = ImageTmp;
		}
		private void EraseImage(Point pos)
		{
			byte[] pixel = ImageEraseList[EraseIdx].Pixel;
			int idx;
			int x_ini = pos.X - EraserL / 2;
			int x_fin = pos.X + EraserL / 2;
			int y_ini = pos.Y - EraserL / 2;
			int y_fin = pos.Y + EraserL / 2;
			for (int x = x_ini; x < x_fin; x++)
			{
				for (int y = y_ini; y < y_fin; y++)
				{
					if (x < 0 || y < 0 || x >= ImageEraseList[EraseIdx].Width || y >= ImageEraseList[EraseIdx].Height)
						continue;
					idx = x * ImageEraseList[EraseIdx].Byte + y * ImageEraseList[EraseIdx].Stride;
					pixel[idx + 3] = 0; // A
				}
			}
			ImageEraseList[EraseIdx].Pixel = pixel;
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
		#endregion

		//完成Redo/Undo 的實作
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

		#region Other User Activities
		private void ButtonNext_Click(object sender, EventArgs e)
		{
			TabControlMain.SelectTab(CustomMethods.Clamp(TabControlMain.SelectedIndex + 1, TabControlMain.TabCount - 1, 0));
		}
		private void ButtonBack_Click(object sender, EventArgs e)
		{
			TabControlMain.SelectTab(CustomMethods.Clamp(TabControlMain.SelectedIndex - 1, TabControlMain.TabCount - 1, 0));
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
