using CycWpfLibrary;
using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
using CycWpfLibrary.WinForm;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static CycWpfLibrary.Math;

namespace WpfPlotDigitizer
{
  public struct AxisType
  {
    public bool Left;
    public bool Top;
    public bool Right;
    public bool Bottom;

    public AxisType(bool left, bool top, bool right, bool bottom)
    {
      Left = left;
      Top = top;
      Right = right;
      Bottom = bottom;
    }
  }

  internal enum TracerState
  {
    Normal = 0,
    OutTurn1 = 1,
    OutTrun2 = 2,
    OutTurned = 3,
  }
  internal class PixelTracer
  {
    public static readonly Vector[] dirs =
    {
      new Vector(1, -1),
      new Vector(1, 0),
      new Vector(-1, 1),
      new Vector(0, 1),
    };

    private int _dirID = 0;
    public int dirID
    {
      get => _dirID;
      set => _dirID = (value >= dirs.Length) ? 0 : value;
    }
    public System.Drawing.Point intPos => pos.ToWinForm();
    public Point pos;
    public TracerState state = TracerState.Normal;
    public (double XMax, double XMin, double YMax, double YMin) boundary;
    public int steps = 0;
    private int stepTotal;

    public PixelTracer(double XMax, double YMax, double XMin, double YMin)
    {
      pos = new Point(XMin + 1, YMin + 1);
      boundary = (XMax, XMin, YMax, YMin);
      stepTotal = (int)(boundary.XMax - boundary.XMin) * (int)(boundary.YMax - boundary.YMin);
    }

    public void Move()
    {
      pos += dirs[dirID];
      UpdateState();
      UpdateDir();
      steps++;
    }
    private bool IsOutsideBoundary() =>
      !IsIn(pos.X, boundary.XMax, boundary.XMin, excludeBoundary: true) ||
      !IsIn(pos.Y, boundary.YMax, boundary.YMin, excludeBoundary: true);
    private void UpdateState()
    {
      switch (state)
      {
        case TracerState.Normal:
          if (IsOutsideBoundary())
            state++;
          break;
        case TracerState.OutTurn1:
          state++;
          break;
        case TracerState.OutTrun2:
          if (IsOutsideBoundary())
            state++;
          else
            state = TracerState.Normal;
          break;
        case TracerState.OutTurned:
          if (!IsOutsideBoundary())
            state = TracerState.Normal;
          break;
        default:
          break;
      }
    }
    private void UpdateDir()
    {
      if (state == TracerState.OutTurn1 || state == TracerState.OutTrun2)
        dirID++;
    }
    public bool IsCompleted() => steps > stepTotal;

  }

  public static class ImageProcessing
  {
    // FilterW
    public static byte[] FilterW(PixelBitmap iptImage, int white = 200)
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

    //GetAxis
    private static bool IsGetAxis(Rect AxRect) => (AxRect.Width > 0 && AxRect.Height > 0) ? true : false;
    private static bool IsTransparent(byte[] pixel, int i) => pixel[i + 3] == 0;
    /// <summary>
    /// 取得<paramref name="iptImage"/>中最長的縱軸與橫軸。
    /// </summary>
    private static Rect GetLongestAxis(PixelBitmap iptImage)
    {
      int L = 0, xTmp = 0, yTmp = 0, idx;
      int width = iptImage.Width, height = iptImage.Height;
      var AxRect = new Rect();
      for (int x = 0; x < width; x++)
      {
        L = 0; yTmp = 0;
        for (int y = 0; y < height; y++)
        {
          //右移x個像素(*一個字節的長度)下移y個像素(*一整行字節的長度)
          idx = x * iptImage.Byte + y * iptImage.Stride;
          if (!IsTransparent(iptImage.Pixel, idx))
          {
            if (L == 0) //如果長度歸零
              yTmp = y; //更新lo索引
            L++; //長度+1
          }
          else //中斷紀錄
          {
            if (L > AxRect.Height) //確認是否比紀錄的最大值還大
            {
              AxRect.Y = yTmp;
              AxRect.Height = L;
            }
            L = 0; //長度歸零
          }
        }
      }
      for (int y = 0; y < height; y++)
      {
        L = 0; xTmp = 0;
        for (int x = 0; x < width; x++)
        {
          // 右移x個像素(*一個字節的長度)下移y個像素(*一整行字節的長度)
          idx = x * iptImage.Byte + y * iptImage.Stride;
          if (!IsTransparent(iptImage.Pixel, idx)) //繼續記錄
          {
            if (L == 0) //如果長度歸零
              xTmp = x; //更新lo索引
            L++; //長度+1
          }
          else //中斷紀錄
          {
            if (L > AxRect.Width) //確認是否比紀錄的最大值還大
            {
              AxRect.X = xTmp;
              AxRect.Width = L;
            }
            L = 0; //長度歸零
          }
        }
      }

      Debug.Assert(IsGetAxis(AxRect));
      return AxRect;
    }

