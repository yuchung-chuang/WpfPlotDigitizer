using CycWpfLibrary;
using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WpfPlotDigitizer.DI;
using System.Windows.Input;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Media.Imaging;
using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics;

namespace WpfPlotDigitizer
{
  public class ErasePageVM : ViewModelBase
  {
    public ErasePageVM()
    {
      UndoCommand = editManager.UndoCommand;
      RedoCommand = editManager.RedoCommand;
      DebugCommand = new RelayCommand(Debug);
      editManager.ObjectChanged += EditManager_ObjectChanged;
    }

    private void EditManager_ObjectChanged()
    {
      OnPropertyChanged(nameof(imageErase));
    }

    public Image<Bgra, byte> imageErase
    {
      get => (editManager.Object as Image<Bgra, byte>)?.Clone();
      //need Clone() to invoke setter during binding, perhaps because of binding to reference type.
      set => editManager.Edit(value);
    }
    public EditManager editManager { get; private set; } = new EditManager();

    public ICommand UndoCommand { get; private set; }
    public ICommand RedoCommand { get; private set; }
    public ICommand DebugCommand { get; private set; }

    private void Debug()
    {
      //(editManager.Object as Image<Bgra, byte>).ToPixelBitmap().Show();
      //IPManager.ImageErase.ToPixelBitmap().Show();
    }
  }
}
