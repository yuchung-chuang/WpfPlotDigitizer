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
    }

    private readonly ImageProcessingVM IPVM = IoC.Get<ImageProcessingVM>();
    private readonly AxisPageVM axisPageVM = IoC.Get<AxisPageVM>();
    private readonly FilterPageVM filterPageVM = IoC.Get<FilterPageVM>();
    private readonly AxisLimitPageVM axisLimitPageVM = IoC.Get<AxisLimitPageVM>();

    public IPageManager PageManager { get; } = IoC.Get<PageManager>();

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
