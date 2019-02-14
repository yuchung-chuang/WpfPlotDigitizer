using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IP = WpfPlotDigitizer.ImageProcessing;
using static WpfPlotDigitizer.DI;
using CycWpfLibrary.Emgu;

namespace WpfPlotDigitizer
{
  public class FilterPageVM : ViewModelBase
  {
    public FilterPageVM()
    {
      InRangeCommand = new RelayCommand<object, Task>(InRangeAsync);
    }

    private Image<Bgra, byte> imageAxis => appData?.ImageAxis;
    private Image<Bgra, byte> imageFilterRGB
    {
      get => appData?.ImageFilterRGB;
      set => appData.ImageFilterRGB = value;
    }
    public BitmapSource ImageSource => imageFilterRGB?.ToBitmapSource();

    public byte FilterRMax { get; set; } = 254;
    public byte FilterRMin { get; set; } = 0;
    public byte FilterGMax { get; set; } = 254;
    public byte FilterGMin { get; set; } = 0;
    public byte FilterBMax { get; set; } = 254;
    public byte FilterBMin { get; set; } = 0;
    public Color FilterMax => Color.FromRgb(FilterRMax, FilterGMax, FilterBMax);
    public Color FilterMin => Color.FromRgb(FilterRMin, FilterGMin, FilterBMin);

    public ICommand InRangeCommand { get; set; }
    public async Task InRangeAsync(object param = null)
    {
      imageFilterRGB = await IP.InRangeAsync(imageAxis, FilterMax, FilterMin);
    }

    public void InRange()
    {
      imageFilterRGB = IP.InRange(imageAxis, FilterMax, FilterMin);
    }
  }
}
