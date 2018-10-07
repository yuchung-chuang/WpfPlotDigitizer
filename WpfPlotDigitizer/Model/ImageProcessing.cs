using CycWpfLibrary.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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
    public static (Size size, Point pos) GetAxis(this PixelBitmap iptImage)
    {
      int L = 0, xTmp = 0, yTmp = 0, idx;
      int width = iptImage.Width, height = iptImage.Height;
      var AxSize = new Size();
      var AxPos = new Point();
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
            if (L > AxSize.Height) //確認是否比紀錄的最大值還大
            {
              AxPos.Y = yTmp;
              AxSize.Height = L;
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
  }
}
