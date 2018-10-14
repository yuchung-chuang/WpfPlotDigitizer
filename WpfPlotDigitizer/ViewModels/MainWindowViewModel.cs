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
      pixelBitmapInput = new PixelBitmap(new Bitmap(@"images/ClickMe.png"));
    }

    public BitmapSource bitmapSourceInput => pixelBitmapInput?.ToBitmapSource();
    public BitmapSource bitmapSourceAxis => pixelBitmapAxis?.ToBitmapSource();
    public double imageWidth => pixelBitmapInput.Width;
    public double imageHeight => pixelBitmapInput.Height;
    protected PixelBitmap pixelBitmapInput { get; set; }
    protected PixelBitmap pixelBitmapFilterW { get; set; }
    protected PixelBitmap pixelBitmapAxis { get; set; }

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

      pixelBitmapFilterW = pixelBitmapInput.Clone() as PixelBitmap;
      pixelBitmapFilterW.Pixel = ImageProcessing.FilterW(pixelBitmapInput);

      AutoGetAxis();
      NextTab();
    }

    public Rect Axis { get; set; }
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
    public double AxisRight => Axis.Right;
    public double AxisBottom => Axis.Bottom;
    public ICommand AutoGetAxisCommand { get; set; }
    private void AutoGetAxis()
    {
      pixelBitmapAxis = pixelBitmapFilterW.Clone() as PixelBitmap;
      Axis = ImageProcessing.GetAxis(pixelBitmapAxis);
    }
    private bool CanAutoGetAxis() => true;
    
  }
}
