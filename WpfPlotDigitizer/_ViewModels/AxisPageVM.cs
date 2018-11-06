﻿using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class AxisPageVM : ViewModelBase<AxisPageVM>
  {

    public AxisPageVM()
    {
      AutoGetAxisCommand = new RelayCommand(AutoGetAxis);
    }

    public PixelBitmap pixelBitmapInput
    {
      get => IoC.Get<ImageProcessingVM>().PBInput;
      set => IoC.Get<ImageProcessingVM>().PBInput = value;
    }
    public PixelBitmap pixelBitmapFilterW
    {
      get => IoC.Get<ImageProcessingVM>().PBFilterW;
      set => IoC.Get<ImageProcessingVM>().PBFilterW = value;
    }
    public Rect Axis
    {
      get => IoC.Get<ImageProcessingVM>().Axis;
      set => IoC.Get<ImageProcessingVM>().Axis = value;
    }

    public BitmapSource bitmapSourceInput => pixelBitmapInput?.ToBitmapSource();
    public double AxisLeft
    {
      get => Axis.Left;
      set
      {
        var axisTmp = Axis;
        axisTmp.X = value;
        Axis = axisTmp;
      }
    }
    public double AxisTop
    {
      get => Axis.Y;
      set
      {
        var axisTmp = Axis;
        axisTmp.Y = value;
        Axis = axisTmp;
      }
    }
    public double AxisWidth
    {
      get => Axis.Width;
      set
      {
        var axisTmp = Axis;
        axisTmp.Width = value;
        Axis = axisTmp;
      }
    }
    public double AxisHeight
    {
      get => Axis.Height;
      set
      {
        var axisTmp = Axis;
        axisTmp.Height = value;
        Axis = axisTmp;
      }
    }

    public ICommand AutoGetAxisCommand { get; set; }
    public void AutoGetAxis()
    {
      Axis = ImageProcessing.GetAxis(pixelBitmapFilterW);
    }
  }
}