    private static bool IsAxisLT(byte[,,] pixel3, Point pos)
    {
      var width = pixel3.GetLength(0);
      var height = pixel3.GetLength(1);
      var yTmp = (int)pos.Y;
      for (int x = (int)pos.X; x < pos.X + width / 2; x++)
      {
        if (pixel3[x, yTmp, 0] == 0)
        {
          return false;
        }
      }

      var xTmp = (int)pos.X;
      for (int y = (int)pos.Y; y < pos.Y + height / 2; y++)
      {
        if (pixel3[xTmp, y, 0] == 0)
        {
          return false;
        }
      }
      return true;
    }
    private static Point GetInterAxisPos(byte[,,] pixel3, Point iniPos)
    {
      var width = pixel3.GetLength(0);
      var height = pixel3.GetLength(1);
      Vector[] dirs = { new Vector(1, 0), new Vector(-1, 1), new Vector(0, 1), new Vector(1, -1) };
      Point output = new Point(-1, -1);
      var pos = iniPos;
      var dirID = 0;
      var failCount = 0;
      var failTimes = 20;
      do
      {
        if (pixel3[(int)pos.X + 1, (int)pos.Y + 1, 0] == 0)
        {
          return pos;
        }
        if (!IsAxisLT(pixel3, pos))
        {
          failCount++;
        }

        pos += dirs[dirID];
        if (!IsIn(pos.X, width / 2, iniPos.X, true) ||
          !IsIn(pos.Y, height / 2, iniPos.Y, true))
        {
          dirID++;
          if (dirID > 3)
          {
            dirID = 0;
          }
        }
      }
      while (failCount < failTimes);
      return iniPos;
    }
    /// <summary>
    /// 搜索<paramref name="pixel3"/>中最靠近中心的左上角坐標軸。
    /// </summary>
    /// <remarks>若搜索不到角點，則回傳<see cref="Point"/>(<see cref="double.NaN"/>,<see cref="double.NaN"/>)。</remarks>
    private static Point GetAxisLT(byte[,,] pixel3)
    {
      var width = pixel3.GetLength(0);
      var height = pixel3.GetLength(1);
      var offset = 5;
      var tracer = new PixelTracer(width / 2, height / 2, offset, offset);
      Point output = new Point(double.NaN, double.NaN);

      while (!tracer.IsCompleted())
      {
        if (IsAxisLT(pixel3, tracer.pos))
        {
          //new PixelBitmap(pixel3).ShowSnapShot();
          return GetInterAxisPos(pixel3, tracer.pos);
        }
        //else
        //{
        //  pixel3[tracer.intPos.X, tracer.intPos.Y, 0] = 255;
        //  pixel3[tracer.intPos.X, tracer.intPos.Y, 1] = 255;
        //  pixel3[tracer.intPos.X, tracer.intPos.Y, 2] = 0;
        //  pixel3[tracer.intPos.X, tracer.intPos.Y, 3] = 0;
        //}

        tracer.Move();
      }
      return output;
    }
    private static Point GetAxisLB(byte[,,] pixel3)
    {
      var pixel3Tmp = pixel3.RotateClockwise();
      var AxisPosTmp = GetAxisLT(pixel3Tmp);
      return new Point(AxisPosTmp.Y,
                        pixel3.GetLength(1) - AxisPosTmp.X);
    }
    private static Point GetAxisRB(byte[,,] pixel3)
    {
      var pixel3Tmp = pixel3.RotateClockwise(times: 2);
      var AxisPosTmp = GetAxisLT(pixel3Tmp);
      return new Point(pixel3.GetLength(0) - AxisPosTmp.X,
                        pixel3.GetLength(1) - AxisPosTmp.Y);
    }
    private static Point GetAxisRT(byte[,,] pixel3)
    {
      var pixel3Tmp = pixel3.RotateClockwise(times: 3);
      var AxisPosTmp = GetAxisLT(pixel3Tmp);
      return new Point(pixel3.GetLength(0) - AxisPosTmp.Y,
                        AxisPosTmp.X);
    }
    private static Rect GetAxisLtRb(PixelBitmap iptImage)
    {
      var pixel3 = iptImage.Pixel3;
      var LT = GetAxisLT(pixel3);
      var RB = GetAxisRB(pixel3);
      return new Rect(LT, RB + new Vector(1, 1));
    }
    private static (Point LT, Point LB, Point RT, Point RB) GetAxisPoints(PixelBitmap iptImage)
    {
      var pixel3 = iptImage.Pixel3;
      return (GetAxisLT(pixel3), GetAxisLB(pixel3), GetAxisRT(pixel3), GetAxisRB(pixel3));
    }
    private static readonly int axisTol = 20;
    public static (Rect, AxisType) GetAxis(PixelBitmap iptImage)
    {
      AxisType axisType = new AxisType();
      var (LT, LB, RT, RB) = GetAxisPoints(iptImage);
      CheckAxisType();

      var axisTmp = new Rect(LT, RB + new Vector(1, 1));
      if (!IsValid(LT) || !IsValid(RB) ||
        iptImage.Width - axisTmp.Width < axisTol ||
        iptImage.Height - axisTmp.Height < axisTol)
        return (GetLongestAxis(iptImage), axisType);
      else
        return (axisTmp, axisType);

      bool IsValid(Point point) => !(double.IsNaN(point.X));
      void CheckAxisType()
      {
        if (IsValid(LT))
        {
          axisType.Left = true;
          axisType.Top = true;
        }
        if (IsValid(RT))
        {
          axisType.Left = true;
          axisType.Bottom = true;
        }
        if (IsValid(LB))
        {
          axisType.Right = true;
          axisType.Top = true;
        }
        if (IsValid(RB))
        {
          axisType.Right = true;
          axisType.Bottom = true;
        }
      }
    }

