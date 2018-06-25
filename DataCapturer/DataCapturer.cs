using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using static MyLibrary.Methods.Math;
using static MyLibrary.Methods.Drawing;
using static MyLibrary.Methods.System;
using MyLibrary.Classes;
using static System.Math;
using System.IO;
using System.Text;

namespace DataCapturer
{
  public partial class DataCapturer : MetroForm
  {
    #region Images
    private PixelImage ImageInput;
    private PixelImage ImageFilterW;
    private PixelImage ImageAxis;
    private PixelImage ImageFilterRGB;
    private PixelImage ImageErase
    {
      get => ImageEraseList[EraseIdx];
      set
      {
        ImageEraseList[EraseIdx] = value;
      }
    }
    private PixelImage ImageEraseTmp = new PixelImage();
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

    #region Constructor
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
    private void UnableControls()
    {
      AllControls = GetAllControls(this);
      foreach (Control control in AllControls)
        control.Enabled = false;

      IEnumerable<Control> TabPages = from control in AllControls
                                      where control is TabPage
                                      select control;
      foreach (Control control in TabPages)
        control.Enabled = true;

      TabControlMain.Enabled = true;
      ButtonBrowse.Enabled = true;
    }
    public DataCapturer()
    {
      InitializeComponent();

      TabControlMain.SelectedIndex = 0;

      ButtonNext.Text = "Next" + " " + StringArrow("right");
      ButtonBack.Text = StringArrow("left") + " " + "Back";

      UnableControls();
      EnableControls();

      this.Icon = Icon.FromHandle(Properties.Resources.icon5.GetHicon());
#if DEBUG
      UpdateImageInput(); // 測試用
#endif
    }
    #endregion

