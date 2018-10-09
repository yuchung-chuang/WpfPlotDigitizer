using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class MainWindowViewModel : ViewModelBase
  {
    public BitmapSource bitmapSourceInput => pixelBitmapInput?.ToBitmapSource();
    public BitmapSource bitmapSourceAxis => pixelBitmapAxis?.ToBitmapSource();

    private PixelBitmap pixelBitmapInput { get; set; }
    private PixelBitmap pixelBitmapFilterW { get; set; }
    private PixelBitmap pixelBitmapAxis { get; set; }
    private PixelBitmap pixelBitmapFilterRGB { get; set; }

    public MainWindowViewModel()
    {
      OpenFileCommand = new RelayCommand(OpenFile);
      NextTabCommand = new RelayCommand(NextTab, CanNextTab);
      BackTabCommand = new RelayCommand(BackTab, CanBackTab);
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
#if DEBUG

      pixelBitmapInput = new BitmapImage(new Uri("C:\\Users\\alex\\Desktop\\MATLAB\\side projects\\DataCapturer\\images\\Figure-1-Scatter-plot-comparing-log2-ratios-of-FPKM-expression-values-for-uninfected.png")).ToPixelBitmap();
#else
      var dialog = new OpenFileDialog();
      dialog.Filter = "Images (*.png, *.jpg)| *.png; *.PNG; *.jpg; *.JPG |All (*.*)|*.*";
      if (dialog.ShowDialog() == false)
      {
        return;
      }
      pixelBitmapInput = new BitmapImage(new Uri(dialog.FileName)).ToPixelBitmap();
#endif
      pixelBitmapFilterW = pixelBitmapInput.Clone() as PixelBitmap;
      pixelBitmapFilterW.Pixel = pixelBitmapInput.FilterW();
      pixelBitmapAxis = pixelBitmapFilterW.Clone() as PixelBitmap;
      //AxisOriginal = pixelBitmapAxis.GetLongestAxis();

      AxisPos = pixelBitmapAxis.GetAxisPosV2();
      AxisOriginal = new Rect(AxisPos.LT, AxisPos.RB);

      NextTab();
      imageAxis.LayoutUpdated -= ImageAxis_LayoutUpdated;
      imageAxis.LayoutUpdated += ImageAxis_LayoutUpdated;

    }

    public Image imageAxis { get; set; } //not MVVM!!
    private void ImageAxis_LayoutUpdated(object sender, EventArgs e)
    {
      AutoGetAxis();
    }

    public ICommand AutoGetAxisCommand { get; set; }
    private Rect AxisOriginal { get; set; }
    private (Point LT, Point RB, Point RT, Point LB) AxisPos { get; set; }
    public double AxisWidth { get; set; }
    public double AxisHeight { get; set; }
    public double AxisLeft { get; set; }
    public double AxisTop { get; set; }
    public double AxisRight { get; set; }
    public double AxisBottom { get; set; }

    public double RTLeft { get; set; }
    public double RTTop { get; set; }
    public double LBLeft { get; set; }
    public double LBTop { get; set; }
    private void AutoGetAxis()
    {
      var ratio = imageAxis.ActualWidth / pixelBitmapAxis.Width;

      AxisRight = AxisPos.RB.X * ratio;
      AxisBottom = AxisPos.RB.Y * ratio;
      AxisLeft = AxisOriginal.X * ratio;
      AxisTop = AxisOriginal.Y * ratio;
      AxisWidth = AxisOriginal.Width * ratio;
      AxisHeight = AxisOriginal.Height * ratio;

      RTLeft = AxisPos.RT.X * ratio;
      RTTop = AxisPos.RT.Y * ratio;
      LBLeft = AxisPos.LB.X * ratio;
      LBTop = AxisPos.LB.Y * ratio;
    }

    public ICommand ManualGetAxisCommand { get; set; }
  }
}
