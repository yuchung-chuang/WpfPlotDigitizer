using CycWpfLibrary.Controls;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class BrowsePageVM : ViewModelBase
  {
    public BrowsePageVM()
    {
      OpenFileCommand = new RelayCommand(OpenFile);
    }

    public PixelBitmap PBInput
    {
      get => IPManager.PBInput;
      set => IPManager.PBInput = value;
    }

    public ICommand OpenFileCommand { get; set; }
    public void OpenFile()
    {
      var dialog = new OpenFileDialog();
      var imageExtensions = ImageExts.String;
      dialog.Filter = "Images|" + imageExtensions + "|All|*.*";
      if (dialog.ShowDialog() == false)
      {
        return;
      }
      PBInput = new BitmapImage(new Uri(dialog.FileName)).ToPixelBitmap();
    }
  }
}
