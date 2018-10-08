using CycWpfLibrary.Media;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using static CycWpfLibrary.Math;
using System;

namespace WpfPlotDigitizer
{
  public static class ImageProcessing
  {
    public static byte[] FilterW(this PixelBitmap iptImage, int white = 200)
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

    private static bool IsGetAxis(Size AxSize) => (AxSize.Width > 0 && AxSize.Height > 0) ? true : false;
    private static bool IsColor(byte[] pixel, int i) => pixel[i + 3] != 0;
    public static (Size size, Point pos) GetAxisVer1(this PixelBitmap iptImage)
    {
      int L = 0, xTmp = 0, yTmp = 0, idx;
      int width = iptImage.Width, height = iptImage.Height;
      var AxSize = new Size();
      var AxPos = new Point();
      for (int x = 0; x < width / 2; x++)
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
            if (L > AxSize.Height) //確認是否比紀錄的最大值還大
            {
              AxPos.Y = yTmp;
              AxSize.Height = L;
            }
            L = 0; //長度歸零
          }
        }
      }
      for (int y = 0; y < height / 2; y++)
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
            if (L > AxSize.Width) //確認是否比紀錄的最大值還大
            {
              AxPos.X = xTmp;
              AxSize.Width = L;
            }
            L = 0; //長度歸零
          }
        }
      }

      Debug.Assert(IsGetAxis(AxSize));
      return (AxSize, AxPos);
    }

    public static bool IsAxisLeftTop(Point pos, PixelBitmap iptImage)
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
    public static Point GetAxisLeftTop(PixelBitmap iptImage)
    {
      var width = iptImage.Width;
      var height = iptImage.Height;
      Vector[] dirs = { new Vector(1,0), new Vector(-1, 1), new Vector(0,1), new Vector(1,-1) };
      var pos = new Point(0,0);
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
      return new Point(-1,-1);
    }
    public static bool IsAxisRightBottom(Point pos, PixelBitmap iptImage)
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
      Vector[] dirs = { new Vector(-1, 0), new Vector(1, -1), new Vector(0,-1), new Vector(-1, 1) };
      var pos = new Point(width-1, height-1);
      var dirID = 0;
      while (pos.X > width / 2 && pos.Y > height / 2)
      {
        if (IsAxisRightBottom(pos, iptImage))
        {
          return pos;
        }

        pos += dirs[dirID];
        if (!IsIn(pos.X, width / 2, width-1, true) ||
          !IsIn(pos.Y, height / 2, height-1, true))
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
    public static (Size size, Point pos) GetAxis(this PixelBitmap iptImage)
    {
      var AxisLTPos = GetAxisLeftTop(iptImage);
      var AxisRBPos = GetAxisRightBottom(iptImage);
      if (AxisLTPos.X < 0 || AxisRBPos.X < 0)
      {
        //Fail
        return (new Size(), new Point());
      }
      else
      {
        var AxisSize = new Size(AxisRBPos.X - AxisLTPos.X, AxisRBPos.Y - AxisLTPos.Y);
        return (AxisSize, AxisLTPos);
      }
    }

  }
}
