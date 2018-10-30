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
  public class AxisPageViewModel : ViewModelBase<AxisPageViewModel>
  {
    public AxisPageViewModel()
    {
      AutoGetAxisCommand = new RelayCommand(AutoGetAxis);
    }

    public PixelBitmap pixelBitmapInput
    {
      get => IoC.Get<ImageProcessingViewModel>().pixelBitmapInput;
      set
      {
        IoC.Get<ImageProcessingViewModel>().pixelBitmapInput = value;
      }
    }
    public PixelBitmap pixelBitmapFilterW
    {
      get => IoC.Get<ImageProcessingViewModel>().pixelBitmapFilterW;
      set
      {
        IoC.Get<ImageProcessingViewModel>().pixelBitmapFilterW = value;
      }
    }
    public Rect Axis
    {
      get => IoC.Get<ImageProcessingViewModel>().Axis;
      set
      {
        IoC.Get<ImageProcessingViewModel>().Axis = value;
      }
    }

    public BitmapSource bitmapSourceInput => pixelBitmapInput?.ToBitmapSource();
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
    public ICommand AutoGetAxisCommand { get; set; }
    public void AutoGetAxis()
    {
      Axis = ImageProcessing.GetAxis(pixelBitmapFilterW);
    }
  }
}
