using CycWpfLibrary.Emgu;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
      get => DI.imageData.ImageSave;
      set => DI.imageData.ImageSave = value;
    }

    public List<Point> data
    {
      get => DI.imageData.Data;
      set => DI.imageData.Data = value;
    }

    public BitmapSource imageSource => imageSave?.ToBitmapSource();

    public ICommand SaveCommand { get; set; }
    public void Save()
    {

    }
  }
}
