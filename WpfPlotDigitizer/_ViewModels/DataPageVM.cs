using CycWpfLibrary.Emgu;
using CycWpfLibrary.MVVM;
using CycWpfLibrary.WinForm;
using Emgu.CV;
using Emgu.CV.CvEnum;
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

    private int dataSize = 1;
    public int DataSize
    {
      get => dataSize;
      set
      {
        dataSize = value;
        ParamChanged();
      }
    }
    private double ratio => ratioInt / 100d;
    private int ratioInt = 100;
    public int RatioInt
    {
      get => ratioInt;
      set
      {
        ratioInt = value;
        ParamChanged();
      }
    }

    public List<Point> Data
    {
      get => imageData.Data;
      set => imageData.Data = value;
    }

    public void ParamChanged()
    {
      var posLists = ImageProcessing.GetDataList(imageOrigin, dataSize);
      var Pos = ImageProcessing.GetData(imageOrigin, posLists, dataSize, ratio);
      Data = ImageProcessing.TransformData(imageOrigin, Pos, imageData.Axis, new Point(1, 1));
      imageDisplay = imageOrigin.Clone();
      var dotSize = DataSize == 1 ? 1 : DataSize / 2;
      foreach (var pos in Pos)
      {
        CvInvoke.Circle(imageDisplay, pos.ToWinForm(), dotSize, Colors.Red.ToMCvScalar(), -1, LineType.AntiAlias);
        CvInvoke.Circle(imageDisplay, pos.ToWinForm(), dotSize, Colors.Black.ToMCvScalar(), 1, LineType.AntiAlias);
      }
      OnPropertyChanged(nameof(imageSource));
    }
  }
}
