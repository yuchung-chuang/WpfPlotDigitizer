using CycWpfLibrary;
using CycWpfLibrary.MVVM;
using System.Windows.Input;
using Emgu.CV;
using Emgu.CV.Structure;

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
