using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public enum AxisType
  {
    None = 0x00,
    Left = 0x01,
    Top = 0x02,
    Right = 0x04,
    Bottom = 0x08,
  }

  public class ImageProcessingVM : ViewModelBase<ImageProcessingVM>
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

    public PixelBitmap PBAxis { get; set; }

    public PixelBitmap PBFilterRGB { get; set; }

  }
}