    #region Step 1: Browse
    //Entry
    private void ButtonBrowse_Click(object sender, EventArgs e)
    {
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
      UpdateImageInput();
    }
    //Works
    private void EnableControls()
    {
      foreach (Control control in AllControls)
        control.Enabled = true;
      TextBoxXBase.Enabled = false;
      TextBoxYBase.Enabled = false;
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
    private void UpdateImageInput()
    {
#if DEBUG
      ImageInput = new PixelImage(new Bitmap("C:\\Users\\alex\\Dropbox (Alex)\\SMCMLAB\\matlab\\DataCapturer\\images\\19451854599_fdc0d1a8d7_c.jpg"));
#else      
      ImageInput = new PixelImage(new Bitmap(openFileDialog.FileName));
#endif
      PictureBoxInput.Image = ImageInput.Bitmap;

      EnableControls();
      UpdateImageSetAxLim();
    }
    private void UpdateImageSetAxLim()
    {
      ImageViewerSetAxLim.Image = ImageInput.Bitmap;

      SliderAxLengthX.Maximum = ImageInput.Bitmap.Width;
      SliderAxLengthX.Value = SliderAxLengthX.Maximum / 2;
      SliderAxLengthY.Maximum = ImageInput.Bitmap.Height;
      SliderAxLengthY.Value = SliderAxLengthY.Maximum / 2;

      byte[] Pixel = FilterW(ImageInput, FilterWMax);
      ImageFilterW = new PixelImage(Pixel, ImageInput.Size);
      GetAxis();

      UpdateAllControls(); //Initialize
    }
    #endregion

    #region Step 2: Set Axis Limits
    //Entry Point
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
    #endregion

    #region Step 3: Get Frame
    private int FilterWMax = 200;
    private Point AxPos = new Point();
    private Size AxSize = new Size();
    private Point OffsetPos = new Point();
    private Size OffsetSize = new Size();
    private bool IsGetAxis => (AxSize.Width > 0 && AxSize.Height > 0) ? true : false;
    private bool IsOffset => (OffsetSize.Width > 0 && OffsetSize.Height > 0) ? true : false;
    private bool IsColor(byte[] pixel, int i) => pixel[i + 3] != 0;

    //Entry Point
    private void SliderAxLengthY_Scroll(object sender, ScrollEventArgs e)
    {
      if (BackgroundWorker.IsBusy != true)
      {
        BackgroundWorker.RunWorkerAsync(new BackgroundArgs(sender));
      }
    }
    private void SliderAxLengthX_Scroll(object sender, ScrollEventArgs e)
    {
      if (BackgroundWorker.IsBusy != true)
      {
        BackgroundWorker.RunWorkerAsync(new BackgroundArgs(sender));
      }
    }
    private void SliderAxisOffset_Scroll(object sender, ScrollEventArgs e)
    {
      if (BackgroundWorker.IsBusy != true)
      {
        BackgroundWorker.RunWorkerAsync(new BackgroundArgs(sender));
      }
    }
    //Background Work			
    private void GetAxis()
    {
      int L = 0, xTmp = 0, yTmp = 0, idx;
      AxSize = new Size(0, 0);
      for (int x = 0; x < ImageFilterW.Width; x++)
      {
        L = 0; yTmp = 0;
        for (int y = 0; y < ImageFilterW.Height; y++)
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
        {
          break;
        }
      }
      for (int y = 0; y < ImageFilterW.Height; y++)
      {
        L = 0; xTmp = 0;
        for (int x = 0; x < ImageFilterW.Width; x++)
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
        {
          break;
        }
      }

      if (!IsGetAxis)
      {
        PictureBoxGetAxis.Image = null;
        Console.WriteLine("GetAxis Error!");
        return;
      }
      OffsetAxis();
    }
    private void OffsetAxis()
    {
      OffsetPos = new Point(AxPos.X + AxisOffset, AxPos.Y + AxisOffset);
      OffsetSize = new Size(AxSize.Width - AxisOffset * 2, AxSize.Height - AxisOffset * 2);

      if (!IsOffset)
      {
        PictureBoxGetAxis.Image = null;
        Console.WriteLine("GetAxis Error!");
        return;
      }

      SetImageAxis();
    }
    private void SetImageAxis()
    {
      ImageAxis = new PixelImage(Crop(ImageInput.Bitmap, new Rectangle(OffsetPos, OffsetSize)));

      SetImageFilterRGB();
    }
    private void SetImageFilterRGB()
    {
      ImageFilterRGB = (PixelImage)ImageAxis.Clone();
      ImageFilterRGB.Pixel = FilterRGB(ImageFilterRGB, "R");
      ImageFilterRGB.Pixel = FilterRGB(ImageFilterRGB, "G");
      ImageFilterRGB.Pixel = FilterRGB(ImageFilterRGB, "B");

      SetImageErase();
    }
    #endregion

    #region Step 4: Filter
    private void RangeSliderRed_Scroll(object sender, EventArgs e)
    {
      if (BackgroundWorker.IsBusy != true)
      {
        BackgroundWorker.RunWorkerAsync(new BackgroundArgs(sender));
      }
    }
    private void RangeSliderGreen_Scroll(object sender, EventArgs e)
    {
      if (BackgroundWorker.IsBusy != true)
      {
        BackgroundWorker.RunWorkerAsync(new BackgroundArgs(sender));
      }
    }
    private void RangeSliderBlue_Scroll(object sender, EventArgs e)
    {
      if (BackgroundWorker.IsBusy != true)
      {
        BackgroundWorker.RunWorkerAsync(new BackgroundArgs(sender));
      }
    }
    private bool IsRGBFilted(byte R, byte G, byte B)
    {
      return (R <= FilterRMax && R >= FilterRMin &&
            G <= FilterGMax && G >= FilterGMin &&
              B <= FilterBMax && B >= FilterBMin) ? true : false;
    }
    private byte[] FilterRGB(PixelImage iptImage, string type)
    {
      byte R, G, B, color;
      byte[] optPixel = (byte[])iptImage.Pixel.Clone(); // 複製值
      int typeN = 2, FilterMax = FilterRMax, FilterMin = FilterRMin;
      switch (type)
      {
        case "R":
          break;
        case "G":
          typeN = 1;
          FilterMax = FilterGMax;
          FilterMin = FilterGMin;
          break;
        case "B":
          typeN = 0;
          FilterMax = FilterBMax;
          FilterMin = FilterBMin;
          break;
        default:
          break;
      }

      int Length = iptImage.Pixel.Length;
      int Byte = iptImage.Byte;
      for (int i = 0; i < Length; i += Byte)
      {
        B = optPixel[i];
        G = optPixel[i + 1];
        R = optPixel[i + 2];
        color = optPixel[i + typeN];
        if (IsColor(optPixel, i) && (!IsIn(color, FilterMax, FilterMin)))
          optPixel[i + 3] = 0; //A
        else if (IsRGBFilted(R, G, B))
          optPixel[i + 3] = 255; //A
      }
      return optPixel;
    }
    private void SetImageErase()
    {
      ImageEraseList.Clear();
      ImageEraseList.Add((PixelImage)ImageFilterRGB.Clone());
      EraseIdx = 0;
      SetImageOutput();
    }
    #endregion

    #region Background Work
    private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs DoWork_e)
    {
      if (ImageInput == null)
      {
        return;
      }
      BackgroundArgs arg = (BackgroundArgs)DoWork_e.Argument;
      if (arg.sender == SliderAxLengthX || arg.sender == SliderAxLengthY)
      {
        GetAxis();
      }
      else if (arg.sender == SliderAxisOffset)
      {
        OffsetAxis();
      }
      else if (arg.sender == RangeSliderRed)
      {
        ImageFilterRGB.Pixel = FilterRGB(ImageFilterRGB, "R");
        SetImageErase();
      }
      else if (arg.sender == RangeSliderGreen)
      {
        ImageFilterRGB.Pixel = FilterRGB(ImageFilterRGB, "G");
        SetImageErase();
      }
      else if (arg.sender == RangeSliderBlue)
      {
        ImageFilterRGB.Pixel = FilterRGB(ImageFilterRGB, "B");
        SetImageErase();
      }
      else if (arg.sender == ImageViewerErase)
      {
        MouseEventArgs e = arg.e as MouseEventArgs;
        if (e.Delta == 0)
        {
          PixelImage ImageErase = (PixelImage)arg.parameters[0];
          PixelImage ImageEraseDisplay = (PixelImage)arg.parameters[1];

          EffectiveMouseLocation = ImageViewerErase.GetEffectiveMouseLocation(e.Location);
          EffectiveMousePos = new Point(ImageViewerErase.ImageBoxPos.X + EffectiveMouseLocation.X, ImageViewerErase.ImageBoxPos.Y + EffectiveMouseLocation.Y);
          if (e.Button == MouseButtons.Left) //IsErasing
            ImageErase = EraseImage(EffectiveMousePos, ImageErase);
          ImageEraseDisplay.Bitmap = DrawEraser(ImageErase, EffectiveMousePos);
        } //Moving
      }
    }

