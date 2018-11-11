using CycWpfLibrary.Controls;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfPlotDigitizer
{
  public class ApplicationVM : ViewModelBase<ApplicationVM>
  {
    public ApplicationVM()
    {
      IoC.Get<PageManager>().TurnNextEvent += OnTurnNextAsync;
      IoC.Get<ImageProcessingVM>().OnPBInputChanged += OnPixelBitmapInputChanged;
      IoC.Get<IoC>().ViewModelsLoaded += OnViewModelsLoaded;
    }

    private void OnViewModelsLoaded()
    {
      IPVM = IoC.Get<ImageProcessingVM>();
      axisPageVM = IoC.Get<AxisPageVM>();
      filterPageVM = IoC.Get<FilterPageVM>();
      axisLimitPageVM = IoC.Get<AxisLimitPageVM>();
      PageManager = IoC.Get<PageManager>();
    }

    //Singleton fields
    private ImageProcessingVM IPVM;
    private AxisPageVM axisPageVM;
    private FilterPageVM filterPageVM;
    private AxisLimitPageVM axisLimitPageVM;

    public IPageManager PageManager { get; private set; }

    /// <summary>
    /// Turn Next Page automatically after seleted an image
    /// </summary>
    private void OnPixelBitmapInputChanged() => PageManager.TurnNext();

    private Task GetAxisTask;
    private Task GetAxisLimitTask;
    /// <summary>
    /// Called whenever <see cref="PageManager.TurnNext"/> is fired.
    /// </summary>
    private void OnTurnNextAsync()
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
          axisPageVM.AutoGetAxis();

          break;
        case ApplicationPages.AxisLimit:
          Application.Current.MainWindow.Cursor = Cursors.Wait;
          GetAxisLimitTask = Task.Run(() =>
          {
            IPVM.PBAxis = IPVM.PBInput.Bitmap
                                          .Crop(IPVM.Axis)
                                          .ToPixelBitmap();
            axisLimitPageVM.GetAxisLimit();
          });
          GetAxisLimitTask.Wait();
          Application.Current.MainWindow.Cursor = Cursors.Arrow;
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
