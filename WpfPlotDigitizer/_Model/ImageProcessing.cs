using CycWpfLibrary;
using CycWpfLibrary.Media;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using static CycWpfLibrary.Math;

namespace WpfPlotDigitizer
{
  public static class ImageProcessing
  {
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

    private static bool IsGetAxis(Rect AxRect) => (AxRect.Width > 0 && AxRect.Height > 0) ? true : false;
    private static bool IsColor(byte[] pixel, int i) => pixel[i + 3] != 0;
    /// <summary>
    /// 取得<paramref name="iptImage"/>中最長的縱軸與橫軸。
    /// </summary>
    public static Rect GetLongestAxis(PixelBitmap iptImage)
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
          if (IsColor(iptImage.Pixel, idx))
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
          if (IsColor(iptImage.Pixel, idx)) //繼續記錄
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
    /// <returns>若搜索不到角點，則回傳<see cref="Point"/>(-1,-1)。</returns>
    private static Point GetAxisLT(byte[,,] pixel3)
    {
      var width = pixel3.GetLength(0);
      var height = pixel3.GetLength(1);
      Vector[] dirs = { new Vector(1, 0), new Vector(-1, 1), new Vector(0, 1), new Vector(1, -1) };
      Point output = new Point(-1, -1);
      var pos = new Point(0, 0);
      var dirID = 0;
      while (pos.X < width / 2 && pos.Y < height / 2)
      {
        if (IsAxisLT(pixel3, pos))
        {
          return GetInterAxisPos(pixel3, pos);
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
    public static Rect GetAxisLtRb(PixelBitmap iptImage)
    {
      var pixel3 = iptImage.Pixel3;
      var LT = GetAxisLT(pixel3);
      var RB = GetAxisRB(pixel3);
      return new Rect(LT, RB + new Vector(1, 1));
    }

    private static int AxisTol = 20;
    public static Rect GetAxis(PixelBitmap iptImage)
    {
      var axisTmp = GetAxisLtRb(iptImage);
      if (iptImage.Width - axisTmp.Width < AxisTol ||
        iptImage.Height - axisTmp.Height < AxisTol)
      {
        return GetLongestAxis(iptImage);
      }
      else
      {
        return axisTmp;
      }
    }

    public static bool IsRGBFilted(Color color, Color Max, Color Min)
    {
      return (color.R <= Max.R && color.R >= Min.R &&
              color.G <= Max.G && color.G >= Min.G &&
              color.B <= Max.B && color.B >= Min.B) ? true : false;
    }
    public static bool IsColor(byte[,,] pixel3, int x, int y) => pixel3[x, y, 0] != 0;
    public static PixelBitmap FilterRGB(PixelBitmap iptImage, Color Max, Color Min, string type)
    {
      byte selectedColor;
      Color colorNow;
      var optImage = iptImage.Clone() as PixelBitmap;
      var optPixel3 = optImage.Pixel3;
      int FilterMax, FilterMin, typeN;
      switch (type)
      {
        default:
        case "R":
          typeN = 1;
          FilterMax = Max.R;
          FilterMin = Min.R;
          break;
        case "G":
          typeN = 2;
          FilterMax = Max.G;
          FilterMin = Min.G;
          break;
        case "B":
          typeN = 3;
          FilterMax = Max.B;
          FilterMin = Min.B;
          break;
      }
      
      var width = iptImage.Width;
      var height = iptImage.Height;
      int Byte = iptImage.Byte;
      for (int x = 0; x < width; x++)
      {
        for (int y = 0; y < height; y++)
        {
          colorNow = new Color
          {
            A = optPixel3[x, y, 0],
            R = optPixel3[x, y, 1],
            G = optPixel3[x, y, 2],
            B = optPixel3[x, y, 3],
          };
          selectedColor = optPixel3[x, y, typeN];
          if (IsColor(optPixel3, x, y) &&
            !IsIn(selectedColor, FilterMax, FilterMin))
            optPixel3[x, y, 0] = 0;
          else if (IsRGBFilted(colorNow, Max, Min))
            optPixel3[x, y, 0] = 255;
        }
      }
      optImage.Pixel3 = optPixel3;
      return optImage;
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
    #endregion
  }
}
