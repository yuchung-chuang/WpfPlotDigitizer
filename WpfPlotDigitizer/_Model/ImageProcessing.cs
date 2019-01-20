using CycWpfLibrary;
using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
using CycWpfLibrary.WinForm;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static CycWpfLibrary.Math;
using static System.Math;
using ct = System.Threading.CancellationToken;
using PB = CycWpfLibrary.Media.PixelBitmap;

namespace WpfPlotDigitizer
{
  #region Helper classes
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
  #endregion

  public static class ImageProcessing
  {
    public static byte[] FilterW(PB iptImage, int white = 200)
    {
      byte blue, green, red;
      byte[] optPixel = (byte[])iptImage.Pixel.Clone(); // 複製值
      for (int i = 0; i < iptImage.Pixel.Length; i += iptImage.Depth)
      {
        blue = iptImage.Pixel[i];   //B
        green = iptImage.Pixel[i + 1]; //G
        red = iptImage.Pixel[i + 2];  //R
        optPixel[i + 3] = (red < white || green < white || blue < white) ? (byte)255 : (byte)0; //A
      }
      return optPixel;
    }

    private static readonly int axisTol = 20;
    public static (Rect, AxisType) GetAxis(PB iptImage)
    {
      AxisType axisType = new AxisType();
      var (LT, LB, RT, RB) = GetAxisPoints();
      CheckAxisType();

      var axisTmp = new Rect(LT, RB + new Vector(1, 1));
      if (!IsValid(LT) || !IsValid(RB) ||
        iptImage.Width - axisTmp.Width < axisTol ||
        iptImage.Height - axisTmp.Height < axisTol)
        return (GetLongestAxis(), axisType);
      else
        return (axisTmp, axisType);

      // Local methods
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
      (Point LT, Point LB, Point RT, Point RB) GetAxisPoints()
      {
        var pixel3 = iptImage.GetPixel3Argb();
        return (GetAxisLT(pixel3), GetAxisLB(pixel3), GetAxisRT(pixel3), GetAxisRB(pixel3));
      }
      /// <summary>
      /// 搜索<paramref name="pixel3"/>中最靠近中心的左上角坐標軸。
      /// </summary>
      /// <remarks>若搜索不到角點，則回傳<see cref="Point"/>(<see cref="double.NaN"/>,<see cref="double.NaN"/>)。</remarks>
      Point GetAxisLT(byte[,,] pixel3)
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
            return GetInterAxisPos(pixel3, tracer.pos);
          }
          tracer.Move();
        }
        return output;
      }
      Point GetAxisLB(byte[,,] pixel3)
      {
        var pixel3Tmp = pixel3.RotateClockwise();
        var AxisPosTmp = GetAxisLT(pixel3Tmp);
        return new Point(AxisPosTmp.Y,
                          pixel3.GetLength(1) - AxisPosTmp.X);
      }
      Point GetAxisRB(byte[,,] pixel3)
      {
        var pixel3Tmp = pixel3.RotateClockwise(times: 2);
        var AxisPosTmp = GetAxisLT(pixel3Tmp);
        return new Point(pixel3.GetLength(0) - AxisPosTmp.X,
                          pixel3.GetLength(1) - AxisPosTmp.Y);
      }
      Point GetAxisRT(byte[,,] pixel3)
      {
        var pixel3Tmp = pixel3.RotateClockwise(times: 3);
        var AxisPosTmp = GetAxisLT(pixel3Tmp);
        return new Point(pixel3.GetLength(0) - AxisPosTmp.Y,
                          AxisPosTmp.X);
      }
      bool IsAxisLT(byte[,,] pixel3, Point pos)
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
      Point GetInterAxisPos(byte[,,] pixel3, Point iniPos)
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

      bool IsGetAxis(Rect AxRect) => (AxRect.Width > 0 && AxRect.Height > 0) ? true : false;
      bool IsTransparent(byte[] pixel, int i) => pixel[i + 3] == 0;
      /// <summary>
      /// 取得影像中最長的縱軸與橫軸。
      /// </summary>
      Rect GetLongestAxis()
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
            idx = x * iptImage.Depth + y * iptImage.Stride;
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
            idx = x * iptImage.Depth + y * iptImage.Stride;
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
    }

    public static Image<Bgra, byte> InRange(Image<Bgra, byte> iptImage, Color Max, Color Min)
    {
      var mask = iptImage.InRange(Min.ToBgra(), Max.ToBgra());
      var optImage = iptImage.Copy(mask);
      optImage.SetValue(Bgras.Transparent, mask.Not());
      return optImage;
    }
    public static async Task<Image<Bgra, byte>> InRangeAsync(Image<Bgra, byte> iptImage, Color Max, Color Min)
    {
      var optImage = new Image<Bgra, byte>(iptImage.Size);
      await Task.Run(() => optImage = InRange(iptImage, Max, Min));
      return optImage;
    }

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

    public static List<List<Point>> GetDataList(Image<Bgra, byte> image, int size)
    {
      var width = image.Width;
      var height = image.Height;
      var posLists = new List<List<Point>>();
      var searched = new bool[height, width];
      for (int col = 0; col < width; col++)
      {
        for (int row = 0; row < height; row++)
        {
          if (searched[row, col] || image.Data[row, col, 3] == 0)
            continue;
          var posList = FloodFill(row, col);

          if (posList.Count > Pow(size, 2))
          {
            posList.OrderBy(p => p.Y).ThenBy(p => p.X);
            posLists.Add(posList);
          }
        }
      }
      return posLists;

      List<Point> FloodFill(int row, int col)
      {
        var posList = new List<Point>();
        int colL, colR;
        bool IsSpan;
        Point pos = new Point(col, row);
        Stack<Point> stack = new Stack<Point>();
        stack.Push(pos);
        while (stack.Count != 0)
        {
          pos = stack.Pop();
          row = (int)pos.Y;
          col = (int)pos.X;
          //向右填充
          while (IsNeedFill())
          {
            posList.Add(new Point(col, row));
            searched[row, col] = true;
            col++;
          }
          colR = col - 1;
          col = (int)pos.X - 1;
          //向左填充
          while (IsNeedFill())
          {
            posList.Add(new Point(col, row));
            searched[row, col] = true;
            col--;
          }
          //處理上面一條掃描線
          col++;
          colL = col;
          row++;
          while (IsIn(row, height - 1, 0) && IsIn(col, colR, 0))
          {
            IsSpan = false;
            while (IsNeedFill())
            {
              IsSpan = true;
              col++;
            }
            if (IsSpan)
            {
              pos.X = col - 1;
              pos.Y = row;
              stack.Push(pos);
              IsSpan = false;
            }
            while (IsIn(col, colR, 0) && IsSkip())
              col++;
          }
          //處理下面一條掃描線
          col = colL;
          row = row - 2;
          while (IsIn(row, height - 1, 0) && IsIn(col, colR, 0))
          {
            IsSpan = false;
            while (IsNeedFill())
            {
              IsSpan = true;
              col++;
            }
            if (IsSpan)
            {
              pos.X = col - 1;
              pos.Y = row;
              stack.Push(pos);
              IsSpan = false;
            }
            while (IsIn(col, colR, 0) && IsSkip())
              col++;
          }
        }
        return posList;

        bool IsNeedFill() => IsIn(row, height - 1, 0) && IsIn(col, width - 1, 0) && !searched[row, col] && image.Data[row, col, 3] != 0;
        bool IsSkip() => searched[row, col] || image.Data[row, col, 3] == 0;
      }
    }
    public static List<Point> GetData(Image<Bgra, byte> image, List<List<Point>> posLists, int size, double ratio)
    {
      var posList = new List<Point>();
      foreach (var list in posLists)
      {
        var pivot = list[0].Add((size / 2, size / 2));
        foreach (var pos in list)
        {
          var delta = pos.Minus(pivot);
          if (delta.X % size == 0 && delta.Y % size == 0 && 
            IsData(pos))
          {
            posList.Add(pos);
          }
        }
      }
      return posList;

      bool IsData(Point pos)
      {
        var area = Pow(size, 2);
        var shift = new Vector(size / 2, size / 2);
        var rect = new Rect(pos, new Size(size, size)).Minus(shift);
        var colMax = image.Width - 1;
        var rowMax = image.Height - 1;
        var count = 0;
        for (int col = (int)rect.X; col < rect.Right; col++)
          for (int row = (int)rect.Y; row < rect.Bottom; row++)
            if (IsIn(row, rowMax, 0) && IsIn(col, colMax, 0) &&
              image.Data[row, col, 3] != 0)
              count++;
        return count >= area * ratio ? true : false;
      }
    }
    public static List<Point> TransformData(Image<Bgra, byte> image, List<Point> positions, Rect axLim, Point @base)
    {
      var width = image.Width;
      var height = image.Height;
      var dataList = new List<Point>();
      Point data;
      foreach (var pos in positions)
      {
        data = new Point
        {
          X = LinConvert(pos.X, width, 0, axLim.Right, axLim.Left),
          Y = LinConvert(height - pos.Y, height, 0, axLim.Bottom, axLim.Top),
        };

        //if (@base.X > 0)
        //  data.X = (float)Pow(
        //    @base.X,
        //    LinConvert(data.X, axLim.Left, axLim.Right,
        //      LogBase(@base.X, axLim.Left),
        //      LogBase(@base.X, axLim.Right))
        //    );
        //if (@base.Y > 0)
        //  data.Y = (float)Pow(
        //    @base.Y,
        //    LinConvert(data.Y, axLim.Top, axLim.Bottom,
        //      LogBase(@base.Y, axLim.Top),
        //      LogBase(@base.Y, axLim.Bottom))
        //    );
        dataList.Add(data);
      }
      return dataList;
    }

    #region Deprecated methods
    [Obsolete]
    public static PB FilterRGB(PB iptImage, Color Max, Color Min, ct token)
    {
      var optPixel = iptImage.Pixel.Clone() as byte[];
      var length = iptImage.Pixel.Length;
      var @byte = iptImage.Depth;
      byte R, G, B;
      // 針對圖像的for-loop會需要執行上萬次，迴圈內的call method開銷會非常大，必須要避免
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
      return new PB(optPixel, iptImage.Size);
    }
    [Obsolete]
    public static async Task<PB> FilterRGBAsync(PB iptImage, Color Max, Color Min, ct token)
    {
      PB optImage = new PB();
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
    [Obsolete]
    public static Image<Rgba, byte> FilterRGB_Emgu(Image<Rgba, byte> image, Color Max, Color Min, ct token)
    {
      var optPixel = image.Data.Clone() as byte[,,];
      var height = image.Height;
      var width = image.Width;
      byte R, G, B;
      for (int row = 0; row < height; row++)
      {
        for (int col = 0; col < width; col++)
        {
          B = optPixel[row, col, 0];
          G = optPixel[row, col, 1];
          R = optPixel[row, col, 2];
          if (R <= Max.R && R >= Min.R &&
              G <= Max.G && G >= Min.G &&
              B <= Max.B && B >= Min.B)
            optPixel[row, col, 3] = 255; //A
          else
            optPixel[row, col, 3] = 0; //A
        }
      }
      //new PB(optPixel).ShowSnapShot();
      return new Image<Rgba, byte>(optPixel);
    }
    [Obsolete]
    public static PB InRange(PB iptImage, Color Max, Color Min)
    {
      return InRange(iptImage.ToImage<Rgba, byte>(), Max, Min).ToPixelBitmap();
    }
    [Obsolete]
    public static async Task<PB> InRangeAsync(PB iptImage, Color Max, Color Min)
    {
      PB optImage = new PB();
      await Task.Run(() => optImage = InRange(iptImage, Max, Min));
      return optImage;
    }
    [Obsolete]
    private static readonly Rgba transparentRgba = new Rgba(0, 0, 0, 0);

    [Obsolete]
    public static Image<Rgba, byte> InRange(Image<Rgba, byte> iptImage, Color Max, Color Min)
    {
      var mask = iptImage.InRange(Min.ToRgba(), Max.ToRgba());
      var optImage = iptImage.Copy(mask);
      optImage.SetValue(transparentRgba, mask.Not());
      return optImage;
    }
    [Obsolete]
    public static async Task<Image<Rgba, byte>> InRangeAsync(Image<Rgba, byte> iptImage, Color Max, Color Min)
    {
      var optImage = new Image<Rgba, byte>(iptImage.Size);
      await Task.Run(() => optImage = InRange(iptImage, Max, Min));
      return optImage;
    }
    
    #endregion
  }
}
