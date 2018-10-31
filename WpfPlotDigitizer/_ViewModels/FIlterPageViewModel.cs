using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class FilterPageViewModel : ViewModelBase<FilterPageViewModel>
  {
    public FilterPageViewModel()
    {
      FilterRMaxCommand = new RelayCommand(FilterRMaxMethod, CanFilter);
      FilterRMinCommand = new RelayCommand(FilterRMinMethod, CanFilter);
      FilterGMaxCommand = new RelayCommand(FilterGMaxMethod, CanFilter);
      FilterGMinCommand = new RelayCommand(FilterGMinMethod, CanFilter);
      FilterBMaxCommand = new RelayCommand(FilterBMaxMethod, CanFilter);
      FilterBMinCommand = new RelayCommand(FilterBMinMethod, CanFilter);
    }

    private PixelBitmap pixelBitmapFilterRGB
    {
      get => IoC.Get<ImageProcessingViewModel>().pixelBitmapFilterRGB;
      set => IoC.Get<ImageProcessingViewModel>().pixelBitmapFilterRGB = value;
    }
    public BitmapSource bitmapSourceFilterRGB => pixelBitmapFilterRGB?.ToBitmapSource();
    public double FilterRMax { get; set; } = 255;
    public double FilterRMin { get; set; } = 0;
    public double FilterGMax { get; set; } = 255;
    public double FilterGMin { get; set; } = 0;
    public double FilterBMax { get; set; } = 255;
    public double FilterBMin { get; set; } = 0;
    public Color FilterMax => Color.FromRgb((byte)FilterRMax, (byte)FilterGMax, (byte)FilterBMax);
    public Color FilterMin => Color.FromRgb((byte)FilterRMin, (byte)FilterGMin, (byte)FilterBMin);

    public ICommand FilterRMaxCommand { get; set; }
    public ICommand FilterRMinCommand { get; set; }
    public ICommand FilterGMaxCommand { get; set; }
    public ICommand FilterGMinCommand { get; set; }
    public ICommand FilterBMaxCommand { get; set; }
    public ICommand FilterBMinCommand { get; set; }
    private void FilterRMaxMethod()
    {
      pixelBitmapFilterRGB = ImageProcessing.FilterRGB(pixelBitmapFilterRGB, FilterMax, FilterMin, "R");
    }
    private void FilterRMinMethod()
    {
      pixelBitmapFilterRGB = ImageProcessing.FilterRGB(pixelBitmapFilterRGB, FilterMax, FilterMin, "R");
    }
    private void FilterGMaxMethod()
    {
      pixelBitmapFilterRGB = ImageProcessing.FilterRGB(pixelBitmapFilterRGB, FilterMax, FilterMin, "G");
    }
    private void FilterGMinMethod()
    {
      pixelBitmapFilterRGB = ImageProcessing.FilterRGB(pixelBitmapFilterRGB, FilterMax, FilterMin, "G");
    }
    private void FilterBMaxMethod()
    {
      pixelBitmapFilterRGB = ImageProcessing.FilterRGB(pixelBitmapFilterRGB, FilterMax, FilterMin, "B");
    }
    private void FilterBMinMethod()
    {
      pixelBitmapFilterRGB = ImageProcessing.FilterRGB(pixelBitmapFilterRGB, FilterMax, FilterMin, "B");
    }
    private bool CanFilter() => pixelBitmapFilterRGB != null;

  }
}