    // FilterRGB
    private static bool IsRGBFilted(Color color, Color Max, Color Min)
    {
      return (color.R <= Max.R && color.R >= Min.R &&
              color.G <= Max.G && color.G >= Min.G &&
              color.B <= Max.B && color.B >= Min.B) ? true : false;
    }
    private static bool IsRGBFilted(byte R, byte G, byte B, Color Max, Color Min)
    {
      return (R <= Max.R && R >= Min.R &&
              G <= Max.G && G >= Min.G &&
              B <= Max.B && B >= Min.B) ? true : false;
    }

    private static bool IsTransparent(byte[,,] pixel3, int x, int y) => pixel3[x, y, 0] == 0;
    public static PixelBitmap FilterRGB(PixelBitmap iptImage, Color Max, Color Min, CancellationToken token)
    {
      var optPixel = iptImage.Pixel.Clone() as byte[];
      var length = iptImage.Pixel.Length;
      var @byte = iptImage.Byte;
      byte R, G, B;
      // For-loop 內千萬不要呼叫函式!!! 速度會變得超級慢!!!
      for (int i = 0; i < length; i += @byte)
      {
        if (token.IsCancellationRequested) // 當工作被取消...
          token.ThrowIfCancellationRequested(); //拋出協作式異常

        B = optPixel[i + 0];
        G = optPixel[i + 1];
        R = optPixel[i + 2];
        if (R <= Max.R && R >= Min.R &&
            G <= Max.G && G >= Min.G &&
            B <= Max.B && B >= Min.B)
          optPixel[i + 3] = 255; //A
        else
          optPixel[i + 3] = 0; //A

      }
      return new PixelBitmap(optPixel, iptImage.Size);
    }
    public static async Task<PixelBitmap> FilterRGBAsync(PixelBitmap iptImage, Color Max, Color Min, CancellationToken token)
    {
      PixelBitmap optImage = new PixelBitmap();
      await Task.Run(() =>
      {
        try
        {
          optImage = FilterRGB(iptImage, Max, Min, token);
        }
        catch (OperationCanceledException)
        {
          token.ThrowIfCancellationRequested();
        }
      }, token);
      return optImage;
    }

    // AutoGetAxisLimits
    public static Tesseract.Character[] OcrImage(Tesseract ocr, IInputArray image)
    {
      ocr.SetImage(image);

      if (ocr.Recognize() != 0)
        throw new Exception("Failed to recognizer image");

      var characters = ocr.GetCharacters();

      return characters;
    }
    public static void DrawCharacters(IInputOutputArray image, Tesseract.Character[] characters)
    {
      var color = Colors.Blue.ToMCvScalar();
      foreach (Tesseract.Character c in characters)
      {
        CvInvoke.Rectangle(image, c.Region, color);
        CvInvoke.PutText(image, c.Text, c.Region.Location, FontFace.HersheyPlain, 1, color);
      }
    }

