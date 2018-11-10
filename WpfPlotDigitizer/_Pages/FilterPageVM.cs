using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class FilterPageVM : ViewModelBase<FilterPageVM>
  {
    private static readonly object key = new object();
    private readonly ImageProcessingVM IPVM = IoC.Get<ImageProcessingVM>();
    private PixelBitmap PBFilterRGB
    {
      get => IPVM.PBFilterRGB;
      set => IPVM.PBFilterRGB = value;
    }
    public BitmapSource bitmapSourceFilterRGB => PBFilterRGB?.ToBitmapSource();

    private byte _filterRMax = 255;
    private byte _filterRMin = 0;
    private byte _filterGMax = 255;
    private byte _filterGMin = 0;
    private byte _filterBMax = 255;
    private byte _filterBMin = 0;

    public byte FilterRMax
    {
      get => _filterRMax;
      set
      {
        if (_filterRMax != value)
        {
          _filterRMax = value;
          FilterMethod(FilterMax, FilterMin, FilterType.Red);
        }
      }
    }
    public byte FilterRMin
    {
      get => _filterRMin;
      set
      {
        if (_filterRMin != value)
        {
          _filterRMin = value;
          FilterMethod(FilterMax, FilterMin, FilterType.Red);
        }
      }
    }
    public byte FilterGMax
    {
      get => _filterGMax;
      set
      {
        if (_filterGMax != value)
        {
          _filterGMax = value;
          FilterMethod(FilterMax, FilterMin, FilterType.Green);
        }
      }
    }
    public byte FilterGMin
    {
      get => _filterGMin;
      set
      {
        if (_filterGMin != value)
        {
          _filterGMin = value;
          FilterMethod(FilterMax, FilterMin, FilterType.Green);
        }
      }
    }
    public byte FilterBMax
    {
      get => _filterBMax;
      set
      {
        if (_filterBMax != value)
        {
          _filterBMax = value;
          FilterMethod(FilterMax, FilterMin, FilterType.Blue);
        }
      }
    }
    public byte FilterBMin
    {
      get => _filterBMin;
      set
      {
        if (_filterBMin != value)
        {
          _filterBMin = value;
          FilterMethod(FilterMax, FilterMin, FilterType.Blue);
        }
      }
    }
    public Color FilterMax => Color.FromRgb(FilterRMax, FilterGMax, FilterBMax);
    public Color FilterMin => Color.FromRgb(FilterRMin, FilterGMin, FilterBMin);
    public (Color Max, Color Min) Filter => (FilterMax, FilterMin);

    private Task FilterTask;
    private CancellationTokenSource cts;
    private void FilterMethod(Color FilterMax, Color FilterMin, FilterType type)
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
          PBFilterRGB = ImageProcessing.FilterRGB(PBFilterRGB, FilterMax, FilterMin, type, cts.Token);
        }
        catch (OperationCanceledException)
        { }
      }, cts.Token);
    }

    public void FilterAllMethod()
    {
      PBFilterRGB = ImageProcessing.FilterRGB(PBFilterRGB, FilterMax, FilterMin, FilterType.Red, CancellationToken.None);
      PBFilterRGB = ImageProcessing.FilterRGB(PBFilterRGB, FilterMax, FilterMin, FilterType.Green, CancellationToken.None);
      PBFilterRGB = ImageProcessing.FilterRGB(PBFilterRGB, FilterMax, FilterMin, FilterType.Blue, CancellationToken.None);
    }
  }
}
