using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Microsoft.Win32;
using System;
using Bitmap = System.Drawing.Bitmap;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class MainWindowViewModel : ViewModelBase
  {
    public BitmapSource bitmapSourceInput => pixelBitmapInput?.ToBitmapSource();
    public BitmapSource bitmapSourceAxis => pixelBitmapAxis?.ToBitmapSource();

    protected PixelBitmap pixelBitmapInput { get; set; }
    protected PixelBitmap pixelBitmapFilterW { get; set; }
    protected PixelBitmap pixelBitmapAxis { get; set; }
    protected PixelBitmap pixelBitmapFilterRGB { get; set; }

    public MainWindowViewModel()
    {
      OpenFileCommand = new RelayCommand(OpenFile);
      NextTabCommand = new RelayCommand(NextTab, CanNextTab);
      BackTabCommand = new RelayCommand(BackTab, CanBackTab);
      pixelBitmapInput = new PixelBitmap(new Bitmap(@"images/ClickMe.png"));
    }

    public TabControl tabControlMain; // not MVVM design! 
    public int TabIndex { get; set; } = 0;
    public ICommand NextTabCommand { get; set; }
    public ICommand BackTabCommand { get; set; }
    private void NextTab()
    {
      TabIndex++;
    }
    private bool CanNextTab()
    {
      return tabControlMain == null || TabIndex < tabControlMain.Items.Count - 1;
    }
    private void BackTab()
    {
      TabIndex--;
    }
    private bool CanBackTab()
    {
      return TabIndex > 0;
    }

    public ICommand OpenFileCommand { get; set; }
    public void OpenFile()
    {

      var dialog = new OpenFileDialog();
      dialog.Filter = "Images (*.png, *.jpg)| *.png; *.PNG; *.jpg; *.JPG |All (*.*)|*.*";
      if (dialog.ShowDialog() == false)
      {
        return;
      }
      pixelBitmapInput = new BitmapImage(new Uri(dialog.FileName)).ToPixelBitmap();

      pixelBitmapFilterW = pixelBitmapInput.Clone() as PixelBitmap;
      pixelBitmapFilterW.Pixel = ImageProcessing.FilterW(pixelBitmapInput);
      pixelBitmapAxis = pixelBitmapFilterW.Clone() as PixelBitmap;
      AxisOriginal = ImageProcessing.GetAxis(pixelBitmapAxis);

      imageAxis.LayoutUpdated -= ImageAxis_LayoutUpdated;
      imageAxis.LayoutUpdated += ImageAxis_LayoutUpdated;

      NextTab();
    }

    public Image imageAxis { get; set; } //not MVVM!!
    private void ImageAxis_LayoutUpdated(object sender, EventArgs e)
    {
      imageRatio = imageAxis.ActualWidth / pixelBitmapAxis.Width;
    }

    public ICommand AutoGetAxisCommand { get; set; }
    public Rect AxisOriginal { get; set; }
    public (Point LT, Point RB, Point RT, Point LB) AxisPos { get; set; }
    public double imageRatio { get; set; }
    public double AxisWidth => AxisOriginal.Width * imageRatio;
    public double AxisHeight => AxisOriginal.Height * imageRatio;
    public double AxisLeft => AxisOriginal.X * imageRatio;
    public double AxisTop => AxisOriginal.Y * imageRatio;
    public double AxisRight => AxisOriginal.Right * imageRatio;
    public double AxisBottom => AxisOriginal.Bottom * imageRatio;

    public ICommand ManualGetAxisCommand { get; set; }
  }
}
