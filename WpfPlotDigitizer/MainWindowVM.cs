using CycWpfLibrary;
using CycWpfLibrary.Controls;
using CycWpfLibrary.MVVM;
using CycWpfLibrary.Resources;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using static WpfPlotDigitizer.DI;
using MahApps.Metro;
using CycWpfLibrary.CustomControls;

namespace WpfPlotDigitizer
{
  public class MainWindowVM : ViewModelBase
  {
    public MainWindowVM()
    {
      TutorialCommand = new RelayCommand(Tutorial);
    }

    public PageManagerBase PageManager => pageManager;

    public ICommand TutorialCommand { get; set; }
    private void Tutorial()
    {
      switch ((ApplicationPages)pageManager.Index)
      {
        case ApplicationPages.Browse:
          new PopupWindow
          {
            Text = "Welcome to use Plot Digitizer! This is an intelligent application that can help you to digitize data from images."
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = browsePage.browseButton,
            Text = "First, just click the button to browse your image!",
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = browsePage.dropBorder,
            Text = "Or you can simply drag and drop your image onto this area!",
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = mainWindow.pageControl.TurnNextButton,
            Text = "After you finish, you can use your mouse to click inside this area, and turn to next page."
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
