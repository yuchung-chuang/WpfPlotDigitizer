﻿using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static WpfPlotDigitizer.Singletons;

namespace WpfPlotDigitizer
{
  public class AxisPageVM : ViewModelBase
  {
    public AxisPageVM()
    {
      GetAxisCommand = new RelayCommand(GetAxis);
      imageData.PropertyChanged += ImageData_PropertyChanged;
    }

    private void ImageData_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(imageData.PBInput))
      {
        OnPropertyChanged(nameof(bitmapSourceInput));
      }
    }

    public PixelBitmap PBInput => imageData?.PBInput;
    public PixelBitmap PBFilterW => imageData?.PBFilterW;
    public Rect Axis
    {
      get
      {
        if (imageData == null)
          return new Rect(0, 0, 0, 0);
        else
          return imageData.Axis;
      }
      set => imageData.Axis = value;
    }
    public AxisType AxisType
    {
      get => imageData.AxisType;
      set => imageData.AxisType = value;
    }

    public BitmapSource bitmapSourceInput => PBInput?.ToBitmapSource();
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

    public ICommand GetAxisCommand { get; set; }
    public void GetAxis()
    {
      (Axis, AxisType) = ImageProcessing.GetAxis(PBFilterW);
    }
  }
}





