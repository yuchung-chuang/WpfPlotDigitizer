using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class AxisPageViewModel : ViewModelBase
  {
    public PixelBitmap pixelBitmapInput { get; private set; }
    public PixelBitmap pixelBitmapFilterW { get; private set; }


    public double imageWidth => pixelBitmapInput == null ? 0 : pixelBitmapInput.Width;
    public double imageHeight => pixelBitmapInput == null ? 0 : pixelBitmapInput.Height;
    public BitmapSource bitmapSourceInput => pixelBitmapInput?.ToBitmapSource();
    public Rect Axis { get; set; }
    public double AxisLeft
    {
      get => Axis.Left;
      set
      {
        var axisTmp = Axis;
        axisTmp.X = value;
        Axis = axisTmp;
      }
    }
    public double AxisTop
    {
      get => Axis.Y;
      set
      {
        var axisTmp = Axis;
        axisTmp.Y = value;
        Axis = axisTmp;
      }
    }
    public double AxisWidth
    {
      get => Axis.Width;
      set
      {
        var axisTmp = Axis;
        axisTmp.Width = value;
        Axis = axisTmp;
      }
    }
    public double AxisHeight
    {
      get => Axis.Height;
      set
      {
        var axisTmp = Axis;
        axisTmp.Height = value;
        Axis = axisTmp;
      }
    }
    public double AxisRight => Axis.Right;
    public double AxisBottom => Axis.Bottom;
    public ICommand AutoGetAxisCommand { get; set; }
    private void AutoGetAxis()
    {
      Axis = ImageProcessing.GetAxis(pixelBitmapFilterW);
    }
  }
}
