using CycWpfLibrary.MVVM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CycWpfLibrary.Media;
using System.Windows.Media;

namespace WpfPlotDigitizer
{
  public class MainWindowViewModel : ViewModelBase
  {
    public MainWindowViewModel()
    {
      OpenFileCommand = new RelayCommand(OpenFile);
      NextTabCommand = new RelayCommand(NextTab, CanNextTab);
      BackTabCommand = new RelayCommand(BackTab, CanBackTab);
    }
    public MainWindowViewModel(TabControl tabControl) : this()
    {
      this.tabControl = tabControl;
    }

    public ICommand OpenFileCommand { get; set; } 
    public BitmapSource bitmapSourceInput { get; set; }
    private PixelBitmap pixelBitmapInput;
    private PixelBitmap pixelBitmapFilterW;
    public void OpenFile()
    {
      var dialog = new OpenFileDialog();
      dialog.Filter = "Image | *.png; *.jpg;";
      if (dialog.ShowDialog() == false)
      {
        return;
      }
      bitmapSourceInput = new BitmapImage(new Uri(dialog.FileName));
      pixelBitmapInput = (bitmapSourceInput as BitmapImage).ToPixelBitmap();

      pixelBitmapFilterW = pixelBitmapInput.Clone() as PixelBitmap;
      pixelBitmapFilterW.Pixel = pixelBitmapInput.FilterW();


    }

    
    private PixelBitmap pixelBitmapAxis;
    private PixelBitmap pixelBitmapFilterRGB;

    public TabControl tabControl; // not MVVM design! 
    public int TabIndex { get; set; } = 0; 
    public ICommand NextTabCommand { get; set; } 
    public ICommand BackTabCommand { get; set; }
    public void NextTab()
    {
      TabIndex++;
    }
    public bool CanNextTab()
    {
      return tabControl == null || TabIndex < tabControl.Items.Count - 1;
    }
    public void BackTab()
    {
      TabIndex--;
    }
    public bool CanBackTab()
    {
      return TabIndex > 0;
    }


    public ICommand ManualGetAxes { get; set; }
    public ICommand AutoGetAxes { get; set; }
  }
}
