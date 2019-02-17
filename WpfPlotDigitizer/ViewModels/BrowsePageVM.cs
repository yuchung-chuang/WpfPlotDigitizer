using CycLibrary.Media;
using CycLibrary.MVVM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

    private string fileName;
    public string FileName
    {
      get => fileName;
      set
      {
        if (ImageFileExtensions.List.Contains(Path.GetExtension(value)))
        {
          fileName = value;
          PBInput = new Bitmap(fileName).ToPixelBitmap();
        }
        else
        {
          OnMessageRequest("Input file should be an image.", MessageTypes.Warning);
        }
      }
    }

    public PixelBitmap PBInput
    {
      get => appData?.PBInput;
      set
      {
        appData.PBInput = value;
        appManager.PageManager.TurnNext(); //設定好就直接翻頁
      }
    }

    public ICommand OpenFileCommand { get; set; }
    public void OpenFile()
    {
      var dialog = new OpenFileDialog();
      var imageExtensions = ImageFileExtensions.String;
      dialog.Filter = "Images|" + imageExtensions + "|All|*.*";
      if (dialog.ShowDialog() == false)
        return;
      FileName = dialog.FileName;
    }
  }
}
