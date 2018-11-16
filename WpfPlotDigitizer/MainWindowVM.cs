using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class MainWindowVM : ViewModelBase
  {
    public double Width { get; set; }
    public double Height { get; set; }

    public string widthStr => Width.ToString();
    public string heightStr => Height.ToString();
  }
}
