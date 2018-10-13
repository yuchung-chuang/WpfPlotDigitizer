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
    public static MainWindowViewModel Instance { get; private set; } = new MainWindowViewModel();
    public MainWindowViewModel()
    {
      OpenFileCommand = new RelayCommand(OpenFile);
      NextTabCommand = new RelayCommand(NextTab, CanNextTab);
      BackTabCommand = new RelayCommand(BackTab, CanBackTab);
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
      Axis = ImageProcessing.GetAxis(pixelBitmapAxis);

      NextTab();
    }

    public ICommand AutoGetAxisCommand { get; set; }
    public Rect Axis { get; set; }
    public double AxisWidth => Axis.Width;
    public double AxisHeight => Axis.Height;
    public double AxisLeft => Axis.X;
    public double AxisTop => Axis.Y;
    public double AxisRight => Axis.Right;
    public double AxisBottom => Axis.Bottom;

    public ICommand ManualGetAxisCommand { get; set; }
  }
}
