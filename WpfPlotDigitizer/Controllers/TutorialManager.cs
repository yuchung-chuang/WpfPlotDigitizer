using CycWpfLibrary.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using WpfPlotDigitizer.Properties;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class TutorialManager
  {
    public TutorialManager()
    {
      CheckFirstVisitApp();
    }

    private static readonly Settings settings = Settings.Default;
    public bool IsFirstVisitApp { set; get; } = settings.FirstApp;

    public void CheckFirstVisitApp()
    {
      if (settings.FirstApp)
      {
        settings.FirstApp = false;
        settings.Save();
      }
    }
    public void CheckFirstVisitPage()
    {
      var page = (ApplicationPages)pageManager.Index;
      var isFirstPropInfo = settings.GetType().GetProperty("First" + page.ToString());
      if ((bool)isFirstPropInfo.GetValue(settings))
      {
        isFirstPropInfo.SetValue(settings, false);
        settings.Save();
        Tutorial();
      }
    }

    private string FindTutorialText(string key)
    {
      return mainWindow.FindResource(key) as string;
    }

    public void Tutorial()
    {
      switch ((ApplicationPages)pageManager.Index)
      {
        case ApplicationPages.Browse:
          new PopupWindow
          {
            Text = FindTutorialText("BrowseIntroTutorial"),
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = browsePage.browseButton,
            Text = FindTutorialText("BrowseButtonTutorial"),
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = browsePage.dropBorder,
            Text = FindTutorialText("BrowseDropTutorial"),
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTargets = new FrameworkElement[]
            {
              mainWindow.PageControl.TurnNextButton,
              mainWindow.PageControl.TurnBackButton,
            },
            Text = FindTutorialText("BrowseTurnTutorial"),
          }.ShowDialog();
          break;
        case ApplicationPages.AxLim:
          new PopupWindow
          {
            Text = FindTutorialText("AxLimIntroTutorial"),
          }.ShowDialog();
          new AxLimPopup
          {
            PlacementTarget = axLimPage.imageViewBox,
            Inlines = new List<Inline>
            {
              new Run(FindTutorialText("AxLimImageTutorial1")),
              new LineBreak(),
              new Run(FindTutorialText("AxLimImageTutorial2")),
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
            Text = FindTutorialText("AxLimLimitsTutorial"),
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTargets = new FrameworkElement[]
            {
              axLimPage.YLog,
              axLimPage.XLog,
            },
            Text = FindTutorialText("AxLimLogsTutorial"),
          }.ShowDialog();

          break;
        case ApplicationPages.Axis:
          new PopupWindow
          {
            Text = FindTutorialText("AxisIntroTutorial"),
          }.ShowDialog();
          new AxisPopup
          {
            PlacementTarget = axisPage.imageViewBox,
            Text = FindTutorialText("AxisImageTutorial"),
          }.ShowDialog();

          break;
        case ApplicationPages.Filter:
          new PopupWindow
          {
            Text = FindTutorialText("FilterIntroTutorial"),
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTargets = new FrameworkElement[]
            {
              filterPage.sliderRed,
              filterPage.sliderGreen,
              filterPage.sliderBlue,
            },
            Text = FindTutorialText("FilterSliderTutorial"),
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
            Text = FindTutorialText("FilterTextBoxTutorial"),
          }.ShowDialog();

          break;
        case ApplicationPages.Erase:
          new PopupWindow
          {
            Text = FindTutorialText("EraseIntroTutorial"),
          }.ShowDialog();
          new ErasePopup
          {
            PlacementTarget = erasePage.imageViewBox,
            Text = FindTutorialText("EraseImageTutorial"),
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTargets = new FrameworkElement[]
            {
              erasePage.undoButton,
              erasePage.redoButton,
            },
            Text = FindTutorialText("EraseButtonTutorial"),
          }.ShowDialog();

          break;
        case ApplicationPages.Data:
          new PopupWindow
          {
            Text = FindTutorialText("DataIntroTutorial"),
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = dataPage.imageViewBox,
            Text = FindTutorialText("DataImageTutorial"),
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = dataPage.sliderDataSize,
            Text = FindTutorialText("DataSliderDataSizeTutorial"),
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = dataPage.sliderCoverRatio,
            Text = FindTutorialText("DataSliderCoverRatioTutorial"),
          }.ShowDialog();

          break;
        case ApplicationPages.Save:
          new PopupWindow
          {
            Text = FindTutorialText("SaveIntroTutorial"),
          }.ShowDialog();
          new SavePopup
          {
            PlacementTarget = savePage.dataPlot,
            Text = FindTutorialText("SavePlotTutorial"),
          }.ShowDialog();
          new PopupWindow
          {
            PlacementTarget = savePage.saveButton,
            Text = FindTutorialText("SaveButtonTutorial"),
          }.ShowDialog();
          break;
        default:
          break;
      }
    }

  }
}
