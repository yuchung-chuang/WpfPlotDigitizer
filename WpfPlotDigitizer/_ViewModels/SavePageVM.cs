using CycWpfLibrary.Emgu;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class SavePageVM : ViewModelBase
  {
    public SavePageVM()
    {
      SaveCommand = new RelayCommand(Save);
    }

    public Image<Bgra, byte> imageSave
    {
      get => imageData.ImageSave;
      set => imageData.ImageSave = value;
    }

    public List<Point> data
    {
      get => imageData.Data;
      set => imageData.Data = value;
    }

    public BitmapSource imageSource => imageSave?.ToBitmapSource();

    public ICommand SaveCommand { get; set; }
    public void Save()
    {

    }
  }
}
