using CycWpfLibrary;
using CycWpfLibrary.CustomControls;
using CycWpfLibrary.Media;
using CycWpfLibrary.Resources;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// MainWindow.xaml 的互動邏輯
  /// </summary>
  public partial class MainWindow : MetroWindow
  {
    public MainWindow()
    {
      InitializeComponent();

      DataContext = mainWindowVM;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      switch ((ApplicationPages)pageManager.Index)
      {
        case ApplicationPages.Browse:
          new PopupWindow
          {
            PlacementTarget = browsePage.browseButton,
            Text = "Click the button to browse your image!",
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = browsePage.dropBorder,
            Text = "Or you can simply drag and drop your image onto this area!",
          }.ShowDialog();

          break;
        case ApplicationPages.AxLim:
          break;
        case ApplicationPages.Axis:
          break;
        case ApplicationPages.Filter:
          break;
        case ApplicationPages.Erase:
          break;
        case ApplicationPages.Data:
          break;
        case ApplicationPages.Save:
          break;
        case ApplicationPages.NumOfPages:
          break;
        default:
          break;
      }
    }
  }
}
