using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Microsoft.Win32;
using System;
using Bitmap = System.Drawing.Bitmap;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class MainWindowDesignModel : MainWindowViewModel
  {
    public MainWindowDesignModel()
    {
      pixelBitmapInput = new PixelBitmap(new Bitmap(@"images/ClickMe.png"));
    }

  }
}
