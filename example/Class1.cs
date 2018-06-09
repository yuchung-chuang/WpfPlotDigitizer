using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace example
{
  public class PixelImage : ICloneable
  {
    private Bitmap _Bitmap;
    public Bitmap Bitmap
    {
      get => _Bitmap;
      set
      {
        _Bitmap = value;
        Bitmap2Pixel();
      }
    }
    private byte[] _Pixel;
    public byte[] Pixel
    {
      get => _Pixel;
      set
      {
        _Pixel = value;
        Pixel2Bitmap();
      }
    }
    public int Byte = 4; //根據Format32bppArgb

    public int Stride => Width * Byte;  //根據Format32bppArgb
    public int Width => _Bitmap.Width;
    public int Height => _Bitmap.Height;
    public Size Size => _Bitmap.Size;
    public static PixelFormat PixelFormat = PixelFormat.Format32bppArgb;

    #region Constructors
    public PixelImage()
    {

    }

    public PixelImage(Bitmap bitmap)
    {
      this.Bitmap = new Bitmap(bitmap); //更新pixel
    }

    public PixelImage(Size size)
    {
      _Bitmap = new Bitmap(size.Width, size.Height); //不更新pixel
    }

    public PixelImage(byte[] pixel, Size size)
    {
      _Bitmap = new Bitmap(size.Width, size.Height); //不更新pixel
      this.Pixel = pixel; //更新bitmap
    }

    public object Clone()
    {
      return new PixelImage()
      {
        _Bitmap = (Bitmap)this._Bitmap.Clone(),
        _Pixel = (byte[])this._Pixel.Clone(),
        Byte = this.Byte
      };
    } //比new PixelIamge(bitmap)快
    #endregion

    #region Private Methods
    private void Pixel2Bitmap()
    {
      //將image鎖定到系統內的記憶體的某個區塊中，並將這個結果交給BitmapData類別的imageData
      BitmapData bitmapData = _Bitmap.LockBits(
      new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height),
      ImageLockMode.ReadOnly,
      PixelFormat);

      //複製pixel到bitmapData中
      Marshal.Copy(_Pixel, 0, bitmapData.Scan0, _Pixel.Length);

      //解鎖
      _Bitmap.UnlockBits(bitmapData);

    }

    private void Bitmap2Pixel()
    {
      //將image鎖定到系統內的記憶體的某個區塊中，並將這個結果交給BitmapData類別的imageData
      BitmapData bitmapData = _Bitmap.LockBits(
        new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height),
        ImageLockMode.ReadOnly,
        PixelFormat);

      //初始化pixel陣列，用來儲存所有像素的訊息
      _Pixel = new byte[bitmapData.Stride * _Bitmap.Height];

      //複製imageData的RGB信息到pixel陣列中
      Marshal.Copy(bitmapData.Scan0, _Pixel, 0, _Pixel.Length);

      //解鎖
      _Bitmap.UnlockBits(bitmapData);

    }
    #endregion
  }
}
