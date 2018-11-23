using CycWpfLibrary;
using CycWpfLibrary.Controls;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfPlotDigitizer
{
  public class ApplicationVM : ViewModelBase
  {
    public ApplicationVM()
    {
      IoC.Get<PageManagerBase>().TurnNextEvent += OnTurnNextAsync;
      IoC.Get<ImageProcessingVM>().OnPBInputChanged += OnPixelBitmapInputChanged;
      IoC.Get<IoC>().ViewModelsLoaded += OnViewModelsLoaded;
    }

    private void OnViewModelsLoaded()
    {
      IPVM = IoC.Get<ImageProcessingVM>();
      axisPageVM = IoC.Get<AxisPageVM>();
      filterPageVM = IoC.Get<FilterPageVM>();
      axisLimitPageVM = IoC.Get<AxisLimitPageVM>();
      PageManager = IoC.Get<PageManagerBase>();
    }

    //Singleton fields
    private ImageProcessingVM IPVM;
    private AxisPageVM axisPageVM;
    private FilterPageVM filterPageVM;
    private AxisLimitPageVM axisLimitPageVM;

    public PageManagerBase PageManager { get; private set; }

    /// <summary>
    /// Turn Next Page automatically after seleted an image
    /// </summary>
    private void OnPixelBitmapInputChanged() => PageManager.TurnNext();

    /// <summary>
    /// Called whenever <see cref="PageManager.TurnNext"/> is fired.
    /// </summary>
    private async void OnTurnNextAsync()
    {
      // call before actually turned next
      switch ((ApplicationPages)PageManager.Index + 1)
      {
        case ApplicationPages.Axis:
          axisPageVM.OnPropertyChanged(nameof(axisPageVM.bitmapSourceInput));

          IPVM.PBFilterW = new PixelBitmap(IPVM.PBInput.Size)
          {
            Pixel = ImageProcessing.FilterW(IPVM.PBInput)
          };
          axisPageVM.GetAxis();

          break;
        case ApplicationPages.AxisLimit:
          IPVM.PBAxis = IPVM.PBInput.Bitmap
                                        .Crop(IPVM.Axis)
                                        .ToPixelBitmap();
          axisLimitPageVM.GetAxisLimit();
          break;
        case ApplicationPages.Filter:
          IPVM.ImageAxis = IPVM.PBAxis.ToImage<Rgba, byte>();
          IPVM.ImageFilterRGB = IPVM.ImageAxis.Clone();
          await filterPageVM.InRangeAsync();
          break;
        case ApplicationPages.Erase:
          break;
        default:
          break;
      }
    }
  }
}
