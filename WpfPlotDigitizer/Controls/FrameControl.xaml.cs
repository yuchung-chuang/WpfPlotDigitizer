using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// FrameControl.xaml 的互動邏輯
  /// </summary>
  public partial class FrameControl : UserControl
  {
    public FrameControl()
    {
      InitializeComponent();

      gridMain.DataContext = this;
      TurnBackCommand = new RelayCommand(TurnBack, CanTurnBack);
      TurnNextCommand = new RelayCommand(TurnNext, CanTurnNext);
    }

    public static readonly DependencyProperty SelectedFrameProperty = DependencyProperty.Register(nameof(SelectedFrame), typeof(Pages), typeof(FrameControl));
    public Pages SelectedFrame
    {
      get { return (Pages)GetValue(SelectedFrameProperty); }
      set { SetValue(SelectedFrameProperty, value); }
    }
    
    public ICommand TurnBackCommand { get; set; }
    private void TurnBack()
    {
      SelectedFrame--;
    }
    private bool CanTurnBack() => SelectedFrame > 0;
    
    public ICommand TurnNextCommand { get; set; }
    private void TurnNext()
    {
      SelectedFrame++;
    }
    private bool CanTurnNext() => SelectedFrame < Pages.NumOfPages - 1;
  }
}
