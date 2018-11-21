using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IP = WpfPlotDigitizer.ImageProcessing;
using ct = System.Threading.CancellationToken;
using cts = System.Threading.CancellationTokenSource;
using System.Windows.Input;

namespace WpfPlotDigitizer
{
  public class FilterPageVM : ViewModelBase
  {
    public FilterPageVM()
    {
      IoC.Get<IoC>().ViewModelsLoaded += OnViewModelsLoaded;
      FilterRGBCommand = new RelayCommand(FilterRGB);
    }

    private void OnViewModelsLoaded()
    {
      IPVM = IoC.Get<ImageProcessingVM>();
    }
    //Singleton fields
    private ImageProcessingVM IPVM;

    private static readonly object key = new object();
    private PixelBitmap PBFilterRGB
    {
      get => IPVM?.PBFilterRGB;
      set => IPVM.PBFilterRGB = value;
    }
    public BitmapSource bitmapSourceFilterRGB => PBFilterRGB?.ToBitmapSource();

    public byte FilterRMax { get; set; } = 255;
    public byte FilterRMin { get; set; } = 0;
    public byte FilterGMax { get; set; } = 255;
    public byte FilterGMin { get; set; } = 0;
    public byte FilterBMax { get; set; } = 255;
    public byte FilterBMin { get; set; } = 0;
    public Color FilterMax => Color.FromRgb(FilterRMax, FilterGMax, FilterBMax);
    public Color FilterMin => Color.FromRgb(FilterRMin, FilterGMin, FilterBMin);

    private Task FilterTask;
    private cts cts;
    public ICommand FilterRGBCommand { get; set; }
    public void FilterRGB()
    {
      if (FilterTask != null && FilterTask.Status != TaskStatus.RanToCompletion)
      {
        cts.Cancel();
      }
      cts = new cts();
      FilterTask = Task.Run(() =>
      {
        try
        {
          PBFilterRGB = IP.FilterRGB(PBFilterRGB, FilterMax, FilterMin, cts.Token);
        }
        catch (OperationCanceledException) { }
      }, cts.Token);
    }

    [Obsolete]
    private void FilterMethod2(Color FilterMax, Color FilterMin, FilterType2 type)
    {
      if (FilterTask != null && FilterTask.Status != TaskStatus.RanToCompletion)
      {
        cts.Cancel();
      }
      cts = new CancellationTokenSource();
      FilterTask = Task.Run(() =>
      {
        try
        {
          PBFilterRGB = ImageProcessing.FilterRGB2(PBFilterRGB, FilterMax, FilterMin, type, cts.Token);
        }
        catch (OperationCanceledException)
        { }
      }, cts.Token);
    }
  }
}
