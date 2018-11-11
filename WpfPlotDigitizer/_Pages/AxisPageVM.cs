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
  public class AxisPageVM : ViewModelBase<AxisPageVM>
  {
    public AxisPageVM()
    {
      AutoGetAxisCommand = new RelayCommand(AutoGetAxis);
      IoC.Get<IoC>().ViewModelsLoaded += OnViewModelsLoaded;
    }

    private void OnViewModelsLoaded()
    {
      IPVM = IoC.Get<ImageProcessingVM>();
    }
    //Singleton fields
    private ImageProcessingVM IPVM;

    public PixelBitmap PBInput => IPVM?.PBInput;
    public PixelBitmap PBFilterW => IPVM?.PBFilterW;
    public Rect Axis
    {
      get
      {
        if (IPVM == null)
          return new Rect(0, 0, 0, 0);
        else
          return IPVM.Axis;
      }
      set => IPVM.Axis = value;
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

    public ICommand AutoGetAxisCommand { get; set; }
    public void AutoGetAxis()
    {
      //Point LT, LB, RT, RB;
      //(LT, LB, RT, RB) = ImageProcessing.GetAxisPoints(PBFilterW);
      Axis = ImageProcessing.GetAxis(PBFilterW);
    }
  }
}






