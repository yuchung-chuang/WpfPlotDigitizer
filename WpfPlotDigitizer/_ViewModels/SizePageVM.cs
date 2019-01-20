using CycWpfLibrary.Emgu;
using CycWpfLibrary.MVVM;
using CycWpfLibrary.WinForm;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class SizePageVM : ViewModelBase
  {
    private Image<Bgra, byte> imageOrigin => imageData.ImageErase;
    public Image<Bgra, byte> imageDisplay { get; set; }
    public BitmapSource imageSource => imageDisplay?.ToBitmapSource();

    private int dataSize = 2;
    public int DataSize
    {
      get => dataSize;
      set
      {
        dataSize = value;
        SizeChanged();
      }
    }

    public List<Point> Data
    {
      get => imageData.Data;
      set => imageData.Data = value;
    }
    public List<Point> Pos { get; set; }

    public void SizeChanged()
    {
      (Data, Pos) = ImageProcessing.GetData(imageOrigin, imageData.Axis, new Point(1,1), DataSize);
      imageDisplay = imageOrigin.Clone();
      foreach (var pos in Pos)
      {
        CvInvoke.Circle(imageDisplay, pos.ToWinForm(), DataSize / 2, Colors.Red.ToMCvScalar(), -1);
        CvInvoke.Circle(imageDisplay, pos.ToWinForm(), DataSize / 2, Colors.Black.ToMCvScalar(), 1);
      }
      OnPropertyChanged(nameof(imageSource));
    }
  }
}
