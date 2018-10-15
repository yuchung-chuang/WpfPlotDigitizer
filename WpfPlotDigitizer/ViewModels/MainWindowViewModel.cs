using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Bitmap = System.Drawing.Bitmap;

namespace WpfPlotDigitizer
{
  public class MainWindowViewModel : ViewModelBase
  {
    public static MainWindowViewModel Instance { get; private set; } = new MainWindowViewModel();
    public MainWindowViewModel()
    {
      OpenFileCommand = new RelayCommand(OpenFile);
      NextTabCommand = new RelayCommand(NextTab, CanNextTab);
      BackTabCommand = new RelayCommand(BackTab, CanBackTab);
      AutoGetAxisCommand = new RelayCommand(AutoGetAxis, CanAutoGetAxis);
      //pixelBitmapInput = new PixelBitmap(new Bitmap(@"images/ClickMe.png"));
    }

    public TabControl tabControlMain; // not MVVM design! 
    public int TabIndex { get; set; } = 0;
    public int TabCount { get; set; }
    public ICommand NextTabCommand { get; set; }
    public ICommand BackTabCommand { get; set; }
    private void NextTab() => TabIndex++;
    private bool CanNextTab() => tabControlMain == null || TabIndex < tabControlMain.Items.Count - 1;
    private void BackTab() => TabIndex--;
    private bool CanBackTab() => TabIndex > 0;

    public ICommand OpenFileCommand { get; set; }
    public void OpenFile()
    {
      var dialog = new OpenFileDialog();
      var imageExtensions = ImageExtensions.String;
      dialog.Filter = "Images|" + imageExtensions + "|All|*.*";
      if (dialog.ShowDialog() == false)
      {
        return;
      }
      pixelBitmapInput = new BitmapImage(new Uri(dialog.FileName)).ToPixelBitmap();
    }

    public BitmapSource bitmapSourceInput => pixelBitmapInput?.ToBitmapSource();
    public double imageWidth => pixelBitmapInput == null ? 0 : pixelBitmapInput.Width;
    public double imageHeight => pixelBitmapInput == null ? 0 : pixelBitmapInput.Height;

    public PixelBitmap _pixelBitmapInput { get; set; }
    public PixelBitmap pixelBitmapInput
    {
      get => _pixelBitmapInput;
      set
      {
        _pixelBitmapInput = value;
        // After update input image, automatically filterW and GetAxis
        pixelBitmapFilterW = pixelBitmapInput.Clone() as PixelBitmap;
        pixelBitmapFilterW.Pixel = ImageProcessing.FilterW(pixelBitmapInput);
        AutoGetAxis();
        NextTab();
      }
    }
    private PixelBitmap pixelBitmapFilterW { get; set; }

    public Rect Axis { get; set; }
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
    public double AxisRight => Axis.Right;
    public double AxisBottom => Axis.Bottom;
    public ICommand AutoGetAxisCommand { get; set; }
    private void AutoGetAxis()
    {
      Axis = ImageProcessing.GetAxis(pixelBitmapFilterW);
    }
    private bool CanAutoGetAxis() => true;

  }
}
