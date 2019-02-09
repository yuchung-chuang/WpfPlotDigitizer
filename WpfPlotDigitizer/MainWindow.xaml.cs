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
      resources.InitializeComponent();
      DataContext = mainWindowVM;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Tutorial();
    }

    private PopupWindowResources resources = new PopupWindowResources();
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
            Text = "This is Axis Limit Page."
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = axLimPage.imageViewBox,
            Inlines = new List<Inline>
            {
              new Run("Here you can view your image."),
              new LineBreak(),
              new Run("You are allowed to manipulate all the image through the entire application by your mouse as follow."),
            },
            Content = MakeContentAxLim(),
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

          Grid MakeContentAxLim()
          {
            var grid = new Grid
            {
              Style = resources["gridStyle"] as Style,
            };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            var leftImage = new Image
            {
              Source = CycResources.MouseLeftButtonUri.ToBitmapSource(),
              Style = resources["gridImageStyle"] as Style,
              Margin = new Thickness(5),
            };
            Grid.SetRow(leftImage, 0);
            Grid.SetColumn(leftImage, 0);
            var wheelImage = new Image
            {
              Source = CycResources.MouseWheelUri.ToBitmapSource(),
              Style = resources["gridImageStyle"] as Style,
            };
            Grid.SetRow(wheelImage, 1);
            Grid.SetColumn(wheelImage, 0);
            var leftText = new TextBlock
            {
              Text = "Pan Image",
              Style = resources["gridTextStyle"] as Style,
            };
            Grid.SetRow(leftText, 0);
            Grid.SetColumn(leftText, 1);
            var wheelText = new TextBlock
            {
              Text = "Zoom Image",
              Style = resources["gridTextStyle"] as Style,
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
          new PopupWindow
          {
            Text = "Here we are in the Axis Page."
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = axisPage.axisControl,
            Text = "The application can automatically find the axis position for you.",
            Content = MakeContentAxis(),
          }.ShowDialog();

          Grid MakeContentAxis()
          {
            var grid = new Grid
            {
              Style = resources["gridStyle"] as Style,
            };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            var imageAdjust = new Image
            {
              Source = CycResources.MouseLeftButtonUri.ToBitmapSource(),
              Style = resources["gridImageStyle"] as Style,
              Margin = new Thickness(5),
            };
            grid.Children.Add(imageAdjust);
            var textAdjust = new TextBlock
            {
              Text = "If it's not accurate enough, you can manually adjust the axis by dragging it with your mouse.",
              Style = resources["gridTextStyle"] as Style,
            };
            Grid.SetColumn(textAdjust, 1);
            grid.Children.Add(textAdjust);
            var stack = new StackPanel
            {
              Orientation = Orientation.Horizontal,
            };
            Grid.SetRow(stack, 1);
            var imageReset1 = new Image
            {
              Source = CycResources.MouseLeftButtonUri.ToBitmapSource(),
              Style = resources["gridImageStyle"] as Style,
            };
            stack.Children.Add(imageReset1);
            var imageReset2 = new Image
            {
              Source = CycResources.MouseLeftButtonUri.ToBitmapSource(),
              Style = resources["gridImageStyle"] as Style,
            };
            stack.Children.Add(imageReset2);
            grid.Children.Add(stack);
            var textReset = new TextBlock
            {
              Text = "If you accidently adjust the axis, you can do a double click to automatically find the axis again.",
              Style = resources["gridTextStyle"] as Style,
            };
            Grid.SetRow(textReset, 1);
            Grid.SetColumn(textReset, 1);
            grid.Children.Add(textReset);
            return grid;
          }
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
