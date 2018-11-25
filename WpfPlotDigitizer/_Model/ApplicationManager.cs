using CycWpfLibrary;
using CycWpfLibrary.Controls;
using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV.Structure;
using System.Threading;
using System.Threading.Tasks;
using static CycWpfLibrary.NativeMethod;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class ApplicationManager
  {
    public ApplicationManager()
    {
      PageManager.TurnNextEvent += OnTurnNextAsync;
    }

    public PageManagerBase PageManager { get; private set; } = new PageManager();

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

          IPManager.PBFilterW = new PixelBitmap(IPManager.PBInput.Size)
          {
            Pixel = ImageProcessing.FilterW(IPManager.PBInput)
          };
          axisPageVM.GetAxis();
          break;
        case ApplicationPages.AxisLimit:
          IPManager.PBAxis = IPManager.PBInput.Bitmap
                                        .Crop(IPManager.Axis)
                                        .ToPixelBitmap();
          axisLimitPageVM.GetAxisLimit();
          break;
        case ApplicationPages.Filter:
          IPManager.ImageAxis = IPManager.PBAxis.ToImage<Bgra, byte>();
          IPManager.ImageFilterRGB = IPManager.ImageAxis.Clone();
          await filterPageVM.InRangeAsync();
          break;
        case ApplicationPages.Erase:
          IPManager.ImageErase = IPManager.ImageFilterRGB.Clone();
          erasePageVM.editor.Init(IPManager.ImageErase);

          break;
        default:
          break;
      }
    }
  }
}
