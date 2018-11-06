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

namespace WpfPlotDigitizer
{
  public class BrowsePageVM : ViewModelBase<BrowsePageVM>
  {
    public BrowsePageVM()
    {
      OpenFileCommand = new RelayCommand(OpenFile);
    }

    public PixelBitmap pixelBitmapInput
    {
      get => IoC.Get<ImageProcessingVM>().pixelBitmapInput;
      set => IoC.Get<ImageProcessingVM>().pixelBitmapInput = value;
    }

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
  }
}
