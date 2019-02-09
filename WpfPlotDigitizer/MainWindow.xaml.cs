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
            PlacementTarget = mainWindow.pageControl.TurnNextButton,
            Text = "After you finish, you can use your mouse to click inside this area, and turn to next page."
          }.ShowDialog();
          break;
        case ApplicationPages.AxLim:
          new PopupWindow
          {
            Text = "This is axis limit page. Please type in all axis limits into the text boxes."
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = axLimPage.imageViewBox,
            Inlines = new List<Inline>
            {
              new Run("Here you can view your image."),
              new LineBreak(),
              new Run("Besides, you can use your mouse to manipulate all the image through the entire application as follow."),
            },
            Content = MakeContent(),
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = axLimPage.YMax,

          };

          Grid MakeContent()
          {
            var grid = new Grid
            {
              HorizontalAlignment = HorizontalAlignment.Center,
            };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            var leftImage = new Image
            {
              Source = CycResources.MouseLeftButtonUri.ToBitmapSource(),
              Height = 30,
              Margin = new Thickness(5),
            };
            Grid.SetRow(leftImage, 0);
            Grid.SetColumn(leftImage, 0);
            var wheelImage = new Image
            {
              Source = CycResources.MouseWheelUri.ToBitmapSource(),
              Height = 30,
            };
            Grid.SetRow(wheelImage, 1);
            Grid.SetColumn(wheelImage, 0);
            var leftText = new TextBlock
            {
              Text = "Pan Image",
              VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetRow(leftText, 0);
            Grid.SetColumn(leftText, 1);
            var wheelText = new TextBlock
            {
              Text = "Zoom Image",
              VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetRow(wheelText, 1);
            Grid.SetColumn(wheelText, 1);
            grid.Children.Add(leftImage);
            grid.Children.Add(wheelImage);
            grid.Children.Add(leftText);
            grid.Children.Add(wheelText);
            return grid;
          }
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
