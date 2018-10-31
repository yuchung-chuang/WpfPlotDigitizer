using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer._ViewModels
{
  public class FilterPageViewModel : ViewModelBase<FilterPageViewModel>
  {
    public FilterPageViewModel()
    {
      FilterRGBCommand = new RelayCommand(FilterRGB);
    }

    private PixelBitmap pixelBitmapFilterRGB { get; set; }
    public BitmapSource bitmapSourceFilterRGB => pixelBitmapFilterRGB?.ToBitmapSource();
    public double FilterRMax { get; set; } = 255;
    public double FilterRMin { get; set; } = 0;
    public double FilterGMax { get; set; } = 255;
    public double FilterGMin { get; set; } = 0;
    public double FilterBMax { get; set; } = 255;
    public double FilterBMin { get; set; } = 0;
    public ICommand FilterRGBCommand { get; set; }
    private void FilterRGB()
    {

    }
  }
}
