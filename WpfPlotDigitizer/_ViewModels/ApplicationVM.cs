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
      IoC.Get<ImageProcessingVM>().OnPixelBitampInputChanged += OnPixelBitmapInputChanged;
    }

    private readonly ImageProcessingVM IPVM = IoC.Get<ImageProcessingVM>();
    private readonly AxisPageVM axisPageVM = IoC.Get<AxisPageVM>();

    public IPageManager PageManager { get; } = IoC.Get<PageManager>();

    private void OnPixelBitmapInputChanged()
    {
      IPVM.pixelBitmapFilterW = new PixelBitmap(IPVM.pixelBitmapInput.Size)
      {
        Pixel = ImageProcessing.FilterW(IPVM.pixelBitmapInput)
      };

      axisPageVM.AutoGetAxis();
      PageManager.TurnNext();
    }

    private void OnTurnNext()
    {
      // call before actually turned next
      switch ((ApplicationPages)PageManager.Index + 1)
      {
        case ApplicationPages.Filter:
          IoC.Get<ImageProcessingVM>().pixelBitmapFilterRGB = new PixelBitmap(IPVM.pixelBitmapInput.Bitmap.Crop(IPVM.Axis));
          break;
        case ApplicationPages.Erase:
          break;
        default:
          break;
      }
    }
  }
}
