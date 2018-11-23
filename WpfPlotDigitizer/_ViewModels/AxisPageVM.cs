using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
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

    public PixelBitmap PBInput => IPManager?.PBInput;
    public PixelBitmap PBFilterW => IPManager?.PBFilterW;
    public Rect Axis
    {
      get
      {
        if (IPManager == null)
          return new Rect(0, 0, 0, 0);
        else
          return IPManager.Axis;
      }
      set => IPManager.Axis = value;
    }
    public AxisType AxisType
    {
      get => IPManager.AxisType;
      set => IPManager.AxisType = value;
    }

    public BitmapSource bitmapSourceInput => PBInput?.ToBitmapSource();
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

    public ICommand GetAxisCommand { get; set; }
    public void GetAxis()
    {
      (Axis, AxisType) = ImageProcessing.GetAxis(PBFilterW);
    }
  }
}