    private void UpdateAllControls()
    {
      if (ImageInput == null)
      {
        return;
      }
      PictureBoxGetAxis.Image = (Bitmap)ImageAxis.Bitmap.Clone();
      PictureBoxFilter.Image = (Bitmap)ImageFilterRGB.Bitmap.Clone();
      ImageViewerErase.Image = (Bitmap)ImageEraseTmp.Bitmap.Clone();
    }
    private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      UpdateAllControls();
    }
    #endregion

    #region Step 5: Erase
    private int _EraseIdx = 0;
    private int EraseIdx
    {
      get => _EraseIdx;
      set
      {
        _EraseIdx = value;
        ImageEraseTmp.Bitmap = (Bitmap)ImageErase.Bitmap.Clone();
      }
    }
    private int EraserL = 20;
    private Point EffectiveMouseLocation;
    private Point EffectiveMousePos;
    //entry
    private void UndoButton_MouseUp(object sender, MouseEventArgs e)
    {
      UndoButtonIsPress = false;
      UpdateUndoButtonColor();

      if (UndoButton.ClientRectangle.Contains(e.Location))
        Undo();
    }
    private void RedoButton_MouseUp(object sender, MouseEventArgs e)
    {
      RedoButtonIsPress = false;
      UpdateRedoButtonColor();

      if (RedoButton.ClientRectangle.Contains(e.Location))
        Redo();
    }
    private void Undo()
    {
      if (EraseIdx > 0)
        EraseIdx -= 1;

      UpdateAllControls();
      SetImageOutput();
    }
    private void Redo()
    {
      if (EraseIdx < ImageEraseList.Count - 1)
        EraseIdx += 1;

      UpdateAllControls();
      SetImageOutput();
    }

    private void ImageViewerErase_MouseMove(object sender, MouseEventArgs e)
    {
      if (BackgroundWorker.IsBusy != true && e.Button != MouseButtons.Right) //右鍵留給drag，就不會有兩個backgroundWorker同時並行
      {
        object[] parameters = { ImageErase, ImageEraseTmp };
        BackgroundWorker.RunWorkerAsync(new BackgroundArgs(sender, e, parameters));
      }
    }
    private void ImageViewerErase_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        ImageEraseList.RemoveRange(EraseIdx + 1, ImageEraseList.Count - EraseIdx - 1); //清除所有原先的Redo
        ImageEraseList.Add((PixelImage)ImageErase.Clone());//或許需要設置ImageEraseList的儲存上限
        EraseIdx += 1;

