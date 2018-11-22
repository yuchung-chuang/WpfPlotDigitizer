using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ct = System.Threading.CancellationToken;
using cts = System.Threading.CancellationTokenSource;
using IP = WpfPlotDigitizer.ImageProcessing;

namespace WpfPlotDigitizer
{
  public class FilterPageVM : ViewModelBase
  {
    public FilterPageVM()
    {
      IoC.Get<IoC>().ViewModelsLoaded += OnViewModelsLoaded;
      //FilterRGBCommand = new RelayCommand<object, Task>(FilterRGBAsync);
      FilterRGBCommand = new RelayCommand<object, Task>(InRangeAsync);
    }

    private void OnViewModelsLoaded()
    {
      IPVM = IoC.Get<ImageProcessingVM>();
    }
    //Singleton fields
    private ImageProcessingVM IPVM;

    private static readonly object key = new object();
    private PixelBitmap PBAxis => IPVM?.PBAxis;
    private PixelBitmap PBFilterRGB
    {
      get => IPVM?.PBFilterRGB;
      set => IPVM.PBFilterRGB = value;
    }
    private Image<Rgba, byte> imageAxis => IPVM?.imageAxis;
    private Image<Rgba, byte> imageFilterRGB
    {
      get => IPVM?.imageFilterRGB;
      set => IPVM.imageFilterRGB = value;
    }
    public BitmapSource bitmapSourceFilterRGB => imageFilterRGB?.ToBitmapSource();
    //public BitmapSource bitmapSourceFilterRGB => PBFilterRGB?.ToBitmapSource();

    public byte FilterRMax { get; set; } = 255;
    public byte FilterRMin { get; set; } = 0;
    public byte FilterGMax { get; set; } = 255;
    public byte FilterGMin { get; set; } = 0;
    public byte FilterBMax { get; set; } = 255;
    public byte FilterBMin { get; set; } = 0;
    public Color FilterMax => Color.FromRgb(FilterRMax, FilterGMax, FilterBMax);
    public Color FilterMin => Color.FromRgb(FilterRMin, FilterGMin, FilterBMin);

    private Task<PixelBitmap> FilterTask;
    private cts cts;
    public ICommand FilterRGBCommand { get; set; }

    public async Task FilterRGBAsync(object param = null)
    {
      if (FilterTask != null && FilterTask.Status != TaskStatus.RanToCompletion)
      {
        cts.Cancel();
      }
      cts = new cts();
      try
      {
        FilterTask = IP.FilterRGBAsync(PBFilterRGB, FilterMax, FilterMin, cts.Token);
        PBFilterRGB = await FilterTask;
      }
      catch (TaskCanceledException) { }
    }

    public async Task InRangeAsync(object param = null)
    {
      imageFilterRGB = await IP.InRangeAsync(imageAxis, FilterMax, FilterMin);
    }
  }
}
