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

    public static readonly DependencyProperty SelectedFrameProperty = DependencyProperty.Register(nameof(SelectedFrame), typeof(ApplicationPages), typeof(FrameControl));
    public ApplicationPages SelectedFrame
    {
      get { return (ApplicationPages)GetValue(SelectedFrameProperty); }
      set { SetValue(SelectedFrameProperty, value); }
    }
    
    public ICommand TurnBackCommand { get; set; }
    public void TurnBack()
    {
      SelectedFrame--;
    }
    public bool CanTurnBack() => SelectedFrame > 0;
    
    public ICommand TurnNextCommand { get; set; }
    public void TurnNext()
    {
      SelectedFrame++;
    }
    public bool CanTurnNext() => SelectedFrame < ApplicationPages.NumOfPages - 1;
  }
}
