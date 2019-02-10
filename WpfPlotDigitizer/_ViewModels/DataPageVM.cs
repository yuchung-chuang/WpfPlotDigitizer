﻿using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
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
using IP = WpfPlotDigitizer.ImageProcessing;

namespace WpfPlotDigitizer
{
  public class DataPageVM : ViewModelBase
  {
    public Rect axLim => appData.AxLim;
    public Point axLogBase => appData.AxLogBase;
    private Image<Bgra, byte> imageOrigin => appData.ImageErase;
    public Image<Bgra, byte> imageDisplay { get; set; }
    public BitmapSource imageSource
    {
      get => imageDisplay?.ToBitmapSource();
      set => imageDisplay = value.ToBitmap().ToImage<Bgra, byte>();
    }

    private int dataSize = 6;
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
    private int ratioInt = 90;
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
      get => appData.Data;
      set => appData.Data = value;
    }
    public Image<Bgra, byte> imageData
    {
      get => appData.ImageData;
      set => appData.ImageData = value;
    }

    public void ParamChanged()
    {
      var posLists = IP.GetDataList(imageOrigin, dataSize);
      var Pos = IP.GetData(imageOrigin, posLists, dataSize, ratio);
      Data = IP.TransformData(imageOrigin, Pos, axLim, axLogBase);
      var dotSize = DataSize == 1 ? 1 : DataSize / 2;
      imageDisplay = imageOrigin.Clone();
      imageData = imageDisplay.CopyBlank();
      IP.DrawData(imageDisplay, Pos, dotSize);
      IP.DrawData(imageData, Pos, dotSize);
      imageDisplay = imageDisplay.Copy(); // invoke twoway binding
    }
  }
}
