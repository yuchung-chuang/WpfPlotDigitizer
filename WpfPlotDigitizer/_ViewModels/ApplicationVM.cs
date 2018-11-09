using CycWpfLibrary.Controls;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfPlotDigitizer
{
  public class ApplicationVM : ViewModelBase<ApplicationVM>
  {
    public ApplicationVM()
    {
      IoC.Get<PageManager>().TurnNextEvent += OnTurnNext;
      IoC.Get<ImageProcessingVM>().OnPBInputChanged += OnPixelBitmapInputChanged;
    }

    private readonly ImageProcessingVM IPVM = IoC.Get<ImageProcessingVM>();
    private readonly AxisPageVM axisPageVM = IoC.Get<AxisPageVM>();
    private readonly FilterPageVM filterPageVM = IoC.Get<FilterPageVM>();
    private readonly AxisLimitPageVM axisLimitPageVM = IoC.Get<AxisLimitPageVM>();
    
    public IPageManager PageManager { get; } = IoC.Get<PageManager>();

    private void OnPixelBitmapInputChanged()
    {
      IPVM.PBFilterW = new PixelBitmap(IPVM.PBInput.Size)
      {
        Pixel = ImageProcessing.FilterW(IPVM.PBInput)
      };

      axisPageVM.AutoGetAxis();
      PageManager.TurnNext();
    }

    private void OnTurnNext()
    {
      // call before actually turned next
      switch ((ApplicationPages)PageManager.Index + 1)
      {
        case ApplicationPages.AxisLimit:
          IPVM.PBAxis = IPVM.PBInput.Bitmap
                                        .Crop(IPVM.Axis)
                                        .ToPixelBitmap();
          axisLimitPageVM.AutoGetAxisLimit();
          break;
        case ApplicationPages.Filter:
          IPVM.PBFilterRGB = IPVM.PBAxis.Clone() as PixelBitmap;
          filterPageVM.FilterAllMethod();
          break;
        case ApplicationPages.Erase:
          break;
        default:
          break;
      }
    }
  }
}
