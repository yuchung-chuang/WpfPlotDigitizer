using CycWpfLibrary;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class AxisPageVM : ViewModelBase
  {
    public AxisPageVM()
    {
      GetAxisCommand = new RelayCommand(GetAxis);
    }

    public PixelBitmap PBInput => appData?.PBInput;
    public PixelBitmap PBFilterW => appData?.PBFilterW;
    public Rect Axis
    {
      get
      {
        if (appData == null)
          return new Rect(0, 0, 0, 0);
        else
          return appData.Axis;
      }
      set => appData.Axis = value;
    }
    public AxisType AxisType
    {
      get => appData.AxisType;
      set => appData.AxisType = value;
    }

    public BitmapSource ImageSource => PBInput?.ToBitmapSource();
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

    public double imageWidth => PBInput?.Width ?? 0;
    public double imageHeight => PBInput?.Height ?? 0;
    public Rect AxisRelative => new Rect(AxisLeft / imageWidth, AxisTop / imageHeight, AxisWidth / imageWidth, AxisHeight / imageHeight);
    public Thickness AxisMargin => new Thickness(AxisLeft, AxisTop, 0, 0);

    public ICommand GetAxisCommand { get; set; }
    public void GetAxis()
    {
      (Axis, AxisType) = ImageProcessing.GetAxis(PBFilterW);
    }
  }
}






