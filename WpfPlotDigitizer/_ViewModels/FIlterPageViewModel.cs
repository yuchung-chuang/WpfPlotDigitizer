using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using CycWpfLibrary.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

    }

    private static readonly object key = new object();
    private PixelBitmap pixelBitmapFilterRGB
    {
      get
      {
        return IoC.Get<ImageProcessingViewModel>().pixelBitmapFilterRGB;
      }
      set
      {
        IoC.Get<ImageProcessingViewModel>().pixelBitmapFilterRGB = value;
      }
    }
    public BitmapSource bitmapSourceFilterRGB => pixelBitmapFilterRGB?.ToBitmapSource();


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
          FilterRMethod();
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
          FilterRMethod();
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
          FilterGMethod();
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
          FilterGMethod();
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
          FilterBMethod();
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
          FilterBMethod();
        }
      }
    }
    public Color FilterMax => Color.FromRgb(FilterRMax, FilterGMax, FilterBMax);
    public Color FilterMin => Color.FromRgb(FilterRMin, FilterGMin, FilterBMin);

    private TaskQueue taskQueue = new TaskQueue(1);
    private Task workTask;
    private CancellationTokenSource cts;
    private void work(object state)
    {
      var token = (CancellationToken)state;
      PixelBitmap pixelBitmap = new PixelBitmap();
      try
      {
        //耗時的工作
        pixelBitmap = ImageProcessing.FilterRGB(pixelBitmapFilterRGB, FilterMax, FilterMin, "R", token);
        pixelBitmapFilterRGB = pixelBitmap;
        Debug.WriteLine("completed");
      }
      catch (OperationCanceledException)
      {
        // 中斷工作
        Debug.WriteLine("cancelled");
      }

      
    }
    private void FilterRMethod()
    {
      if (workTask != null && workTask.Status != TaskStatus.RanToCompletion)
      {
        cts.Cancel(); //通知取消工作
        Debug.WriteLine("Cancel!");
      }
      cts = new CancellationTokenSource(); // 初始化cts物件
      workTask = new Task(work, cts.Token); //初始化Task物件
      workTask.Start(); //背景執行Task
    }

    private void FilterGMethod()
    {
      
    }
    private void FilterBMethod()
    {

    }
  }
}
