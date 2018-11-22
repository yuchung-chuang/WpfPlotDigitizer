using CycWpfLibrary;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class ImageProcessingVM : ViewModelBase
  {

    public PixelBitmap PBFilterW { get; set; }

    public event Action OnPBInputChanged;
    private PixelBitmap _PBInput;
    public PixelBitmap PBInput
    {
      get => _PBInput;
      set
      {
        _PBInput = value;
        OnPBInputChanged?.Invoke();
      }
    }

    public Rect Axis { get; set; }
    public AxisType AxisType { get; set; }

    public PixelBitmap PBAxis { get; set; }

    public PixelBitmap PBFilterRGB { get; set; }
    public Image<Rgba, byte> imageAxis { get; set; }
    public Image<Rgba, byte> imageFilterRGB { get; set; }

  }
}
