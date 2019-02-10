using CycWpfLibrary;
using CycWpfLibrary.CustomControls;
using CycWpfLibrary.Media;
using CycWpfLibrary.Resources;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
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
      Tutorial();
    }

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
            PlacementTargets = new FrameworkElement[]
            {
              mainWindow.pageControl.TurnNextButton,
              mainWindow.pageControl.TurnBackButton,
            },
            Text = "In the entire application, you can click inside both sides of the screen to turn next/back to the other pages."
          }.ShowDialog();
          break;
        case ApplicationPages.AxLim:
          new PopupWindow
          {
            Text = "This is Axis Limit Page, where you should input the axis limits."
          }.ShowDialog();
          new AxLimPopup
          {
            PlacementTarget = axLimPage.imageViewBox,
            Inlines = new List<Inline>
            {
              new Run("Here you can view your image."),
              new LineBreak(),
              new Run("You are allowed to manipulate all the image through the entire application by your mouse as follow."),
            },
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTargets = new FrameworkElement[] 
            {
              axLimPage.YMax,
              axLimPage.YMin,
              axLimPage.XMax,
              axLimPage.XMin,
            },
            Text = "According to your image, please type in all axis limits into the text boxes."
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTargets = new FrameworkElement[]
            {
              axLimPage.YLog,
              axLimPage.XLog,
            },
            Text = "If the axis in your image is in logarithm scale, you can specify its base through these text boxes."
          }.ShowDialog();

          break;
        case ApplicationPages.Axis:
          new PopupWindow
          {
            Text = "Here we are in the Axis Page, where you should input the axis position."
          }.ShowDialog();
          new AxisPopup
          {
            PlacementTarget = axisPage.axisControl,
            Text = "The application can automatically find the axis position for you, where you can also manually adjust the axis.",
          }.ShowDialog();

          break;
        case ApplicationPages.Filter:
          new PopupWindow
          {
            Text = "Here is Filter Page, where you can filter out unwanted information of certain colors."
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTargets = new FrameworkElement[]
            {
              filterPage.sliderRed,
              filterPage.sliderGreen,
              filterPage.sliderBlue,
            },
            Text = "You can either drag red, green, and blue range sliders to filter out colors outside the range,"
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTargets = new FrameworkElement[]
            {
              filterPage.textBoxRMax,
              filterPage.textBoxRMin,
              filterPage.textBoxGMax,
              filterPage.textBoxGMin,
              filterPage.textBoxBMax,
              filterPage.textBoxBMin,
            },
            Text = "or you can type in color code directly to select colors within specific range."
          }.ShowDialog();

          break;
        case ApplicationPages.Erase:
          new PopupWindow
          {
            Text = "Here is Erase Page, where you can erase out unwanted information by an eraser tool."
          }.ShowDialog();
          new ErasePopup
          {
            PlacementTarget = erasePage.imageGrid,
            Text = "You can erase the image by holding mouse right button. Besides, you can also pan and zoom image as usual!",
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTargets = new FrameworkElement[]
            {
              erasePage.undoButton,
              erasePage.redoButton,
            },
            Text = "You can also use undo and redo buttons to edit the image at your convenience.",
          }.ShowDialog();

          break;
        case ApplicationPages.Data:
          new PopupWindow
          {
            Text = "Here is Data Page, where you can adjust the data captured from the image."
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = dataPage.imageViewBox,
            Text = "The captured data is displayed as red dots here."
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = dataPage.sliderDataSize,
            Text = "You can drag this slider to adjust the size of each data point.",
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = dataPage.sliderCoverRatio,
            Text = "Or you can adjust the cover ratio (how much covered area counts a data point) by dragging this slider.",
          }.ShowDialog();

          break;
        case ApplicationPages.Save:
          new PopupWindow
          {
            Text = "Here is Save Page, where you can check your captured data and save it into a file.",
          }.ShowDialog();
          new SavePopup
          {
            PlacementTarget = savePage.dataPlot,
            Text = "Take a look at your data points here, you can also track the data coordinate and reset the view port.",
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = savePage.saveButton,
            Text = "Finally, You can save your data into several formats by clicking it. Well done!",
          }.ShowDialog();
          break;
        default:
          break;
      }
    }
  }
}