    public static Tesseract InitializeOcr(string path, string lang, OcrEngineMode mode, string whiteList)
    {
      try
      {
        if (string.IsNullOrEmpty(path))
          path = ".";

        var pathFinal = path.Length == 0 || path.Substring(path.Length - 1, 1).Equals(Path.DirectorySeparatorChar.ToString()) ?
          path : string.Format("{0}{1}", path, Path.DirectorySeparatorChar);
        var ocr = new Tesseract();
        ocr.Init(pathFinal, lang, mode);
        ocr.SetVariable("tessedit_char_whitelist", whiteList);
        return ocr;
      }
      catch (Exception)
      {
        return null;
      }
    }


    #region Deprecated methods
    private static bool IsAxisLeftTop(Point pos, PixelBitmap iptImage)
    {
      var width = iptImage.Width;
      var height = iptImage.Height;
      var pixel3 = iptImage.Pixel3;
      var yTmp = (int)pos.Y;
      for (int x = (int)pos.X; x < pos.X + width / 2; x++)
      {
        if (pixel3[x, yTmp, 0] == 0)
        {
          return false;
        }
      }

      var xTmp = (int)pos.X;
      for (int y = (int)pos.Y; y < pos.Y + height / 2; y++)
      {
        if (pixel3[xTmp, y, 0] == 0)
        {
          return false;
        }
      }
      return true;
    }
    private static Point GetAxisLeftTop(PixelBitmap iptImage)
    {
      var width = iptImage.Width;
      var height = iptImage.Height;
      Vector[] dirs = { new Vector(1, 0), new Vector(-1, 1), new Vector(0, 1), new Vector(1, -1) };
      var pos = new Point(0, 0);
      var dirID = 0;
      while (pos.X < width / 2 && pos.Y < height / 2)
      {
        if (IsAxisLeftTop(pos, iptImage))
        {
          return pos;
        }

        pos += dirs[dirID];
        if (!IsIn(pos.X, width / 2, 0, true) ||
          !IsIn(pos.Y, height / 2, 0, true))
        {
          dirID++;
          if (dirID > 3)
          {
            dirID = 0;
          }
        }
      }
      // Fail
      return new Point(-1, -1);
    }
    private static bool IsAxisRightBottom(Point pos, PixelBitmap iptImage)
    {
      var width = iptImage.Width;
      var height = iptImage.Height;
      var pixel3 = iptImage.Pixel3;
      var yTmp = (int)pos.Y;
      for (int x = (int)pos.X; x > pos.X - width / 2; x--)
      {
        if (pixel3[x, yTmp, 0] == 0)
        {
          return false;
        }
      }

      var xTmp = (int)pos.X;
      for (int y = (int)pos.Y; y > pos.Y - height / 2; y--)
      {
        if (pixel3[xTmp, y, 0] == 0)
        {
          return false;
        }
      }
      return true;
    }
    private static Point GetAxisRightBottom(PixelBitmap iptImage)
    {
      var width = iptImage.Width;
      var height = iptImage.Height;
      Vector[] dirs = { new Vector(-1, 0), new Vector(1, -1), new Vector(0, -1), new Vector(-1, 1) };
      var pos = new Point(width - 1, height - 1);
      var dirID = 0;
      while (pos.X > width / 2 && pos.Y > height / 2)
      {
        if (IsAxisRightBottom(pos, iptImage))
        {
          return pos;
        }

        pos += dirs[dirID];
        if (!IsIn(pos.X, width / 2, width - 1, true) ||
          !IsIn(pos.Y, height / 2, height - 1, true))
        {
          dirID++;
          if (dirID > 3)
          {
            dirID = 0;
          }
        }
      }
      // Fail
      return new Point(-1, -1);
    }
    [Obsolete("Use GetAxis Instead", true)]
    public static (Point LT, Point RB) GetAxisPosV1(PixelBitmap iptImage)
    {
      var AxisLTPos = GetAxisLeftTop(iptImage);
      var AxisRBPos = GetAxisRightBottom(iptImage);
      if (AxisLTPos.X < 0 || AxisRBPos.X < 0)
      {
        //Fail
        return (new Point(-1, -1), new Point(-1, -1));
      }
      else
      {
        //var AxisSize = new Size(AxisRBPos.X - AxisLTPos.X, AxisRBPos.Y - AxisLTPos.Y);
        //return (AxisSize, AxisLTPos);
        return (AxisLTPos, AxisRBPos);
      }
    }