        ImageViewerErase_MouseMove(sender, e);
      }
    }
    private void ImageViewerErase_MouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        SetImageOutput();
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
    // background work
    private Bitmap DrawEraser(PixelImage ImageErase, Point pos)
    {
      Bitmap imageTmp = (Bitmap)ImageErase.Bitmap.Clone();
      using (Graphics graphics = Graphics.FromImage(imageTmp))//DrawImage會導致畫面閃爍
      {
        using (Pen pen = new Pen(MetroColors.Black, 5))
        {
          graphics.DrawRectangle(pen, pos.X - EraserL / 2, pos.Y - EraserL / 2, EraserL, EraserL);
        }
        using (Pen pen = new Pen(MetroColors.Blue, 3))
        {
          graphics.DrawRectangle(pen, pos.X - EraserL / 2, pos.Y - EraserL / 2, EraserL, EraserL);
        }
      };
      return imageTmp;
    }
    private PixelImage EraseImage(Point pos, PixelImage ImageErase)
    {
      byte[] pixel = (byte[])ImageErase.Pixel.Clone();
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
      return ImageErase;
    }

    private void SetImageOutput()
    {
      UpdateUndoButtonColor();
      UpdateRedoButtonColor();

      ImageOutput = (PixelImage)ImageErase.Clone();
      UpdateImageOutput();
    }
    #endregion

    #region Step 6: Output 
    private void UpdateImageOutput()
    {
      PictureBoxOutput.Image = ImageOutput.Bitmap;
    }

    private bool IsDataOK()
    {
      if (TextBoxXlo.Text == string.Empty || TextBoxYlo.Text == string.Empty || TextBoxXhi.Text == string.Empty || TextBoxYhi.Text == string.Empty || (CheckBoxXLog.Checked && TextBoxXBase.Text == string.Empty) || (CheckBoxYLog.Checked && TextBoxYBase.Text == string.Empty))
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
    private List<PointF> GetData()
    {
      int idx, ImageAxis_x, ImageAxis_y;
      float DataAxis_x, DataAxis_y;
      float xlo = float.Parse(TextBoxXlo.Text);
      float xhi = float.Parse(TextBoxXhi.Text);
      float ylo = float.Parse(TextBoxYlo.Text);
      float yhi = float.Parse(TextBoxYhi.Text);
      float xbase = float.Parse(TextBoxXBase.Text);
      float ybase = float.Parse(TextBoxYBase.Text);
      List<PointF> optPoints = new List<PointF>();

      for (int x = 0; x < ImageOutput.Width; x++)
      {
        for (int y = 0; y < ImageOutput.Height; y++)
        {
          //右移x個像素(*一個字節的長度)下移y個像素(*一整行字節的長度)
          idx = x * ImageOutput.Byte + y * ImageOutput.Stride;
          if (!IsColor(ImageOutput.Pixel, idx))
            continue;

          ImageAxis_x = x;
          ImageAxis_y = ImageOutput.Height - y;

          DataAxis_x = LinConvert(ImageAxis_x, ImageOutput.Width, 0, xhi, xlo);
          DataAxis_y = LinConvert(ImageAxis_y, ImageOutput.Height, 0, yhi, ylo);

          if (CheckBoxXLog.Checked)
            DataAxis_x = (float)Pow(xbase, LinConvert(DataAxis_x, xlo, xhi, LogBase(xbase, xlo), LogBase(xbase, xhi)));
          if (CheckBoxYLog.Checked)
            DataAxis_y = (float)Pow(ybase, LinConvert(DataAxis_y, ylo, yhi, LogBase(ybase, ylo), LogBase(ybase, yhi)));

          optPoints.Add(new PointF(DataAxis_x, DataAxis_y));
        }
      }
      return optPoints;
    }
    List<PointF> Data = new List<PointF>();
    private void UpdateData()
    {
      if (!IsDataOK())
        return;
      Data = GetData();
      DataGridView.DataSource = Data;
    }

    private bool IsBusySaving = false;
    private bool SaveAsExcel()
    {
      try
      {
        var excel = new Excel.Application()
        {
          Visible = false,
          DisplayAlerts = false,
        };
        excel.Workbooks.Add(Type.Missing);
        var wBook = excel.Workbooks[1];
        wBook.Activate();
        var wSheet = (Excel._Worksheet)wBook.Worksheets[1];
        wSheet.Activate();

        excel.Cells[1, 1] = "X";
        excel.Cells[1, 2] = "Y";
        int dataCount = Data.Count;
        for (int i = 0; i < dataCount; i++)
        {
          excel.Cells[i + 2, 1] = Data[i].X;
          excel.Cells[i + 2, 2] = Data[i].Y;
        }
        wBook.SaveAs(saveFileDialog.FileName);
        wBook.Close(false);
        excel.Quit();
        System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
    private bool SaveAsExcelFast()
    {
      // ToArray
      int dataCount = Data.Count;
      object[,] dataArray = new object[Data.Count + 1, 2];
      dataArray[0, 0] = "X";
      dataArray[0, 1] = "Y";
      for (int i = 0; i < dataCount; i++)
      {
        dataArray[i + 1, 0] = Data[i].X;
        dataArray[i + 1, 1] = Data[i].Y;
      }

      var excel = new Excel.Application()
      {
        Visible = false,
        DisplayAlerts = false,
      };
      var wBook = excel.Workbooks.Add(Type.Missing);
      var wSheet = (Excel._Worksheet)wBook.Worksheets[1];
      try
      {
        wBook.Activate();
        wSheet.Activate();

        string finalColLetter = "B";
        string excelRange = string.Format("A1:{0}{1}",
            finalColLetter, dataCount + 1);

        wSheet.get_Range(excelRange, Type.Missing).Value2 = dataArray;
        wBook.SaveAs(saveFileDialog.FileName);
        wBook.Close(false);
        excel.Quit();
        return true;
      }
      catch (Exception)
      {
        return false;
      }
      finally
      {
        System.Runtime.InteropServices.Marshal.ReleaseComObject(wSheet);
        System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
        wSheet = null;
        excel = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }
    }
    private bool SaveAsCSV()
    {
      try
      {
        string strPath = saveFileDialog.FileName;

        StringBuilder content = new StringBuilder();
        content.AppendLine("X,Y");
        int dataCount = Data.Count;
        for (int i = 0; i < dataCount; i++)
        {
          content.AppendLine(Data[i].X.ToString() + "," + Data[i].Y.ToString());
        }

        using (var fs = File.OpenWrite(strPath))
        using (var sw = new StreamWriter(fs))
        {
          sw.Write(content.ToString());
        }
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
    private bool SaveAsTXT()
    {
      try
      {
        string strPath = saveFileDialog.FileName;

        StringBuilder content = new StringBuilder();
        content.AppendLine("X\tY");
        int dataCount = Data.Count;
        for (int i = 0; i < dataCount; i++)
        {
          content.AppendLine(Data[i].X.ToString() + "\t" + Data[i].Y.ToString());
        }

        using (var fs = File.OpenWrite(strPath))
        using (var sw = new StreamWriter(fs))
        {
          sw.Write(content.ToString());
        }
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
    private void ButtonSave_Click(object sender, EventArgs e)
    {
      if (IsBusySaving)
      {
        return;
      }
      IsBusySaving = true;
      bool IsSucessfulSave = false;
      saveFileDialog.Filter = "Excel (.xlsx) | *.xlsx |CSV (.csv) | *.csv |TXT (.txt) | *.txt";
      if (saveFileDialog.ShowDialog() == DialogResult.OK)
      {
        switch (saveFileDialog.FilterIndex)
        {
          case 1:
            IsSucessfulSave = SaveAsExcelFast();
            break;
          case 2:
            IsSucessfulSave = SaveAsCSV();
            break;
          case 3:
            IsSucessfulSave = SaveAsTXT();
            break;
          default:
            break;
        }

      }
      else
      {
        return;
      }

      IsBusySaving = false;
      if (IsSucessfulSave)
      {
        MessageBox.Show("Sucessfully saved!");
      }
      else
      {
        MessageBox.Show("Sorry... there's something wrong while saving...");
      }

    }
    #endregion

    #region Other User Activities
    private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (TabControlMain.SelectedIndex == TabControlMain.TabCount - 1)
      {
        UpdateData();
      }

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
      TabControlMain.SelectTab(Clamp(TabControlMain.SelectedIndex + 1, TabControlMain.TabCount - 1, 0));
    }
    private void ButtonBack_Click(object sender, EventArgs e)
    {
      TabControlMain.SelectTab(Clamp(TabControlMain.SelectedIndex - 1, TabControlMain.TabCount - 1, 0));
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
