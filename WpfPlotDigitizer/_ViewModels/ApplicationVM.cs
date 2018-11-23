using CycWpfLibrary.Controls;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV.Structure;
using CycWpfLibrary.Emgu;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class ApplicationVM : ViewModelBase
  {
    public ApplicationVM()
    {
      PageManager.TurnNextEvent += OnTurnNextAsync;
      IPVM.OnPBInputChanged += OnPixelBitmapInputChanged;
    }

    public PageManagerBase PageManager { get; private set; } = new PageManager();

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
          IPVM.ImageAxis = IPVM.PBAxis.ToImage<Bgra, byte>();
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
