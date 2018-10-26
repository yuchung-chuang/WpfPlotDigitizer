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
      TurnBackCommand = IoC.Get<ApplicationViewModel>().TurnBackCommand;
      TurnNextCommand = IoC.Get<ApplicationViewModel>().TurnNextCommand;
    }

    public static readonly DependencyProperty SelectedFrameProperty = DependencyProperty.Register(nameof(CurrentPage), typeof(ApplicationPages), typeof(FrameControl));
    public ApplicationPages CurrentPage
    {
      get { return (ApplicationPages)GetValue(SelectedFrameProperty); }
      set { SetValue(SelectedFrameProperty, value); }
    }

    public ICommand TurnBackCommand { get; set; }

    public ICommand TurnNextCommand { get; set; }
  }
}
