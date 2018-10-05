using CycWpfLibrary.MVVM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

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
    public Uri ImageSource { get; set; } 
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
  }
}