    [Obsolete]
    public static Tesseract.Character[] OcrImageThreshold(Tesseract ocr, Mat imageInput, Mat imageOutput, double threshold)
    {
      Mat imgGrey = new Mat();
      CvInvoke.CvtColor(imageInput, imgGrey, ColorConversion.Bgr2Gray);
      Mat imgThresholded = new Mat();
      CvInvoke.Threshold(imgGrey, imgThresholded, threshold, 255, ThresholdType.Binary);

      ocr.SetImage(imgThresholded);
      var characters = ocr.GetCharacters();
      imageOutput = imgThresholded;

      return characters;
    }
    [Obsolete]
    public static Tesseract.Character[] OcrImage(Tesseract ocr, Mat imageInput, Mat imageOutput)
    {
      if (imageInput.NumberOfChannels == 1)
        CvInvoke.CvtColor(imageInput, imageOutput, ColorConversion.Gray2Bgr);
      else
        imageInput.CopyTo(imageOutput);

      ocr.SetImage(imageOutput);

      if (ocr.Recognize() != 0)
        throw new Exception("Failed to recognizer image");

      var characters = ocr.GetCharacters();

      if (characters.Length != 0)
        return characters;

      characters = OcrImageThreshold(ocr, imageInput, imageOutput, 65);

      if (characters.Length != 0)
        return characters;

      characters = OcrImageThreshold(ocr, imageInput, imageOutput, 190);

      return characters;
    }
    [Obsolete]
    public static void DrawCharacters(Mat image, Tesseract.Character[] characters)
    {
      var color = Colors.Red.ToMCvScalar();
      foreach (Tesseract.Character c in characters)
      {
        CvInvoke.Rectangle(image, c.Region, color);
        CvInvoke.PutText(image, c.Text, c.Region.Location, FontFace.HersheyPlain, 1, color);
      }
    }

    [Obsolete]
    public static PixelBitmap FilterRGB2(PixelBitmap iptImage, Color Max, Color Min, FilterType2 type, CancellationToken token)
    {
      var optPixel = iptImage.Pixel.Clone() as byte[];
      int FilterMax, FilterMin, typeN;
      switch (type)
      {
        default:
        case FilterType2.Red:
          typeN = 2;
          FilterMax = Max.R;
          FilterMin = Min.R;
          break;
        case FilterType2.Green:
          typeN = 1;
          FilterMax = Max.G;
          FilterMin = Min.G;
          break;
        case FilterType2.Blue:
          typeN = 0;
          FilterMax = Max.B;
          FilterMin = Min.B;
          break;
      }

      var length = iptImage.Pixel.Length;
      int @byte = iptImage.Byte;
      byte color;
      Color colorNow;
      for (int i = 0; i < length; i += @byte)
      {
        // 當工作被取消...
        if (token.IsCancellationRequested)
          break;

        colorNow = new Color
        {
          B = optPixel[i + 0],
          G = optPixel[i + 1],
          R = optPixel[i + 2],
        };
        color = optPixel[i + typeN];
        if (!IsTransparent(optPixel, i) && (!IsIn(color, FilterMax, FilterMin)))
          optPixel[i + 3] = 0; //A
        else if (IsRGBFilted(colorNow, Max, Min))
          optPixel[i + 3] = 255; //A
      }

      //當工作被取消...
      if (token.IsCancellationRequested)
        //拋出協作式異常
        token.ThrowIfCancellationRequested();

      return new PixelBitmap(optPixel, iptImage.Size);
    }
    #endregion
  }

  [Obsolete]
  public enum FilterType2
  {
    Red,
    Green,
    Blue
  }
  [Obsolete]
  /// <summary>
  /// 偶數為Min, 奇數為Max
  /// </summary>
  public enum FilterType
  {
    RMax = 5,
    RMin = 4,
    GMax = 3,
    GMin = 2,
    BMax = 1,
    BMin = 0,
  }
  [Obsolete]
  public static class FilterTypeExtention
  {
    /// <summary>
    /// 根據<see cref="FilterType"/>取得<see cref="PixelBitmap.Pixel"/>中顏色的位移索引。
    /// </summary>
    /// <remarks>int division rounds toward zero</remarks>
    public static int GetColorIndex(this FilterType type) => (int)type / 2;
  }
}
