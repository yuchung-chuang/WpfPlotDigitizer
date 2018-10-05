using CycWpfLibrary.MVVM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfPlotDigitizer
{
  public class MainWindowViewModel : ViewModelBase
  {
    public MainWindowViewModel()
    {
      OpenFileCommand = new RelayCommand(OpenFile);
    }

    public ICommand OpenFileCommand { get; set; } //bind with button
    public Uri ImageSource { get; set; } // bind with image
    public void OpenFile()
    {
      var dialog = new OpenFileDialog();
      dialog.Filter = "Image | *.png; *.jpg;";
      if (dialog.ShowDialog() == false)
      {
        return;
      }
      ImageSource = new Uri(dialog.FileName);
    }
  }
}
